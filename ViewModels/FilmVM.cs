using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;

using DataRepository;

namespace ViewModels
{
	[DebuggerDisplay("{Name} ({Id}), [{EditState}]")]
	public class FilmVM : _BaseVM
	{
		#region ----- Commands ------------------------------------------------------
		private class EditSchauspielerCommand : _CmdBase, ICommand
		{
			public EditSchauspielerCommand(_BaseVM vModel) : base(vModel, "Schauspieler _bearbeiten") { }

			public bool CanExecute(object? _) => true;
			public void Execute(object? _) => ((FilmVM)_vModel).EditSchauspieler?.Invoke(this, _vModel as FilmVM);
		}
		#endregion



		private readonly Film _film = new Film();
		private Film? _backup;
		private readonly SchauspielerlisteVM _darsteller = new SchauspielerlisteVM();



		public int Id
		{
			get => _film.Id;
			set
			{
				_errors[nameof(Id)] = new List<string>();

				if (value <= 0) _errors[nameof(Id)].Add("A film's Id must be greater than 0.");

				if (_film.Id != value)
				{
					BeforePropertyChange();
					_film.Id = value;
					SetDirty(nameof(Id));
				}

				OnErrorsChanged(nameof(Id));
			}
		}

		public string? Name
		{
			get => _film.Name;
			set
			{
				_errors[nameof(Name)] = new List<string>();

				if (string.IsNullOrWhiteSpace(value)) _errors[nameof(Name)].Add("Provide a name for the film.");
				else value = value.Trim();

				if (_film.Name != value)
				{
					BeforePropertyChange();
					_film.Name = value;
					SetDirty(nameof(Name));
				}

				OnErrorsChanged(nameof(Name));
			}
		}

		public TimeSpan Duration
		{
			get => _film.Duration;
			set
			{
				_errors[nameof(Duration)] = new List<string>();

				if (value.TotalSeconds <= 30 || value.TotalHours >= 6) _errors[nameof(Duration)].Add("The film's duration must be less than 6 hours and more than 30 seconds.");

				if (_film.Duration != value)
				{
					BeforePropertyChange();
					_film.Duration = value;
					SetDirty(nameof(Duration));
				}

				OnErrorsChanged(nameof(Duration));
			}
		}

		public GenreEnum Genre
		{
			get => _film.Genre;
			set
			{
				if (_film.Genre != value)
				{
					BeforePropertyChange();
					_film.Genre = value;
					SetDirty(nameof(Genre));
				}
			}
		}

		public SchauspielerlisteVM Darsteller
		{
			get => _darsteller;
			set => SetDarsteller(value, false);
		}



		/// <summary>
		///		Gets the command for editing a film's actors.
		/// </summary>
		public ICommand EditSchauspielerCmd { get; init; }

		/// <summary>
		///		Gets all commands supported by this view model.
		/// </summary>
		public override ICommand[] Commands => new ICommand[] { EditSchauspielerCmd, ConfirmCmd, CancelCmd };



		/// <summary>
		///		Wird ausgelöst, wenn die Liste der Schauspieler eines
		///		Films bearbeitet werden soll.
		/// </summary>
		public event EventHandler<FilmVM?>? EditSchauspieler;



		/// <summary>
		///		Erstellt ein neues, uninitialisiertes, temporäres <see cref="FilmVM"/>-Objekt.
		/// </summary>
		public FilmVM() : this(0) { }

		/// <summary>
		///		Erstellt ein neues, uninitialisiertes, temporäres <see cref="FilmVM"/>-Objekt.
		/// </summary>
		/// <param name="id">
		///		Der Datenbankschlüssel des Datensatzes.
		/// </param>
		public FilmVM(int id) : this(id, EditStateEnum.Temporary) { }

		/// <summary>
		///		Erstellt ein neues, uninitialisiertes <see cref="FilmVM"/>-Objekt.
		/// </summary>
		/// <param name="id">
		///		Der Datenbankschlüssel des Datensatzes.
		/// </param>
		/// <param name="editState">
		///		Bearbeitungszustand des ViewModel-Objekts.
		/// </param>
		public FilmVM(int id, EditStateEnum editState)
		{
			EditSchauspielerCmd = new EditSchauspielerCommand(this);

			Id = id;

			// Fehlerliste und Bindings initialisieren (möglicherweise enthalten die aktuellen Daten ja bereits Fehler)
#pragma warning disable CA2245
			Name = Name;
			Duration = Duration;
			Genre = Genre;
			Darsteller = Darsteller;
#pragma warning restore CA2245

			_darsteller.CollectionChanged += Darsteller_CollectionChanged;

			_backup = null;
			EditState = editState;  // muss nach Eigenschaftszuweisung erfolgen, da jede Eigenschaftszuweisung das Objekt invalidiert
		}

