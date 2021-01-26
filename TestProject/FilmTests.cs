using System;

using DataRepository;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ViewModels;

namespace ViewModelTests
{
	[TestClass]
	public class FilmTests
	{
		private readonly FilmVM _valid = new FilmVM() { Id = 1, Name = "Test", Duration = TimeSpan.FromMinutes(1), Genre = GenreEnum.Action, Darsteller = new SchauspielerlisteVM(new Schauspieler[] { new() { Id = 1, Name = "Test2", Geschlecht = GeschlechtEnum.Männlich } }) };



		[TestMethod]
		public void IsValidCheck()
		{
			Assert.IsFalse(_valid.HasErrors);
		}

		[TestMethod]
		public void IdLessThanZeroCheck()
		{
			_valid.Id = -1;

			Assert.IsTrue(_valid.HasErrors);
		}

		[TestMethod]
		public void NameIsNullCheck()
		{
			_valid.Name = null;

			Assert.IsTrue(_valid.HasErrors);
		}

		[TestMethod]
		public void NameIsEmptyCheck()
		{
			_valid.Name = " ";

			Assert.IsTrue(_valid.HasErrors);
		}

		[TestMethod]
		public void DurationIsTooSmallCheck()
		{
			_valid.Duration = TimeSpan.FromSeconds(30);

			Assert.IsTrue(_valid.HasErrors);
		}

		[TestMethod]
		public void DurationIsTooLargeCheck()
		{
			_valid.Duration = TimeSpan.FromHours(3);

			Assert.IsTrue(_valid.HasErrors);
		}

		[TestMethod]
		public void DarstellerIsEmptyCheck()
		{
			_valid.Darsteller = new SchauspielerlisteVM(Array.Empty<Schauspieler>());

			Assert.IsTrue(_valid.HasErrors);
		}
	}
}
