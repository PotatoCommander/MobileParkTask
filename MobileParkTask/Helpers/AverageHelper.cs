using MobileParkTask.Models;

namespace MobileParkTask.Helpers
{
    public static class AverageHelper
    {
        public static List<SensorAverage> GetAverages(IEnumerable<SensorData> sensorDataList, int n)
        {
            var averageValues = sensorDataList
                .GroupBy(x => x.SensorType)
                .Select(x => new SensorAverage()
                {
                    SensorType = x.Key,
                    Average = x
                        .TakeLast(x.Count() < n ? x.Count() : n)
                        .Average(s => s.SensorValue)

                });

            return averageValues.ToList();
        }
    }
}
