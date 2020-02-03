using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeM8
{
    public enum RequestType
    {
        Post,
        Get
    }
    
    public enum ControllerType
    {
        Home,
        User,
        App
    }

    public static class Helper
    {
        public static async Task<T> ApiCall<T>(RequestType type, ControllerType controller, string actionName, string inputParams = null, bool IsSecure = true) where T : class
        {
            if (type == RequestType.Post && string.IsNullOrWhiteSpace(inputParams))
                throw new ArgumentNullException();
            else if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentNullException();
            else if (type == RequestType.Get && !string.IsNullOrWhiteSpace(inputParams))
                throw new ArgumentNullException();
            else if (string.IsNullOrWhiteSpace(Utility.User.accessToken) || Utility.SharedSecret == null)
                throw new InvalidOperationException();

            T response = default(T);

            string url = $"{Utility.BaseURL}/api/{controller}/{actionName}?username={Utility.User.Username}";

            switch (type)
            {
                case RequestType.Get:
                    response = JsonConvert
                        .DeserializeObject<T>(
                        await httpGetAsync(url, IsSecure));
                    break;
                case RequestType.Post:
                    response = JsonConvert
                        .DeserializeObject<T>(
                        await httpPostAsync(url, inputParams, IsSecure));
                    break;
            }
            return response;
        }

        #region httpPostAsync
        public static async Task<string> httpPostAsync(string url, string inputParams, bool IsSecure = false)
        {
            string response_val = "";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 1000 * 60 * 60 * 24;
            AESCrypt AES = default(AESCrypt);

            try
            {
                if (IsSecure)
                {
                    AES = new AESCrypt(Utility.SharedSecret, new byte[16]);
                    inputParams = AES.Encrypt(inputParams);
                }
                
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(inputParams);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var bufferStream = streamReader.ReadToEnd();

                    try
                    {
                        response_val = (IsSecure) ? AES.Decrypt(bufferStream) : bufferStream;
                    }
                    catch
                    {
                        response_val = (IsSecure) ? bufferStream : throw new Exception();
                    }
                }
            }
            catch(Exception e)
            {
                var asd = e.Message;
                response_val = JsonConvert.SerializeObject(new BaseResponseModel { responseText = "İnternet bağlantınız da sorun olabilir", responseVal = -3 });
            }
            return response_val;
        }
        #endregion

        #region httpGetAsync
        public static async Task<string> httpGetAsync(string url, bool IsSecure = false)
        {
            string responseVal = default(string);
            AESCrypt AES = default(AESCrypt);
            try
            {
                if (IsSecure)
                {
                    AES = new AESCrypt(Utility.SharedSecret, new byte[16]);
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseVal = (IsSecure) ? AES.Decrypt(streamReader.ReadToEnd()) : streamReader.ReadToEnd();
                }
            }
            catch
            {
                responseVal = JsonConvert.SerializeObject(new BaseResponseModel { responseText = "İnternet bağlantınız da sorun olabilir.", responseVal = -3 });
            }

            return responseVal;
        }
        #endregion

        public static async Task<string> GenericGetRequestAsync(string actionName,
            Dictionary<string, string> parameters,
            string controllerName = "home")
        {
            var requestUrl = $"{Utility.BaseURL}/api/{controllerName}/{actionName}";
            if (parameters != null)
            {
                requestUrl += "?";
                var Keys = new List<string>(parameters.Keys);

                for (int i = 0; i < Keys.Count; i++)
                {
                    if (i == Keys.Count - 1)
                    {
                        requestUrl += $"{Keys[i]}={parameters[Keys[i]]}";
                        continue;
                    }
                    requestUrl += $"{Keys[i]}={parameters[Keys[i]]}&";
                }
            }

            var result = await httpGetAsync(requestUrl);

            return result;
        }

        public static string ConvertCollectionToString<T>(List<T> element,char seperator)
        {
            string buffer = null;
            for(int i = 0; i < element.Count; i++)
            {
                if (i == element.Count - 1)
                {
                    buffer += $"{element[i]}";
                    continue;
                }
                buffer += $"{element[i]}{seperator}";
            }
            return buffer;
        }
    }
}
