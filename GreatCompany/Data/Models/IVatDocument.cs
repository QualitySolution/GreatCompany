namespace GreatCompany.Data.Models;

/// <summary>документ с суммой, включающей НДС. Налоговый режим берётся со счёта</summary>
public interface IVatDocument {
	decimal? Cost { get; set; }
	decimal? Vat { get; set; }
	Account Account { get; }

	/// <summary>дата, по которой определяется ставка НДС. Если её нет, берётся сегодняшняя</summary>
	DateTime? TaxDate { get; }
}
