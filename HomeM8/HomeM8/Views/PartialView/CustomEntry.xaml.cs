using HomeM8.Models.EventArguments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views.PartialView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomEntry : ContentView
	{
        public string Name { get; set; }

        public ICommand ReturnCommand { get; set; }

        public event EventHandler<EntryEventArgs> Returned;

        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
            nameof(KeyboardValue),
            typeof(Keyboard),
            typeof(CustomEntry),
            default(Keyboard),
            BindingMode.TwoWay);
        public Keyboard KeyboardValue { get { return (Keyboard)GetValue(KeyboardProperty); }set { SetValue(KeyboardProperty, value); } }

        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
            nameof(IsPassword),
            typeof(bool),
            typeof(CustomEntry),
            default(bool),
            BindingMode.TwoWay);
        public bool IsPassword { get { return (bool)GetValue(IsPasswordProperty); } set { SetValue(IsPasswordProperty, value); } }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            nameof(ImageSourceValue),
            typeof(ImageSource),
            typeof(CustomEntry),
            default(ImageSource),
            BindingMode.TwoWay);
        public ImageSource ImageSourceValue { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }

        public static readonly BindableProperty SourceTextProperty = BindableProperty.Create(
            nameof(SourceText),
            typeof(string),
            typeof(CustomEntry),
            string.Empty,
            BindingMode.TwoWay);
        public string SourceText { get { return (string)GetValue(SourceTextProperty); } set { SetValue(SourceTextProperty, value); } }

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
            nameof(ErrorText),
            typeof(string),
            typeof(CustomEntry),
            string.Empty,
            BindingMode.TwoWay);
        public string ErrorText { get { return (string)GetValue(ErrorTextProperty); } set { SetValue(ErrorTextProperty, value); } }

        public static readonly BindableProperty PlaceholderStringProperty = BindableProperty.Create(
            nameof(PlaceholderString),
            typeof(string),
            typeof(CustomEntry),
            string.Empty,
            BindingMode.TwoWay,
            propertyChanged:(bindableObject,oldValue,newValue)=> 
            {
                var customView = bindableObject as CustomEntry;
                customView.PwEntry.Placeholder = (string)newValue;
            });
        public string PlaceholderString { get { return (string)GetValue(PlaceholderStringProperty); } set { SetValue(PlaceholderStringProperty, value); } }

        public async void AnimateError(bool decision)
        {
            if (decision)
            {
                errorGrid.IsVisible = true;
                await errorGrid.LayoutTo(new Rectangle(errorGrid.X, errorGrid.Y, errorGrid.Width, 18), 500, Easing.BounceIn);
            }
            else
            {
                await errorGrid.LayoutTo(new Rectangle(errorGrid.X, errorGrid.Y, errorGrid.Width, 0), 500, Easing.BounceOut);
                errorGrid.IsVisible = false;
            }
        }
        public void FocusEntry()
        {
            PwEntry.Focus();
        }
		public CustomEntry ()
		{
			InitializeComponent ();
            ReturnCommand = new Command(() =>
            {
                Returned?.Invoke(this, new EntryEventArgs(Name));
            });
            PwEntry.Focused += (sender, e) => AnimateError(false);
            BindingContext = this;
		}
	}
}