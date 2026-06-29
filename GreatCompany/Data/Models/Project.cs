using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Nominative = "проект", NominativePlural = "проекты")]
public class Project : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	public virtual string Name { get => name; set => SetField(ref name, value); }

	int divisionId;
	public virtual int DivisionId { get => divisionId; set => SetField(ref divisionId, value); }
}

public class ProjectRow : IReferenceRow {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string DivisionName { get; set; } = "";
}
