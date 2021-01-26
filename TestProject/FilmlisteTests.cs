using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ViewModels;

namespace ViewModelTests
{
	[TestClass]
	public class FilmlisteTests
	{
		private static void CheckRemoveCmdCanExecute(FilmlisteVM liste)
		{
			if (liste.Count == 0) Assert.IsFalse(liste.RemoveCmd.CanExecute(null));
			else foreach (FilmVM film in liste) Assert.IsTrue(liste.RemoveCmd.CanExecute(film));
		}



		[TestMethod]
		public void FilmListIsValid()
		{
			FilmlisteVM filme = new FilmlisteVM();

			Assert.AreEqual(2, filme.Count);

			FilmVM f = filme[0];

			Assert.AreEqual(1, f.Id);
			Assert.AreEqual("Terminator", f.Name);
			Assert.AreEqual(3, f.Darsteller.Count);

			f = filme[1];

			Assert.AreEqual(2, f.Id);
			Assert.AreEqual("Krieg der Sterne", f.Name);
			Assert.AreEqual(6, f.Darsteller.Count);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));
		}

		[TestMethod]
		public void CanAddFilmToList()
		{
			FilmlisteVM filme = new FilmlisteVM();

			int count = filme.Count;

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));

			filme.AddCmd.Execute(null);

			Assert.AreEqual(count + 1, filme.Count);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));
		}

		[TestMethod]
		public void CanAddFilmToEmptyList()
		{
			FilmlisteVM filme = new FilmlisteVM();

			while (filme.Count > 0 && filme.RemoveCmd.CanExecute(filme[0])) filme.RemoveCmd.Execute(filme[0]);

			Assert.AreEqual(0, filme.Count);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsTrue(filme.SaveListCmd.CanExecute(null));

			filme.AddCmd.Execute(null);

			Assert.AreEqual(1, filme.Count);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));
		}

		[TestMethod]
		public void CanRemoveFilmFromList()
		{
			FilmlisteVM filme = new FilmlisteVM();

			int count = filme.Count;

			Assert.IsTrue(count > 0);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));

			filme.RemoveCmd.Execute(filme[0]);

			Assert.AreEqual(count - 1, filme.Count);

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsTrue(filme.SaveListCmd.CanExecute(null));
		}

		[TestMethod]
		public void CannotRemoveIfListIsEmpty()
		{
			FilmlisteVM filme = new FilmlisteVM();

			filme.Clear();

			Assert.IsTrue(filme.AddCmd.CanExecute(null));
			CheckRemoveCmdCanExecute(filme);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));

			Assert.ThrowsException<ArgumentNullException>(() => filme.RemoveCmd.Execute(null));
		}


		[TestMethod]
		public void CanSaveList()
		{
			FilmlisteVM filme = new FilmlisteVM();

			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));

			filme[0].Id = 1000;
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));

			filme[0].ConfirmCmd.Execute(null);
			Assert.IsTrue(filme.SaveListCmd.CanExecute(null));

			filme.SaveListCmd.Execute(null);
			Assert.IsFalse(filme.SaveListCmd.CanExecute(null));
		}
	}
}
