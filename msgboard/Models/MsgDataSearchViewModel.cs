using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace msgboard.Models
{
    public class MsgDataSearchViewModel

    {
        public string SearchKeyword { get; set; }
        public List<msgdata> msgdatas { get; set; }
    }
}