using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetProgramm
{
    internal class UDPServer
    {
        public async Task ServerListenerAsync()
        {
            UdpClient udpClient = new UdpClient(45512);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);            
            Console.WriteLine("Сервер ждет сообщение от клиента");

            CancellationTokenSource cts = new CancellationTokenSource();
            bool canWork = true;
            while (!cts.IsCancellationRequested)
            {
                byte[] buffer = udpClient.Receive(ref iPEndPoint);

                var messageTxt = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($"получено {buffer.Length} байт") ;
                                
                byte[] reply = Encoding.UTF8.GetBytes("Сообщение получено");
                
                int bytes = await udpClient.SendAsync(reply, iPEndPoint);
                Console.WriteLine($"отправлено {bytes} байт");

                Message? message = Message.DeserializeMessgeFromJSON(messageTxt);
                if(message.Text.ToLower().Equals("exit") ) cts.Cancel();
                message.PrintGetMessageFrom();
            }
        }
    }
}
