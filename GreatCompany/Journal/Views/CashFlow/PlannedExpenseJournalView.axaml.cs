using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.CashFlow;

namespace GreatCompany.Journal.Views.CashFlow;

public partial class PlannedExpenseJournalView : UserControl {
	public PlannedExpenseJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is PlannedExpenseJournalViewModel vm)
			vm.RowActivated();
	}
}
