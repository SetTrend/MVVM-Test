using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
	public abstract class _BaseVM : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		#region ----- Commands ------------------------------------------------------
		protected class _CmdBase
		{
			protected _BaseVM _vModel;
			public string Text { get; init; }

			public event EventHandler? CanExecuteChanged;


			protected _CmdBase(_BaseVM vModel, string text)
			{
				_vModel = vModel;
				Text = text;
			}


			public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}


		private class ConfirmCommand : _CmdBase, ICommand
		{
			public ConfirmCommand(_BaseVM vModel) : base(vModel, "_Änderungen bestätigen") { }

			public bool CanExecute(object? _) => (_vModel.EditState == EditStateEnum.Dirty || _vModel.EditState == EditStateEnum.Temporary) && !_vModel.HasErrors;
			public void Execute(object? _)
			{
				_vModel.EditState = EditStateEnum.Confirmed;
				_vModel.UpdateAllCommandStates();
			}
		}


		private class CancelCommand : _CmdBase, ICommand
		{
			public CancelCommand(_BaseVM vModel) : base(vModel, "_Rückgängig") { }

			public bool CanExecute(object? _) => _vModel.EditState == EditStateEnum.Dirty;
			public void Execute(object? _)
			{
				_vModel.Undo();
				_vModel.EditState = EditStateEnum.Clean;
				_vModel.UpdateAllCommandStates();
			}
		}
		#endregion



		private EditStateEnum _editState;

		protected readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();



		/// <summary>
		///		Liefert das <see cref="ICommand"/>-Objekt zum
		///		Bestätigen von Änderungen.
		/// </summary>
		public ICommand ConfirmCmd { get; init; }

		/// <summary>
		///		Liefert das <see cref="ICommand"/>-Objekt zum
		///		Rückgängigmachen von Änderungen.
		/// </summary>
		public ICommand CancelCmd { get; init; }

		/// <summary>
		///		Liefert alle <see cref="ICommand"/>-Objekte,
		///		die von diesem ViewModel unterstützt werden.
		/// </summary>
		public virtual ICommand[] Commands => new ICommand[] { ConfirmCmd, CancelCmd };



		/// <summary>
		///		Gibt den Bearbeitungszustand eines ViewModel-Objekts wieder.
		/// </summary>
		public EditStateEnum EditState
		{
			get => _editState;

			internal set
			{
				_editState = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditState)));
			}
		}


		public bool HasErrors => (from e in _errors
															where e.Value.Count > 0
															select e).Any();

		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
		public event PropertyChangedEventHandler? PropertyChanged;


		public IEnumerable GetErrors(string? propertyName) => !string.IsNullOrWhiteSpace(propertyName) && _errors.ContainsKey(propertyName) ? _errors[propertyName] : new List<string>();




		/// <summary>
		///		Initialisiert die allgemein verfügbaren <see cref="ICommands"/>
		///		des ViewModels
		/// </summary>
		protected _BaseVM()
		{
			ConfirmCmd = new ConfirmCommand(this);
			CancelCmd = new CancelCommand(this);
		}



		/// <summary>
		///		Löst das <see cref="ErrorsChanged"/>-Ereignis aus.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void OnErrorsChanged(string propertyName)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) throw new InvalidOperationException(nameof(propertyName));

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		/// <summary>
		///		Setzt den <see cref="EditState"/>-Status und feuert das
		///		<see cref="PropertyChanged"/>-Ereignis ab.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void SetDirty(string propertyName)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) throw new InvalidOperationException(nameof(propertyName));

			if (EditState != EditStateEnum.Temporary) EditState = EditStateEnum.Dirty;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

			UpdateAllCommandStates();
		}

		/// <summary>
		///		Aktualisiert den Zustand aller <see cref="ICommand"/>-Objekte.
		/// </summary>
		protected void UpdateAllCommandStates()
		{
			foreach (_CmdBase? cmd in Commands) cmd?.OnCanExecuteChanged();
		}



		/// <summary>
		///		Wird ausgeführt, bevor eine Eigenschaft des Objekts verändert wird.
		/// </summary>
		protected abstract void BeforePropertyChange();

		/// <summary>
		///		Macht Änderungen am aktuellen ViewModel-Objekt rückgängig.
		/// </summary>
		protected abstract void Undo();

		/// <summary>
		///		Speichert das aktuelle ViewModel-Objekt.
		/// </summary>
		internal abstract void Save();
	}
}
