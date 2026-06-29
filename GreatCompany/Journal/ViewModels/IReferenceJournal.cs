using GreatCompany.Data.Models;

namespace GreatCompany.Journal.ViewModels;

// Позволяет открыть справочный журнал в режиме выбора, не зная его конкретного типа строки
public interface IReferenceJournal {
	void EnableSelect();
	event Action<ReferenceItem>? ItemSelected;
}
