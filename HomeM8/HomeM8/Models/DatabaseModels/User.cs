using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace HomeM8
{
    public class User
    {
        [PrimaryKey]
        public string accessToken { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; }
        public string UserNameSurname { get; set; }
        public bool LoggedIn { get; set; }
        public string ConnectedHomes { get; set; }
        public DateTime LastLoggedInDate { get; set; }
        public int? CurrentHome { get; set; }

        public List<int> GetConnectedHomes() => ConnectedHomes?.Split(',').Select(each => Convert.ToInt32(each)).ToList();

        public void AddHome(int homeId)
        {
            var listOfHomes = ConnectedHomes.Split(',').ToList();
            listOfHomes.Add(homeId.ToString());
            SetValue(nameof(ConnectedHomes), Helper.ConvertCollectionToString(listOfHomes, ','));
        }

        public void LeaveHome()
        {
            var listOfHomes = ConnectedHomes.Split(',').ToList();
            listOfHomes.Remove(CurrentHome.ToString());

            CurrentHome = listOfHomes.Count > 0 ? new int?(Convert.ToInt32(listOfHomes.FirstOrDefault(e => true))) : null;

            SetValue(nameof(ConnectedHomes), Helper.ConvertCollectionToString(listOfHomes, ','));
        }

        public void SetValue<T>(string nameOfProp , T value)
        {
            var prop = GetType().GetProperties().FirstOrDefault(each => each.Name == nameOfProp);

            if (prop == null)
                throw new NullReferenceException();

            //It will throw expection if the T value is wrong type regarding to the nameOfProp
            var check = (T)prop.GetValue(this);

            using (var con = DependencyService.Get<IDatabase>().GetConnection(ConnectionType.Login))
            {
                prop.SetValue(this, value);

                con.BeginTransaction();
                con.Update(this);
                con.Commit();
            }
        }
    }
}
