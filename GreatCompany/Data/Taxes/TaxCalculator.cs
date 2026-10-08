using GreatCompany.Data.Models;

namespace GreatCompany.Data.Taxes;

/// <summary>
/// Ответы на вопросы по налогам. Сумма в документах всегда с НДС,
/// поэтому НДС выделяется из неё, а не начисляется сверху.
/// </summary>
public static class TaxCalculator {
	/// <summary>ставки НДС по дате документа, от новых к старым</summary>
	static readonly (DateTime From, decimal Percent)[] VatRates = {
		(new DateTime(2026, 1, 1), 22m),
		(DateTime.MinValue, 20m),
	};

	/// <summary>ставка НДС в процентах на дату документа. Без даты берётся сегодняшняя</summary>
	public static decimal GetVatRatePercent(DateTime? date) {
		var day = date ?? DateTime.Today;
		return VatRates.First(x => day >= x.From).Percent;
	}

	/// <summary>на сколько рублей введённый вручную НДС может отличаться от расчётного</summary>
	public const decimal VatTolerance = 1m;

	/// <summary>нужно ли показывать и считать НДС при налоговом режиме. Без режима НДС нет</summary>
	public static bool HasVat(TaxRegime? regime) => regime == TaxRegime.Vat;

	/// <summary>НДС, выделенный из суммы с НДС. Без НДС по режиму всегда 0</summary>
	public static decimal CalculateVat(TaxRegime? regime, decimal? costWithVat, DateTime? date) {
		if(!HasVat(regime) || costWithVat is not { } cost)
			return 0m;

		var rate = GetVatRatePercent(date);
		return Math.Round(cost * rate / (100m + rate), 2, MidpointRounding.AwayFromZero);
	}

	/// <summary>сообщение об ошибке, если НДС не сходится с суммой, иначе null</summary>
	public static string? CheckVat(TaxRegime? regime, decimal? costWithVat, decimal? vat, DateTime? date) {
		var actual = vat ?? 0m;

		if(!HasVat(regime))
			return actual == 0m ? null : "при этом налоговом режиме НДС должен быть равен 0";

		// НДС 0 на счёте с НДС - документ без НДС (зарплата, налоги, субсидии и т.п.)
		if(actual == 0m)
			return null;

		var expected = CalculateVat(regime, costWithVat, date);
		return Math.Abs(actual - expected) <= VatTolerance
			? null
			: $"сумма НДС {actual:N2} не сходится с расчётной {expected:N2} (допустимое отклонение {VatTolerance:N0} ₽)";
	}
}
