using System;
using System.Threading;

namespace HU320
{
    public partial class HU320
    {
        //--------------------- UART --------------------------
        #region UART
        //Globals

        //Enumerated types
        public enum UART_baudRate
        {
            _2400bps = 2400,
            _4800bps = 4800,
            _9600bps = 9600, 
            _14400bps = 14400,
            _19200bps = 19200, 
            _28800bps = 28800,
            _38400bps = 38400,
            _57600bps = 57600,
            _76800bps = 76800,
            _115200bps = 115200,
            _125kbps = 125000,
            _250kbps = 250000,
            _500kbps = 500000,
            _1Mbps  = 1000000
        }

        public enum UART_stopBits
        {
            oneStopBit = 0x00, twoStopBits = 0x08
        }

        private enum UART_Instr : byte
        {
            write = 0x10,
            asyncReceive = 0x11,
            setBaud = 0x20,
            setRegisters = 0x21
        }

        //methods
        /// <summary>
        /// Sets up the UART, causes the functions of the pins PD2/PD3 to be overridden by the UART
        /// </summary>
        /// <param name="rate">Baudrate to be used</param>
        /// <param name="stopbits">Number of stop bits to be used (1 or 2)</param>
        public void UART_setup(UART_baudRate rate, UART_stopBits stopbits)
        {
            UART_enabled = true;

            byte UCSR1A = 0x02;                             //double speed mode enabled (always better accuracy at 8MHz)
            byte UCSR1B = 0xD8;                             //Set interrupts to enabled, enable transmitter and reciever (data reg empty interrupt disabled)
            byte UCSR1C = (byte)(0x06 + (byte)stopbits);    //async operation, parity disabled, 8bit frames, settable stopbits
            byte UCSR1D = 0x00;                             //CTS/RTS disabled

            //double speed baudrate calc
            UInt16 BR = (UInt16)(clockSpeed_Hz / (8 * (int)rate) - 1);
            byte[] b = BitConverter.GetBytes(BR);

            //data expected: UCSR1A UCSR1B UCSR1C UCSR1D UBRR1H UBRR1L
            byte[] data = { UCSR1A, UCSR1B, UCSR1C, UCSR1D, b[0], b[1] };
            msgSend(section.UART, (byte)UART_Instr.setRegisters, data);            
        }

        /// <summary>
        /// (Advanced) Sets up the UART. Causes the functions of the pins PD2/PD3 to be overridden by the UART
        /// Sets the registers directly, so all features can be accessed
        /// </summary>
        public void UART_setup(byte UCSR1A, byte UCSR1B, byte UCSR1C, byte UCSR1D, byte UBRR1H, byte UBRR1L)
        {
            UART_enabled = !((UCSR1A == 0) && (UCSR1B == 0) && (UCSR1C == 0) && (UCSR1D == 0) && (UBRR1H == 0) && (UBRR1L == 0));

            //data expected: UCSR1A UCSR1B UCSR1C UCSR1D UBRR1H UBRR1L
            byte[] data = { UCSR1A, UCSR1B, UCSR1C, UCSR1D, UBRR1H, UBRR1L };
            msgSend(section.UART, (byte)UART_Instr.setRegisters, data);            
        }

