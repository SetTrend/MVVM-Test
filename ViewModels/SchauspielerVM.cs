using System.Collections.Generic;
using System.Diagnostics;

using DataRepository;

namespace ViewModels
{
	[DebuggerDisplay("{Name} ({Id}), [{EditState}]")]
	public class SchauspielerVM : _BaseVM
	{
		private readonly Schauspieler _darsteller = new Schauspieler();
		private Schauspieler? _backup;

		public int Id
		{
			get => _darsteller.Id;
			set
			{
				_errors[nameof(Id)] = new List<string>();

				if (value < 0) _errors[nameof(Id)].Add("An actor's Id must be greater than or equal to 0.");

				if (_darsteller.Id != value)
				{
					BeforePropertyChange();
					_darsteller.Id = value;
					SetDirty(nameof(Id));
				}

				OnErrorsChanged(nameof(Id));
			}
		}

		public string? Name
		{
			get => _darsteller.Name;
			set
			{
				_errors[nameof(Name)] = new List<string>();

				if (string.IsNullOrWhiteSpace(value)) _errors[nameof(Name)].Add("Provide an actor name.");
				else value = value.Trim();

				if (_darsteller.Name != value)
				{
					BeforePropertyChange();
					_darsteller.Name = value;
					SetDirty(nameof(Name));
				}

				OnErrorsChanged(nameof(Name));
			}
		}

		public GeschlechtEnum Geschlecht
		{
			get => _darsteller.Geschlecht;
			set
			{
				if (_darsteller.Geschlecht != value)
				{
					BeforePropertyChange();
					_darsteller.Geschlecht = value;
					SetDirty(nameof(Geschlecht));
				}
			}
		}



		/// <summary>
		///		Erstellt ein neues, uninitialisiertes, temporäres <see cref="SchauspielerVM"/>-Objekt.
		/// </summary>
		public SchauspielerVM() : this(0) { }

		/// <summary>
		///		Erstellt ein neues, uninitialisiertes, temporäres <see cref="SchauspielerVM"/>-Objekt.
		/// </summary>
		/// <param name="id">
		///		Der Datenbankschlüssel des Datensatzes.
		/// </param>
		public SchauspielerVM(int id) : this(id, EditStateEnum.Temporary) { }

		/// <summary>
		///		Erstellt ein neues, uninitialisiertes <see cref="SchauspielerVM"/>-Objekt.
		/// </summary>
		/// <param name="id">
		///		Der Datenbankschlüssel des Datensatzes.
		/// </param>
		/// <param name="editState">
		///		Bearbeitungszustand des ViewModel-Objekts.
		/// </param>
		public SchauspielerVM(int id, EditStateEnum editState)
		{
			Id = id;

			// Fehlerliste initialisieren (möglicherweise enthalten die aktuellen Daten ja bereits Fehler)
#pragma warning disable CA2245
			Name = Name;
			Geschlecht = Geschlecht;
#pragma warning restore CA2245

			EditState = editState;  // muss nach Eigenschaftszuweisung erfolgen, da jede Eigenschaftszuweisung das Objekt invalidiert
		}

		/// <summary>
		///		Erstellt ein neues <see cref="SchauspielerVM"/>-Objekt
		///		anhand eines <see cref="Schauspieler"/>-Objekts.
		/// </summary>
		/// <param name="darsteller">
		///		<see cref="Schauspieler"/>-Objekt, dessen Daten
		///		übernommen werden.
		/// </param>
		public SchauspielerVM(Schauspieler darsteller)
		{
			Id = darsteller.Id;
			Name = darsteller.Name;
			Geschlecht = darsteller.Geschlecht;

			_backup = null;
			EditState = EditStateEnum.Clean;  // muss nach Eigenschaftszuweisung erfolgen, da jede Eigenschaftszuweisung das Objekt invalidiert
		}



		/// <summary>
		///		Liefert <c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		<see cref="Schauspieler"/>-Objekt, dessen Daten mit dem aktuellen
		///		ViewModel-Objekt verglichen werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </returns>
		public bool Equals(Schauspieler darsteller) => _darsteller.Id == darsteller.Id && _darsteller.Name == darsteller.Name && _darsteller.Geschlecht == darsteller.Geschlecht;

		/// <summary>
		///		Liefert <c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		<see cref="SchauspielerVM"/>-Objekt, dessen Daten mit dem aktuellen
		///		ViewModel-Objekt verglichen werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </returns>
		public bool Equals(SchauspielerVM darsteller) => _darsteller.Id == darsteller.Id && _darsteller.Name == darsteller.Name && _darsteller.Geschlecht == darsteller.Geschlecht;


		/// <summary>
		///		Wird ausgeführt, bevor eine Eigenschaft des Objekts verändert wird.
		/// </summary>
		protected override void BeforePropertyChange()
		{
			if (_backup == null) _backup = new Schauspieler() { Id = _darsteller.Id, Name = _darsteller.Name, Geschlecht = _darsteller.Geschlecht };
		}

		/// <summary>
		///		Macht Änderungen am aktuellen ViewModel-Objekt rückgängig.
		/// </summary>
		protected override void Undo()
		{
			if (_backup != null)
			{
				Id = _backup.Id;
				Name = _backup.Name;
				Geschlecht = _backup.Geschlecht;

				_backup = null;
			}
		}

		/// <summary>
		///		Speichert das aktuelle ViewModel-Objekt.
		/// </summary>
		internal override void Save()
		{
			_backup = null;
			EditState = EditStateEnum.Clean;
		}
	}
}
