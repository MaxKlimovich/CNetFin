
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Abstracts;
using UnitTests.Models;

namespace UnitTests.Services
{
    public class Server 
    {
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        private readonly IMessageSource _messageSouce;
        private IPEndPoint ep;
        public Server(IMessageSource messageSouce)
        {
            _messageSouce = messageSouce;
            ep = new IPEndPoint(IPAddress.Any, 0);
        }

        bool work = true;
        public void Stop()
        {
            work = false;
        }

        private async Task Register(NetMessage message)
        {
            Console.WriteLine($" Message Register name = {message.NickNameFrom}");

            if (clients.TryAdd(message.NickNameFrom, message.EndPoint))
            {
                using (ChatContext context = new ChatContext())
                {
                    context.Users.Add(new User() { FullName = message.NickNameFrom });
                    await context.SaveChangesAsync();
                }

            }

        }
        private async Task RelyMessage(NetMessage message)
        {
            if (clients.TryGetValue(message.NickNameTo, out IPEndPoint ep))
            {
                int? id = 0;
                using (var ctx = new ChatContext())
                {
                    var fromUser = ctx.Users.First(x => x.FullName == message.NickNameFrom);
                    var toUser = ctx.Users.First(x => x.FullName == message.NickNameTo);
                    var msg = new Message { UserFrom = fromUser, UserTo = toUser, IsSent = false, Text = message.Text };
                    ctx.Messages.Add(msg);

                    ctx.SaveChanges();

                    id = msg.MessageId;
                }

                message.Id = id;

                await _messageSouce.SendAsync(message, ep);

                Console.WriteLine($"Message Relied, from = {message.NickNameFrom} to = {message.NickNameTo}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }

        }

        async Task ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);

            using (var ctx = new ChatContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.MessageId == id);

                if (msg != null)
                {
                    msg.IsSent = true;
                    await ctx.SaveChangesAsync();
                }
            }
        }

        private async Task ProcessMessage(NetMessage message)
        {
            switch (message.Command)
            {
                case Command.Register: await Register(message); break;
                case Command.Message: await RelyMessage(message); break;
                case Command.Confirmation: await ConfirmMessageReceived(message.Id); break;
            }
        }

        public async Task Start()
        {

            Console.WriteLine("Сервер ожидает сообщения ");

            while (work)
            {
                try
                {
                    var message = _messageSouce.Receive(ref ep);
                    Console.WriteLine(message.ToString());
                    await ProcessMessage(message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }


        }

    }
}
