using System;
using System.Collections;
using System.Collections.Generic;

namespace KR
{
    /// <summary>
    /// [7,4] Hamming encoder/decoder used in DETECTION-ONLY mode.
    ///
    /// Encoding: each 4-bit nibble → 8 bits (7 Hamming bits + 1 overall parity bit).
    /// Each raw byte → 2 encoded bytes (low nibble + high nibble).
    ///
    /// Bit layout inside each encoded byte (LSB = bit 0):
    ///   bit 0 = d1   bit 1 = d2   bit 2 = d3   bit 3 = p1
    ///   bit 4 = d4   bit 5 = p2   bit 6 = p3   bit 7 = p_overall
    ///
    /// Parity equations:
    ///   p1 = d1 ^ d2 ^ d3   (covers bit positions 0,1,2)
    ///   p2 = d1 ^ d2 ^ d4   (covers bit positions 0,1,4)
    ///   p3 = d1 ^ d3 ^ d4   (covers bit positions 0,2,4)
    ///
    /// Syndrome: s1 = p1^d1^d2^d3,  s2 = p2^d1^d2^d4,  s3 = p3^d1^d3^d4
    ///
    /// Decoding (detection-only — Hamming's correcting capability is NOT used,
    /// because the real-channel error multiplicity is unknown: a 3-bit error
    /// looks like a correctable 1-bit error and would be "fixed" into the wrong
    /// codeword). Any detected anomaly triggers a re-request of the frame:
    ///   syndrome = 0 AND overall parity OK  → data accepted
    ///   any other case                       → error detected → return null (RET_CHUNK)
    /// </summary>
    class Coding
    {
        // ── Cyrillic Win-1251 ↔ Unicode helpers ──────────────────────────────
        private static int UnicodeToWin(int u) => u - 848;
        private static int WinToUnicode(int w) => w + 848;

        // ── Encode a single 4-bit nibble → 8-bit codeword ────────────────────
        private static byte EncodeNibble(BitArray d)
        {
            // Data bits
            bool d1 = d[0], d2 = d[1], d3 = d[2], d4 = d[3];

            // Hamming parity bits
            bool p1 = d1 ^ d2 ^ d3;          // covers d1,d2,d3
            bool p2 = d1 ^ d2 ^ d4;          // covers d1,d2,d4
            bool p3 = d1 ^ d3 ^ d4;          // covers d1,d3,d4

            // Overall parity (even parity over all 7 bits)
            bool p0 = d1 ^ d2 ^ d3 ^ d4 ^ p1 ^ p2 ^ p3;

            // Pack into one byte: bit7=p0, bit6=p3, bit5=p2, bit4=d4, bit3=p1, bit2=d3, bit1=d2, bit0=d1
            int b = 0;
            if (d1) b |= 0x01;
            if (d2) b |= 0x02;
            if (d3) b |= 0x04;
            if (p1) b |= 0x08;
            if (d4) b |= 0x10;
            if (p2) b |= 0x20;
            if (p3) b |= 0x40;
            if (p0) b |= 0x80;
            return (byte)b;
        }

        /// <summary>
        /// Decode one 8-bit codeword in DETECTION-ONLY mode.
        /// Returns the 4-bit nibble as a BitArray, or null if any error is detected
        /// (non-zero syndrome or broken overall parity). The caller must request a
        /// retransmission rather than attempt to correct, because the actual error
        /// multiplicity in a real channel is unknown — a 3-bit error pattern would
        /// look like a correctable 1-bit error and be "corrected" into the wrong
        /// codeword.
        /// </summary>
        public static BitArray DecodeNibble(byte raw, out bool errorDetected)
        {
            errorDetected = false;

            bool d1 = (raw & 0x01) != 0;
            bool d2 = (raw & 0x02) != 0;
            bool d3 = (raw & 0x04) != 0;
            bool p1 = (raw & 0x08) != 0;
            bool d4 = (raw & 0x10) != 0;
            bool p2 = (raw & 0x20) != 0;
            bool p3 = (raw & 0x40) != 0;

            bool s1 = p1 ^ d1 ^ d2 ^ d3;
            bool s2 = p2 ^ d1 ^ d2 ^ d4;
            bool s3 = p3 ^ d1 ^ d3 ^ d4;
            int  syndrome  = (s3 ? 4 : 0) | (s2 ? 2 : 0) | (s1 ? 1 : 0);
            bool overallOk = (CountBits(raw) % 2) == 0;

            if (syndrome != 0 || !overallOk)
            {
                errorDetected = true;
                return null;
            }

            var result = new BitArray(4);
            result[0] = d1;
            result[1] = d2;
            result[2] = d3;
            result[3] = d4;
            return result;
        }

