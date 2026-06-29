using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Templates;

namespace GreatCompany.Journal.Views.Templates;

public partial class PaymentTemplateJournalView : UserControl {
	public PaymentTemplateJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is PaymentTemplateJournalViewModel vm)
			vm.RowActivated();
	}
}
