using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class RegisterPageTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public string UsernameEntryPlaceholderString { get; set; }
        public string PasswordEntryPlaceholderString { get; set; }
        public string RepeatPasswordEntryPlaceholderString { get; set; }
        public string EmailEntryPlaceholderString { get; set; }
        public string PageTitle { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonText2 { get; set; }
        public string NameSurnameEntryPlaceholderString { get; set; }
        public string PhoneEntryPlaceholderString { get; set; }
    }
}
