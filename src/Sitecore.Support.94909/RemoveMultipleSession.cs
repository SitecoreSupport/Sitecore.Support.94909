namespace Sitecore.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.LoggedIn;
    using Sitecore.Web.Authentication;
    using System.Web;

    class RemoveMultipleSession
    {
        #region original code
        public void Process(LoggedInArgs args)
        {
            try
            {
                string userName = args.Username.ToLower();
                string currentSessionID = HttpContext.Current.Session.SessionID;

                // Using args.Username.ToLower() & s.Username.ToLower() due to capital issues
                foreach (DomainAccessGuard.Session item in from s in DomainAccessGuard.Sessions
                                                           where s.UserName.ToLower() == userName && s.SessionID != currentSessionID
                                                           select s)
                {
                    string sessionID = item.SessionID;

                    DomainAccessGuard.Kick(sessionID);
                    List<string> list = HttpContext.Current.Application["SessionsToKick"] as List<string>;
                    if (list == null)
                    {
                        list = new List<string>();
                        HttpContext.Current.Application["SessionsToKick"] = list;
                    }
                    list.Add(sessionID);

                    Log.Info($"The {sessionID} session of the {userName} user was kicked", (object)this);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString(), ex, (object)this);
            }
        }
        #endregion

        public RemoveMultipleSession()
        {

        }
    }
}