using System;
using System.Web;

namespace BasePlus.Common.API
{
    public class UpdateWithDateViewModel
    {
        // DateTime dateStart, [FromBody] DateTime dateEnd, [FromBody] DateTime newDate
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public DateTime newDate { get; set; }
    }
}
