using System.Net.Sockets;
using System.Net;
using ChatCommon.Abstractions;
using ChatCommon.Models;

namespace ChatApp

{
    public class Client<T>
    {
        public readonly string _name;
       
        public IMessageSourceClient<T> _messageSource;
        public T remoteEndPoint;
        public Client(IMessageSourceClient<T> messageSourceClient, string name)
        {
            this._name = name;          

            _messageSource = messageSourceClient;
            remoteEndPoint = _messageSource.CreateEndpoint();
        }

        public UdpClient udpClientClient = new UdpClient();
        public async Task ClientListener()
        {
            while (true)
            {
                try
                {
                    var messageReceived = _messageSource.Receive(ref remoteEndPoint);

                    Console.WriteLine($"Получено сообщение от {messageReceived.NickNameFrom}:");
                    Console.WriteLine(messageReceived.Text);

                    await Confirm(messageReceived, remoteEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при получении сообщения: " + ex.Message);
                }
            }
        }

        public async Task Confirm(NetMessage message, T remoteEndPoint)
        {
            message.Command = Command.Confirmation;
            await _messageSource.SendAsync(message, remoteEndPoint);
        }


        public void Register(T remoteEndPoint)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Text = null, Command = Command.Register, EndPoint = ep };
            _messageSource.SendAsync(message, remoteEndPoint);
            Console.WriteLine("Мы тута");
        }

        public async Task ClientSender()
        {


            Register(remoteEndPoint);

            while (true)
            {
                try
                {
                    Console.Write("Введите  имя получателя: ");
                    var nameTo = Console.ReadLine();
                   
                    Console.Write("Введите сообщение и нажмите Enter: ");
                    var messageText = Console.ReadLine();

                    var message = new NetMessage() { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = messageText };

                    await _messageSource.SendAsync(message, remoteEndPoint);

                    Console.WriteLine("Сообщение отправлено.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        public async Task Start()
        {
            //udpClientClient = new UdpClient(port);

            new Thread(async () => await ClientListener()).Start();

            await ClientSender();

        }
    }

}
