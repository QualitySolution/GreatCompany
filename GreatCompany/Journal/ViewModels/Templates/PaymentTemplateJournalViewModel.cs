using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Templates;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Templates;

public class PaymentTemplateJournalViewModel : ReferenceJournalViewModelBase<PaymentTemplateRow> {
	readonly Repository repo;

	public PaymentTemplateJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "Шаблоны платежей";
		Reload();
	}

	protected override IEnumerable<PaymentTemplateRow> Load(string search) => repo.PaymentTemplates(search);
	protected override void Create() => OpenCard<PaymentTemplateViewModel>(0);
	protected override void Edit(PaymentTemplateRow row) => OpenCard<PaymentTemplateViewModel>(row.Id);
	protected override void Delete(PaymentTemplateRow row) { repo.Delete<PaymentTemplate>(row.Id); Reload(); }
}
