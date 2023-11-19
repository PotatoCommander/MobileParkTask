using System.Net.Sockets;

namespace MobileParkTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string url = "127.0.0.1";
            const int port = 5000;
            var listener = new TcpListener(url, port);

            Console.WriteLine("Enter the command (start, stop, info, statistics)");
            while (true)
            {
                var command = Console.ReadLine()?.ToLower();
                switch (command)
                {
                    case "start":
                        await listener.StartAsync();
                        break;
                    case "stop":
                        break;
                    case "info":
                        break;
                    case "statistics":
                        break;
                    default:
                        Console.WriteLine("Command do not exist!");
                        break;
                }
            }
        }
    }

    class SensorData
    {
        public byte SensorType { get; set; }
        public double SensorValue { get; set; }
    }

    public class TcpListener
    {
        public string Url { get; }

        public int Port { get; }

        private readonly TcpClient _tcpClient;

        private NetworkStream _stream;

        public TcpListener(string url, int port)
        {
            Url = url;
            Port = port;
            _tcpClient = new TcpClient();
        }

        public async Task StartAsync()
        {
            try
            {
                await _tcpClient.ConnectAsync(Url, Port);
                _stream = _tcpClient.GetStream();
                Console.WriteLine($"Connected to: {Url} : {Port}");
                ReceiveData();
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to connect to: {Url} : {Port}");
            }
        }

        public void Stop()
        {

        }

        public void Info()
        {

        }

        public void Statistics()
        {

        }

        private void DecodeData()
        {

        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    using var reader = new BinaryReader(_stream);
                    int messageLength = BitConverter.ToUInt16(reader.ReadBytes(2), 0);
                    var unixTime = reader.ReadInt64();
                    var emulatorId = reader.ReadInt32();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}