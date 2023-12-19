using UnitTests.Services;

namespace UnitTests
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args[0].Equals("Server"))
            {
                var netmsgsorce = new UdpMessageSouce();
                var server = new Server(netmsgsorce);
                await server.Start();
            }
            else
            {
                var client = new Client("Vasya", "172.0.0.1", 12345);
                await client.Start();
            }
        }
    }
}