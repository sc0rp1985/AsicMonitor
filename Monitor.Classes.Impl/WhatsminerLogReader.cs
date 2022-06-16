using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Monitor.Classes.Impl
{
    public class WhatsminerLogReader : IMinerLogReader
    {
        public async Task<string> ReadLog(string ip, int port, LogMode mode)
        {
            var cmd = mode == LogMode.Summary ? "summary" : "devs";
            var logstr = await ReadLogFromMiner(ip, port, cmd);
            return logstr;
        }

        async Task<string> ReadLogFromMiner(string ip, int port, string cmd)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            using var tempSocket =
                   new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
               await tempSocket.ConnectAsync(ipe);
            }
            catch (Exception ex)
            {                
                throw new ApplicationException($"Ошибка соединения",ex);
            }
            Byte[] bytesReceived = new Byte[256];
            int bytes = 0;
            string page = "";
            if (tempSocket.Connected)
            {
                await tempSocket.SendAsync(Encoding.ASCII.GetBytes(cmd),0);
                do
                {
                    //bytes = tempSocket.Receive(bytesReceived, bytesReceived.Length, SocketFlags.None);
                    bytes = await tempSocket.ReceiveAsync(bytesReceived, SocketFlags.None);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);                
                return page;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}