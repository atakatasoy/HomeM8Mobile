using HomeM8.Models.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views.PartialView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomInput : ContentView
	{
        public string Name { get; set; }
        public ICommand ReturnCommand { get; set; }
        public event EventHandler<EntryEventArgs> Returned;

        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
           nameof(TitleText),
           typeof(string),
           typeof(CustomInput),
           string.Empty,
           BindingMode.TwoWay);
        public string TitleText { get { return (string)GetValue(TitleTextProperty); } set { SetValue(TitleTextProperty, value); } }

        public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
            nameof(PlaceholderText),
            typeof(string),
            typeof(CustomInput),
            string.Empty,
            BindingMode.TwoWay);
        public string PlaceholderText { get { return (string)GetValue(PlaceholderTextProperty); }set { SetValue(PlaceholderTextProperty, value); } }

        public static readonly BindableProperty EntryTextProperty = BindableProperty.Create(
            nameof(EntryText),
            typeof(string),
            typeof(CustomInput),
            string.Empty,
            BindingMode.TwoWay);
        public string EntryText { get { return (string)GetValue(EntryTextProperty); } set { SetValue(EntryTextProperty, value); } }

        public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(CustomInput),
            Color.FromHex("#ffffff"),
            BindingMode.TwoWay);
        public new Color BackgroundColor{ get { return (Color)GetValue(BackgroundColorProperty); } set { SetValue(BackgroundColorProperty, value); } }

        public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
            nameof(LineColor),
            typeof(Color),
            typeof(CustomInput),
            Color.FromHex("#ffffff"),
            BindingMode.TwoWay);
        public Color LineColor { get { return (Color)GetValue(LineColorProperty); } set { SetValue(LineColorProperty, value); } }

        public void FocusEntry()
        {
            entry.Focus();
        }

        public CustomInput ()
		{
			InitializeComponent ();
            ReturnCommand = new Command(() => Returned?.Invoke(this, new EntryEventArgs(Name)));
            BindingContext = this;
		}
	}
}