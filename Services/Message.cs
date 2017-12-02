using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptLink
{
    public class Message
    {
        public DateTime Sent { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string SenderAlias { get; set; }
		public Dictionary<string, Hash> Attachments { get; set; }
        
    }
}
