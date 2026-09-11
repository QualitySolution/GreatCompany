using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace GreatCompany.Views;

/// <summary>Кнопки внизу карточки</summary>
public partial class CardButtons : UserControl {
	public static readonly StyledProperty<ICommand?> SaveProperty =
		AvaloniaProperty.Register<CardButtons, ICommand?>(nameof(Save));

	public static readonly StyledProperty<ICommand?> CancelProperty =
		AvaloniaProperty.Register<CardButtons, ICommand?>(nameof(Cancel));

	public CardButtons() => InitializeComponent();

	public ICommand? Save {
		get => GetValue(SaveProperty);
		set => SetValue(SaveProperty, value);
	}

	public ICommand? Cancel {
		get => GetValue(CancelProperty);
		set => SetValue(CancelProperty, value);
	}
}
