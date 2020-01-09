using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class LoginPageTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public string UsernameEntryPlaceholderString { get; set; }
        public string PasswordEntryPlaceholderString { get; set; }
        public string LoginButtonString { get; set; }
        public string ForgotPasswordButtonString { get; set; }
        public string RegisterButtonString { get; set; }
    }
}
