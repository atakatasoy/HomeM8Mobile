using HomeM8.Models;
using HomeM8.Views.Account;
using HomeM8.Views.Home;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MenuPage menuPage;
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();
            menuPage = menu_page;
            
            Detail.BackgroundColor = (Color)Application.Current.Resources["PageWrapperColor"];
            MasterBehavior = MasterBehavior.Popover;
            using (var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
            {
                if (con.Table<User>().FirstOrDefault(each => each.LoggedIn) is User loggedUser)
                { 
                    EllipticCurve elliptic = new EllipticCurve(false);
                    Task.Run(() => elliptic.KeyExchange(loggedUser.Username)).Wait();
                    if (elliptic.IsSucceeded)
                    {
                        Utility.User = loggedUser;
                        Utility.SharedSecret = elliptic.SharedSecret;
                        var homePageKey= (int)MenuItemType.Home;
                        if (!MenuPages.ContainsKey(homePageKey)) MenuPages.Add(homePageKey, new NavigationPage(new HomePage()));
                        Detail = MenuPages[homePageKey];
                        IsGestureEnabled = true;
                    }
                }
                else IsGestureEnabled = false;
            }
            var staticClassInitiation = Utility.BaseURL;
        }
        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Home:
                        MenuPages.Add(id, new NavigationPage(new HomePage()));
                        break;
                    case (int)MenuItemType.ForgotPassword:
                        MenuPages.Add(id, new NavigationPage(new ForgotPasswordPage()));
                        break;
                    case (int)MenuItemType.Login:
                        MenuPages.Add(id, new NavigationPage(new LoginPage()));
                        break;
                    case (int)MenuItemType.Register:
                        MenuPages.Add(id, new NavigationPage(new RegisterPage()));
                        break;
                    case (int)MenuItemType.Account:
                        MenuPages.Add(id, new NavigationPage(new AccountPage()));
                        break;
                    case (int)MenuItemType.Logout:
                        break;
                }
            }

            if (id == (int)MenuItemType.Logout)
            {
                using(var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
                {
                    con.BeginTransaction();
                    var bufferUser = con.Table<User>().FirstOrDefault(each => each.Username == Utility.User.Username);
                    bufferUser.LoggedIn = false;
                    con.Update(bufferUser);
                    con.Commit();
                }
                IsPresented = false;
                IsGestureEnabled = false;
                if (MenuPages.ContainsKey((int)MenuItemType.Login)) Detail = MenuPages[(int)MenuItemType.Login];
                else Detail = new NavigationPage(new LoginPage());
                return;
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;
                
                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}