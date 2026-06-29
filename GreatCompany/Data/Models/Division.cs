using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Nominative = "дивизион", NominativePlural = "дивизионы")]
public class Division : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	public virtual string Name { get => name; set => SetField(ref name, value); }

	int? parentDivisionId;
	public virtual int? ParentDivisionId { get => parentDivisionId; set => SetField(ref parentDivisionId, value); }
}

public class DivisionRow : IReferenceRow {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string? ParentName { get; set; }
}
