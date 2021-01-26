using System.Windows;
using System.Windows.Controls;

using ViewModels;

namespace WPF
{
	/// <summary>
	/// Interaktionslogik für DarstellerWindow.xaml
	/// </summary>
	public partial class DarstellerWindow : Window
	{
		public FilmVM Film { get; init; }



		public DarstellerWindow(FilmVM film)
		{
			Film = film;

			InitializeComponent();

			Film.Darsteller.ListSaved += Darsteller_ListSaved;
		}

		private void Darsteller_ListSaved(object? sender, System.EventArgs e)
		{
			Film.Darsteller.ListSaved -= Darsteller_ListSaved;

			Close();
		}

		private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Film.Darsteller.UpdateAllCommandStates();   // wird benötigt, wenn kein Schauspieler selektiert war und nun ein Schauspieler selektiert ist bzw. umgekehrt.
		}
	}
}
