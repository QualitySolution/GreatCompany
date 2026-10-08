using System.ComponentModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using GreatCompany.Data.Analytics;
using GreatCompany.ViewModels.Analytics;

namespace GreatCompany.Views.Analytics;

public partial class DivisionDetailsView : UserControl {
	static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");

	/// <summary>
	/// сама сумма не меняется, округляется только текст. Ноль не показываем, иначе таблица рябит нулями
	/// </summary>
	static string FormatMoney(decimal value, AmountFormat format) => format switch {
		AmountFormat.Rubles => Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString("#,##0;-#,##0;''", Russian),
		AmountFormat.Thousands => Math.Round(value / 1000m, 0, MidpointRounding.AwayFromZero).ToString("#,##0;-#,##0;''", Russian),
		_ => value.ToString("#,##0.00;-#,##0.00;''", Russian),
	};

	DivisionDetailsViewModel? viewModel;

	public DivisionDetailsView() {
		InitializeComponent();
		dataGrid.LoadingRow += (_, e) =>
			e.Row.FontWeight = viewModel != null && ReferenceEquals(e.Row.DataContext, viewModel.Table.Rows.LastOrDefault())
				? FontWeight.Bold
				: FontWeight.Normal;
	}

	protected override void OnDataContextChanged(EventArgs e) {
		base.OnDataContextChanged(e);
		if(viewModel != null)
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;
		viewModel = DataContext as DivisionDetailsViewModel;
		if(viewModel != null)
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
		Rebuild();
	}

	void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if(e.PropertyName is nameof(DivisionDetailsViewModel.Table) or nameof(DivisionDetailsViewModel.AmountFormat))
			Rebuild();
	}

	// колонки зависят от данных: статьи, проекты или счета, по которым что-то было за период
	void Rebuild() {
		var table = viewModel?.Table ?? CashFlowTable.Empty;
		var format = viewModel?.AmountFormat ?? AmountFormat.Rubles;
		var converter = new FuncValueConverter<decimal, string>(value => FormatMoney(value, format));
		dataGrid.ItemsSource = null;
		dataGrid.Columns.Clear();

		dataGrid.Columns.Add(new DataGridTextColumn {
			Header = "Месяц", Binding = new Binding(nameof(CashFlowRow.Title)), Width = new DataGridLength(130),
		});
		for(var i = 0; i < table.Columns.Count; i++)
			dataGrid.Columns.Add(MoneyColumn(table.Columns[i], i, converter));
		dataGrid.Columns.Add(MoneyColumn("Итого", table.Columns.Count, converter));

		dataGrid.ItemsSource = table.Rows;
	}

	static DataGridTextColumn MoneyColumn(string header, int index, IValueConverter converter) => new() {
		Header = header,
		Binding = new Binding($"{nameof(CashFlowRow.Values)}[{index}]") { Converter = converter, Mode = BindingMode.OneWay },
		Width = new DataGridLength(120),
		CellStyleClasses = { "number" },
	};
}
