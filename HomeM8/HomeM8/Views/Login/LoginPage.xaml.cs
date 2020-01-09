using HomeM8.Models.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        public static LoginPage UI;
        public LoginPageViewModel vM;
		public LoginPage ()
		{
			InitializeComponent ();
            UI = this;
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = vM = new LoginPageViewModel();
            vM.SetUI(this);
            WrapperGrid.TranslateTo(600, 0, 0);
            userEntry.Name = "User";
            passEntry.Name = "Pass";
            turkish.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => vM.ChangeConfiguration(1)) });
            english.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => vM.ChangeConfiguration(2)) });
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await WrapperGrid.TranslateTo(0, 0, 500, Easing.SinIn);
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            await WrapperGrid.TranslateTo(-600, 0, 500, Easing.SinOut);
        }
        internal void DisplayCustomAlert(string name)
        {
            if (name == "Username")
            {
                userEntry.AnimateError(true);
            }
            else
            {
                passEntry.AnimateError(true);
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            ForgotPasswordPage page = new ForgotPasswordPage();
            await WrapperGrid.TranslateTo(-600, 0, 500, Easing.SinOut);
            await UI.Navigation.PushAsync(page, false);
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            RegisterPage page = new RegisterPage();
            await WrapperGrid.TranslateTo(-600, 0, 500, Easing.SinOut);
            await UI.Navigation.PushAsync(page, false);
        }

        private void Entry_Returned(object sender, EntryEventArgs e)
        {
            if (e.Name == "User")
            {
                passEntry.FocusEntry();
            }
            else if (e.Name == "Pass")
            {
                vM.LoginCommand.Execute(null);
            }
        }
    }
}