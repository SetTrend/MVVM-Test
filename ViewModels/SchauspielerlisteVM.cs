using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using DataRepository;

namespace ViewModels
{
	public class SchauspielerlisteVM : _ListBaseVM<SchauspielerVM>
	{
		#region ----- Commands ------------------------------------------------------
		private abstract class _ListeCmdBase : _ListeCmdEventBase
		{
			protected _ListBaseVM<SchauspielerVM> _listeVM;
			public string Text { get; init; }


			public _ListeCmdBase(_ListBaseVM<SchauspielerVM> darstellerVM, string text)
			{
				_listeVM = darstellerVM;
				Text = text;
			}
		}


		private class AddCommand : _ListeCmdBase, ICommand
		{
			public AddCommand(_ListBaseVM<SchauspielerVM> darstellerVM) : base(darstellerVM, "Schauspieler _hinzufügen") { }

			public bool CanExecute(object? _) => true;
			public void Execute(object? _) => _listeVM.Add(new SchauspielerVM(_listeVM.Count == 0 ? 1 : _listeVM.Max(s => s.Id) + 1));
		}


		private class RemoveCommand : _ListeCmdBase, ICommand
		{
			public RemoveCommand(_ListBaseVM<SchauspielerVM> darstellerVM) : base(darstellerVM, "Schauspieler _entfernen") { }

			public bool CanExecute(object? darsteller) => darsteller != null;
			public void Execute(object? darsteller)
			{
				if (darsteller is SchauspielerVM s)
				{
					if (_listeVM.Contains(s))
					{
						_listeVM.Remove(s);
						_listeVM.HasDeletedItems = true;
						OnCanExecuteChanged();
					}
					else throw new ArgumentException("Actor to remove doesn't exist in list.");
				}
				else throw darsteller is null
						? new ArgumentNullException(nameof(darsteller))
						: new ArgumentException("Object to remove from actors list isn't an actor.");
			}
		}


		private class SaveListCommand : _ListeCmdBase, ICommand
		{
			public SaveListCommand(_ListBaseVM<SchauspielerVM> darstellerVM) : base(darstellerVM, "Schauspielerliste _speichern") { }

			public bool CanExecute(object? _) => _listeVM.CanSave;
			public void Execute(object? _)
			{
				// in einer realen Anwendung würde hier der Code zum Persistieren der Liste stehen.
				foreach (SchauspielerVM s in _listeVM) s.EditState = EditStateEnum.Clean;

				_listeVM.HasDeletedItems = false;

				OnCanExecuteChanged();

				((SchauspielerlisteVM)_listeVM).ListSaved?.Invoke(this, EventArgs.Empty);
			}
		}
		#endregion



		/// <summary>
		///		Wird ausgelöst, nachdem die Liste erfolgreich gespeichert wurde.
		/// </summary>
		public event EventHandler? ListSaved;



		/// <summary>
		///		Erstellt eine neue, leere <see cref="SchauspielerlisteVM"/>-Sammlung.
		/// </summary>
		public SchauspielerlisteVM() : this(Array.Empty<Schauspieler>()) { }

		/// <summary>
		///		Erstellt eine neue <see cref="SchauspielerlisteVM"/>-Sammlung.
		/// </summary>
		public SchauspielerlisteVM(Schauspieler[] schauspieler) : this(schauspieler.Select(s => new SchauspielerVM(s))) { }

		/// <summary>
		///		Erstellt eine neue <see cref="SchauspielerlisteVM"/>-Sammlung.
		/// </summary>
		public SchauspielerlisteVM(IEnumerable<SchauspielerVM> schauspieler) : base(schauspieler)
		{
			AddCmd = new AddCommand(this);
			RemoveCmd = new RemoveCommand(this);
			SaveListCmd = new SaveListCommand(this);
		}



		/// <summary>
		///		Liefert <c>true</c>, wenn die Liste der aktuellen Schauspieler mit
		///		der Liste der zu vergleichenden Schauspieler übereinstimmt;
		///		sonst <c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		Liste der Schauspieler, die mit der aktuellen Liste verglichen
		///		werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Liste der aktuellen Schauspieler mit
		///		der Liste der zu vergleichenden Schauspieler übereinstimmt;
		///		sonst <c>false</c>.
		/// </returns>
		public bool Equals(Schauspieler[] darsteller)
		{
			if (Count != darsteller.Length) return false;

			for (int i = darsteller.Length; i > 0;) if (!this[--i].Equals(darsteller[i])) return false;

			return true;
		}

		/// <summary>
		///		Liefert <c>true</c>, wenn die Liste der aktuellen Schauspieler mit
		///		der Liste der zu vergleichenden Schauspieler übereinstimmt;
		///		sonst <c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		Liste der Schauspieler, die mit der aktuellen Liste verglichen
		///		werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Liste der aktuellen Schauspieler mit
		///		der Liste der zu vergleichenden Schauspieler übereinstimmt;
		///		sonst <c>false</c>.
		/// </returns>
		public bool Equals(IList<SchauspielerVM> darsteller)
		{
			if (Count != darsteller.Count) return false;

			for (int i = darsteller.Count; i > 0;) if (!this[--i].Equals(darsteller[i])) return false;

			return true;
		}



		public static implicit operator Schauspieler[](SchauspielerlisteVM liste) => liste.Select(s => new Schauspieler() { Id = s.Id, Name = s.Name, Geschlecht = s.Geschlecht }).ToArray();
	}
}
