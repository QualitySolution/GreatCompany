using Avalonia.Controls;
using Avalonia.Input;
using GreatCompany.Journal.ViewModels.Reference;

namespace GreatCompany.Journal.Views.Reference;

public partial class ExpenseArticleJournalView : UserControl {
	public ExpenseArticleJournalView() => InitializeComponent();

	void OnRowDoubleTapped(object? sender, TappedEventArgs e) {
		if(DataContext is ExpenseArticleJournalViewModel vm)
			vm.RowActivated();
	}
}
