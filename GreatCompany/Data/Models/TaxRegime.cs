using System.ComponentModel.DataAnnotations;

namespace GreatCompany.Data.Models;

public enum TaxRegime {
	[Display(Name = "НДС")] Vat,
	[Display(Name = "ИП")] Entrepreneur,
	[Display(Name = "Наличка")] Cash
}
