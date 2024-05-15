using Statq.StatisticsEngine;
using Statq.StatisticsEngine.Data;

namespace Statq.Unit.Tests;

public class StatisticsAggregatorTests
{

    [Fact]
    public void No_Data_Should_Not_Crash()
    {
        var agregator = GetStatisticsAggregator();
        var startTime = new DateTime(2017, 01, 03);
        var data = new List<Measurement>();

        var result = agregator.Sample(startTime, data);
        Assert.Empty(result);
    }

    [Fact]
    public void Two_Exact_Time_But_Different_Hours_Should_Create_Separate()
    {
        var requiredValue1 = 35.22;
        var requiredValue2 = 98.78;
        var agregator = GetStatisticsAggregator();
        var startTime = new DateTime(2017, 01, 03);
        var data = new List<Measurement>
        {
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 10, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = requiredValue1
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 12, 10, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = requiredValue2
            },
        };
        data.Shuffle();

        var result = agregator.Sample(startTime, data);
        Assert.Single(result.Values);
        var values = result.Values.Single();
        Assert.Equal(2, values.Count);
        Assert.Equal(requiredValue1, values.First().MeasurementValue);
        Assert.Equal(requiredValue2, values.Second().MeasurementValue);
    }

    [Fact]
    public void Two_Exact_End_Date_Times_Exist_Should_Take_One()
    {
        var requiredValue1 = 35.22;
        var requiredValue2 = 98.78;
        var agregator = GetStatisticsAggregator();
        var startTime = new DateTime(2017, 01, 03);
        var data = new List<Measurement>
        {
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 04, 45),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 35.79
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 08, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 80.14
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 10, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = requiredValue1
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 10, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = requiredValue2
            },
        };
        data.Shuffle();

        var result = agregator.Sample(startTime, data);
        Assert.Single(result.Values);
        var values = result.Values.Single();
        Assert.Equal(2, values.Count);
        var resultValue = values.Last();
        Assert.True(resultValue.MeasurementValue == requiredValue1 || resultValue.MeasurementValue == requiredValue2);
    }

    [Fact]
    public void Validate_If_Takes_Measurements_When_Ends_On_Limit()
    {
        var requiredValue = 98.78;
        var agregator = GetStatisticsAggregator();
        var startTime = new DateTime(2017, 01, 03);
        var data = new List<Measurement>
        {
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 04, 45),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 35.79
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 08, 45),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 35.79
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 10, 00),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = requiredValue
            },
        };
        data.Shuffle();

        var result = agregator.Sample(startTime, data);
        Assert.Single(result.Values);
        var values = result.Values.Single();
        Assert.Equal(2, values.Count);
        var resultValue = values.Last();
        Assert.Equal(requiredValue, resultValue.MeasurementValue);
    }

    [Fact]
    public void Validate_If_Takes_Latest_Measurements()
    {
        var agregator = GetStatisticsAggregator();
        var startTime = new DateTime(2017, 01, 03);
        var data = new List<Measurement>
        {
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 04, 45),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 35.79
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 01, 18),
                MeasurementType = MeasurementType.SpO2,
                MeasurementValue = 98.78
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 09, 07),
                MeasurementType =  MeasurementType.Temp ,
                MeasurementValue = 35.01
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 03, 34),
                MeasurementType = MeasurementType.SpO2,
                MeasurementValue = 96.49
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 02, 01),
                MeasurementType = MeasurementType.Temp,
                MeasurementValue = 35.82
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 05, 00),
                MeasurementType = MeasurementType.SpO2,
                MeasurementValue = 97.17
            },
            new Measurement
            {
                MeasurementTime = new DateTime(2017, 01, 03, 10, 05, 01),
                MeasurementType = MeasurementType.SpO2,
                MeasurementValue = 95.08
            }
        };
        data.Shuffle();

        var result = agregator.Sample(startTime, data);
        Assert.Equal(2, result.Count);
        var tempValues = result[MeasurementType.Temp];
        Assert.Equal(2, tempValues.Count);
        Assert.Equal(new DateTime(2017, 01, 03, 10, 05, 0, 0), tempValues.First().MeasurementTime);
        Assert.Equal(35.79, tempValues.First().MeasurementValue);
        Assert.Equal(35.01, tempValues.Second().MeasurementValue);
        Assert.Equal(new DateTime(2017, 01, 03, 10, 10, 0, 0), tempValues.Second().MeasurementTime);

        var spo2Values = result[MeasurementType.SpO2];
        Assert.Equal(2, spo2Values.Count);
        Assert.Equal(97.17, spo2Values.First().MeasurementValue);
        Assert.Equal(new DateTime(2017, 01, 03, 10, 05, 0, 0), spo2Values.First().MeasurementTime);
        Assert.Equal(95.08, spo2Values.Second().MeasurementValue);
        Assert.Equal(new DateTime(2017, 01, 03, 10, 10, 0, 0), spo2Values.Second().MeasurementTime);

    }

    private IStatisticsAggregator GetStatisticsAggregator()
    {
        return new StatisticsAggregator();
    }
}