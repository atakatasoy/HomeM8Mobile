using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HomeM8.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XFPlatform = Xamarin.Forms.Platform.Android.Platform;
using HomeM8.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(TransparentPage))]
namespace HomeM8.Droid
{
    public class TransparentPage : ITransparancier
    {
        Dictionary<KeyValuePair<string, Dialog>, bool> dialogDic = new Dictionary<KeyValuePair<string, Dialog>, bool>();

        public void Hide(string name)
        {
            List<KeyValuePair<string, Dialog>> dialogList = new List<KeyValuePair<string, Dialog>>(dialogDic.Keys);

            for (int i = 0; i < dialogList.Count; i++)
            {
                if (dialogList[i].Key == name)
                {
                    if (dialogDic[dialogList[i]])
                    {
                        dialogList[i].Value.Dismiss();
                        dialogDic[dialogList[i]] = false;
                        return;
                    }
                }
            }
        }

        public void Show(ContentPage page, string name, bool InitiateFlag)
        {
            if (Xamarin.Forms.Application.Current.MainPage != null)
            {
                page.Parent = Xamarin.Forms.Application.Current.MainPage;
            }
            else return;

            List<KeyValuePair<string, Dialog>> kvpList = new List<KeyValuePair<string, Dialog>>(dialogDic.Keys);

            for (int i = 0; i < kvpList.Count; i++)
            {
                if (kvpList[i].Key == name)
                {
                    if (InitiateFlag)
                    {
                        dialogDic.Remove(kvpList[i]);
                        kvpList[i].Value.Dispose();
                        break;
                    }
                    if (dialogDic[kvpList[i]]) return;
                    kvpList[i].Value.Show();
                    dialogDic[kvpList[i]] = true;
                    return;
                }
            }

            // Run the Layout Rendering Cycle for the page
            page.Layout(new Rectangle(0, 0,
            Xamarin.Forms.Application.Current.MainPage.Width,
            Xamarin.Forms.Application.Current.MainPage.Height));
            // Get the native renderered instance for our page
            var nativePageRendererInstance = page.GetOrCreateRenderer((name == "busyPage") ? true : false);
            bool ShouldCancel = (name == "changePage") ? true : false;
            // Get the native page for our page
            Android.Views.View nativePageView = nativePageRendererInstance.View;
            if (nativePageView.Parent != null)
            {
                nativePageView.RemoveFromParent();
            }
            // Create the native transparent Dialog instance to embed our page
            Dialog dialog = new Dialog(CrossCurrentActivity.Current.Activity);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetCancelable(ShouldCancel);
            dialog.SetCanceledOnTouchOutside(ShouldCancel);
            dialog.SetContentView(nativePageView);
            Window window = dialog.Window;
            window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            window.ClearFlags(WindowManagerFlags.DimBehind);
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            dialogDic.Add(new KeyValuePair<string, Dialog>(name, dialog), true);

            dialog.Show();
        }
    }
    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable,bool deneme=false)
        {
            var renderer = XFPlatform.GetRenderer(bindable);
            if (renderer == null||deneme)
            {
                if (renderer != null)
                {
                    renderer.Dispose();
                    //renderer = null;
                }
                renderer = XFPlatform.CreateRendererWithContext(bindable, CrossCurrentActivity.Current.Activity);
                XFPlatform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}