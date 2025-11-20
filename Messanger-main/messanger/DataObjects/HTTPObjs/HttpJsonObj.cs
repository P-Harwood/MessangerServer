using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects.HTTPObjs
{
    public class HttpJsonObj
    {
        public bool successfull { get; set; } = false;
        public HttpStatusCode responseCode { set; get; }
        public string jsonRespnse {  set; get; }
    }
}
