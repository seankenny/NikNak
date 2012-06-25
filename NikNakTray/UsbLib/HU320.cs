using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using USBInterface;

namespace HU320
{
    public partial class HU320
    {
        public struct deviceInfo
        {
            public string firmwareVersion;
            public productID prodID;
            public UInt16 serialNumber;
            public string description;
        }

        #region constructors

        /// <summary>
        /// Creates a new instance of the HU320 device.
        /// </summary>
        /// <param name="VID">USB Vendor ID - eg 0x06ba</param>
        /// <param name="PID">USB Product ID - eg 0x000a</param>
        /// <param name="serialNumber">Serial number of the target hardware</param>
        public HU320(ushort VID, ushort PID, ushort serialNumber)
        {
            init(VID, PID, serialNumber);
        }

        /// <summary>
        /// Creates a new instance of the HU320 device. Assumes the default VID/PID
        /// </summary>
        /// <param name="serialNumber"></param>
        public HU320(ushort serialNumber)
        {
            init(DEFAULT_VID, DEFAULT_PID, serialNumber);
        }

        /// <summary>
        /// Used if only one device is connected, finds the serial number automatically
        /// Throws an exception if more than one device is found
        /// </summary>
        public HU320()
        {
            HIDInterface.interfaceDetails[] devices = HIDInterface.getConnectedDevices();
            int numExpectedDevices = 0;
            int deviceIndex = 0;
            for(int i = 0; i < devices.Length; i++)
            {
                if ((devices[i].VID == DEFAULT_VID) && (devices[i].PID == DEFAULT_PID))
                {
                    numExpectedDevices++;
                    deviceIndex = i;
                }
            }

            if (numExpectedDevices == 0)
                throw new Exception("No HU320 devices could be found!");
            else if (numExpectedDevices > 1)
                throw new Exception("Only one HU320 may be connected, " + numExpectedDevices.ToString() + " found.");

            init(devices[deviceIndex].VID, devices[deviceIndex].PID, (ushort)devices[deviceIndex].serialNumber);

        }

        //sets up the USB interface
        private void init(ushort VID, ushort PID, ushort serialNumber)
        {
            //Initialise globals
            this.clockSpeed_Hz = 8000000; //8MHz
            this.UART_rxBytesThreshold = 10;
            this.SPI_rxBytesThreshold = 1;
            //this.I2C_rxBytesThreshold = 1;
            this.UART_RXbuffer = new byte[0];
            this.SPI_RXbuffer = new byte[0];


            usbI = new HIDInterface(VID, PID, serialNumber, true);    //create an interface to a specific unit
            usbI.dataReceived += new HIDInterface.dataReceivedEvent(usbI_dataReceived);


            //this eventwaithandle is used to flag when data has been received from a read
            waitReply = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        #endregion

        /// <summary>
        /// This method handles events from the USB interface class, this will split into more events for UART SPI etc
        /// If the message is not an async read, the eventWaitHandle (waitReply) will be signalled, otherwise and event will be triggered
        /// </summary>
        /// <param name="message"></param>
        private void usbI_dataReceived(byte[] message)
        {
            //message[0] = always 0 for a HID report
            RX_msg = new HU320_USB_msg(message);

            //If the message is an async read message, store the data - and optionally trigger the correct event
            if ((RX_msg.sect == section.UART) && (RX_msg.instruction == (byte)UART_Instr.asyncReceive) && UART_enabled)
            {
                //store the data
                int oldBufferLength = UART_RXbuffer.Length;
                Array.Resize(ref UART_RXbuffer, UART_RXbuffer.Length + RX_msg.data.Length);
                Array.Copy(RX_msg.data, 0, UART_RXbuffer, oldBufferLength, RX_msg.data.Length);
                this.UART_bytesToRead = UART_RXbuffer.Length;

                if ((UART_dataReceived != null) && (UART_RXbuffer.Length >= UART_rxBytesThreshold))
                    UART_dataReceived(this);
            }
            else if ((RX_msg.sect == section.SPI_Ext) && (RX_msg.instruction == (byte)SPI_instr.asyncReply) && SPI_enabled)
            {
                //store the data
                int oldBufferLength = SPI_RXbuffer.Length;
                Array.Resize(ref SPI_RXbuffer, SPI_RXbuffer.Length + RX_msg.data.Length);
                Array.Copy(RX_msg.data, 0, SPI_RXbuffer, oldBufferLength, RX_msg.data.Length);
                this.SPI_bytesToRead = SPI_RXbuffer.Length;

                if ((SPI_dataReceived != null) && (SPI_RXbuffer.Length >= SPI_rxBytesThreshold))
                    SPI_dataReceived(this);
            }
            //else if ((RX_msg.sect == section.I2C) && (RX_msg.instruction == (byte)I2C_instr.asyncReply) && (I2C_dataReceived != null) && (I2C_RXbuffer.Length >= I2C_rxBytesThreshold))
                //I2C_dataReceived(this);

            else     //The message is a normal reply
                waitReply.Set();
        }

        /// <summary>
        /// Queries the operating system for the serial numbers of the devices conneced with the specified VID/PID 
        /// </summary>
        /// <param name="VID">VID to search for</param>
        /// <param name="PID">PID to search for</param>
        /// <returns>Array of the connected device's serial numbers</returns>
        public static int[] getConnectedDevices(ushort VID, ushort PID)
        {
            HIDInterface.interfaceDetails[] devs = HIDInterface.getConnectedDevices();

            //find how many devices fit the expected type
            int numExpectedDevices = 0;
            foreach (HIDInterface.interfaceDetails D in devs)
                if ((D.VID == VID) && (D.PID == PID))
                    numExpectedDevices++;

            //create an array of the matching serial numbers
            int[] SNs = new int[numExpectedDevices];
            int matchIndex = 0;
            for (int i = 0; i < devs.Length; i++)
            {
                if ((devs[i].VID == VID) && (devs[i].PID == PID))
                {
                    SNs[matchIndex] = devs[i].serialNumber;
                    matchIndex++;
                }
            }
            return SNs;
        }

        /// <summary>
        /// Queries the operating system for the serial numbers of the devices conneced with the default VID/PID
        /// </summary>
        /// <returns>Array of the connected device's serial numbers</returns>
        public static int[] getConnectedDevices()
        {
            return getConnectedDevices(DEFAULT_VID, DEFAULT_PID);
        }

        /// <summary>
        /// Stops the flow of HID reports and disconnects the device
        /// </summary>
        public void close()
        {
            usbI.close();
        }

    }
}
