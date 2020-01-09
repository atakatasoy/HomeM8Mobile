using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeM8
{
    public class NotificationsResponseModel : BaseResponseModel
    {
        public ObservableCollection<NotificationModel> notificationsList { get; set; }
    }
}
