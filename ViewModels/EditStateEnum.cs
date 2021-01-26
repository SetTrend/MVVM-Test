namespace ViewModels
{
	/// <summary>
	///		Gibt den Zustand eines ViewModel-Objekts wieder.
	/// </summary>
	public enum EditStateEnum
	{
		/// <summary>
		///		Das ViewModel-Objekt stammt unverändert aus der Datenbank.
		/// </summary>
		Clean,

		/// <summary>
		///		Das ViewModel-Objekt wurde verändert, aber noch nicht bestätigt.
		/// </summary>
		Dirty,

		/// <summary>
		///		Das ViewModel-Objekt wurde hinzugefügt, aber noch nicht bestätigt.
		/// </summary>
		Temporary,

		/// <summary>
		///		Die Änderungen des ViewModel-Objekts wurden bestätigt.
		/// </summary>
		Confirmed
	}
}
