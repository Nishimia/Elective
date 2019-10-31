using Microsoft.AspNet.Identity;

namespace ElectiveBLL.Interfaces.ModelInterfaces
{
	/// <summary>
	///     Minimal interface for a user with id
	/// </summary>
	public interface IElectiveUser<TKey> : IUser<TKey>
	{
		/// <summary>
		///		User's first name
		/// </summary>
		string FirstName { get; set; }

		/// <summary>
		///		User's second name
		/// </summary>
		string SecondName { get; set; }
		
		/// <summary>
		///		
		/// </summary>
		bool Banned { get; set; }
	}
}
