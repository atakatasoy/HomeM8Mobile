using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views.PartialView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPopup : PopupPage
	{
		public ErrorPopup ()
		{
			InitializeComponent ();
            BackgroundColor = new Color(0, 0, 0, 0.5);
            CloseWhenBackgroundIsClicked = false;
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}