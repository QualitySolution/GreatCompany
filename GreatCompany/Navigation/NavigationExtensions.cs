using Avalonia.Controls;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels;
using QS.Navigation;

namespace GreatCompany.Navigation;

public static class NavigationExtensions {
	// Выбор из справочника — модальное окно поверх вкладки/окна, из которого его открыли.
	// Журналу задаем фиксированный размер: сам по содержимому он не умеет вменяемо рассчитаться (DataGrid).
	public static void OpenReferenceSelect<TJournal>(this INavigationManager nav, IDialogViewModel master, Action<ReferenceItem> onChosen)
		where TJournal : class, IDialogViewModel, IReferenceJournal {
		var page = ((AvaloniaNavigationManager)nav).OpenViewModelAsWindow<TJournal>(master, OpenPageOptions.IgnoreHash,
			vm => vm.EnableSelect(),
			window => {
				window.SizeToContent = SizeToContent.Manual;
				window.Width = 850;
				window.Height = 500;
			});
		Action<ReferenceItem> handler = null!;
		handler = item => {
			page.ViewModel.ItemSelected -= handler;
			nav.ForceClosePage(page);
			onChosen(item);
		};
		page.ViewModel.ItemSelected += handler;
	}
}
