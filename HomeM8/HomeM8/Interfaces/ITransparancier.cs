using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeM8.Interfaces
{
    public interface ITransparancier
    {
        void Hide(string name);
        void Show(ContentPage page, string name, bool InitiateFlag);
    }
}
