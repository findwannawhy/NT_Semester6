using System.IO.Ports;

namespace KR
{
    /// <summary>
    /// Variant 5: 2 PCs connected via RS232C null-modem cable.
    /// COM-port parameters are configured by each user individually at startup.
    /// </summary>
    class PortSetup
    {
        public string   PortName { get; set; } = "COM1";
        public int      BaudRate { get; set; } = 9600;
        public int      DataBits { get; set; } = 8;
        public Parity   Parity   { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;

        public SerialPort OpenPort()
        {
            var port = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits)
            {
                ReadTimeout     = SerialPort.InfiniteTimeout,
                WriteTimeout    = 5000,
                RtsEnable       = true,
                DtrEnable       = true,
                DiscardNull     = false,
                ReadBufferSize  = 8192,
                WriteBufferSize = 8192,
            };
            port.Open();
            return port;
        }

        public string SettingsDescription =>
            $"{BaudRate} bps, {DataBits} бит, чётность: {Parity}, стоп: {StopBits}";
    }
}
