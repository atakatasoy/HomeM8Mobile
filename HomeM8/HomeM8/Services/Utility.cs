using HomeM8.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace HomeM8
{
    public static class Utility
    {
        public static string UnexpectedMessage { get; internal set; } = "Beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

        public static string BaseURL { get; set; } = "http://192.168.1.26:5999";

        public static User User { get; set; } = new User();

        #region SecurityRelated
        public static byte[] SharedSecret { get; set; }

        //Hata verirse ctor da 2048 int
        public static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        public static string EncryptRSA(string plainText)
        {
            var plainBytes = Encoding.Unicode.GetBytes(plainText);

            var cypherBytes = RSA.Encrypt(plainBytes, true);

            return Convert.ToBase64String(cypherBytes);
        }

        public static string DecryptRSA(string cypherText)
        {
            var cypherBytes = Convert.FromBase64String(cypherText);

            var plainBytes = RSA.Decrypt(cypherBytes, true);

            return Encoding.Unicode.GetString(plainBytes);
        }

        public static bool VerifyDataRSA(string originalText,string signedText)
        {
            var originalBytes = Convert.FromBase64String(originalText);
            var signedBytes = Convert.FromBase64String(signedText);

            try
            {
                return RSA.VerifyData(originalBytes, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
            }
            catch
            {
                return false;
            }   
        }
        #endregion

        #region IndicatorRelated
        static bool IsIndicatorVisible = false;
        public static bool ShowIndicator
        {
            get
            {
                return IsIndicatorVisible;
            }
            set
            {
                if (value && !IsIndicatorVisible)
                {
                    IsIndicatorVisible = value;
                    Device.BeginInvokeOnMainThread(() => ShowTransparentElement(busyPage, nameof(busyPage),true));
                }
                if (!value && IsIndicatorVisible)
                {
                    IsIndicatorVisible = value;
                    Device.BeginInvokeOnMainThread(() => HideTransparentElement(nameof(busyPage)));
                }
            }
        }

        public static ContentPage busyPage; 
        #endregion

        static Utility()
        {
            //applanguage initiate
            busyPage = new ContentPage()
            {
                BackgroundColor = new Color(0, 0, 0, 0.5),
                Content = new StackLayout()
                {
                    Padding = 30,
                    Spacing = 20,
                    WidthRequest = 250,
                    BackgroundColor = Color.Transparent,
                    Children =
                            {
                                new Lottie.Forms.AnimationView()
                                {
                                    IsEnabled=true,
                                    Loop=true,
                                    IsPlaying=true,
                                    Animation="loading.json",
                                    Margin=new Thickness(15,0,15,0),
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                }
                            },
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                }
            };
            RSAParameters RSAKey = default(RSAParameters);

            Task.Run(() =>
            {
                var key = Helper.httpGetAsync($"http://192.168.1.26:5999/api/user/GetRSAPublicKey").Result;

                var sr = new System.IO.StringReader(key);

                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

                RSAKey = (RSAParameters)xs.Deserialize(sr);
            }).Wait();

            RSA.ImportParameters(RSAKey);
        }

        public static void ShowTransparentElement(ContentPage page, string name, bool InitiateFlag = false)
        {
            DependencyService.Get<ITransparancier>().Show(page, name, InitiateFlag: InitiateFlag);
        }
        public static void HideTransparentElement(string name)
        {
            DependencyService.Get<ITransparancier>().Hide(name);
        }

    }
}
