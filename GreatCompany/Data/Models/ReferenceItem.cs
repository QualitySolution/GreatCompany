namespace GreatCompany.Data.Models;

// только то, что нужно показать и выбрать
public sealed class ReferenceItem(int id, string name) {
	public int Id { get; } = id;
	public string Name { get; } = name;

	public override string ToString() => Name;
}
