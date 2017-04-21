// <copyright file="MetricAggregator.cs" company="Hark.">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MetricAggregator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Metric Aggregator.
    /// </summary>
    /// <remarks>
    /// Application Insights will eventually have this built in.
    /// </remarks>
    public class MetricAggregator : IDisposable
    {
        /// <summary>
        /// Aggrecation Interval in Seconds.
        /// </summary>
        private const int IntervalSeconds = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricAggregator"/> class.
        /// </summary>
        /// <param name="name">The Metric Name.</param>
        /// <param name="useAverage">A value indicating whether the aggregator should use the average of the values.</param>
        public MetricAggregator(string name, bool useAverage = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;

            this.UseAverage = useAverage;

            Task.Factory.StartNew(
                async () =>
                {
                    while (this.Running)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds));

                        var metric = this.Aggregate();
                        if (metric == null)
                        {
                            continue;
                        }

                        this.TelemetryClient.TrackMetric(metric);
                    }
                }, TaskCreationOptions.LongRunning);
        }

        private TelemetryClient TelemetryClient { get; } = new TelemetryClient();

        private List<double> Telemetry { get; } = new List<double>();

        private bool Running { get; set; } = true;

        private string Name { get; }

        private bool UseAverage { get; }

        /// <summary>
        /// Track Metric.
        /// </summary>
        /// <param name="value">The Metric Value.</param>
        public void TrackMetric(double value)
        {
            lock (this)
            {
                this.Telemetry.Add(value);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Running = false;
        }

        private MetricTelemetry Aggregate()
        {
            lock (this)
            {
                if (!this.Telemetry.Any())
                {
                    return null;
                }

                var mean = this.Telemetry.Average();
                var variance = this.Telemetry.Sum(v => Math.Pow(v - mean, 2)) / this.Telemetry.Count;

                var metric = new MetricTelemetry
                {
                    Timestamp = DateTime.UtcNow,
                    Name = this.Name,
                    Count = this.Telemetry.Count,
                    Max = this.Telemetry.Max(),
                    Min = this.Telemetry.Min(),
                    StandardDeviation = Math.Sqrt(variance),
                    Sum =
                        this.UseAverage
                            ? mean
                            : this.Telemetry.Count,
                };

                this.Telemetry.Clear();

                return metric;
            }
        }
    }
}
