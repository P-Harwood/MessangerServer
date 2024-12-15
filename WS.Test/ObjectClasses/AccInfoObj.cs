using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Test.ObjectClasses
{
    internal class AccInfoObj
    {
        public string Result { get; set; } = "Error";

        public string? username { get; set; }
        public int? userId { get; set; }
        public byte[]? passwordSalt { get; set; }   
        public byte[]? passwordHash { get; set; }   

        public string? errorMessage { get; set; }

    }
}
