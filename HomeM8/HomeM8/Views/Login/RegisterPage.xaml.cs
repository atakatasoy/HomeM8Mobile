using HomeM8.Models.EventArguments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        public RegisterPageViewModel vM;
		public RegisterPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = vM = new RegisterPageViewModel();
            vM.SetUI(this);
            WrapperGrid.TranslateTo(600, 0, 0);
            secondStack.TranslateTo(700, 0, 500, Easing.CubicOut);
            secondStack.IsVisible = false;
            passEntry.Name = "Pass";
            rePassEntry.Name = "RePass";
            nameEntry.Name = "Name";
            phoneEntry.Name = "Phone";
            EmailEntry.Focused += (sender, e) => AnimateError(nameof(vM.Email), false);
            UsernameEntry.Focused += (sender, e) => AnimateError(nameof(vM.Username), false);
            EmailEntry.ReturnCommand = new Command(() => nameEntry.FocusEntry());
            UsernameEntry.ReturnCommand = new Command(()=>passEntry.FocusEntry());
            LottieEmail.OnPlay += (sender, e) => LottieEmail.IsVisible = true;
            LottieUser.OnPlay += (sender, e) => LottieUser.IsVisible = true;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await WrapperGrid.TranslateTo(0, 0, 500, Easing.SinIn);
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            await WrapperGrid.TranslateTo(600, 0, 500, Easing.SinOut);
        }
        internal async void CallNextPage()
        {
            secondStack.IsVisible = true;
            await Task.WhenAll(secondStack.TranslateTo(0, 0, 500, Easing.CubicIn), firstStack.TranslateTo(-700, 0, 500, Easing.CubicOut));
            firstStack.IsVisible = false;
        }

        private async void UsernameEntry_Unfocused(object sender,FocusEventArgs e)
        {
            vM.IsUsernameValidationRunning = true;
            vM.UsernameCheckVisible = true;
            var entry = (Entry)sender;
            entry.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(entry.Text))
            {
                vM.ValidUsername = false;
                vM.Error = "Kullanıcı adı boş bırakılamaz";
                AnimateError(nameof(vM.Username), true);
                LottieUser.Animation = "Unsuccessful.json";
            }
            else if (entry.Text.Length > 4 && entry.Text.Length < 12)
            {
                var serviceResponse = JsonConvert.DeserializeObject<BaseResponseModel>(await Helper.httpGetAsync($"{Utility.BaseURL}/api/user/checkusername?username={entry.Text}"));
                if (serviceResponse.responseVal == 0)
                {
                    vM.ValidUsername = true;
                    LottieUser.Animation = "Successful.json";
                }
                else
                {
                    vM.ValidUsername = false;
                    vM.Error = serviceResponse.responseText;
                    AnimateError(nameof(vM.Username), true);
                    LottieUser.Animation = "Unsuccessful.json";
                }
            }
            else
            {
                vM.ValidUsername = false;
                vM.Error = "Kullanıcı adınız 4 karakterden kısa 12 karakterden uzun olamaz";
                AnimateError(nameof(vM.Username), true);
                LottieUser.Animation = "Unsuccessful.json";
            }

            LottieUser.Play();
            entry.IsEnabled = true;
            vM.UsernameCheckVisible = false;
            vM.IsUsernameValidationRunning = false;
        }

        private async void EmailEntry_Unfocused(object sender, FocusEventArgs e)
        {
            vM.IsEmailValidationRunning = true;
            vM.EmailCheckVisible = true;
            var entry = ((Entry)sender);    
            entry.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(entry.Text))
            {
                vM.ValidEmail = false;
                vM.Error = "Mail adresi boş bırakılamaz";
                AnimateError(nameof(vM.Email), true);
                LottieEmail.Animation = "Unsuccessful.json";
            }
            else if (entry.Text.Length > 12)
            {
                if (new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase).IsMatch(entry.Text))
                {
                    var valid = JsonConvert.DeserializeObject<BaseResponseModel>(await Helper.httpGetAsync($"{Utility.BaseURL}/api/user/checkemail?email={entry.Text}"));
                    if (valid.responseVal == 0)
                    {
                        vM.ValidEmail = true;
                        LottieEmail.Animation = "Successful.json";
                    }
                    else
                    {
                        vM.ValidEmail = false;
                        LottieEmail.Animation = "Unsuccessful.json";
                        vM.Error = valid.responseText;
                        AnimateError(nameof(vM.Email), true);
                    }
                }
                else
                {
                    vM.ValidEmail = false;
                    LottieEmail.Animation = "Unsuccessful.json";
                    vM.Error = "Lütfen geçerli bir mail adresi giriniz";
                    AnimateError(nameof(vM.Email), true);
                }
            }
            else
            {
                vM.ValidEmail = false;
                vM.Error = "Lütfen geçerli bir mail adresi giriniz";
                AnimateError(nameof(vM.Email), true);
                LottieEmail.Animation = "Unsuccessful.json";
            }

            LottieEmail.Play();
            entry.IsEnabled = true;
            vM.EmailCheckVisible = false;
            vM.IsEmailValidationRunning = false;
        }

        internal void DisplayCustomAlert(KeyValuePair<string,string> name,bool useFullString=false)
        {
            if (useFullString)
            {
                vM.Error = name.Key;
                AnimateError("Username", true);
            }
            else
            {
                vM.Error = name.Key;
                switch (name.Value)
                {
                    case nameof(vM.Username):
                        AnimateError(name.Value, true);
                        break;
                    case nameof(vM.Password):
                        passEntry.AnimateError(true);
                        break;
                    case nameof(vM.RepeatPassword):
                        rePassEntry.AnimateError(true);
                        break;
                    case nameof(vM.PhoneNumber):
                        phoneEntry.AnimateError(true);
                        break;
                    case nameof(vM.NameSurname):
                        nameEntry.AnimateError(true);
                        break;
                    case nameof(vM.Email):
                        AnimateError(name.Value, true);
                        break;
                }
            }
        }

        internal void TriggerInvalidEmailAnimation()
        {
            if (LottieEmail.Animation == null) LottieEmail.Animation = "Unsuccessful.json";
            vM.Error = "Lütfen geçerli bir email adresi giriniz";
            AnimateError(nameof(vM.Email), true);
            LottieEmail.Play();
        }

        async void AnimateError(string name,bool decision)
        {
            if (decision)
            {
                if (name == "Username")
                {
                    userErrorGrid.IsVisible = true;
                    await userErrorGrid.LayoutTo(new Rectangle(userErrorGrid.X, userErrorGrid.Y, userErrorGrid.Width, 18), 500, Easing.BounceIn);
                }
                else
                {
                    mailErrorGrid.IsVisible = true;
                    await mailErrorGrid.LayoutTo(new Rectangle(mailErrorGrid.X, mailErrorGrid.Y, mailErrorGrid.Width, 18), 500, Easing.BounceIn);
                }
            }
            else
            {
                if (name == "Username")
                {
                    await userErrorGrid.LayoutTo(new Rectangle(userErrorGrid.X, userErrorGrid.Y, userErrorGrid.Width, 0), 500, Easing.BounceIn);
                    userErrorGrid.IsVisible = false;
                }
                else
                {
                    await mailErrorGrid.LayoutTo(new Rectangle(mailErrorGrid.X, mailErrorGrid.Y, mailErrorGrid.Width, 0), 500, Easing.BounceIn);
                    mailErrorGrid.IsVisible = false;
                }
            }
        }

        private void Entry_Returned(object sender,EntryEventArgs e)
        {
            if (e.Name == "Pass")
            {
                rePassEntry.FocusEntry();
            }
            else if (e.Name == "RePass")
            {
                vM.RegisterCommand.Execute(null);
            }
            else if (e.Name == "Name")
            {
                phoneEntry.FocusEntry();
            }
            else if (e.Name == "Phone")
            {
                vM.RegisterCommand.Execute(null);
            }
        }

        private void EmailEntry_Focused(object sender, FocusEventArgs e)
        {
            LottieEmail.IsVisible = false;
        }

        private void UsernameEntry_Focused(object sender, FocusEventArgs e)
        {
            LottieUser.IsVisible = false;
        }

        internal void TriggerInvalidUsernameAnimation()
        {
            if (LottieUser.Animation == null) LottieUser.Animation = "Unsuccessful.json";
            vM.Error = "Lütfen geçerli bir kullanıcı adı giriniz";
            AnimateError(nameof(vM.Username), true);
            LottieUser.Play();
        }
    }
}