using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8.Models
{
    public enum MenuItemType
    {
        Home,
        Login,
        Register,
        ForgotPassword,
        Logout,
        Account
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
