using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.CashFlow;

namespace GreatCompany.Journal.Views.CashFlow;

public partial class PlannedIncomeJournalView : UserControl {
	public PlannedIncomeJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is PlannedIncomeJournalViewModel vm)
			vm.RowActivated();
	}
}
