namespace GreatCompany.Data.Analytics;

/// <param name="Values">по одному значению на каждый месяц графика</param>
public record ChartSeries(string Title, decimal[] Values);

/// <param name="Months">первые числа месяцев, ось X</param>
public record ChartData(IReadOnlyList<DateTime> Months, IReadOnlyList<ChartSeries> Series) {
	public static ChartData Empty { get; } = new(Array.Empty<DateTime>(), Array.Empty<ChartSeries>());
}
