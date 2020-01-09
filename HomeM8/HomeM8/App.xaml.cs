using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HomeM8.Views;
using HomeM8.Models;
using System.ComponentModel;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HomeM8
{
    public partial class App : Application
    {   
        MainPage RootPage => Current.MainPage as MainPage;

        public App()
        {
            InitializeComponent();
            AppConfigurationModel.SetColors();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
