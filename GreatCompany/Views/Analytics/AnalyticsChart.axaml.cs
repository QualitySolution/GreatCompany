using Avalonia;
using Avalonia.Controls;
using GreatCompany.Data.Analytics;

namespace GreatCompany.Views.Analytics;

/// <summary>линии по месяцам, по одной на каждую серию</summary>
public partial class AnalyticsChart : UserControl {
	public static readonly StyledProperty<ChartData?> ChartProperty =
		AvaloniaProperty.Register<AnalyticsChart, ChartData?>(nameof(Chart));

	public ChartData? Chart { get => GetValue(ChartProperty); set => SetValue(ChartProperty, value); }

	public AnalyticsChart() => InitializeComponent();

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);
		if(change.Property == ChartProperty)
			Redraw();
	}

	void Redraw() {
		var chart = Chart ?? ChartData.Empty;
		var plot = this.plot.Plot;
		plot.Clear();

		var xs = chart.Months.Select(x => x.ToOADate()).ToArray();
		foreach(var series in chart.Series) {
			var line = plot.Add.Scatter(xs, series.Values.Select(v => (double)v).ToArray());
			line.LegendText = series.Title;
		}

		plot.Axes.DateTimeTicksBottom();
		plot.ShowLegend();
		plot.Axes.AutoScale();
		this.plot.Refresh();
	}
}
