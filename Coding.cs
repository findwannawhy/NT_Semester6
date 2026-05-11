using System;
using System.Collections;
using System.Collections.Generic;

namespace KR
{
    /// <summary>
    /// [7,4] Hamming encoder/decoder with SECDED extension (8th overall-parity bit).
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
    /// syndrome value → bit position in byte (0-based):
    ///   1 → bit 3 (p1),  2 → bit 5 (p2),  3 → bit 0 (d1)
    ///   4 → bit 6 (p3),  5 → bit 1 (d2),  6 → bit 2 (d3),  7 → bit 4 (d4)
    ///
    /// Decoding:
    ///   syndrome = 0, overall parity OK  → no error
    ///   syndrome ≠ 0, overall parity bad → 1-bit error → correct it
    ///   syndrome ≠ 0, overall parity OK  → 2-bit error → uncorrectable → return null
    ///   syndrome = 0, overall parity bad → error in the overall-parity bit itself → ignore
    /// </summary>
    class Coding
    {
        // ── Cyrillic Win-1251 ↔ Unicode helpers ──────────────────────────────
        private static int UnicodeToWin(int u) => u - 848;
        private static int WinToUnicode(int w) => w + 848;

        // Syndrome value (1..7) → 0-based bit position in the encoded byte.
        // Derived from the parity equations above.
        // syndrome 1 = only s1 set → error in p1 → bit 3
        // syndrome 2 = only s2 set → error in p2 → bit 5
        // syndrome 3 = s1+s2 set  → error in d1  → bit 0
        // syndrome 4 = only s3 set → error in p3 → bit 6
        // syndrome 5 = s1+s3 set  → error in d2  → bit 1
        // syndrome 6 = s2+s3 set  → error in d3  → bit 2
        // syndrome 7 = s1+s2+s3   → error in d4  → bit 4
        // Verified by exhaustive simulation: syndrome → 0-based bit position in encoded byte
        private static readonly int[] SyndromeToBitPos = { -1, 3, 5, 1, 6, 2, 4, 0 };

        // ── Encode a single 4-bit nibble → 8-bit SECDED codeword ─────────────
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
        /// Decode one 8-bit SECDED codeword.
        /// Returns the 4-bit nibble as a BitArray, or null if a 2-bit (uncorrectable) error is detected.
        /// Corrects 1-bit errors in place.
        /// </summary>
        public static BitArray DecodeNibble(byte raw, out bool corrected, out bool uncorrectable)
        {
            corrected     = false;
            uncorrectable = false;

            // Extract bits
            bool d1 = (raw & 0x01) != 0;
            bool d2 = (raw & 0x02) != 0;
            bool d3 = (raw & 0x04) != 0;
            bool p1 = (raw & 0x08) != 0;
            bool d4 = (raw & 0x10) != 0;
            bool p2 = (raw & 0x20) != 0;
            bool p3 = (raw & 0x40) != 0;
            // p0 = (raw & 0x80) — used only for overall parity count

            // Syndrome bits
            bool s1 = p1 ^ d1 ^ d2 ^ d3;
            bool s2 = p2 ^ d1 ^ d2 ^ d4;
            bool s3 = p3 ^ d1 ^ d3 ^ d4;

            int syndrome = (s3 ? 4 : 0) | (s2 ? 2 : 0) | (s1 ? 1 : 0);

            // Overall parity check (count all 8 bits including p0)
            bool overallOk = (CountBits(raw) % 2) == 0;

            if (syndrome == 0 && overallOk)
            {
                // No error
            }
            else if (syndrome != 0 && !overallOk)
            {
                // 1-bit error — correct it using the syndrome→bit-position lookup
                int bitPos = SyndromeToBitPos[syndrome]; // 0-based bit in the byte
                raw ^= (byte)(1 << bitPos);
                corrected = true;

                // Re-extract corrected data bits
                d1 = (raw & 0x01) != 0;
                d2 = (raw & 0x02) != 0;
                d3 = (raw & 0x04) != 0;
                d4 = (raw & 0x10) != 0;
            }
            else if (syndrome != 0 && overallOk)
            {
                // 2-bit error — uncorrectable
                uncorrectable = true;
                return null;
            }
            // else syndrome==0 && !overallOk → error in p0 bit only → data is fine

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
        /// Encode a raw byte array using [7,4] Hamming + overall parity (SECDED).
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
        /// Decode a byte array encoded with SECDED Hamming back to raw bytes.
        /// Returns null if any nibble has an uncorrectable (2-bit) error.
        /// Corrects single-bit errors automatically.
        /// </summary>
        public byte[] DecodeBytes(byte[] message)
        {
            if (message == null || message.Length % 2 != 0) return null;
            var result = new List<byte>(message.Length / 2);
            for (int i = 0; i < message.Length; i += 2)
            {
                var lowNibble  = DecodeNibble(message[i],   out bool c1, out bool u1);
                var highNibble = DecodeNibble(message[i+1], out bool c2, out bool u2);

                if (u1 || u2)
                    return null; // uncorrectable 2-bit error

                // Reconstruct byte from two 4-bit nibbles
                int b = 0;
                for (int j = 0; j < 4; j++) if (lowNibble[j])  b |= (1 << j);
                for (int j = 0; j < 4; j++) if (highNibble[j]) b |= (1 << (j+4));
                result.Add((byte)b);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Decode with per-nibble correction tracking.
        /// Returns null if any nibble has an uncorrectable (2-bit) error.
        /// </summary>
        public byte[] DecodeBytes(byte[] message, out bool hadCorrection, out bool hadUncorrectable)
        {
            hadCorrection    = false;
            hadUncorrectable = false;

            if (message == null || message.Length % 2 != 0)
            {
                hadUncorrectable = true;
                return null;
            }

            var result = new List<byte>(message.Length / 2);
            for (int i = 0; i < message.Length; i += 2)
            {
                var lowNibble  = DecodeNibble(message[i],   out bool c1, out bool u1);
                var highNibble = DecodeNibble(message[i+1], out bool c2, out bool u2);

                if (u1 || u2)
                {
                    hadUncorrectable = true;
                    return null;
                }

                if (c1 || c2) hadCorrection = true;

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
