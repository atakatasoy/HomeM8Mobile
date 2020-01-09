using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8
{
    public class CalendarEventsResponseModel : BaseResponseModel
    {
        public List<CalendarEventModel> requestedEvents { get; set; }
    }
}
