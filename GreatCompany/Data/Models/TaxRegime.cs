namespace GreatCompany.Data.Models;

public enum TaxRegime { Vat, Entrepreneur, Cash }

public sealed class TaxRegimeOption(TaxRegime value, string title) {
	public TaxRegime Value { get; } = value;
	public string Title { get; } = title;

	public override string ToString() => Title;
}

public static class TaxRegimes {
	public static readonly IReadOnlyList<TaxRegimeOption> All = new[] {
		new TaxRegimeOption(TaxRegime.Vat, "НДС"),
		new TaxRegimeOption(TaxRegime.Entrepreneur, "ИП"),
		new TaxRegimeOption(TaxRegime.Cash, "Наличка"),
	};

	public static string TitleOf(TaxRegime value) => All.First(o => o.Value == value).Title;
}
