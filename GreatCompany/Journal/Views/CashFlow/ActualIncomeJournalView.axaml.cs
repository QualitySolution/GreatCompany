using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.CashFlow;

namespace GreatCompany.Journal.Views.CashFlow;

public partial class ActualIncomeJournalView : UserControl {
	public ActualIncomeJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is ActualIncomeJournalViewModel vm)
			vm.RowActivated();
	}
}
