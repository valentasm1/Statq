using Statq.StatisticsEngine.Data;

namespace Statq.StatisticsEngine;

public interface IStatisticsAggregator
{
    Dictionary<MeasurementType, List<Measurement>> Sample(DateTime startOfSampling,
        List<Measurement> unsampledMeasurements);
}