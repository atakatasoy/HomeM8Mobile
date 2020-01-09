using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8
{
    public class LoginResponseModel : BaseResponseModel
    {
        public string accessToken { get; set; }
        public string nameSurname { get; set; }
        public int? CurrentHome { get; set; }
        public List<int> ConnectedHomes { get; set; }
        public int userType { get; set; }
    }
}
