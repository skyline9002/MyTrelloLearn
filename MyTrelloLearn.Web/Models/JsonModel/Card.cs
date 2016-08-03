using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models.JsonModel
{
    public class Card
    {
        public string id { get; set; }
        public string dateLastActivity { get; set; }
        public string name { get; set; }
        public bool closed { get; set; }
        public Badges badges { get; set; }
        public string idlist { get; set; }
        public string idboard { get; set; }
        public string listname { get; set; }
        public IEnumerable<Action> actions { get; set; }
    }

    public class Badges
    {
        public int comments { get; set; }
    }
}