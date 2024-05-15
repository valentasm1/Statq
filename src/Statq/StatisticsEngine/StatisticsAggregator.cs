using Statq.StatisticsEngine.Data;

namespace Statq.StatisticsEngine;

public class StatisticsAggregator : IStatisticsAggregator
{
    private const int SamplingIntervalInMinutes = 5;

    public Dictionary<MeasurementType, List<Measurement>> Sample(DateTime startOfSampling, List<Measurement> unsampledMeasurements)
    {
        var result = new Dictionary<MeasurementType, List<Measurement>>();
        var byType = unsampledMeasurements
            .GroupBy(m => m.MeasurementType)
            .ToDictionary(x => x.Key, y => y.ToList());

        foreach (var unsampledMeasurement in byType)
        {
            var sampled = Sample(unsampledMeasurement.Value.Where(x => x.MeasurementTime > startOfSampling).ToList());
            result.Add(unsampledMeasurement.Key, sampled);
        }

        return result;
    }

    private List<Measurement> Sample(ICollection<Measurement> unsampledMeasurementValues)
    {
        if (unsampledMeasurementValues.Count == 0)
        {
            return new List<Measurement>();
        }

        var byTime = unsampledMeasurementValues
            .GroupBy(x => ToSampleDate(x.MeasurementTime))
            .ToDictionary(x => x.Key, y => y.ToList());

        var result = new List<Measurement>();
        foreach (var byMinutePair in byTime)
        {
            var measurement = byMinutePair.Value.OrderBy(x => x.MeasurementTime).Last();
            result.Add(new Measurement()
            {
                MeasurementTime = byMinutePair.Key,
                MeasurementType = measurement.MeasurementType,
                MeasurementValue = measurement.MeasurementValue
            });
        }

        return result.OrderBy(x => x.MeasurementTime).ToList();
    }

    private static DateTime ToSampleDate(DateTime date)
    {
        var isExactMinute = date.Minute % SamplingIntervalInMinutes == 0;
        if (isExactMinute && date.Second == 0)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0);
        }

        var nextMinute = date.Minute - date.Minute % SamplingIntervalInMinutes + SamplingIntervalInMinutes;
        var result = new DateTime(date.Year, date.Month, date.Day, date.Hour, nextMinute, 0, 0);
        return result;
    }
}