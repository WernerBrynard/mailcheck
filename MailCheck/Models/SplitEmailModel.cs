using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.Models
{
    public class SplitEmailModel
    {
        public string TopLevelDomain { get; set; }
        public string SecondLevelDomain { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public string Address { get; set; }
    }
}
