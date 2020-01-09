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
	public partial class ForgotPasswordPage : ContentPage
	{
        public ForgotPasswordPageViewModel vM;
		public ForgotPasswordPage ()
		{
            InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = vM = new ForgotPasswordPageViewModel();
            WrapperGrid.TranslateTo(600, 0, 0);
            vM.SetUI(this);
            passEntry.Name = "Pass";
            userEntry.Name = "User";
            validationEntry.Name = "Validation";
            rePassEntry.Name = "RePass";
            thirdStack.TranslateTo(700, 0, 500, Easing.SinIn);
            thirdStack.IsVisible = false;
            secondStack.TranslateTo(700, 0, 500, Easing.SinIn);
            secondStack.IsVisible = false;
		}
        public void DisplayCustomAlert(KeyValuePair<string,string> error)
        {
            vM.Error = error.Value;
            if (error.Key == "User")
            {
                userEntry.AnimateError(true);
            }
            else if (error.Key == "Pass")
            {
                passEntry.AnimateError(true);
            }
            else if (error.Key == "Validation")
            {
                validationEntry.AnimateError(true);
            }
        }
        private void Entry_Returned(object sender,EntryEventArgs e)
        {
            if (e.Name == "User")
            {
                vM.SendCommand.Execute(null);
            }
            else if (e.Name == "Pass")
            {
                rePassEntry.FocusEntry();
            }
            else if (e.Name == "RePass")
            {
                vM.NewPasswordCommand.Execute(null);
            }
            else if (e.Name == "Validation")
            {
                vM.SendValidationCommand.Execute(null);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await WrapperGrid.TranslateTo(0, 0, 500, Easing.SinIn);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await WrapperGrid.TranslateTo(-600, 0, 500, Easing.SinOut);
        }

        internal async void CallValidationStackLayout()
        {
            secondStack.IsVisible = true;
            await Task.WhenAll(firstStack.TranslateTo(-700, 0, 500, Easing.SinOut), secondStack.TranslateTo(0, 0, 500, Easing.SinIn));
            firstStack.IsVisible = false;
        }

        internal async void CallNewPasswordStackLayout()
        {
            thirdStack.IsVisible = true;
            await Task.WhenAll(secondStack.TranslateTo(-700, 0, 500, Easing.SinOut), thirdStack.TranslateTo(0, 0, 500, Easing.SinIn));
            secondStack.IsVisible = false;
        }
    }
}