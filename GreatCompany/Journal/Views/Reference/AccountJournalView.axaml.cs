using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Reference;

namespace GreatCompany.Journal.Views.Reference;

public partial class AccountJournalView : UserControl {
	public AccountJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is AccountJournalViewModel vm)
			vm.RowActivated();
	}
}
