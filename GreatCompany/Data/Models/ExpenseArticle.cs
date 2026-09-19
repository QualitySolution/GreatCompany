using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Feminine, Nominative = "статья расхода", NominativePlural = "статьи расхода")]
public class ExpenseArticle : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[RequiredField]
	[MaxText(255)]
	public virtual string Name { get => name; set => SetField(ref name, value); }
}
