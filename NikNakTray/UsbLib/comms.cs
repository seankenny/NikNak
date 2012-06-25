using System;
using System.Collections.Generic;
using System.Text;

namespace HU320
{
    public partial class HU320
    {

        private enum section : byte
        {
            UART = 0x10,
            SPI_Ext = 0x12,
            I2C = 0x14,
            GPIO = 0x15,
            PSU = 0x18,
            timers = 0x1B,
            LCD = 0x1C,
            LEDs = 0x30,
            EEPROM = 0x31,
            internalCalls = 0x3A
        }

        /// <summary>
        /// Message structure:
        ///     U8  Section
        ///     U8  Instruction
        ///     U16 Flags
        ///     U8  Datalength
        /// </summary>
        private struct HU320_USB_msg
        {
            public section sect;
            public byte instruction;
            public UInt16 flags;
            public byte[] data;

            //bytewise constructor
            public HU320_USB_msg(byte[] msg)
            {
                //msg[0] is always 0 for an HID transfer
                this.sect = (section)msg[1];
                this.instruction = msg[2];
                this.flags = BitConverter.ToUInt16(msg, 3);

                data = new byte[msg[5]];
                Array.Copy(msg, headerLength + 1, data, 0, data.Length);
            }

            public byte[] flatten()
            {
                byte[] msg = new byte[data.Length + 5];
                msg[0] = (byte)sect;
                msg[1] = instruction;
                msg[2] = (byte)(flags & 0xFF);
                msg[3] = (byte)(flags >> 8);
                msg[4] = (byte)(data.Length);

                Array.Copy(data, 0, msg, 5, data.Length);
                return msg;
            }
        }

        /// <summary>
        /// The message header includes the datalength, which allows the message to span
        /// more than one HID report. Useful for long UART/SPI transfers
        /// </summary>
        private void msgSend(section s, byte instruction, byte[] data) 
        {
            HU320_USB_msg M = new HU320_USB_msg();
            M.sect = s;
            M.instruction = instruction;
            M.flags = 0x0000;
            M.data = data;

            byte[] b = M.flatten();
            usbI.write(b);
        }

        /// <summary>
        /// Use this method if no data packet is required
        /// </summary>
        /// <param name="s">Firmware section to access</param>
        /// <param name="instruction">Instruction to execute</param>
        private void msgSend(section s, byte instruction)
        {
            byte[] b = new byte[0];
            msgSend(s, instruction, b);
        }

        private bool msgSendGetReply(section s, byte instruction, byte[] data, out byte[] replyData)
        {
            replyData = new byte[0];
                                                        //TODO - add retries for failed messages
            msgSend(s, instruction, data);              //TODO - check section and instruction here
            bool replyOK = waitReply.WaitOne(replyTimeout);

            if (replyOK)
                replyData = RX_msg.data;
            else
                System.Threading.Thread.Sleep(10);
            return replyOK;
        }

        private bool msgSendGetReply(section s, byte instruction, out byte[] replyData)
        {
            replyData = new byte[0];
                                                            //TODO - add retries for failed messages
            msgSend(s, instruction);                        //TODO - check section and instruction here
            bool replyOK = waitReply.WaitOne(replyTimeout);

            if (replyOK)
                replyData = RX_msg.data;

            return replyOK;
        }

 
    }

}