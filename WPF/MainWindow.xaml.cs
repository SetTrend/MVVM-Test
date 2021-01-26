using System.Windows;

using ViewModels;

namespace WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}



		private void OpenSchauspielerWindow(object? sender, FilmVM? film)
		{
			if (film != null)
			{
				DarstellerWindow dw = new DarstellerWindow(film) { Owner = this };

				dw.ShowDialog();
			}
		}



		private void Filmliste_SelectionChanged(object? sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			((FilmlisteVM)Filmliste.ItemsSource).UpdateAllCommandStates();  // wird benötigt, wenn kein Film selektiert war und nun ein Film selektiert ist bzw. umgekehrt.

			foreach (FilmVM f in e.RemovedItems) f.EditSchauspieler -= OpenSchauspielerWindow;
			foreach (FilmVM f in e.AddedItems) f.EditSchauspieler += OpenSchauspielerWindow;
		}

		private void ListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OpenSchauspielerWindow(sender, Filmliste.SelectedItem as FilmVM);
		}
	}
}
