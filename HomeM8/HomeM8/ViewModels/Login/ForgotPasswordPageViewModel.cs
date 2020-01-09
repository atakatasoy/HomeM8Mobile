using HomeM8.Models;
using HomeM8.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class ForgotPasswordPageViewModel : INotifyPropertyChanged
    {
        MainPage RootPage => Application.Current.MainPage as MainPage;

        public string Username { get; set; }

        public string ValidationCode{ get; set; }

        public string Error { get; set; }

        public string NewPassword { get; set; }

        public string NewPassword2 { get; set; }

        public ICommand NewPasswordCommand { get; set; }

        public ICommand SendCommand { get; set; }

        public ICommand SendValidationCommand { get; set; }

        private byte[] sharedSecret = null;

        public ForgotPasswordPage UI;
        public ForgotPasswordPageViewModel()
        {
            NewPasswordCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(NewPassword2))
                {
                    if (NewPassword == NewPassword2)
                    {
                        Utility.ShowIndicator = true;

                        AESCrypt AES = new AESCrypt(sharedSecret, new byte[16]);

                        var cipheredParameters = AES.Encrypt(JsonConvert.SerializeObject(new
                        {
                            NewPassword,
                            ValidationCode
                        }));

                        var newPasswordResponse = JsonConvert.DeserializeObject<BaseResponseModel>(
                            AES.Decrypt(await Helper.httpPostAsync($"{Utility.BaseURL}/api/user/SetNewPassword?username={Username}", cipheredParameters)));

                        if (newPasswordResponse.responseVal == 0)
                        {
                            Utility.SharedSecret = sharedSecret;

                            var serializedParameters = JsonConvert.SerializeObject(new { password = NewPassword });

                            var loginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(
                                await Helper.httpPostAsync($"{Utility.BaseURL}/api/user/login?username={Username}", serializedParameters, IsSecure: true));

                            if (loginResponse.responseVal==0)
                            {
                                using(var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
                                {
                                    var bufferUser = con.Table<User>().FirstOrDefault(each => each.accessToken == loginResponse.accessToken);
                                    string connectedHomes = Helper.ConvertCollectionToString(loginResponse.ConnectedHomes, ',');

                                    if (bufferUser != null)
                                    {
                                        con.BeginTransaction();
                                        bufferUser.LoggedIn = true;
                                        bufferUser.Username = Username;
                                        bufferUser.UserNameSurname = loginResponse.nameSurname;
                                        bufferUser.ConnectedHomes = connectedHomes;
                                        bufferUser.CurrentHome = loginResponse.CurrentHome;
                                        bufferUser.LastLoggedInDate = DateTime.Now;
                                        con.Update(bufferUser);
                                        con.Commit();
                                        Utility.User = bufferUser;
                                    }
                                    else
                                    {
                                        User user = new User()
                                        {
                                            accessToken = loginResponse.accessToken,
                                            LastLoggedInDate = DateTime.Now,
                                            LoggedIn = true,
                                            Username = Username,
                                            CurrentHome = loginResponse.CurrentHome,
                                            ConnectedHomes = connectedHomes,
                                            UserType = loginResponse.userType,
                                            UserNameSurname = loginResponse.nameSurname
                                        };
                                        con.Insert(user);
                                        Utility.User = user;
                                    }
                                }
                                RootPage.IsGestureEnabled = true;
                                await RootPage.NavigateFromMenu((int)MenuItemType.Home); 
                            }
                            else
                            {
                                UI.DisplayCustomAlert(new KeyValuePair<string, string>("Pass", loginResponse.responseText));
                            }
                        }
                        else
                        {
                            UI.DisplayCustomAlert(new KeyValuePair<string, string>("Pass", newPasswordResponse.responseText));
                        }
                        Utility.ShowIndicator = false;
                    }
                    else
                    {
                        UI.DisplayCustomAlert(new KeyValuePair<string, string>("Pass", "Parolarınız aynı değil"));
                    }
                }
                else
                {
                    UI.DisplayCustomAlert(new KeyValuePair<string, string>("Pass", "Parola alanı boş bırakılamaz"));
                }
            });

            SendValidationCommand = new Command(async () =>
            {
                int mValidationCode = 0;
                bool error = false;

                try
                {
                    mValidationCode = Convert.ToInt32(ValidationCode);
                }
                catch { error = true; }

                if (!string.IsNullOrWhiteSpace(ValidationCode))
                {
                    if (!error)
                    {
                        if (mValidationCode > 99999)
                        {
                            Utility.ShowIndicator = true;

                            EllipticCurve elliptic = new EllipticCurve(false);

                            await Task.Run(() =>
                            {
                                elliptic.KeyExchange(Username);
                            });

                            if (elliptic.IsSucceeded)
                            {
                                sharedSecret = elliptic.SharedSecret;

                                AESCrypt AES = new AESCrypt(sharedSecret, new byte[16]);

                                var cipheredParameters = AES.Encrypt(JsonConvert.SerializeObject(new
                                {
                                    ValidationCode = mValidationCode
                                }));

                                var validationVCodeResponse = JsonConvert.DeserializeObject<BaseResponseModel>(
                                    AES.Decrypt(await Helper.httpPostAsync($"{Utility.BaseURL}/api/user/ValidateVCode?username={Username}", cipheredParameters)));

                                Utility.ShowIndicator = false;

                                if (validationVCodeResponse.responseVal == 0)
                                {
                                    UI.CallNewPasswordStackLayout();
                                }
                                else
                                {
                                    UI.DisplayCustomAlert(new KeyValuePair<string, string>("Validation", validationVCodeResponse.responseText));
                                }
                            }
                        }
                        else
                        {
                            UI.DisplayCustomAlert(new KeyValuePair<string, string>("Validation", "Doğrulama kodu 6 haneli olmalıdır"));
                        }
                    }
                    else
                    {
                        UI.DisplayCustomAlert(new KeyValuePair<string, string>("Validation", "Doğrulama kodu sadece sayı olmalıdır"));
                    } 
                }
                else
                {
                    UI.DisplayCustomAlert(new KeyValuePair<string, string>("Validation", "Doğrulama kodu boş bırakılamaz"));
                }
            });

            SendCommand = new Command(async () =>
            {

                if (!string.IsNullOrWhiteSpace(Username))
                {
                    if (Username.Length > 3)
                    {
                        Utility.ShowIndicator = true;

                        var validationResponse = JsonConvert.DeserializeObject<BaseResponseModel>(await Helper.httpGetAsync($"{Utility.BaseURL}/api/user/GetValidationCode?username={Username}"));

                        Utility.ShowIndicator = false;

                        if (validationResponse.responseVal == 0)
                        {
                            UI.CallValidationStackLayout();
                        }
                        else
                        {
                            UI.DisplayCustomAlert(new KeyValuePair<string, string>("User", validationResponse.responseText));
                        }
                    }
                    else
                    {
                        UI.DisplayCustomAlert(new KeyValuePair<string, string>("User", "Kullanıcı adı 4 karakterden uzun olmalıdır"));
                    }
                }
                else
                {
                    UI.DisplayCustomAlert(new KeyValuePair<string, string>("User", "Kullanıcı adı boş bırakılamaz"));
                }
            });
        }

        internal void SetUI(Page forgotPasswordPage)
        {
            UI = (ForgotPasswordPage)forgotPasswordPage;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

    }
}
