using System;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
	/// <summary>
	///		Observable collection of film descriptors.
	/// </summary>
	public class FilmlisteVM : _ListBaseVM<FilmVM>
	{
		#region ----- Commands ------------------------------------------------------
		private abstract class _ListeCmdBase : _ListeCmdEventBase
		{
			protected _ListBaseVM<FilmVM> _filmeVM;
			public string Text { get; init; }


			public _ListeCmdBase(_ListBaseVM<FilmVM> filmeVM, string text)
			{
				_filmeVM = filmeVM;
				Text = text;
			}
		}


		private class AddCommand : _ListeCmdBase, ICommand
		{
			public AddCommand(_ListBaseVM<FilmVM> filmeVM) : base(filmeVM, "Film _hinzufügen") { }

			public bool CanExecute(object? _) => true;
			public void Execute(object? _) => _filmeVM.Add(new FilmVM(_filmeVM.Count == 0 ? 1 : _filmeVM.Max(f => f.Id) + 1));
		}


		private class RemoveCommand : _ListeCmdBase, ICommand
		{
			public RemoveCommand(_ListBaseVM<FilmVM> filmeVM) : base(filmeVM, "Film _entfernen") { }

			public bool CanExecute(object? filmVM) => filmVM != null;
			public void Execute(object? filmVM)
			{
				if (filmVM is FilmVM f)
				{
					if (_filmeVM.Contains(f))
					{
						_filmeVM.Remove(f);
						_filmeVM.HasDeletedItems = true;
						OnCanExecuteChanged();
					}
					else throw new ArgumentException("Film to remove doesn't exist in list.");
				}
				else throw filmVM is null
						? new ArgumentNullException(nameof(filmVM))
						: new ArgumentException("Object to remove from film list isn't a film.");
			}
		}


		private class SaveListCommand : _ListeCmdBase, ICommand
		{
			public SaveListCommand(_ListBaseVM<FilmVM> filmeVM) : base(filmeVM, "Filmliste _speichern") { }

			public bool CanExecute(object? _) => _filmeVM.CanSave;
			public void Execute(object? _)
			{
				// in einer realen Anwendung würde hier der Code zum Persistieren der Liste stehen.
				foreach (FilmVM f in _filmeVM)
				{
					f.EditState = EditStateEnum.Clean;

					foreach (SchauspielerVM s in f.Darsteller) s.EditState = EditStateEnum.Clean;
				}

				_filmeVM.HasDeletedItems = false;

				OnCanExecuteChanged();
			}
		}
		#endregion



		/// <summary>
		///		Creates a new <see cref="FilmlisteVM"/> collection with demo data.
		/// </summary>
		public FilmlisteVM() : base(TestData.Filmliste)
		{
			AddCmd = new AddCommand(this);
			RemoveCmd = new RemoveCommand(this);
			SaveListCmd = new SaveListCommand(this);
		}



		/// <summary>
		///		Liefert <c>true</c>, wenn die Liste der aktuellen Filme mit
		///		der Liste der zu vergleichenden Filme übereinstimmt; sonst
		///		<c>false</c>.
		/// </summary>
		/// <param name="darsteller">
		///		Liste der Filme, die mit der aktuellen Liste verglichen
		///		werden sollen.
		/// </param>
		/// <returns>
		///		<c>true</c>, wenn die Liste der aktuellen Filme mit der Liste
		///		der zu vergleichenden Filme übereinstimmt; sonst <c>false</c>.
		/// </returns>
		public bool Equals(FilmlisteVM filme)
		{
			if (Count != filme.Count) return false;

			for (int i = Count; i > 0;) if (this[i] != filme[i]) return false;

			return true;
		}
	}
}