        /// <summary>
        /// Disables the UART. Returns the pin functions to their normal operation
        /// </summary>
        public void UART_disable()
        {
            UART_setup(0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
        }
        
        public void UART_write(byte[] data) 
        {
            msgSend(section.UART, (byte)UART_Instr.write, data);
        }

        /// <summary>
        /// Reads a number of bytes from the UART RX buffer
        /// If more bytes are requested than available, all available bytes are returned
        /// </summary>
        /// <param name="numBytesToRead">The number of bytes to be read</param>
        /// <returns>Read data</returns>
        public byte[] UART_read(int numBytesToRead)
        {
            if (numBytesToRead > UART_bytesToRead)  //if this number is big, just return all the data
                numBytesToRead = UART_bytesToRead;

            //copy data to be returned
            byte[] returnData = new byte[numBytesToRead];
            Array.Copy(UART_RXbuffer, 0, returnData, 0, numBytesToRead);

            //remove return data from array
            Array.Reverse(UART_RXbuffer);
            Array.Resize(ref UART_RXbuffer, UART_RXbuffer.Length - numBytesToRead);
            Array.Reverse(UART_RXbuffer);

            return returnData;
        }

        /// <summary>
        /// Sets the baudrate to be used by the UART block
        /// </summary>
        /// <param name="rate">Baudrate - enumerated type</param>
        public void UART_setBaudRate(UART_baudRate rate)    //uses double speed operation(UCSR1A = 0x02)
        {
            UInt16 BR = (UInt16)(clockSpeed_Hz / (8 * (int)rate) - 1);
            byte[] b = BitConverter.GetBytes(BR);
            msgSend(section.UART, (byte)UART_Instr.setBaud, b);
        }
        
        //delegate for receive event
        #endregion

        //--------------------- SPI ---------------------------
        #region SPI

        #region SPI_enums
        private enum SPI_instr
        {
            doTransaction = 0x10,
            asyncReply = 0x11,
            setCtrlRegs = 0x12
        }

        public enum SPI_dataOrder { LSB_first = 0x20, MSB_first = 0x00 }
        public enum SPI_masterSlave { master = 0x10, slave = 0x80 } //slave enables interrupt

        public enum SPI_mode
        {
            mode_0 = 0x00, mode_1 = 0x04, mode_2 = 0x08, mode_3 = 0x0C
        }

        public enum SPI_baudRate
        {
            _62500bps = 0x03, 
            _125kbps = 0x02,
            _250kbps = 0x06,
            _500kbps = 0x01,
            _1Mbps = 0x05,
            _2Mbps = 0x00,
            _4Mbps = 0x04
        }

        #endregion
        //globals
        private EventWaitHandle SPI_replyWait;

        //Methods
        /// <summary>
        /// (Advanced) Sets up SPI controller.
        /// Allows access to all functions, see Atmel AT90USB162 datasheet for details
        /// Data for all SPI replies will come through the SPI_DataReceived event
        /// </summary>
        /// <param name="SPCR">SPI Control register</param>
        /// <param name="SPSR">SPI Status register</param>
        public void SPI_setup(byte SPCR, byte SPSR) 
        {
            byte[] b = { SPCR, SPSR };
            msgSend(section.SPI_Ext, (byte)SPI_instr.setCtrlRegs, b);

            if ((SPCR == 0x00) && (SPSR == 0x00))
                SPI_enabled = false;
            else
                SPI_enabled = true;
        }

        /// <summary>
        /// Sets up the SPI controller using simple enumerated types
        /// </summary>
        /// <param name="MS">Select master or slave SPI mode</param>
        /// <param name="mode">Selects clock phase and polarity based on standard SPI mode types</param>
        /// <param name="order">Data order, MSB or LSB first</param>
        /// <param name="rate">Data clock rate to be used (no effect in slave mode)</param>
        public void SPI_setup(SPI_masterSlave MS, SPI_mode mode, SPI_dataOrder order, SPI_baudRate rate)
        {
            byte SPCR = (byte)(0x40 | (byte)MS | (byte)mode | (byte)order | (byte)rate);
            byte SPSR = (byte)((byte)rate >> 2);

            SPI_setup(SPCR, SPSR);

            //if this is set as master mode, then we need an event to allow the data to be read
            SPI_replyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            
            if (MS == SPI_masterSlave.master)
                SPI_dataReceived += new SPI_dataReceivedEvent(HU320_SPI_dataReceived);
            else
                SPI_dataReceived -= new SPI_dataReceivedEvent(HU320_SPI_dataReceived);
        }

        private void HU320_SPI_dataReceived(object sender)
        {
            //allows data to be read by the recieving
            SPI_replyWait.Set();
        }

        /// <summary>
        /// Performs an SPI read/write operation when configured as master
        /// SPI_Setup() must be called prior to using this method
        /// </summary>
        /// <param name="inData">Byte array to send, right padded with the number of bytes to read</param>
        /// <param name="outData">All bytes shifted in during SPI transaction</param>
        /// <param name="port">Port name (B,C,D) which includes the correct chip select pin</param>
        /// <param name="chipSelectBitMask">Bit mask of chip select pin to drive during this transaction</param>
        public void SPI_transaction(byte[] inData, out byte[] outData, char port, byte chipSelectBitMask)
        {
            byte[] dataBytes = new byte[inData.Length + 2];
            dataBytes[0] = (byte)port;
            dataBytes[1] = chipSelectBitMask;
            Array.Copy(inData, 0, dataBytes, 2, inData.Length);

            msgSend(section.SPI_Ext, (byte)SPI_instr.doTransaction, dataBytes);
            //wait for reply, if none comes, send no data back
            bool replyOK = SPI_replyWait.WaitOne(replyTimeout);
            if (replyOK)
                outData = RX_msg.data;
            else
                outData = new byte[0];
        }

        #endregion

        //--------------------- I2C ---------------------------
        #region I2C

        private enum I2C_instr
        {
            //high level functions
            write = 0x10,
            read = 0x11,    
            setup = 0x12,   
            //low level functions
            sendStart = 0x13,
            sendStop = 0x14,
            sendBytes = 0x15,
            readBytes = 0x16,
            checkErrors = 0x17,
            asyncReply = 0x18,
            busScan = 0x19
        }

        public enum I2C_error
        {
            noError = 0x00, missingACK = 0x01, arbitrationLost = 0x02, setupFail = 0x03
        }
        
        //globals

        //Methods
        /// <summary>
        /// Enables the I2C transceiver
        /// </summary>
        public void I2C_setup() 
        { 
            msgSend(section.I2C, (byte)I2C_instr.setup);
            I2C_enabled = true;
        }

        /// <summary>
        /// Scans through all possible I2C addresses, and checks for an ACK at each address
        /// </summary>
        /// <param name="validAddresses">A list of addresses of I2C devices detected on the bus</param>
        public void I2C_busScan(out byte[] validAddresses)
        {
            byte[] temp;
            byte[] addrs = new byte[128];
            byte increment = 2;

            //protocol for the busscan is startaddr, endaddr, increment.
            byte[] b = { 1, 100, increment };   //using the 2 as the increment tests only the addresses with write in the LSB
            msgSendGetReply(section.I2C, (byte)I2C_instr.busScan, b, out temp);
            Array.Copy(temp, 0, addrs, 0, temp.Length);

            byte[] b1 = { 101, 200, increment };   //using the 2 as the increment tests only the addresses with write in the LSB
            msgSendGetReply(section.I2C, (byte)I2C_instr.busScan, b1, out temp);
            Array.Copy(temp, 0, addrs, 50, temp.Length);

            byte[] b2 = { 201, 255, increment };   //using the 2 as the increment tests only the addresses with write in the LSB
            msgSendGetReply(section.I2C, (byte)I2C_instr.busScan, b2, out temp);
            Array.Copy(temp, 0, addrs, 100, temp.Length);

            //the addrs array now contains a full list of 128 possible addresses, with either a true (0) or false (0x20) next to it
            int numDevices = 0;
            for (int i = 0; i < addrs.Length; i++)  //find how many devices are present
                if (addrs[i] == 0x00)   //true
                    numDevices++;

            int addrIndex = 0;
            validAddresses = new byte[numDevices];
            if (numDevices == 0)
                return;

            for (int i = 0; i < addrs.Length; i++)  //record the addresses of the connected devices
                if (addrs[i] == 0x00)   //true
                {
                    validAddresses[addrIndex] = (byte)((i * increment) >> 1);  //remove the read/write flag when saving the address number
                    addrIndex++;
                }
        }

        /// <summary>
        /// Performs a one-step write operation including start and stop instructions. all device addressing must be included in the byte array
        /// </summary>
        /// <param name="data">bytes to write to the bus, including address</param>
        public void I2C_write(byte[] data) 
        { 
            byte[] b = new byte[data.Length + 1];
            b[0] = (byte)data.Length;
            Array.Copy(data, 0, b, 1, data.Length);

            msgSend(section.I2C, (byte)I2C_instr.write, b);        
        }

        /// <summary>
        /// Writes data to the I2C bus using a 7bit device address and a 16bit internal address
        /// </summary>
        /// <param name="deviceAddress">7bit address of the I2C device</param>
        /// <param name="internalAddress">internal address to write to</param>
        /// <param name="data">Data bytes to write</param>
        public void I2C_write(byte deviceAddress, UInt16 internalAddress, byte[] data)
        {
            byte[] b = new byte[data.Length + 3];
            b[0] = (byte)((deviceAddress << 1) & 0xFE);
            b[1] = (byte)(internalAddress & 0xFF);
            b[2] = (byte)(internalAddress >> 8);
            Array.Copy(data, 0, b, 3, data.Length);
            I2C_write(b);
        }

        /// <summary>
        /// Writes data to the I2C bus using a 7bit device address and an 8bit internal address
        /// </summary>
        /// <param name="deviceAddress">7bit address of the I2C device</param>
        /// <param name="internalAddress">internal address to write to</param>
        /// <param name="data">Data bytes to write</param>
        public void I2C_write(byte deviceAddress, byte internalAddress, byte[] data)
        {
            byte[] b = new byte[data.Length + 2];
            b[0] = (byte)((deviceAddress << 1) & 0xFE);
            b[1] = internalAddress;
            Array.Copy(data, 0, b, 2, data.Length);
            I2C_write(b);
        }
        
        /// <summary>
        /// Flexible I2C read, allows an arbitrary number of bytes to precede the repeated start (allows 8 or 16bit addressing)
        /// </summary>
        /// <param name="writeData">Address / control bytes. The last byte in this array forms the second address (normally sent with the read flag set). This byte is sent after the repeated start command</param>
        /// <param name="numBytesToRead">Number of bytes to read from the specified slave</param>
        /// <param name="outData">The data returned by the slave device</param>
        public void I2C_read(byte[] writeData, int numBytesToRead, out byte[] outData) 
        {
            byte[] b = new byte[writeData.Length + 2];
            b[0] = (byte)(writeData.Length);
            b[1] = (byte)numBytesToRead;
            Array.Copy(writeData, 0, b, 2, writeData.Length);
            msgSendGetReply(section.I2C, (byte)I2C_instr.read, b, out outData);
        }

        /// <summary>
        /// Quick I2C read using a 16bit internal address
        /// </summary>
        /// <param name="deviceAddress">7bit address of the I2C device</param>
        /// <param name="internalAddress">internal address to read bytes from</param>
        /// <param name="numBytesToRead">The number of bytes to read from the I2C device</param>
        /// <param name="outData">The data bytes read from the I2C device</param>
        public void I2C_read(byte deviceAddress, UInt16 internalAddress, int numBytesToRead, out byte[] outData)
        {
            byte[] writeData = { (byte)((deviceAddress << 1) & 0xFE), (byte)(internalAddress & 0xFF), (byte)(internalAddress >> 8), (byte)((deviceAddress << 1) | 0x01) }; //The last bytes is ORed with 0x01 to set the read flag
            I2C_read(writeData, numBytesToRead, out outData);
        }

        /// <summary>
        /// Quick I2C read using an 8bit internal address
        /// </summary>
        /// <param name="deviceAddress">7bit address of the I2C device</param>
        /// <param name="internalAddress">internal address to read bytes from</param>
        /// <param name="numBytesToRead">The number of bytes to read from the I2C device</param>
        /// <param name="outData">The data bytes read from the I2C device</param>
        public void I2C_read(byte deviceAddress, byte internalAddress, int numBytesToRead, out byte[] outData)
        {
            byte[] writeData = { (byte)((deviceAddress << 1) & 0xFE), internalAddress, (byte)((deviceAddress << 1) | 0x01) }; //The last bytes is ORed with 0x01 to set the read flag
            I2C_read(writeData, numBytesToRead, out outData);
        }

        /// <summary>
        /// Sends an I2C start condition. If the lines cannot be driven correctly, an error is generated. 
        /// Also clears all existing errors
        /// </summary>
        public void I2C_sendStart() 
        {
            msgSend(section.I2C, (byte)I2C_instr.sendStart);
        }

        /// <summary>
        /// Sends an I2C stop condition
        /// </summary>
        public void I2C_sendStop() 
        {
            msgSend(section.I2C, (byte)I2C_instr.sendStop);
        }

        /// <summary>
        /// writes the specified bytes to the bus, must be preceded by a start condition
        /// </summary>
        /// <param name="data">Bytes to be written</param>
        public void I2C_writeData(byte[] data) 
        {
            byte[] b = new byte[data.Length + 1];
            b[0] = (byte)data.Length;
            Array.Copy(data, 0, b, 1, data.Length);

            msgSend(section.I2C, (byte)I2C_instr.sendBytes, b);       
        }

        /// <summary>
        /// Attempts to read the specified number of bytes from the bus.
        /// </summary>
        /// <param name="numBytes">Number of bytes to read</param>
        /// <param name="outData">data read from the bus</param>
        public void I2C_readData(byte numBytes, out byte[] outData) 
        { 
            byte[] b = {numBytes};
            msgSendGetReply(section.I2C, (byte)I2C_instr.readBytes, b, out outData);            
        }

        /// <summary>
        /// Reads the error code held in memory. Each time a new operation occurs, the error code is overwritten
        /// Calling this method clears the error code to 0 (no error)
        /// </summary>
        /// <returns>Enumerated type of the error encountered</returns>
        public I2C_error I2C_checkError()
        {
            byte[] replyData;
            msgSendGetReply(section.I2C, (byte)I2C_instr.checkErrors, out replyData);
            return (I2C_error)replyData[0];
        }

        #endregion

        //--------------------- GPIO --------------------------
        #region GPIO

        private enum GPIO_Instr : byte
        {
            writePort = 0x10,
            readPort = 0x11,
            setDirPort = 0x12
        }

        //Port at a time
        /// <summary>
        /// Sets the data direction of all pins in the specified port, covered by the mask
        /// </summary>
        /// <param name="port">Port ID letter (B, C, D)</param>
        /// <param name="direction">bit mask, 1 - output, 0 - input</param>
        /// <param name="mask">bit mask, 1 - pin is changed, 0 - pin is unchanged</param>
        public void GPIO_setDirPort(char port, byte direction, byte mask) 
        {
            byte[] data = { (byte)port, direction, mask };
            msgSend(section.GPIO, (byte)GPIO_Instr.setDirPort, data);
        }

        /// <summary>
        /// Sets or clears the specified pins based on the mask
        /// Also used to configure internal pullup resistors when pins are set as input
        /// </summary>
        /// <param name="port">Port ID letter (B, C, D)</param>
        /// <param name="portValue">bit mask, 1 - high, 0 - low</param>
        /// <param name="mask">bit mask, 1 - pin is changed, 0 - pin is unchanged</param>
        public void GPIO_writePort(char port, byte portValue, byte mask) 
        {
            byte[] data = { (byte)port, portValue, mask };
            msgSend(section.GPIO, (byte)GPIO_Instr.writePort, data);
        }

        /// <summary>
        /// Reads the state of all pins in the port
        /// </summary>
        /// <param name="port">Port ID letter (B, C, D)</param>
        /// <param name="portValue">The value read from the specified port</param>
        public void GPIO_readPort(char port, out byte portValue) 
        {
            portValue = 0;
            byte[] data = { (byte)port };
            byte[] inData;

            bool replyOK = msgSendGetReply(section.GPIO, (byte)GPIO_Instr.readPort, data, out inData);
            if (replyOK && (inData.Length > 0))
                portValue = inData[0];
        }

        #endregion

        //--------------------- Timers ------------------------
        #region Timers

        private enum timerInstr
        {
            setControlRegs = 0x10,
            setMaskAndFlags = 0x11,
            setOCRs = 0x12,
            setCounterReg = 0x13,
            setServoSweep = 0x14,
            setAllRegisters = 0x15
        }

        //High level timer functions
        /// <summary>
        /// Initialises the timer to allow pulse counting. Each time the specific edge is detected on the T1 pin (PB4), 
        /// the value of the 16bit counter register is incremented. The value of the counter register can be found using
        /// timer_getTimerValue()
        /// </summary>
        /// <param name="useFallingEdge"></param>
        public void timer_setupPulseCounting(bool useFallingEdge)
        {
            byte TCCRB = (byte)(0x06 | Convert.ToByte(useFallingEdge));
            timer_setControlRegisters(0x00, TCCRB, 0x00);
            //zero TCNT1
            timer_clearTimerValue();
        }

        /// <summary>
        /// Generates a PWM signal on PC5 with the specified frequency and duty cycle
        /// </summary>
        /// <param name="frequency">The output frequency in Hz of the PWM signal value must be in the range 125-160000 Hz</param>
        /// <param name="dutyCycle_percent">Duty cycle to be applied (0-100%)</param>
        public void timer_setupPWM(int frequency, double dutyCycle_percent)
        {
            //coerce inputs
            frequency = utilities.utilities.coerce(frequency, 160000, 125);
            dutyCycle_percent = utilities.utilities.coerce(dutyCycle_percent, 100, 0);

            //This mode uses the "Fast PWM" function. The counter runs from 0x0000 to OCR1A.
            //OC1B (PC5) is set when TCNT1 == OCR1B, OC1C is not used.
            //TCCR1A = 0b00 10 00 11    (OCA, OCB, OCC, WGM)
            byte TCCR1A = 0x23;
            //TCCR1B = 0b000 11 001     (ICP WGM clockSelect)
            byte TCCR1B = 0x19;
            byte TCCR1C = 0x00;

            double _OCR1A = (1 / ((1 / (double)clockSpeed_Hz) * (double)frequency));
            UInt16 OCR1A = (UInt16)_OCR1A; //register top
            UInt16 OCR1B = (UInt16)(dutyCycle_percent * (double)OCR1A / 100);

            timer_setMasksAndFlags(0x00, 0x00);
            timer_setOutputCompareRegisters(OCR1A, OCR1B, 0x0000);
            timer_setControlRegisters(TCCR1A, TCCR1B, TCCR1C);
        }

        /// <summary>
        /// Provides an interface for standard hobby/RC servos
        /// </summary>
        /// <param name="period_us">Period of the pulse train (normally 10000us)</param>
        /// <param name="pulse_us">Pulse used to set the servo angle, normally 1000-2000us full scale</param>
        public void timer_setServoPosition(ushort period_us, ushort pulse_us)
        {
            //This mode uses the "Fast PWM" function. The counter runs from 0x0000 to OCR1A.
            //clock is divided by 8 to give a max period of 65ms, 1us resolution @ 8MHz
            //OC1B (PC5) is set when TCNT1 == OCR1B, OC1C is not used.
            //TCCR1A = 0b00 10 00 11    (OCA, OCB, OCC, WGM)
            byte TCCR1A = 0x23;
            //TCCR1B = 0b000 11 010     (ICP WGM clockSelect)   
            byte TCCR1B = 0x1A;
            byte TCCR1C = 0x00;

            UInt16 OCR1A = (UInt16)(clockSpeed_Hz / 8000000 * period_us); //register top
            UInt16 OCR1B = (UInt16)(clockSpeed_Hz / 8000000 * pulse_us);

            byte TIMSK = 0x00;
  
            timer_setAllRegisters(TIMSK, 0x00, OCR1A, OCR1B, 0x0000, TCCR1A, TCCR1B, TCCR1C);
        }

        public void timer_setServoSweep(ushort pulseEnd_us, byte increment_us)
        {
            byte[] data = {(byte)(pulseEnd_us << 8), (byte)(pulseEnd_us), increment_us};
            msgSend(section.timers, (byte)timerInstr.setServoSweep, data);
        }

        /// <summary>
        /// Disables all functions of the timer and disconnects output compare pins
        /// </summary>
        public void timer_disable()
        {
            timer_setControlRegisters(0x00, 0x00, 0x00);
        }

        //For all messages for the timer section:
        //timerID, data
        /// <summary>
        /// Advanced - Allows all functions of the timer subsystem to be configured. Refer to AT90USB162 datasheet for details
        /// </summary>
        public void timer_setControlRegisters(byte TCCRA, byte TCCRB, byte TCCRC)
        {
            byte[] data = {0x01, TCCRA, TCCRB, TCCRC };
            msgSend(section.timers, (byte)timerInstr.setControlRegs, data);
        }
        /// <summary>
        /// Advanced - Set individual values of output compare registers. Refer to AT90USB162 datasheet for details
        /// </summary>
        public void timer_setOutputCompareRegisters(UInt16 OCRA, UInt16 OCRB, UInt16 OCRC)
        {
            byte[] data = { 0x01, 
                              (byte)(OCRA & 0xFF),
                              (byte)(OCRA >> 8),
                              (byte)(OCRB & 0xFF),
                              (byte)(OCRB >> 8),
                              (byte)(OCRC & 0xFF),
                              (byte)(OCRC >> 8)};
            msgSend(section.timers, (byte)timerInstr.setOCRs, data);
        }

        /// <summary>
        /// Advanced - Allows all functions of the timer subsystem to be configured. Refer to AT90USB162 datasheet for details
        /// </summary>
        public void timer_setMasksAndFlags(byte TIMSK, byte TIFR)
        {
            byte[] data = { 0x01, TIMSK, TIFR };
            msgSend(section.timers, (byte)timerInstr.setMaskAndFlags, data);
        }

        /// <summary>
        /// sets the value of TCNT1 to zero
        /// </summary>
        public void timer_clearTimerValue()
        {
            timer_setTimerValue(0);
        }

        /// <summary>
        /// Sets a seed value into the timer register
        /// </summary>
        /// <param name="value">16bit seed value</param>
        public void timer_setTimerValue(UInt16 value)
        {
            byte[] b = BitConverter.GetBytes(value);
            internal_genericMemoryWrite(0x84, b);
        }

        /// <summary>
        /// Reads the current value of TCNT1
        /// </summary>
        /// <returns>UInt16 value of TCNT0 and TCNT1</returns>
        public UInt16 timer_getTimerValue()
        {
            byte[] b = new byte[2];
            internal_genericMemoryRead(0x84, b.Length, out b);
            return BitConverter.ToUInt16(b, 0);
        }

        private void timer_setAllRegisters(byte TIMSK, byte TIFR, UInt16 OCRA, UInt16 OCRB, UInt16 OCRC, byte TCCRA, byte TCCRB, byte TCCRC)
        {
            byte[] data = { 0x01, 
                              TIMSK,
                              TIFR,
                              TCCRA,
                              TCCRB,
                              TCCRC,
                              (byte)(OCRA & 0xFF),
                              (byte)(OCRA >> 8),
                              (byte)(OCRB & 0xFF),
                              (byte)(OCRB >> 8),
                              (byte)(OCRC & 0xFF),
                              (byte)(OCRC >> 8)};
            msgSend(section.timers, (byte)timerInstr.setAllRegisters, data);
        }

        #endregion

        //--------------------- LCD ---------------------------
        #region LCD

        private enum LCD_instr : byte
        {
            write = 0x10, init = 0x12, clear = 0x13
        }

        /// <summary>
        /// Initialises the LCD screen, this method must be called before the screen can be written to
        /// The LCD port is fixed on port D
        /// </summary>
        public void LCD_init()
        {
            GPIO_setDirPort('D', 0xFF, 0xFF);
            msgSend(section.LCD, (byte)LCD_instr.init);
        }

        /// <summary>
        /// Clears the LCD screen and sets the cursor back to (0,0)
        /// </summary>
        public void LCD_clear()
        {
            msgSend(section.LCD, (byte)LCD_instr.clear);
        }

        /// <summary>
        /// Writes a string to the LCD, any text going over the end of the first line will wrap onto the second line
        /// </summary>
        /// <param name="s">The ASCII string to write to the display</param>
        public void LCD_write(string s)
        {
            byte[] b = new byte[s.Length + 1];
            byte[] stringBytes = utilities.utilities.StrToByteArray(s);

            //first byte written is the line number 0x80 or 0xC0, lines 1 and 2 respectivly
            b[0] = 0x80;
            Array.Copy(stringBytes, 0, b, 1, stringBytes.Length);
            LCD_write(b);
        }

        public enum LCD_line : byte {line0 = 0x80, line1 = 0xC0};

        /// <summary>
        /// Writes a string to the specified location on the LCD screen
        /// </summary>
        /// <param name="s">String to write to the LCD</param>
        /// <param name="line">Line number, 0 or 1</param>
        /// <param name="x_index">X-location on the screen (0 is far left)</param>
        public void LCD_write(string s, LCD_line line, int x_index)
        {
            x_index = utilities.utilities.coerce(x_index, 40, 0);

            byte[] b = new byte[s.Length + 1];
            b[0] = (byte)((byte)line + x_index);
            Array.Copy(utilities.utilities.StrToByteArray(s), 0, b, 1, s.Length);
            LCD_write(b);
        }

        /// <summary>
        /// Writes a byte array to the specified location on the LCD screen
        /// </summary>
        /// <param name="dataToWrite">Bytes to write to the LCD</param>
        /// <param name="line">Line number, 0 or 1</param>
        /// <param name="x_index">X-location on the screen (0 is far left)</param>
        public void LCD_write(byte[] dataToWrite, LCD_line line, int x_index)
        {
            x_index = utilities.utilities.coerce(x_index, 40, 0);

            byte[] b = new byte[dataToWrite.Length + 1];
            b[0] = (byte)((byte)line + x_index);
            Array.Copy(dataToWrite, 0, b, 1, dataToWrite.Length);
            LCD_write(b);
        }

        /// <summary>
        /// Writes a byte array to the display, use this to specify a particular character set manually. The first character must define the line number
        /// </summary>
        /// <param name="dataToWrite">The byte array to write, specific to your LCD</param>
        public void LCD_write(byte[] dataToWrite)
        {
            //Array.Resize(ref dataToWrite, dataToWrite.Length + 1);
            //dataToWrite[dataToWrite.Length] = (byte)dataToWrite.Length;

            msgSend(section.LCD, (byte)LCD_instr.write, dataToWrite);

        }

        #endregion

        //--------------------- LED ---------------------------
        #region LED
        /*
        public enum LED_state : byte
        {
            Off = 0x10,
            Solid = 0x11,
        }

        public void LED_off()
        {
            GPIO_writePort(LED_PORT, LED_PIN, LED_PIN);
        }

        public void LED_solid()
        {
            GPIO_writePort(LED_PORT, (byte)(0xFF - LED_PIN), LED_PIN);
        }*/
        #endregion 

        //--------------------- PSU ---------------------------
        #region PSU
        /*
        public bool PSU_on
        {
            set
            {
                if (value)
                {
                    GPIO_setDirPort(PSU_PORT, 0xFF, PSU_PIN);
                    GPIO_writePort(PSU_PORT, PSU_PIN, PSU_PIN);
                }
                else
                    GPIO_writePort(PSU_PORT, (byte)(0xFF - PSU_PIN), PSU_PIN);
                _PSU_on = value;
            }
            get
            {
                return _PSU_on;
            }
        }

        public void PSU_off()
        {
            GPIO_writePort(LED_PORT, LED_PIN, LED_PIN);
        }

        public void PSU_solid()
        {
            GPIO_writePort(LED_PORT, (byte)(0xFF - LED_PIN), LED_PIN);
        }*/
        #endregion

        //--------------------- Internal ----------------------
        #region Internal

        private enum internal_Inst : byte
        {
            getDeviceInfo = 0x20, getStatus = 0x21, genericMemoryWrite = 0x22, genericMemoryRead = 0x23
        }

        /// <summary>
        /// Gets the identification information contained in the attached IC
        /// </summary>
        /// <returns>A struct with the specified data</returns>
        public deviceInfo internal_getDeviceInfo()
        {
            byte[] b;
            msgSendGetReply(section.internalCalls, (byte)internal_Inst.getDeviceInfo, out b);

            deviceInfo D = new deviceInfo();
            byte tableVersion = b[0];
            if (tableVersion > maxDeviceInfoTableVersion)
                throw new Exception("USB device requires a newer version of the library to display the device info");
            
            //only version 1 is currently supported
            D.firmwareVersion = b[3].ToString() + "." + b[2].ToString();
            D.prodID = (productID)b[4];
            D.description = productDescriptions[b[4]];
            D.serialNumber = (ushort)usbI.productInfo.serialNumber;
            
            return D;
        }

        /// <summary>
        /// Advanced - Writes an array of bytes to any memory register. Use with caution!
        /// </summary>
        /// <param name="address">8bit address of the memory location to begin writing from</param>
        /// <param name="data">data bytes to write to the given memory location, and the following locations (max 32)</param>
        public void internal_genericMemoryWrite(byte address, byte[] data)
        {
            int numBytes = data.Length;
            if (numBytes > 32)
                numBytes = 32;

            byte[] b = new byte[numBytes + 2];
            b[0] = address;
            b[1] = (byte)numBytes;
            Array.Copy(data, 0, b, 2, numBytes);

            msgSend(section.internalCalls, (byte)internal_Inst.genericMemoryWrite, b);
        }

        /// <summary>
        /// Advanced - Reads an array of bytes from any memory register
        /// </summary>
        /// <param name="address">8bit address of the memory location to begin reading from</param>
        /// <param name="data">data bytes read from the given memory location, and the following locations (max 32)</param>
        public void internal_genericMemoryRead(byte address, int numBytes, out byte[] data)
        {
            if (numBytes > 32)
                numBytes = 32;

            byte[] b = { address, (byte)numBytes};
            msgSendGetReply(section.internalCalls, (byte)internal_Inst.genericMemoryRead, b, out data);
        }

        #endregion

        //--------------------- EEPROM ------------------------
        #region EEPROM_0x31

        private enum EEPROM_Instr : byte { write = 0x10, read = 0x11 }

        /// <summary>
        /// Writes data to the EEPROM. 512 bytes are available
        /// Maximum number of bytes to be written at a time is limited by the HID report size to 57 bytes
        /// </summary>
        /// <param name="bytesToWrite">The data bytes to write</param>
        /// <param name="address">The starting adress from which the bytes will be written. Valid 0-511</param>
        public void EEPROM_write(byte[] bytesToWrite, UInt16 address)
        {
            if ((address + bytesToWrite.Length - 1) > 511)
                throw new Exception("EEPROM: Write address must be in the range 0-511");

            byte[] data = new byte[bytesToWrite.Length + 2];
            data[0] = (byte)(address & 0xFF);
            data[1] = (byte)(address >> 8);
            Array.Copy(bytesToWrite, 0, data, 2, bytesToWrite.Length);

            msgSend(section.EEPROM,(byte)EEPROM_Instr.write, data);
            
            //Put in a delay after an eeprom write 
            System.Threading.Thread.Sleep(bytesToWrite.Length * 2);
        }

        /// <summary>
        /// Read data from the EEPROM. 512 bytes are available
        /// </summary>
        /// <param name="address">The starting adress from which the bytes will be read. Valid 0-511</param>
        /// <param name="numBytesToRead">The number of bytes required - max 255</param>
        /// <returns>Data read from the EEPROM</returns>
        public byte[] EEPROM_read(UInt16 address, int numBytesToRead)
        {
            if ((address + numBytesToRead - 1) > 511)
                throw new Exception("EEPROM: Read address must be in the range 0-511");

            byte[] data = new byte[3];
            data[0] = (byte)(address & 0xFF);
            data[1] = (byte)(address >> 8);
            data[2] = (byte)numBytesToRead;

            byte[] inData = new byte[numBytesToRead];

            msgSendGetReply(section.EEPROM, (byte)EEPROM_Instr.read, data, out inData);

            return inData;
        }

        #endregion
    }
}