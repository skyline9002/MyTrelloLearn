using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models.JsonModel
{
    public class Action
    {
        public string id { get; set; }
        public ActionData data { get; set; }
        public string type { get; set; }
        public string date { get; set;}
        public Member memberCreator { get; set; }
    }

    public class ActionData
    {
        public string text { get; set; }
    }

    public class Member
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public string username { get; set; }
    }

    public class CommentAction
    {
        public string IdCard { get; set; }
        public string AuthorName { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public string Text { get; set; }
    }
}