using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Templates;

namespace GreatCompany.Journal.Views.Templates;

public partial class AccrualTemplateJournalView : UserControl {
	public AccrualTemplateJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is AccrualTemplateJournalViewModel vm)
			vm.RowActivated();
	}
}
