namespace SineSignal.Ottoman
{
	public interface ICouchDocumentSession
	{
		void Store(object entity);
		T Load<T>(string id) where T : new();
		void Delete<T>(T entity);
		void SaveChanges();
	}
}
