using GreatCompany.Data.Models;
using GreatCompany.Data.Taxes;
using QS.DomainModel.Entity;
using QS.DomainModel.UoW;

namespace GreatCompany.Data.Planning;

public enum PlanKind { Income, Expense }

/// <param name="Created">сколько планов создано</param>
/// <param name="Existing">по скольким шаблонам план на этот месяц уже был</param>
public record PlanFillResult(int Created, int Existing);

/// <summary>
/// Заполняет план на месяц по включённым шаблонам.
/// План, уже созданный по шаблону в этом месяце, не дублируется и не меняется, поэтому заполнение можно повторять.
/// </summary>
public static class PlansFromTemplates {
	public static PlanFillResult Fill(IUnitOfWork uow, PlanKind kind, DateTime month) {
		var from = new DateTime(month.Year, month.Month, 1);
		var to = from.AddMonths(1);
		var session = uow.Session;

		return kind == PlanKind.Income
			? Fill(uow,
				session.Query<AccrualTemplate>().Where(x => !x.IsDisabled),
				session.Query<PlannedIncome>().Where(x => x.AccrualTemplate != null && x.Date >= from && x.Date < to)
					.Select(x => x.AccrualTemplate!.Id),
				t => Plan(new PlannedIncome { AccrualTemplate = t, Date = t.PlanDate(month) }, p => p.FillFrom(t)))
			: Fill(uow,
				session.Query<PaymentTemplate>().Where(x => !x.IsDisabled),
				session.Query<PlannedExpense>().Where(x => x.PaymentTemplate != null && x.Date >= from && x.Date < to)
					.Select(x => x.PaymentTemplate!.Id),
				t => Plan(new PlannedExpense { PaymentTemplate = t, Date = t.PlanDate(month) }, p => p.FillFrom(t)));
	}

	static PlanFillResult Fill<TTemplate>(IUnitOfWork uow, IQueryable<TTemplate> templates, IQueryable<int> filledIds,
		Func<TTemplate, object> createPlan) where TTemplate : IDomainObject {
		var filled = filledIds.ToHashSet();
		var all = templates.ToList();
		var missing = all.Where(t => !filled.Contains(t.Id)).ToList();

		missing.ForEach(t => uow.Save(createPlan(t)));
		uow.Commit();
		return new(missing.Count, all.Count - missing.Count);
	}

	// НДС пересчитывается на дату плана, ставка могла смениться
	static TPlan Plan<TPlan>(TPlan plan, Action<TPlan> fill) where TPlan : IVatDocument {
		fill(plan);
		plan.Vat = TaxCalculator.CalculateVat(plan.Account.TaxRegime, plan.Cost, plan.TaxDate);
		return plan;
	}
}
