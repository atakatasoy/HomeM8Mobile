using Newtonsoft.Json;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        HomePageViewModel viewModel;
        CultureInfo desiredCulture = null;
        int CurLanguage;
        public HomePage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = new HomePageViewModel();
            SetCalendarConfigurations();
            calender.MonthChanged += (sender, args) => 
            {
                monthLabel.Text = $"{desiredCulture.DateTimeFormat.GetMonthName(calender.MoveToDate.Month)} {args.CurrentValue.Year}";
            };
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.SetNotifications();
            await viewModel.SetCalendarEvents();
            viewModel.CheckCurrentHome();
            if (LanguageConfigChanged()) SetCalendarConfigurations();
        }
        bool LanguageConfigChanged()
        {
            return CurLanguage != AppConfigurationModel.CurrentLanguage;
        }
        void SetCalendarConfigurations()
        {
            if (AppConfigurationModel.CurrentLanguage == 1)
            {
                calender.FirstDayofWeek = 1;
                desiredCulture = new CultureInfo("tr-TR");
            }
            else
            {
                calender.FirstDayofWeek = 0;
                desiredCulture = new CultureInfo("en-US");
            }
            CurLanguage = AppConfigurationModel.CurrentLanguage;
            calender.Locale = desiredCulture;
            CalendarResourceManager.Manager = new System.Resources.ResourceManager("HomeM8.Resources.Syncfusion.SfCalendar.Forms", GetType().GetTypeInfo().Assembly);
            monthLabel.Text = $"{desiredCulture.DateTimeFormat.GetMonthName(calender.MoveToDate.Month)} {calender.MoveToDate.Year}";
        }
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            notificationBell.Play();
        }
    }
}