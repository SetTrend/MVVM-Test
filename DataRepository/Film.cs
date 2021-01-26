using System;

namespace DataRepository
{
	public class Film
	{
		public int Id;
		public string? Name;
		public TimeSpan Duration;
		public GenreEnum Genre;
		public Schauspieler[] Darsteller = new Schauspieler[0];
	}
}
