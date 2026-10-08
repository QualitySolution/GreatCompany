using System.ComponentModel.DataAnnotations;

namespace GreatCompany.ViewModels.Analytics;

/// <summary>вид сумм в таблице, сами суммы не меняются</summary>
public enum AmountFormat {
	[Display(Name = "С копейками")] Kopecks,
	[Display(Name = "Целые")] Rubles,
	[Display(Name = "В тысячах")] Thousands
}
