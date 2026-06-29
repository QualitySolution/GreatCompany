using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Reference;

namespace GreatCompany.Journal.Views.Reference;

public partial class ProjectJournalView : UserControl {
	public ProjectJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is ProjectJournalViewModel vm)
			vm.RowActivated();
	}
}
