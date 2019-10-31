namespace ElectiveBLL.Interfaces.ModelInterfaces
{
	/// <summary>
	///     Minimal interface for a subject with id and name
	/// </summary>
	public interface ISubject<TKey>
	{
		/// <summary>
		///		Subject ID
		/// </summary>
		TKey Id { get; set; }

		/// <summary>
		///		Name of the subject
		/// </summary>
		string SubjectName { get; set; }
	}
}
