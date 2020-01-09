using HomeM8.Models.Home;
using Newtonsoft.Json;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeM8
{   
    public class HomePageViewModel : INotifyPropertyChanged
    {
        Random rnd = new Random(); 
        public ObservableCollection<NotificationModel> ItemList { get; set; } = new ObservableCollection<NotificationModel>();
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public CalendarEventCollection CalendarInlineEvents { get; set; } = new CalendarEventCollection();

        bool mNoConnectedHomeToAccount = true;
        public bool NoConnectedHomeToAccount
        {
            get
            {
                return mNoConnectedHomeToAccount;
            }
            set
            {
                if (mNoConnectedHomeToAccount != value)
                {
                    mNoConnectedHomeToAccount = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(NoConnectedHomeToAccount)));
                    BottomTabbarViewModel.Instance.BottomTabbarVisible = !value;
                }
            }
        }

        public void CheckCurrentHome()
        {
            if (Utility.User.CurrentHome != null)
            {
                NoConnectedHomeToAccount = false;
            }
            else
            {
                NoConnectedHomeToAccount = true;
            }
        }

        public async Task SetNotifications()
        {
            var response = await Helper.ApiCall<NotificationsResponseModel>(RequestType.Post, ControllerType.Home, "getnotifications", JsonConvert.SerializeObject(new
            {
                AccessToken = Utility.User.accessToken,
                HomeID = Utility.User.CurrentHome
            }), IsSecure: true);

            ItemList = new ObservableCollection<NotificationModel>();

            if (response.responseVal == 0)
            {
                ItemList = response.notificationsList;
            }
            else
            {
                //show error
            }
        }

        public async Task SetCalendarEvents()
        {
            var response = await Helper.ApiCall<CalendarEventsResponseModel>(RequestType.Post, ControllerType.Home, "getcalendarevents", JsonConvert.SerializeObject(new
            {
                AccessToken = Utility.User.accessToken,
                HomeID = Utility.User.CurrentHome
            }), IsSecure: true);

            CalendarInlineEvents = new CalendarEventCollection();

            if (response.responseVal == 0)
            {
                foreach(var each in response.requestedEvents)
                {
                    DateTime eachDate = Convert.ToDateTime(each.ExpectedDate);
                    DateTime shouldEnd = new DateTime(eachDate.Year, eachDate.Month, eachDate.Day, 23, 59, 59);
                    CalendarInlineEventModel item = new CalendarInlineEventModel()
                    {
                        StartTime = eachDate,
                        EndTime = shouldEnd,
                        Subject = each.EventExplanation,
                        PayerName = each.PayerName,
                        PaymentAmount = each.PaymentAmount,
                        Color = Color.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)),
                        Paid = each.Paid
                    };
                    CalendarInlineEvents.Add(item);
                }
            }
            else
            {
                //show error
            }
        }
    }
}
