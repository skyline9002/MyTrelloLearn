using Microsoft.Web.WebPages.OAuth;
using MyTrelloLearn.Web.Models;


namespace MyTrelloLearn.Web.App_Start
{
    public static class AuthConfig
    {

        public static void RegisterAuth()
        {
            // Create and register our custom Trello client as an OAuth client provider
            var trelloClient = new TrelloClient(
                consumerKey: AuthHelper.ApplicationKey,
                consumerSecret: AuthHelper.ApplicationSecret,
                appName: "My Trello Learn");
            OAuthWebSecurity.RegisterClient(trelloClient, "Trello", null);
        }

    }
}