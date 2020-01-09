using HomeM8.Models;
using HomeM8.Views;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeM8
{
    public class AppConfigurationModel : INotifyPropertyChanged
    {
        public static AppConfigurationModel Instance { get; set; } = new AppConfigurationModel();

        public DecidePageTemplate DecidePageContent { get; set; }
        public HomePageTemplate HomePageContent { get; set; }
        public LoginPageTemplate LoginPageContent { get; set; }
        public ForgotPasswordPageTemplate ForgotPasswordPageContent { get; set; }
        public AppColors AppColorConfiguration { get; set; }
        public RegisterPageTemplate RegisterPageContent { get; set; }
        public AccountPageTemplate AccountPageContent { get; set; }
        public AppTemplate ApplicationContent { get; set; }

        public static int CurrentLanguage { get; set; }
        public static int DefaultLanguage { get; set; } = 1;
        static bool Busy { get; set; }

        public static void ChangeConfiguration(int id)
        {
            if (!Busy)
            {
                Busy = true;
                Utility.ShowIndicator = true;
                if (id != CurrentLanguage)
                {
                    AppConfigurationModel buffer = default(AppConfigurationModel);
                    bool success = false;

                    using (var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.AppConfig))
                    {
                        success = TryToUpdateCurrentConfigInDb(out buffer, con, new AppConfig { ConfigInt = id });
                    }
                    if (success)
                    {
                        CurrentLanguage = id;
                        SetConfigurations(buffer);
                        SetColors();
                    }
                    else
                    {
                        (Application.Current.MainPage as MainPage).DisplayAlert("HomeM8", "İnternet bağlantınızda sorun olabilir", "Tamam");
                    }
                }
                Busy = false;
                Utility.ShowIndicator = false;
            }
        }

        public static void Init()
        {
            AppConfigurationModel configModel = null;
            AppConfig dbRow = null;
            try
            {
                using(var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.AppConfig))
                {
                    dbRow = con.Table<AppConfig>().FirstOrDefault(each => true);

                    if (TryToUpdateCurrentConfigInDb(out configModel, con, dbRow)) { }
                    else
                    {
                        configModel = JsonConvert
                            .DeserializeObject<AppConfigurationModel>(dbRow.ApplicationConfiguration);
                    }
                    CurrentLanguage = dbRow.ConfigInt;
                    SetConfigurations(configModel);
                }
            }
            catch(Exception e)
            {
                //wtf
                var mes = e.Message;
            }
        }

        static void SetConfigurations(AppConfigurationModel configModel)
        {
            Instance.DecidePageContent = configModel.DecidePageContent;
            Instance.AppColorConfiguration = configModel.AppColorConfiguration;
            Instance.ForgotPasswordPageContent = configModel.ForgotPasswordPageContent;
            Instance.LoginPageContent = configModel.LoginPageContent;
            Instance.RegisterPageContent = configModel.RegisterPageContent;
            Instance.HomePageContent = configModel.HomePageContent;
            Instance.ApplicationContent = configModel.ApplicationContent;
            Instance.AccountPageContent = configModel.AccountPageContent;
        }

        static bool TryToUpdateCurrentConfigInDb(out AppConfigurationModel bufferConfigModel,SQLiteConnection con,AppConfig dbRow)
        {
            AppConfigResponseModel buffer = null;

            Task.Run(() => buffer = JsonConvert.DeserializeObject<AppConfigResponseModel>(Helper
                .httpGetAsync($"{Utility.BaseURL}/api/app/GetAppConfiguration?id={dbRow.ConfigInt}").Result)).Wait();

            if (buffer.responseVal == 0)
            {
                con.Table<AppConfig>().Delete(each => true);

                con.Insert(new AppConfig() { ConfigInt = dbRow.ConfigInt, ApplicationConfiguration = buffer.languageJsonString });

                bufferConfigModel = JsonConvert.DeserializeObject<AppConfigurationModel>(buffer.languageJsonString);
                return true;
            }
            else
            {
                bufferConfigModel = null;
                return false;
            }
        }

        public static void SetColors()
        {
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.AppInfoStringsColor)] = Color.FromHex(Instance.AppColorConfiguration.AppInfoStringsColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.ButtonColor)] = Color.FromHex(Instance.AppColorConfiguration.ButtonColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.ButtonTextColor)] = Color.FromHex(Instance.AppColorConfiguration.ButtonTextColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.InputFrameBorderColor)] = Color.FromHex(Instance.AppColorConfiguration.InputFrameBorderColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.LoginEntryBackground)] = Color.FromHex(Instance.AppColorConfiguration.LoginEntryBackground);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.PageWrapperColor)] = Color.FromHex(Instance.AppColorConfiguration.PageWrapperColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.NavigationPrimary)] = Color.FromHex(Instance.AppColorConfiguration.NavigationPrimary);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.GridLinesColor)] = Color.FromHex(Instance.AppColorConfiguration.GridLinesColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.MoneyColor)] = Color.FromHex(Instance.AppColorConfiguration.MoneyColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.NotificationOwnerTextColor)] = Color.FromHex(Instance.AppColorConfiguration.NotificationOwnerTextColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.WarningColor)] = Color.FromHex(Instance.AppColorConfiguration.WarningColor);
            Application.Current.Resources[nameof(Instance.AppColorConfiguration.PermissionColor)] = Color.FromHex(Instance.AppColorConfiguration.PermissionColor);
            //((Style)Application.Current.Resources["navigationPage"]).Setters.FirstOrDefault(each => each.Property.PropertyName == "BarBackgroundColor").Value = Color.FromHex(Instance.AppColorConfiguration.NavigationPrimary);
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
