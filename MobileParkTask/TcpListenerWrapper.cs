using System.Net.Sockets;
using MobileParkTask.Helpers;
using MobileParkTask.Models;

namespace MobileParkTask;

public class TcpListenerWrapper
{
    public string Url { get; }

    public int Port { get; }

    public List<SensorData> SensorDataList { get; } = new List<SensorData>();

    public bool IsRunning { get; private set; }

    private readonly int _averageCount;

    private TcpClient _tcpClient;

    private NetworkStream _stream;

    private object _locker = new();

    public TcpListenerWrapper(string url, int port, int averageCount)
    {
        Url = url;
        Port = port;
        _averageCount = averageCount;
    }

    public async Task StartAsync()
    {
        try
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(Url, Port);
            _stream = _tcpClient.GetStream();
            IsRunning = true;

            Console.WriteLine($"Connected to: {Url} : {Port}");
            await DecodeData();
        }
        catch (Exception)
        {
            Console.WriteLine($"Unable to connect to: {Url} : {Port}");
        }
    }

    public void Stop()
    {
        IsRunning = false;
        _tcpClient.Close();
        Console.WriteLine("Transmission stopped.");
    }

    public void GetInfo()
    {
        Console.WriteLine("Average overall:");
        ShowAverage(SensorDataList.Count);
    }

    public void GetStatus()
    {
        Console.WriteLine(
            $"Connection Status: {_tcpClient.Connected}, number of sensor values: {SensorDataList.Count}");
    }

    private void ShowAverage(int n)
    {
        var averageValues = AverageHelper.GetAverages(SensorDataList, n);

        Console.Write("-AVG- ");
        foreach (var value in averageValues)
        {
            Console.Write($"SensorType: {value.SensorType}, Avr: {Math.Round(value.Average, 2)}; ");
        }

        Console.WriteLine("\n");
    }

    private async Task DecodeData()
    {
        while (IsRunning)
        {
            try
            {
                var lengthBytes = new byte[2];
                await _stream.ReadAsync(lengthBytes, 0, 2);
                var batchLength = BitConverter.ToUInt16(lengthBytes, 0);

                var dataBuffer = new byte[batchLength];
                await _stream.ReadAsync(dataBuffer, 0, batchLength);


                using var ms = new MemoryStream(dataBuffer);
                using var reader = new BinaryReader(ms);

                var unixTime = reader.ReadInt64();
                var emulatorId = reader.ReadInt32();

                while (ms.Position < ms.Length)
                {
                    var sensorType = reader.ReadByte();
                    var sensorValue = reader.ReadDouble();

                    Console.WriteLine(
                        $"----- Sensor Type: {sensorType}, Value: {sensorValue}, Id: {emulatorId}, Time: {DateTimeOffset.FromUnixTimeSeconds(unixTime)}");

                    lock (_locker)
                    {
                        SensorDataList.Add(new SensorData()
                        {
                            EmulatorId = emulatorId,
                            UnixTime = unixTime,
                            SensorType = sensorType,
                            SensorValue = sensorValue,
                        });

                        if (SensorDataList.Count % 3 == 0)
                        {
                            ShowAverage(_averageCount);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error during data decoding.");
            }
        }
    }
}