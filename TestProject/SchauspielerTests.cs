
using DataRepository;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ViewModels;

namespace ViewModelTests
{
	[TestClass]
	public class SchauspielerTests
	{
		private readonly SchauspielerVM _valid = new SchauspielerVM(1) { Name = "Test", Geschlecht = GeschlechtEnum.Männlich };



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
	}
}
