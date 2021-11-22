using System;
using System.Web;

namespace BasePlus.Common.DTO
{
    public class RestResult<T>
    {
        public bool IsSuccessful { get; set; }
        public string TimeElapsed { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}
