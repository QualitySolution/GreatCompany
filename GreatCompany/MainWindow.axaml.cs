using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using QS.Navigation;
using QS.Project.Versioning.ViewModels;

namespace GreatCompany;

public partial class MainWindow : Window {
	private readonly AvaloniaNavigationManager? navigationManager;
	private readonly Dictionary<NavigationViewItem, Action> menuItems = [];

	public MainWindow() {
		InitializeComponent();
	}

	public MainWindow(
		AvaloniaNavigationManager navigationManager,
		string? login,
		string? sessionId,
		string? baseTitle) {
		this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

		InitializeComponent();
		navigationManagerView.DataContext = navigationManager;
		Title = MakeTitle(login, baseTitle);

		RegMenuItemActions();
		Closing += OnClosing;
	}

	private static string MakeTitle(string? login, string? baseTitle) {
		var title = "QS: Великая компания";
		if(!string.IsNullOrWhiteSpace(baseTitle))
			title += $" (БД: {baseTitle})";
		if(!string.IsNullOrWhiteSpace(login))
			title += $" - {login}";
		return title;
	}

	private void RegMenuItemActions() {
		menuItems.Add(changeLogMenuItem, () => navigationManager?.OpenViewModel<ChangeLogViewModel>(null));
	}

	private void OnNavViewSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e) {
		if(e.SelectedItem is NavigationViewItem item && menuItems.TryGetValue(item, out var action))
			action();
	}

	private void OnClosing(object? sender, WindowClosingEventArgs e) {
		if(navigationManager == null)
			return;

		foreach(var page in navigationManager.Pages.ToList())
			navigationManager.AskClosePage(page, CloseSource.AppQuit);
	}
}
