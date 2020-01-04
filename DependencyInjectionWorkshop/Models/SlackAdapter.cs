﻿using SlackAPI;

namespace DependencyInjectionWorkshop.Models
{
    public interface INotification
    {
        void Notify(string accountId, string message);
    }

    public class Notification : INotification
    {
        public Notification()
        {
        }

        public void Notify(string accountId, string message)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(response1 => { }, "my channel", message, "my bot name");
        }
    }
}