using QS.Navigation;

namespace GreatCompany.Navigation;

// Хэш страницы = тип VM + ctor-аргументы, чтобы не плодить дубликаты
public class CashFlowPageHashGenerator : IPageHashGenerator {
	public string GetHash<TViewModel>(IDialogViewModel master, Type[] ctorTypes, object[] ctorValues)
		=> Hash(typeof(TViewModel), ctorValues);

	public string GetHashNamedArgs<TViewModel>(IDialogViewModel master, IDictionary<string, object> ctorArgs)
		=> Hash(typeof(TViewModel), ctorArgs.Values.ToArray());

	static string Hash(Type viewModel, object[] ctorValues)
		=> viewModel.FullName + string.Concat(ctorValues.Select(v => $"#{v}"));
}
