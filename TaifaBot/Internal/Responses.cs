using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaifaBot.Internal
{
    internal static class Responses
    {
        public const string Features =
            "* Answer questions about Taifa: Try 'Taifa'\n\n"
            + "* Diagnose issues with taifa: Try 'Fix my Taifa laptop'\n\n"
            + "* Send feedback to Taifa: Try 'I want to send a feedback'\n\n";

        public const string WelcomeMessage =
            "Hi there\n\n"
            + "I am TaifaBot. Designed to be your taifa digital assistant.  \n"
            + "Currently I have the following features  \n"
            + Features
            + "You can type 'Help' to get this information again";

        public const string HelpMessage =
            "I can do the following   \n"
            + Features
            + "What would you like me to do?";
    }
}