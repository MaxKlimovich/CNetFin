using System.Net;
using System.Net.NetworkInformation;
using Threads;

//int[] arr1 = {1 ,2 , 5, 6, 88, 12 };
//int[] arr2 = { 1, 2, 5, 6, 88, 12 };

//int sum1 = 0;
//int sum2 = 0;
//Thread tr1 = new Thread((sum1 => sum1 = Calc.GetSum(arr1)));
//Thread tr2 = new Thread((sum2 => sum2 = Calc.GetSum(arr2)));
//tr1.Start();
//tr2.Start();

//tr1.Join(1000);
//tr2.Join(1000);

//Console.WriteLine($"{sum1} + {sum2} = {sum1 + sum2}");

const string sait = "yandex.ru";

IPAddress[] iPAddress = Dns.GetHostAddresses(sait, System.Net.Sockets.AddressFamily.InterNetwork);

Dictionary<IPAddress, long> pings = new();

foreach (var item in iPAddress)
{
    Thread tr = new Thread(() =>
    {
        Ping ping = new Ping();
        PingReply pr = ping.Send(item);
        pings.Add(item, pr.RoundtripTime);
        Console.WriteLine($"{item} {pr.RoundtripTime}");

    });
    tr.Start();
    tr.Join();
}

var minPing = pings.Min(x=>x.Value);
Console.WriteLine();
Console.WriteLine(minPing);