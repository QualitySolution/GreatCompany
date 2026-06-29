namespace GreatCompany.Data.Models;

// Строка справочного журнала, которую можно выбрать в пикере.
public interface IReferenceRow {
	int Id { get; }
	string Name { get; }
}
