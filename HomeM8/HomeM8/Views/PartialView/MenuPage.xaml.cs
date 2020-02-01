using HomeM8.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView GetMenuListView => ListViewMenu;
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        bool ExternalAccess = false;

        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem{Id=MenuItemType.Home,Title="Home"},
                new HomeMenuItem{Id=MenuItemType.Account,Title="Account"},
                new HomeMenuItem{Id=MenuItemType.Logout,Title="Logout"}
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ExternalAccess)
                {
                    ExternalAccess = false;
                    return;
                }
                if (e.SelectedItem == null)
                {
                    return;
                }
                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                if (id == (int)MenuItemType.Logout)
                {
                    ListViewMenu.SelectedItem = null;
                }
                await RootPage.NavigateFromMenu(id);
            };
        }
        public void ChangeSelectedItemWithoutForwarding(int id)
        {
            ExternalAccess = true;
            ListViewMenu.SelectedItem = menuItems.Find(each => (int)each.Id == id) ?? null;
        }
    }
}