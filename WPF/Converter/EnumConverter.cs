using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.Converter
{
	internal class EnumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value.ToString() ?? "";
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Enum.Parse(targetType, value.ToString() ?? "");
	}
}
