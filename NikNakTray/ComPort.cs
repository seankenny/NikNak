using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace NikNakTray
{
    class ComPort
    {
        public void Initialise()
        {
            //SerialPort serialport = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
            SerialPort serialport = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            //serialport.PortName = "COM3";

            if (serialport.IsOpen) serialport.Close();
            // SerialPort.RtsEnable = true; // Request-to-send
            // SerialPort.DtrEnable = true; // Data-terminal-ready
            serialport.ReadTimeout = 15000; // tried this, but didn't help
            serialport.WriteTimeout = 150; // tried this, but didn't help
            

            serialport.DataReceived += SerialPort_DataReceived;
            serialport.Open();
            //try
            //{
            //    string line = serialport.ReadLine();
            //}catch(Exception ex)
            //{
            //    throw ex;
            //}
        }

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Process received data
            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            var bytesRead = sp.BytesToRead;
            while (bytesRead > 0)
            {
                bytesRead = sp.Read(buffer, 0, buffer.Length);

                
            }
            // message has successfully been received
            var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
    }
}
