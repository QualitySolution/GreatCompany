using System.ComponentModel;
using ReactiveUI.Primitives;
using GreatCompany.Data.Models;
using GreatCompany.Data.Taxes;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Project.Domain;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.ViewModels;

public abstract class CardViewModelBase<TEntity> : EntityDialogViewModelBase<TEntity>
	where TEntity : class, IDomainObject, new() {
	protected CardDependencies Deps { get; }

	protected CardViewModelBase(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps.UnitOfWork, deps.Navigation, deps.Validator, deps.ChangeWatcher) {
		Deps = deps;

		SaveCommand = ReactiveCommand.Create(() => { SaveAndClose(); });
		CancelCommand = ReactiveCommand.Create(() => Close(true, CloseSource.Cancel));

		if(Entity is INotifyPropertyChanged entity)
			entity.PropertyChanged += (_, _) => HasChanges = true;

		if(Entity is IVatDocument vatDocument && Entity is INotifyPropertyChanged vatSource)
			vatSource.PropertyChanged += (_, e) => OnVatInputChanged(vatDocument, e.PropertyName);
	}

	/// <summary>у документа без НДС по налоговому режиму счёта поле НДС не показывается</summary>
	public bool ShowVat => Entity is IVatDocument document && TaxCalculator.HasVat(document.Account?.TaxRegime);

	// сумма всегда с НДС, поэтому НДС пересчитывается при смене суммы, счёта (режима) или даты (ставки).
	// вручную его можно поправить только после, в пределах допуска, это проверяет валидация документа
	void OnVatInputChanged(IVatDocument document, string? propertyName) {
		if(propertyName is not (nameof(IVatDocument.Cost) or nameof(IVatDocument.Account) or "Date"))
			return;

		document.Vat = TaxCalculator.CalculateVat(document.Account?.TaxRegime, document.Cost, document.TaxDate);
		OnPropertyChanged(nameof(ShowVat));
	}

	// в библиотечном диалоге Entity - поле, а вьюхи биндятся на свойства
	public new TEntity Entity => base.Entity;

	public ReactiveCommand<RxVoid, RxVoid> SaveCommand { get; }
	public ReactiveCommand<RxVoid, RxVoid> CancelCommand { get; }

	public override bool Save() {
		if(!base.Save())
			return false;

		HasChanges = false;
		return true;
	}

	protected override bool Validate() {
		if(base.Validate())
			return true;

		var errors = string.Join("\n• ", validator.Results.Select(r => r.ErrorMessage));
		Deps.Interactive.ShowMessage(ImportanceLevel.Warning, "Проверьте заполнение:\n• " + errors, "Не сохранено");
		return false;
	}
}