        private static int CountBits(byte b)
        {
            int count = 0;
            while (b != 0) { count += b & 1; b >>= 1; }
            return count;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Encode a raw byte array using [7,4] Hamming + overall parity bit.
        /// Each input byte → 2 output bytes.
        /// </summary>
        public byte[] EncodeBytes(byte[] data)
        {
            if (data == null || data.Length == 0) return new byte[0];
            var result = new List<byte>(data.Length * 2);
            foreach (byte b in data)
            {
                var low  = new BitArray(4);
                var high = new BitArray(4);
                for (int i = 0; i < 4; i++) low[i]  = ((b >> i)     & 1) == 1;
                for (int i = 0; i < 4; i++) high[i] = ((b >> (i+4)) & 1) == 1;

                result.Add(EncodeNibble(low));
                result.Add(EncodeNibble(high));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Decode a byte array in DETECTION-ONLY mode.
        /// Returns null if any nibble has a detected error (non-zero syndrome or
        /// broken overall parity) — the caller must request a retransmission.
        /// </summary>
        public byte[] DecodeBytes(byte[] message)
        {
            return DecodeBytes(message, out _);
        }

        /// <summary>
        /// Decode with error-detection flag.
        /// Returns null and sets <paramref name="errorDetected"/> = true if any
        /// nibble fails its syndrome / overall-parity check.
        /// </summary>
        public byte[] DecodeBytes(byte[] message, out bool errorDetected)
        {
            errorDetected = false;

            if (message == null || message.Length % 2 != 0)
            {
                errorDetected = true;
                return null;
            }

            var result = new List<byte>(message.Length / 2);
            for (int i = 0; i < message.Length; i += 2)
            {
                var lowNibble  = DecodeNibble(message[i],   out bool e1);
                var highNibble = DecodeNibble(message[i+1], out bool e2);

                if (e1 || e2)
                {
                    errorDetected = true;
                    return null;
                }

                int b = 0;
                for (int j = 0; j < 4; j++) if (lowNibble[j])  b |= (1 << j);
                for (int j = 0; j < 4; j++) if (highNibble[j]) b |= (1 << (j+4));
                result.Add((byte)b);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Encode a string using [7,4] Hamming + overall parity.
        /// </summary>
        public byte[] Encode(string message)
        {
            if (string.IsNullOrEmpty(message)) return new byte[0];
            var bytes = new List<byte>();
            foreach (char ch in message)
            {
                int code = ch;
                if (code >= 1040 && code <= 1103) code = UnicodeToWin(code);
                bytes.Add((byte)(code & 0xFF));
            }
            return EncodeBytes(bytes.ToArray());
        }

        /// <summary>
        /// Decode a byte array encoded with Hamming back to a string.
        /// Returns null if any nibble has an uncorrectable error.
        /// </summary>
        public string Decode(byte[] message)
        {
            var raw = DecodeBytes(message);
            if (raw == null) return null;
            var sb = new System.Text.StringBuilder();
            foreach (byte b in raw)
            {
                int code = b;
                if (code >= 192 && code <= 255) code = WinToUnicode(code);
                sb.Append((char)code);
            }
            return sb.ToString();
        }
    }
}
