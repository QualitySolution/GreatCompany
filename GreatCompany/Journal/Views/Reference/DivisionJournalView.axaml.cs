using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Reference;

namespace GreatCompany.Journal.Views.Reference;

public partial class DivisionJournalView : UserControl {
	public DivisionJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is DivisionJournalViewModel vm)
			vm.RowActivated();
	}
}
