using HomeM8.Models;
using HomeM8.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class BottomTabbarViewModel : INotifyPropertyChanged
    {
        public static BottomTabbarViewModel Instance { get; set; } = new BottomTabbarViewModel();

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        MainPage RootPage => Application.Current.MainPage as MainPage;
        public bool BottomTabbarVisible { get; set; } = false;
        public ICommand BottomTabbarCommand { get; set; }

        public BottomTabbarViewModel()
        {
            BottomTabbarCommand = new Command(async (parameter) =>
            {
                var choice = (int)parameter;
                switch (choice)
                {
                    case 0:
                        var homePage = (int)MenuItemType.Home;
                        await RootPage.NavigateFromMenu(homePage);
                        RootPage.menuPage.ChangeSelectedItemWithoutForwarding(homePage);
                        break;
                    case 1:
                        var accountPage = (int)MenuItemType.Account;
                        await RootPage.NavigateFromMenu(accountPage);
                        RootPage.menuPage.ChangeSelectedItemWithoutForwarding(accountPage);
                        break;
                }
            });
        }
    }
}
