using ChatCommon.Abstractions;
using ChatCommon.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ChatApp
{
    public class UdpMessageSourceClient : IMessageSourceClient<IPEndPoint>
    {
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _udpEndPoint;
        public UdpMessageSourceClient(string Ip = "172.0.0.1", int port = 0)
        {
            _udpClient = new UdpClient(5527);
            _udpEndPoint = new IPEndPoint(IPAddress.Parse(Ip), port);
        }

        public IPEndPoint CreateEndpoint()
        {
            return new IPEndPoint(IPAddress.Any, 0);
        }

        public IPEndPoint GetServer()
        {
            return _udpEndPoint;
        }

        public NetMessage Receive(ref IPEndPoint ep)
        {
            byte[] data = _udpClient.Receive(ref ep);
            string str = Encoding.UTF8.GetString(data);
            return NetMessage.DeserializeMessgeFromJSON(str) ?? new NetMessage();
        }

        public async Task SendAsync(NetMessage message, IPEndPoint ep)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message.SerialazeMessageToJSON());

            await _udpClient.SendAsync(buffer, buffer.Length, ep);
        }
    }
}
