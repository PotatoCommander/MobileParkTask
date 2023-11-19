namespace MobileParkTask.Models;

public class SensorData
{
    public int EmulatorId { get; set; }

    public long UnixTime { get; set; }

    public byte SensorType { get; set; }

    public double SensorValue { get; set; }
}