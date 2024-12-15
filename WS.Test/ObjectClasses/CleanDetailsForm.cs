using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Test.ObjectClasses
{
    class CleanDetailsForm
    {
        public string Result { get; set; } = "Error";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
