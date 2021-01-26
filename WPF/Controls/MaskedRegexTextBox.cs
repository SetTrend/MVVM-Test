using System;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF.Controls
{
	/// <summary>
	/// Interaktionslogik für MaskedRegexTextBox.xaml
	/// </summary>
	public partial class MaskedRegexTextBox : TextBox
	{
		public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(MaskedRegexTextBox), new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = false, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, IsAnimationProhibited = true });



		public string? Mask
		{
			get => (string?)GetValue(MaskProperty);
			set => SetValue(MaskProperty, value);
		}



		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == TextProperty)
			{
				if (!(string.IsNullOrWhiteSpace(Mask) || string.IsNullOrWhiteSpace((string?)e.NewValue) || Regex.IsMatch((string)e.NewValue, $"^{Mask.Trim()}$")))
				{
					SystemSounds.Beep.Play();

					BindingExpression? be = GetBindingExpression(TextProperty);
					object? source = be.ResolvedSource;
					PropertyInfo? propInfo = source.GetType().GetProperty(be.ResolvedSourcePropertyName);

					if (source != null && propInfo != null)
					{
						MethodInfo? parse = propInfo.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });

						if (parse != null) propInfo.SetValue(source, parse.Invoke(null, new object[] { e.OldValue }));
						else if (propInfo.PropertyType == typeof(string)) propInfo.SetValue(source, e.OldValue);

						be.UpdateTarget();
					}

					if (SelectionLength == 0) Dispatcher.BeginInvoke(new Action(() => SelectionLength = 1));
				}
			}
		}
	}
}
