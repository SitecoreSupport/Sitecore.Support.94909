using Sitecore.Pipelines.LoggedIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
    public class RemoveMultipleSession : LoggedInProcessor
    {
        public override void Process(LoggedInArgs args)
        {
            try
            {
                var userName = args.Username;
                var currentSessionID = HttpContext.Current.Session.SessionID;
                var userSessions = Sitecore.Web.Authentication.DomainAccessGuard.Sessions.Where(s => s.UserName == userName && s.SessionID != currentSessionID);
                foreach (var session in userSessions)
                {
                    var sessionId = session.SessionID;
                    Sitecore.Web.Authentication.DomainAccessGuard.Kick(sessionId);
                    var sessions = HttpContext.Current.Application["SessionsToKick"] as List<string>;

                    if (sessions == null)
                    {
                        sessions = new List<string>();
                        HttpContext.Current.Application["SessionsToKick"] = sessions;
                    }

                    sessions.Add(sessionId);

                    Sitecore.Diagnostics.Log.Info(string.Format("The {0} session of the {1} user was kicked", sessionId, userName), this);
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.ToString(), ex, this);
            }
        }
    }
}