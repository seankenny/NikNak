using System;
using System.Threading;
using USBInterface;

namespace HU320
{
    public partial class HU320
    {
        private const int headerLength = 5;
        private const int maxDeviceInfoTableVersion = 1;
        private const char LED_PORT = 'C';
        private const byte LED_PIN = 0x10;
        private const char PSU_PORT = 'C';
        private const byte PSU_PIN = 0x04;

        private const ushort DEFAULT_VID = 0x20A0;
        private const ushort DEFAULT_PID = 0x4186;

        private EventWaitHandle waitReply;
        private int replyTimeout = 100;  //ms
        private HU320_USB_msg RX_msg;

        /// <summary>
        /// The crystal frequency in use (default 8MHz) This value should only be changed along with with custom firmware
        /// </summary>
        private int clockSpeed_Hz { get; set; }    //8meg by default

        private HIDInterface usbI;

        public event UART_dataReceivedEvent UART_dataReceived;    //The calling class can subscribe to these events
        public delegate void UART_dataReceivedEvent(object sender);
        private byte[] UART_RXbuffer;
        public int UART_rxBytesThreshold { set; get; }
        public int UART_bytesToRead { set; get; }
        private bool UART_enabled = false;
        
        public event SPI_dataReceivedEvent SPI_dataReceived;
        public delegate void SPI_dataReceivedEvent(object sender);
        public byte[] SPI_RXbuffer;
        public int SPI_rxBytesThreshold { set; get; }
        public int SPI_bytesToRead { set; get; }       //TODO make this functional
        private bool SPI_enabled = false;

        //public event I2C_dataReceivedEvent I2C_dataReceived;
        //public delegate void I2C_dataReceivedEvent(object sender);
        //public byte[] I2C_RXbuffer;
        //public int I2C_rxBytesThreshold { set; get; }
        //public int I2C_bytesToRead = 0;
        /// <summary>
        /// Returns true if the I2C transceiver has been enabled
        /// </summary>
        private bool I2C_enabled{ set; get; }

        public enum productID : byte
        {
            IFB100 = 0,
            HU320 = 1
        }

        //this array is indexed by the productID enum
        private string[] productDescriptions = {"IFB100", "Helion Micro - HU-320"};
    }
}