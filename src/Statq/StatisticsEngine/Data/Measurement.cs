using System.Diagnostics;

namespace Statq.StatisticsEngine.Data;

[DebuggerDisplay("{MeasurementType} {MeasurementTime} {MeasurementValue}")]
public class Measurement
{
    public DateTime MeasurementTime { get; init; }
    public double MeasurementValue { get; init; }
    public MeasurementType MeasurementType { get; init; }
}