using System;

using DataRepository;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ViewModels;

namespace ViewModelTests
{
	[TestClass]
	public class SchauspielerlisteTests
	{
		private readonly SchauspielerlisteVM _valid = new SchauspielerlisteVM(new Schauspieler[] { new() { Id = 1, Name = "Test", Geschlecht = GeschlechtEnum.Männlich } });



		[TestMethod]
		public void ActorListIsValid()
		{
			Assert.AreEqual(1, _valid.Count);

			SchauspielerVM actor = _valid[0];

			Assert.AreEqual(1, actor.Id);
			Assert.AreEqual("Test", actor.Name);
			Assert.AreEqual(GeschlechtEnum.Männlich, actor.Geschlecht);
		}

		[TestMethod]
		public void CanAddActorToEmptyList()
		{
			SchauspielerlisteVM list = new SchauspielerlisteVM();

			Assert.AreEqual(0, list.Count);
			Assert.IsTrue(list.AddCmd.CanExecute(null));
			Assert.IsFalse(list.RemoveCmd.CanExecute(null));
			Assert.IsFalse(list.SaveListCmd.CanExecute(null));

			list.AddCmd.Execute(null);

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(1, list[0].Id);
			Assert.IsTrue(list.AddCmd.CanExecute(null));
			Assert.IsTrue(list.RemoveCmd.CanExecute(list[0]));
			Assert.IsFalse(list.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Temporary, list[0].EditState);
		}

		[TestMethod]
		public void CanAddActorToList()
		{
			Assert.AreEqual(1, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsFalse(_valid.SaveListCmd.CanExecute(null));

			_valid.AddCmd.Execute(null);

			Assert.AreEqual(1, _valid[0].Id);
			Assert.AreEqual(2, _valid[1].Id);
			Assert.AreEqual(2, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[1]));
			Assert.IsFalse(_valid.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Clean, _valid[0].EditState);
			Assert.AreEqual(EditStateEnum.Temporary, _valid[1].EditState);
		}

		[TestMethod]
		public void CanSaveList()
		{
			Assert.AreEqual(1, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsFalse(_valid.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Clean, _valid[0].EditState);

			_valid[0].Name = "Test2";

			Assert.AreEqual(1, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsFalse(_valid.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Dirty, _valid[0].EditState);

			Assert.IsTrue(_valid[0].ConfirmCmd.CanExecute(null));
			_valid[0].ConfirmCmd.Execute(null);

			Assert.AreEqual(1, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsTrue(_valid.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Confirmed, _valid[0].EditState);

			_valid.SaveListCmd.Execute(null);

			Assert.AreEqual(1, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			Assert.IsFalse(_valid.SaveListCmd.CanExecute(null));
			Assert.AreEqual(EditStateEnum.Clean, _valid[0].EditState);
		}

		[TestMethod]
		public void CanRemoveActorFromList()
		{
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			_valid.RemoveCmd.Execute(_valid[0]);

			Assert.AreEqual(0, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsFalse(_valid.RemoveCmd.CanExecute(null));
			Assert.IsTrue(_valid.SaveListCmd.CanExecute(null));
		}

		[TestMethod]
		public void CannotRemoveIfListIsEmpty()
		{
			Assert.IsTrue(_valid.RemoveCmd.CanExecute(_valid[0]));
			_valid.RemoveCmd.Execute(_valid[0]);

			Assert.AreEqual(0, _valid.Count);
			Assert.IsTrue(_valid.AddCmd.CanExecute(null));
			Assert.IsFalse(_valid.RemoveCmd.CanExecute(null));
			Assert.IsTrue(_valid.SaveListCmd.CanExecute(null));

			Assert.ThrowsException<ArgumentNullException>(() => _valid.RemoveCmd.Execute(null));
		}
	}
}
