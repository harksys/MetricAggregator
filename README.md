# Metric Aggregator for Azure Application Insights

Metric Aggerator gives you a convenient way to send metrics while reducing bandwidth.

It aggregates your metrics in your app, sending the aggregated statistics to the portal at intervals of one minute.

When dealing with systems that perform many operations per minute, maintaining visible telemetry and sending individual metrics up to  application insights can be costly on your bandwidth and ultiamtely your bill.

Aggregation of metrics is a useful way to maintain in-depth visibility in an efficient fashion and the Metric Aggregator does this for you. 

## Counting and Averaging

The Metric Aggregator has support for both counting and averaging, this makes it easier to track telemetry based on time duration.

## Example

```c#

// Counting occurences of tasks
var counter = MetricAggregator.CreateMetric($"Task-Count");
counter.TrackMetric(1.0);

// Tracking average duration of task
var time = MetricAggregator.CreateMetric("Task-Duration", true); // set the useAverage paramater to true for average mode
time.TrackMetric(durationMilliseconds);
```

## Application Insights Configuration

The Metric Aggregator uses the TelemetryClient and will work automatically providing you have setup your ApplicationInsights.config with your Instrumentation Key in your project.
