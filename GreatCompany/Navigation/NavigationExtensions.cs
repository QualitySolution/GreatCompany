using Avalonia.Threading;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels;
using QS.Navigation;

namespace GreatCompany.Navigation;

public static class NavigationExtensions {
	public static void OpenReferenceSelect<TJournal>(this INavigationManager nav, IDialogViewModel master, Action<ReferenceItem> onChosen)
		where TJournal : class, IDialogViewModel, IReferenceJournal {
		var page = nav.OpenViewModel<TJournal>(master, OpenPageOptions.IgnoreHash, vm => vm.EnableSelect());
		Action<ReferenceItem> handler = null!;
		handler = item => {
			page.ViewModel.ItemSelected -= handler;
			onChosen(item);
			// если onChosen открыл новую вкладку — на неё и вернемся, иначе на ту, что открыла выбор
			var target = nav.CurrentPage != null && nav.CurrentPage != page ? nav.CurrentPage : nav.FindPage(master);
			nav.ForceClosePage(page);
			if(target != null)
				Dispatcher.UIThread.Post(() => nav.SwitchOn(target));
		};
		page.ViewModel.ItemSelected += handler;
	}
}
