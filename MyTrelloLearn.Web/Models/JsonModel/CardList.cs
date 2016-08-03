using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models.JsonModel
{
    public class CardList
    {
        public string id { get; set; }
        public string name { get; set; }
        public string idBoard { get; set; }
        public Card[] cards { get; set; }
    }
}