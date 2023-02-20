using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Mind.Business.Data
{
    public class LogData
    {
        public string Message { get; set; }
        public string email { get; set; }
        public string action { get; set; }
        public DateTime logDate { get; set; }
    }
}
