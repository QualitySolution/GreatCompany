using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Nominative = "статья расхода", NominativePlural = "статьи расхода")]
public class ExpenseArticle : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	public virtual string Name { get => name; set => SetField(ref name, value); }
}
