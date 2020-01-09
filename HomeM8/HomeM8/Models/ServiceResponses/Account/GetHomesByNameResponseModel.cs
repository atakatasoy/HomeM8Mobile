using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8
{
    public class GetHomesByNameResponseModel : BaseResponseModel
    {
        public List<GetHomesByNameModel> requestedHomes { get; set; }
    }
}
