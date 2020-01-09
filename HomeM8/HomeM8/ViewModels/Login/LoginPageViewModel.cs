using HomeM8.Models;
using HomeM8.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public MainPage MainPage => (Application.Current.MainPage as MainPage);

        public string Username { get; set; }

        public string Error { get; set; }

        public string Password { get; set; }

        public ICommand LoginCommand { get; set; }

        public byte[] SharedSecret { get; set; }

        LoginPage UI;

        public void SetUI(Page Ui) => UI = (LoginPage)Ui;

        public LoginPageViewModel()
        {
            using (var con=DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
            {
                if(con.Table<User>().OrderByDescending(each => each.LastLoggedInDate).FirstOrDefault(each => true) is User potentialUser)
                {
                    Username = potentialUser.Username;
                }
            }
            LoginCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    Error = "Kullanıcı adı boş olamaz";
                    UI.DisplayCustomAlert(nameof(Username));
                    return;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        Error = "Şifre boş olamaz";
                        UI.DisplayCustomAlert(nameof(Password));
                        return;
                    }
                }

                Utility.ShowIndicator = true;

                EllipticCurve elliptic = new EllipticCurve(false);

                KeyValuePair<byte[], EstablishSharedSecretResponseModel> response = default(KeyValuePair<byte[], EstablishSharedSecretResponseModel>);
                
                await Task.Run(() => response = elliptic.KeyExchange(Username));

                if (elliptic.IsSucceeded)
                {
                    Utility.SharedSecret = elliptic.SharedSecret;

                    var parameters = new
                    {
                        Password
                    };

                    var serializedParameters = JsonConvert.SerializeObject(parameters);

                    var loginResponse = await Helper.httpPostAsync($"{Utility.BaseURL}/api/user/login?username={Username}", serializedParameters, IsSecure: true);

                    Utility.ShowIndicator = false;

                    try
                    {
                        var plainLoginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(loginResponse);

                        if (plainLoginResponse.responseVal == 0)
                        {
                            Password = "";
                            using(var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
                            {
                                var bufferUser = con.Table<User>().FirstOrDefault(each => each.accessToken == plainLoginResponse.accessToken);
                                string connectedHomes = Helper.ConvertCollectionToString(plainLoginResponse.ConnectedHomes, ',');

                                if (bufferUser != null)
                                {
                                    con.BeginTransaction();
                                    bufferUser.LoggedIn = true;
                                    bufferUser.UserType = plainLoginResponse.userType;
                                    bufferUser.UserNameSurname = plainLoginResponse.nameSurname;
                                    bufferUser.ConnectedHomes = connectedHomes;
                                    bufferUser.CurrentHome = plainLoginResponse.CurrentHome;
                                    bufferUser.LastLoggedInDate = DateTime.Now;
                                    con.Update(bufferUser);
                                    con.Commit();
                                    Utility.User = bufferUser;
                                }
                                else
                                {
                                    var user = new User()
                                    {
                                        accessToken = plainLoginResponse.accessToken,
                                        LastLoggedInDate = DateTime.Now,
                                        ConnectedHomes=connectedHomes,
                                        LoggedIn = true,
                                        CurrentHome = plainLoginResponse.CurrentHome,
                                        Username = Username,
                                        UserNameSurname = plainLoginResponse.nameSurname,
                                        UserType = plainLoginResponse.userType
                                    };
                                    con.Insert(user);
                                    Utility.User = user;
                                }
                            }
                            MainPage.IsGestureEnabled = true;
                            await MainPage.NavigateFromMenu((int)MenuItemType.Home);
                        }
                        else
                        {
                            Error = plainLoginResponse.responseText;
                            UI.DisplayCustomAlert(nameof(Username));
                        }
                    }
                    catch(Exception e)
                    {
                        await UI.DisplayAlert("Homem8", e.Message, "Tamam");
                    }
                }
                else
                {
                    Utility.ShowIndicator = false;
                    Error = response.Value.responseText;
                    UI.DisplayCustomAlert(nameof(Username));
                }
            });
        }
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void ChangeConfiguration(int id)
        {
            AppConfigurationModel.ChangeConfiguration(id);
        }
    }
}
