using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.CashFlow;

namespace GreatCompany.Journal.Views.CashFlow;

public partial class ActualExpenseJournalView : UserControl {
	public ActualExpenseJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is ActualExpenseJournalViewModel vm)
			vm.RowActivated();
	}
}
