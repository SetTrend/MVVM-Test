using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Converter
{
	/// <summary>
	///		Konvertiert einen <see cref="EditStateEnum"/>-Wert in eine Resource.
	/// </summary>
	internal class StateImageConverter : IValueConverter
	{
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try   // im Designer ist "Application.Current.MainWindow" == null
			{
				return Application.Current.MainWindow.TryFindResource(value.ToString());
			}
			catch
			{
				return null;
			}
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
