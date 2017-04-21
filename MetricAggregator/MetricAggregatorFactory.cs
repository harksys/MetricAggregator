namespace MetricAggregator
{
    public static class MetricAggregatorFactory
    {
        /// <summary>
        /// Create Metric Aggregator.
        /// </summary>
        /// <param name="name">The Metric Name.</param>
        /// <param name="useAverage">A value that indicates whether the values should be averaged when sent.</param>
        /// <returns>The <see cref="MetricAggregator"/>.</returns>
        public static MetricAggregator Create(string name, bool useAverage = false) => new MetricAggregator(name, useAverage);
    }
}