		/// <summary>
		///		Erstellt ein neues <see cref="FilmVM"/>-Objekt anhand eines
		///		<see cref="Film"/>-Objekts.
		/// </summary>
		/// <param name="film">
		///		<see cref="Film"/>-Objekt, dessen Daten übernommen werden.
		/// </param>
		public FilmVM(Film film)
		{
			EditSchauspielerCmd = new EditSchauspielerCommand(this);

			Id = film.Id;
			Name = film.Name;
			Duration = film.Duration;
			Genre = film.Genre;
			Darsteller = new SchauspielerlisteVM(film.Darsteller);

			_darsteller.CollectionChanged += Darsteller_CollectionChanged;

			_backup = null;
			EditState = EditStateEnum.Clean;  // muss nach Eigenschaftszuweisung erfolgen, da jede Eigenschaftszuweisung das Objekt invalidiert
		}



		/// <summary>
		///		Validiert die Darstellerliste und synchronisiert das
		///		Darsteller-ViewModel mit dem Datenobjekt.
		/// </summary>
		/// <param name="value">
		///		<see cref="SchauspielerlisteVM"/>-Liste mit aktuellen
		///		Darstellern des Films.
		/// </param>
		/// <param name="fromWithinDarsteller">
		///		<c>true</c>, wenn diese Funktion aus einer intrinsischen
		///		Veränderung der Darstellerliste selbst statt über die
		///		Eigenschaft aufgerufen wurde; sonst <c>false</c>.
		/// </param>
		private void SetDarsteller(SchauspielerlisteVM value, bool fromWithinDarsteller)
		{
			_errors[nameof(Darsteller)] = new List<string>();

			if (value == null)
			{
				_errors[nameof(Darsteller)].Add("Die Liste der Darsteller ist ungültig.");
				value = new SchauspielerlisteVM();
			}
			else if (value.Count == 0) _errors[nameof(Darsteller)].Add("Die Liste der Darsteller ist leer.");

			if (!value.Equals(_film.Darsteller))
			{
				BeforePropertyChange();

				if (!fromWithinDarsteller)
				{
					_darsteller.Clear();
					foreach (SchauspielerVM s in value) _darsteller.Add(s);
				}

				_film.Darsteller = _darsteller;

				SetDirty(nameof(Darsteller));
			}

			OnErrorsChanged(nameof(Darsteller));
		}

		/// <summary>
		///		Validiert die Schauspielerliste, wenn sich die Anzahl der
		///		Elemente der Liste verändert.
		/// </summary>
		/// <param name="sender">
		///		Unbenutzt.
		/// </param>
		/// <param name="e">
		///		Unbenutzt.
		/// </param>
		private void Darsteller_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => SetDarsteller(Darsteller, true);



		/// <summary>
		///		Liefert <c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		<see cref="Film"/>-Objekt, dessen Daten mit dem aktuellen
		///		ViewModel-Objekt verglichen werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Werte des aktuellen ViewModels mit
		///		dem zu vergleichenden Objekt übereinstimmen; sonst <c>false</c>.
		/// </returns>
		public bool Equals(Film film) => _film.Id == film.Id && _film.Name == film.Name && _film.Duration == film.Duration && _film.Genre == film.Genre && _darsteller.Equals(film.Darsteller);

		/// <summary>
		///		Wird ausgeführt, bevor eine Eigenschaft des Objekts verändert wird.
		/// </summary>
		protected override void BeforePropertyChange()
		{
			if (_backup == null)
			{
				_backup = new Film() { Id = _film.Id, Name = _film.Name, Duration = _film.Duration, Genre = _film.Genre };
				_backup.Darsteller = _darsteller;
			}
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
				Duration = _backup.Duration;
				Genre = _backup.Genre;
				Darsteller = new SchauspielerlisteVM(_backup.Darsteller);

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
