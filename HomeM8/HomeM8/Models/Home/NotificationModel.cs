using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class NotificationModel : INotifyPropertyChanged
    {
        public string OwnerNameSurname { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationName { get; set; }
        public string NotificationCommentCount { get; set; }
        public string NotificationType { get; set; }
        public string CreateDate { get; set; }
        public int ExpectedAnswerRange { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
