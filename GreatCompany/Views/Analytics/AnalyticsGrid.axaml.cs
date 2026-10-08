using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using GreatCompany.Data.Analytics;
using GreatCompany.ViewModels.Analytics;

namespace GreatCompany.Views.Analytics;

/// <summary>
/// таблица аналитики по месяцам. Колонки зависят от данных, поэтому строятся заново при смене таблицы или вида сумм
/// </summary>
public partial class AnalyticsGrid : UserControl {
	public static readonly StyledProperty<CashFlowTable?> TableProperty =
		AvaloniaProperty.Register<AnalyticsGrid, CashFlowTable?>(nameof(Table));

	public static readonly StyledProperty<AmountFormat> AmountFormatProperty =
		AvaloniaProperty.Register<AnalyticsGrid, AmountFormat>(nameof(AmountFormat), AmountFormat.Rubles);

	public CashFlowTable? Table { get => GetValue(TableProperty); set => SetValue(TableProperty, value); }
	public AmountFormat AmountFormat { get => GetValue(AmountFormatProperty); set => SetValue(AmountFormatProperty, value); }

	static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");

	public AnalyticsGrid() {
		InitializeComponent();
		grid.LoadingRow += (_, e) =>
			e.Row.FontWeight = e.Row.DataContext is CashFlowRow { IsTotal: true } ? FontWeight.Bold : FontWeight.Normal;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);
		if(change.Property == TableProperty || change.Property == AmountFormatProperty)
			Rebuild();
	}

	void Rebuild() {
		var table = Table ?? CashFlowTable.Empty;
		var format = AmountFormat;
		var converter = new FuncValueConverter<decimal, string>(value => FormatMoney(value, format));

		grid.ItemsSource = null;
		grid.Columns.Clear();
		grid.Columns.Add(new DataGridTextColumn {
			Header = "Месяц", Binding = new Binding(nameof(CashFlowRow.Title)), Width = new DataGridLength(130),
		});
		for(var i = 0; i < table.Columns.Count; i++)
			grid.Columns.Add(new DataGridTextColumn {
				Header = table.Columns[i],
				Binding = new Binding($"{nameof(CashFlowRow.Values)}[{i}]") { Converter = converter, Mode = BindingMode.OneWay },
				Width = new DataGridLength(120),
				CellStyleClasses = { "number" },
			});
		grid.ItemsSource = table.Rows;
	}

	/// <summary>сама сумма не меняется, округляется только текст. Ноль не показываем, иначе таблица рябит нулями</summary>
	static string FormatMoney(decimal value, AmountFormat format) => format switch {
		AmountFormat.Rubles => Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString("#,##0;-#,##0;''", Russian),
		AmountFormat.Thousands => Math.Round(value / 1000m, 0, MidpointRounding.AwayFromZero).ToString("#,##0;-#,##0;''", Russian),
		_ => value.ToString("#,##0.00;-#,##0.00;''", Russian),
	};
}
