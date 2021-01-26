using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
	public abstract class _ListBaseVM<T> : ObservableCollection<T> where T : _BaseVM
	{
		#region ----- Commands ------------------------------------------------------
		protected abstract class _ListeCmdEventBase
		{
			public event EventHandler? CanExecuteChanged;

			public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}


		/// <summary>
		///		Verhindert, dass die Roslyn Nullable-Checks
		///		Warnungen ausgeben.
		/// </summary>
		private class DummyCommand : ICommand
		{
#pragma warning disable CS0067
			public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

			public bool CanExecute(object? parameter) => throw new NotImplementedException();
			public void Execute(object? parameter) => throw new NotImplementedException();
		}
		#endregion



		internal bool HasDeletedItems;



		/// <summary>
		///		Liefert das <see cref="ICommand"/>-Objekt,
		///		um ein Element der Liste hinzuzufügen.
		/// </summary>
		public ICommand AddCmd { get; init; } = new DummyCommand();

		/// <summary>
		///		Liefert das <see cref="ICommand"/>-Objekt,
		///		um ein Element aus der Liste zu entfernen.
		/// </summary>
		public ICommand RemoveCmd { get; init; } = new DummyCommand();

		/// <summary>
		///		Liefert das <see cref="ICommand"/>-Objekt,
		///		um die Liste abzuspeichern.
		/// </summary>
		public ICommand SaveListCmd { get; init; } = new DummyCommand();


		/// <summary>
		///		Liefert alle <see cref="ICommand"/>-Objekte,
		///		die von diesem ViewModel unterstützt werden.
		/// </summary>
		public virtual ICommand[] Commands => new ICommand[] { AddCmd, RemoveCmd, SaveListCmd };



		/// <summary>
		/// <c>true</c>, wenn mindestens ein Datensatz vorhanden ist
		/// und mindestens ein Datensatz gelöscht wurde oder gespeichert
		/// werden kann; sonst <c>false</c>.
		/// </summary>
		internal bool CanSave => (HasDeletedItems || this.Any(t => t.EditState == EditStateEnum.Confirmed)) && !this.Any(t => t.EditState == EditStateEnum.Dirty || t.EditState == EditStateEnum.Temporary);



		protected _ListBaseVM() { }

		protected _ListBaseVM(IEnumerable<T> collection) : base(collection) { }

		protected _ListBaseVM(List<T> list) : base(list) { }



		/// <summary>
		///		Wird aufgerufen, wenn die Eigenschaft eines Elements der Liste
		///		verändert wurden, um die <see cref="ICommand"/>-Objekte zu aktualisieren.
		/// </summary>
		/// <param name="sender">
		///		Unbenutzt.
		/// </param>
		/// <param name="e">
		///		Unbenutzt.
		/// </param>
		protected void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e) => UpdateAllCommandStates();

		/// <summary>
		///		Aktualisiert den Zustand aller <see cref="ICommand"/>-Objekte.
		/// </summary>
		public void UpdateAllCommandStates()
		{
			foreach (_ListeCmdEventBase? cmd in Commands) cmd?.OnCanExecuteChanged();
		}


		/// <summary>
		///		Wird aufgerufen, wenn sich die Liste der Elemente ändert.
		///		Verknüpft alle <see cref="ICommand"/>-Objekte mit Datenänderungen
		///		an den jeweiligen ViewModel-Objekten der Liste.
		/// </summary>
		/// <param name="e">
		///		Liste der Veränderten Elemente der Liste.
		/// </param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (e.OldItems != null) foreach (_BaseVM item in e.OldItems) item.PropertyChanged -= ItemPropertyChanged;
			if (e.NewItems != null) foreach (_BaseVM item in e.NewItems) item.PropertyChanged += ItemPropertyChanged;
		}
	}
}
