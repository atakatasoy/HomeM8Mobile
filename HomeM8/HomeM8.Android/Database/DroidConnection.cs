using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HomeM8.Droid.Database;
using HomeM8;
using SQLite;
using System.IO;
using HomeM8.Models;

[assembly: Xamarin.Forms.Dependency(typeof(DroidConnection))]
namespace HomeM8.Droid.Database
{
    public class DroidConnection : IDatabase
    {
        string loginFileFullPath;
        string configFileFullPath;
        public SQLiteConnection GetConnection(ConnectionType conType)
        {
            try
            {
                var fileName= "HomeM8DB.db";
                var configFileName = "AppConfiguration.db";
                var documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                loginFileFullPath = Path.Combine(documentPath, fileName);
                configFileFullPath = Path.Combine(documentPath, configFileName);
                if (conType == ConnectionType.AppConfig)
                {
                    if (!File.Exists(configFileFullPath))
                    {
                        using (var binaryReader = new BinaryReader(Android.App.Application.Context.Assets.Open(configFileName)))
                        {
                            using (var binaryWriter = new BinaryWriter(new FileStream(configFileFullPath, FileMode.Create)))
                            {
                                byte[] buffer = new byte[2048];
                                int length = 0;
                                while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    binaryWriter.Write(buffer, 0, length);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!File.Exists(loginFileFullPath))
                    {
                        using (var binaryReader = new BinaryReader(Android.App.Application.Context.Assets.Open(fileName)))
                        {
                            using (var binaryWriter = new BinaryWriter(new FileStream(loginFileFullPath, FileMode.Create)))
                            {
                                byte[] buffer = new byte[2048];
                                int length = 0;
                                while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    binaryWriter.Write(buffer, 0, length);
                                }
                            }
                        }
                    }
                }

                var con = new SQLiteConnection((conType == ConnectionType.Login) ? loginFileFullPath : configFileFullPath);
                return con;
            }
            catch
            {
                return null;
            }
        }
    }
}