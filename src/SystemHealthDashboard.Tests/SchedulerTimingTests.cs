using SystemHealthDashboard.Metrics.Models;
using SystemHealthDashboard.Metrics.Schedulers;
using Xunit;

namespace SystemHealthDashboard.Tests;

public class SchedulerTimingTests
{
    [Fact]
    public void MetricScheduler_CollectsMetricsAtInterval()
    {
        int collectCount = 0;
        var scheduler = new MetricScheduler<MetricData>(
            () =>
            {
                Interlocked.Increment(ref collectCount);
                return new MetricData(collectCount);
            },
            intervalMs: 100,
            historySize: 10
        );

        scheduler.Start();
        Thread.Sleep(450); // Should collect ~4 times (0ms, 100ms, 200ms, 300ms, 400ms)
        scheduler.Stop();

        Assert.InRange(collectCount, 3, 6); // Allow some tolerance for timing
    }

    [Fact]
    public void MetricScheduler_StoresHistory()
    {
        int counter = 0;
        var scheduler = new MetricScheduler<MetricData>(
            () => new MetricData(++counter),
            intervalMs: 50,
            historySize: 5
        );

        scheduler.Start();
        Thread.Sleep(300); // Collect several metrics
        scheduler.Stop();

        var history = scheduler.GetHistory();
        Assert.NotEmpty(history);
        Assert.True(history.Count <= 5); // Respects history size
    }

    [Fact]
    public void MetricScheduler_UpdatesCurrentMetric()
    {
        var scheduler = new MetricScheduler<MetricData>(
            () => new MetricData(DateTime.Now.Millisecond),
            intervalMs: 50
        );

        scheduler.Start();
        Thread.Sleep(150);
        
        var currentMetric = scheduler.GetCurrentMetric();
        Assert.NotNull(currentMetric);
        
        scheduler.Stop();
    }

    [Fact]
    public void MetricScheduler_RaisesMetricUpdatedEvent()
    {
        int eventCount = 0;
        var scheduler = new MetricScheduler<MetricData>(
            () => new MetricData(42),
            intervalMs: 50
        );

        scheduler.MetricUpdated += (sender, metric) =>
        {
            Interlocked.Increment(ref eventCount);
        };

        scheduler.Start();
        Thread.Sleep(200);
        scheduler.Stop();

        Assert.True(eventCount >= 2);
    }

    [Fact]
    public void MetricScheduler_CanStartAndStop()
    {
        var scheduler = new MetricScheduler<MetricData>(
            () => new MetricData(1),
            intervalMs: 50
        );

        scheduler.Start();
        Thread.Sleep(100); // Wait for at least one collection
        var currentMetric = scheduler.GetCurrentMetric();
        scheduler.Stop();

        Assert.NotNull(currentMetric);
        Assert.Equal(1, currentMetric.Value);
    }

    [Fact]
    public void RingBuffer_MaintainsSize()
    {
        var buffer = new RingBuffer<int>(3);
        
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);
        buffer.Add(4);
        buffer.Add(5);

        var items = buffer.GetAll();
        Assert.Equal(3, items.Count);
        Assert.Contains(3, items);
        Assert.Contains(4, items);
        Assert.Contains(5, items);
    }

    [Fact]
    public void RingBuffer_PreservesOrder()
    {
        var buffer = new RingBuffer<int>(5);
        
        buffer.Add(10);
        buffer.Add(20);
        buffer.Add(30);

        var items = buffer.GetAll();
        Assert.Equal(3, items.Count);
        Assert.Equal(10, items[0]);
        Assert.Equal(20, items[1]);
        Assert.Equal(30, items[2]);
    }
}
