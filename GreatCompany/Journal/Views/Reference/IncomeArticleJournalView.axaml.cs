using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Reference;

namespace GreatCompany.Journal.Views.Reference;

public partial class IncomeArticleJournalView : UserControl {
	public IncomeArticleJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is IncomeArticleJournalViewModel vm)
			vm.RowActivated();
	}
}
