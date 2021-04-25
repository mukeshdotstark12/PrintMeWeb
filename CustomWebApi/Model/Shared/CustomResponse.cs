using CustomWebApi.Model.Album;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Shared
{
    public class CustomResponse
    {
        public IEnumerable<object> data { get; set; }
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public string description { get; set; }
        public int SKUID { get; set; }
    }

    public class CustomResponse2
    {
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public string description { get; set; }
        public object data { get;set;}
        public int SKUID { get; set; }
    }
}
