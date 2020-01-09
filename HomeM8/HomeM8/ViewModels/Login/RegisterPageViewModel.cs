using HomeM8.Models;
using HomeM8.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class RegisterPageViewModel : INotifyPropertyChanged
    {
        MainPage RootPage => Application.Current.MainPage as MainPage;

        #region Private Members

        RegisterPage UI;

        KeyValuePair<string,string> ControlPageValues()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return new KeyValuePair<string, string>("Email boş bırakılamaz", nameof(Email));
            }
            else if(string.IsNullOrWhiteSpace(NameSurname))
            {
                return new KeyValuePair<string, string>("Ad Soyad boş bırakılamaz",nameof(NameSurname));
            }
            else if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                return new KeyValuePair<string, string>("Telefon numarası boş bırakılamaz", nameof(PhoneNumber));
            }
            else
            {
                if (PhoneNumber[0] == '5' && PhoneNumber.Length == 10)
                {
                    return new KeyValuePair<string, string>("Ok", null);
                }
                else
                {
                    return new KeyValuePair<string, string>("Lütfen geçerli bir telefon numarası girin", nameof(PhoneNumber));
                }
            }
        }

        KeyValuePair<string,string> ControlSecondPageValues()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return new KeyValuePair<string, string>("Kullanıcı adı boş bırakılamaz", nameof(Username));
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                return new KeyValuePair<string, string>("Şifre boş bırakılamaz",nameof(Password));
            }
            else if (string.IsNullOrWhiteSpace(RepeatPassword))
            {
                return new KeyValuePair<string, string>("Şifre tekrarı boş bırakılamaz",nameof(RepeatPassword));
            }
            else
            {
                if (Username.Length < 4)
                {
                    return new KeyValuePair<string, string>("Kullanıcı adınız 4 karakterden uzun olamaz", nameof(Username));
                }
                else if (Username.Length > 12)
                {
                    return new KeyValuePair<string, string>("Kullanıcı adınız 12 karakterden uzun olamaz",nameof(Username));
                }
                else
                {
                    if (Password != RepeatPassword)
                    {
                        return new KeyValuePair<string, string>("Şifreleriniz uyuşmuyor", nameof(Password));
                    }
                    else
                    {
                        if (Password.Length < 6)
                        {
                            return new KeyValuePair<string, string>("Şifreniz 6 karakterden kısa olamaz",nameof(Password));
                        }
                        else if (Password.Length > 12)
                        {
                            return new KeyValuePair<string, string>("Şifreniz 12 karakterden uzun olamaz", nameof(Password));
                        }
                        else
                        {
                            return new KeyValuePair<string, string>("Ok", null);
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Members

        #region First Page

        public string Email { get; set; }
        public string NameSurname { get; set; }
        public string PhoneNumber { get; set; }

        #endregion

        #region Second Page

        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; } 

        #endregion

        public string ButtonText { get; set; }
        public ICommand RegisterCommand { get; set; }
        public int CurrentPage { get; set; }
        public bool EmailCheckVisible { get; set; }
        public bool UsernameCheckVisible { get; set; }
        public bool ValidEmail { get; set; }
        public bool ValidUsername { get; set; }
        public bool IsEmailValidationRunning { get; set; }
        public bool IsUsernameValidationRunning { get; set; }
        public string Error { get; set; }

        public void SetUI(Page Ui) => UI = (RegisterPage)Ui;

        #endregion

        public RegisterPageViewModel()
        {
            ButtonText = AppConfigurationModel.Instance.RegisterPageContent.ButtonText1;
            RegisterCommand = new Command(async () =>
            {
                switch (CurrentPage)
                {
                    case 0:

                        if (!IsEmailValidationRunning)
                        {
                            if (ValidEmail)
                            {
                                var returnVal = ControlPageValues();

                                if (returnVal.Key == "Ok")
                                {
                                    CurrentPage = 1;
                                    UI.CallNextPage();
                                    ButtonText = AppConfigurationModel.Instance.RegisterPageContent.ButtonText2;
                                }
                                else
                                {
                                    UI.DisplayCustomAlert(returnVal);
                                }

                            }
                            else
                            {
                                UI.TriggerInvalidEmailAnimation();
                            } 
                        }
                        else
                        {
                            //lütfen bekleyiniz
                        }
                        break;
                    case 1:

                        if (!IsUsernameValidationRunning)
                        {
                            if (ValidUsername)
                            {
                                var returnVal2 = ControlSecondPageValues();

                                if (returnVal2.Key == "Ok")
                                {
                                    string errorLog = null;

                                    Utility.ShowIndicator = true;

                                    await Task.Run(() =>
                                    {
                                        EllipticCurve elliptic = new EllipticCurve(true);

                                        var buffer = elliptic.KeyExchange(Username);

                                        if (elliptic.IsSucceeded)
                                        {
                                            Utility.SharedSecret = elliptic.SharedSecret;

                                            var serializedRegisterParameters = JsonConvert.SerializeObject(new
                                            {
                                                Email,
                                                Password,
                                                NameSurname,
                                                phoneNumber = PhoneNumber,
                                            });

                                            try
                                            {
                                                var response = JsonConvert.DeserializeObject<BaseResponseModel>(
                                                    Helper.httpPostAsync($"{Utility.BaseURL}/api/user/register?username={Username}", serializedRegisterParameters, IsSecure: true).Result);

                                                if (response.responseVal == 0)
                                                {
                                                    var serializedLoginParameters = JsonConvert.SerializeObject(new { password = Password });

                                                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(
                                                        Helper.httpPostAsync($"{Utility.BaseURL}/api/user/login?username={Username}", serializedLoginParameters, IsSecure: true).Result);

                                                    if (loginResponse.responseVal == 0)
                                                    {
                                                        Utility.ShowIndicator = false;
                                                        using(var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
                                                        {
                                                            User bufferUser = new User()
                                                            {
                                                                accessToken = loginResponse.accessToken,
                                                                LastLoggedInDate = DateTime.Now,
                                                                ConnectedHomes = Helper.ConvertCollectionToString(loginResponse.ConnectedHomes, ','),
                                                                CurrentHome = loginResponse.CurrentHome,
                                                                LoggedIn = true,
                                                                Username = Username,
                                                                UserType = loginResponse.userType,
                                                                UserNameSurname = loginResponse.nameSurname
                                                            };
                                                            con.Insert(bufferUser);
                                                            Utility.User = bufferUser;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        errorLog = loginResponse.responseText;
                                                    }
                                                }
                                                else
                                                {
                                                    errorLog = response.responseText;
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                errorLog = Utility.UnexpectedMessage;
                                            }
                                        }
                                        else
                                        {
                                            errorLog = elliptic.Response.responseText;
                                        }
                                    });

                                    Utility.ShowIndicator = false;

                                    if (errorLog != null)
                                    {
                                        UI.DisplayCustomAlert(new KeyValuePair<string, string>(errorLog, null), true);
                                    }
                                    else
                                    {
                                        RootPage.IsGestureEnabled = true;
                                        await RootPage.NavigateFromMenu((int)MenuItemType.Home);
                                    }
                                }
                                else
                                {
                                    UI.DisplayCustomAlert(returnVal2);
                                }
                            }
                            else
                            {
                                UI.TriggerInvalidUsernameAnimation();
                            } 
                        }
                        else
                        {
                            //Lütfen bekleyiniz
                        }
                        break;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
