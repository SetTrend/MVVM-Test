using System.Collections.Generic;

using DataRepository;

namespace ViewModels
{
	public static class TestData
	{
		public static List<FilmVM> Filmliste => new()
		{
			new FilmVM(new Film()
			{
				Id = 1,
				Name = "Terminator",
				Duration = new(1, 30, 0),
				Genre = GenreEnum.Horror,
				Darsteller = new Schauspieler[]
				{
					new() { Id = 1, Name = "Arnold Schwarzenegger", Geschlecht = GeschlechtEnum.Männlich},
					new() { Id = 2, Name = "Linda Hamilton", Geschlecht = GeschlechtEnum.Weiblich},
					new() { Id = 3, Name = "Michael Biehn", Geschlecht = GeschlechtEnum.Männlich}
				}
			}),

			new FilmVM(new Film()
			{
				Id = 2,
				Name = "Krieg der Sterne",
				Duration = new(1, 45, 0),
				Genre = GenreEnum.ScienceFiction,
				Darsteller = new Schauspieler[]
				{
					new() { Id = 4, Name = "Mark Hamill", Geschlecht = GeschlechtEnum.Männlich},
					new() { Id = 5, Name = "Harrison Ford", Geschlecht = GeschlechtEnum.Männlich},
					new() { Id = 6, Name = "Carrie Fisher", Geschlecht = GeschlechtEnum.Weiblich},
					new() { Id = 7, Name = "Alec Guinness", Geschlecht =  GeschlechtEnum.Männlich},
					new() { Id = 8, Name = "David Prowse", Geschlecht = GeschlechtEnum.Männlich},
					new() { Id = 9, Name = "Anthony Daniels", Geschlecht = GeschlechtEnum.Männlich}
				}
			})
		};
	}
}
