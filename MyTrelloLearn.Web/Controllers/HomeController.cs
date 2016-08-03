using MyTrelloLearn.Web.Filters;
using MyTrelloLearn.Web.Models;
using MyTrelloLearn.Web.Models.JsonModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyTrelloLearn.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        //GET /Home/Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        //Post /Home/Preload
        /// <summary>
        /// Preload data for index page.
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult Preload()
        {
            try
            {
                using (var db = new ModelContext())
                {
                    var userProfile = db.UserProfiles.Include("ExtraData").FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                    var userId = userProfile.ExtraData.FirstOrDefault(a => a.Key == "id").Value;
                    var token = userProfile.ExtraData.FirstOrDefault(a => a.Key == "accesstoken").Value;
                    this.Session["token"] = token;
                    var boards = RequestAllBoards(userId);
                    return Json(new
                    {
                        UserName = this.User.Identity.Name,
                        Boards = boards
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    ErrorMessage = "Sorry, Error Occurred, Please Try Again Latter."
                });
            }
        }

        //Post /Home/SearchCards?boardId=[boardId]
        [HttpPost]
        public JsonResult SearchCards(string boardId)
        {
            try
            {
                var cardList = RequestCardsByBoardId(boardId);
                return Json(new { 
                    CardList = cardList
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    ErrorMessage = "Sorry, Error Occurred, Please Try Again Latter."
                });
            }
        }

        //Post /Home/SearchComments?cardId=[cardId]
        [HttpPost]
        public JsonResult SearchComments(string cardId)
        {
            try
            {
                var comments = RequestCommentsByCarId(cardId);
                return Json(new
                {
                    Comments = comments,
                    CardId = cardId
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    ErrorMessage = "Sorry, Error Occurred, Please Try Again Latter."
                });
            }
        }

        //Post /Home/PostComments?cardId=[cardId]&text=[text]
        [HttpPost]
        public JsonResult PostComments(string cardId,string text)
        {
            try
            {
                var action = PostCommentByCarId(cardId, text);
                return Json(new
                {
                    Action = action,
                    IsSuccess = true,
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = "Sorry, Error Occurred, Please Try Again Latter."
                });
            }
        }


        /// <summary>
        /// Request all boards from particular member.
        /// </summary>
        /// <param name="userId">memberId</param>
        /// <returns>list of boards</returns>
        private IEnumerable<Board> RequestAllBoards(string userId)
        {
            string baseUrl = "https://api.trello.com/1/members/{0}/boards?key={1}&token={2}&fields=name";
            HttpWebRequest request = WebRequest.CreateHttp(string.Format(baseUrl, userId, AuthHelper.ApplicationKey, this.Session["token"]));
            var response  =request.GetResponse();
            string result = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            IEnumerable<Board> boards =  JsonConvert.DeserializeObject<IEnumerable<Board>>(result);
            return boards;
        }

        /// <summary>
        /// Request all cards from particular board.
        /// </summary>
        /// <param name="boardId">boardId</param>
        /// <returns>list of cards</returns>
        private IEnumerable<Card> RequestCardsByBoardId(string boardId)
        {
            string baseUrl = "https://api.trello.com/1/boards/{0}/lists?fields=name,idBoard&cards=open&card_fields=badges,dateLastActivity,name&key={1}&token={2}";
            HttpWebRequest request = WebRequest.CreateHttp(string.Format(baseUrl, boardId, AuthHelper.ApplicationKey, this.Session["token"]));
            var response = request.GetResponse();
            string result = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            IEnumerable<CardList> cardList = JsonConvert.DeserializeObject<IEnumerable<CardList>>(result);

            List<Card> returnList = new List<Card>();

            foreach (var list in cardList)
            {
                foreach (var card in list.cards)
                {
                    card.idboard = list.idBoard;
                    card.idlist = list.id;
                    card.listname = list.name;
                    //convert to shorter string.
                    var date = DateTime.Parse(card.dateLastActivity);
                    card.dateLastActivity = date.ToString("yyyy-MM-dd");
                    returnList.Add(card);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Request all comments from particular card
        /// </summary>
        /// <param name="cardId">cardId</param>
        /// <returns>List of Comment Action</returns>
        private IEnumerable<CommentAction> RequestCommentsByCarId(string cardId)
        {

            string baseUrl = "https://api.trello.com/1/cards/{0}?fields=name,idList&member_fields=fullName&actions=commentCard&key={1}&token={2}";

              HttpWebRequest request = WebRequest.CreateHttp(string.Format(baseUrl, cardId, AuthHelper.ApplicationKey, this.Session["token"]));
              var response = request.GetResponse();
              string result = string.Empty;
              using (var stream = response.GetResponseStream())
              {
                  using (var reader = new StreamReader(stream))
                  {
                      result = reader.ReadToEnd();
                  }
              }
              Card card = JsonConvert.DeserializeObject<Card>(result);

              var resultList = new List<CommentAction>();
             
              foreach (var commentAction in card.actions)
              {
                  if (commentAction.type == "commentCard")
                  {
                      var item = new CommentAction();
                      item.AuthorName = commentAction.memberCreator.fullName;
                      item.Date = DateTime.Parse(commentAction.date);
                      item.DateStr = item.Date.ToString();
                      item.Text = commentAction.data.text;
                      item.IdCard = card.id;
                      resultList.Add(item);
                  }
              }

              return resultList;
        }

        /// <summary>
        /// Add new comment to particular card
        /// </summary>
        /// <param name="carId">cardId</param>
        /// <param name="text">text to add</param>
        /// <returns>added comment</returns>
        private CommentAction PostCommentByCarId(string carId, string text)
        {
            string baseUrl = "https://api.trello.com/1/cards/{0}/actions/comments?key={1}&token={2}&text={3}";
            HttpWebRequest request = WebRequest.CreateHttp(string.Format(baseUrl, carId, AuthHelper.ApplicationKey, this.Session["token"],text));
            request.Method = "POST";
            var response = request.GetResponse();
            string result = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            MyTrelloLearn.Web.Models.JsonModel.Action action = JsonConvert.DeserializeObject<MyTrelloLearn.Web.Models.JsonModel.Action>(result);

            return new CommentAction()
            {
                 AuthorName = action.memberCreator.fullName,
                 Date = DateTime.Parse(action.date),
                 DateStr = DateTime.Parse(action.date).ToString(),
                 Text = action.data.text,
            };            
        
        }

    }
}