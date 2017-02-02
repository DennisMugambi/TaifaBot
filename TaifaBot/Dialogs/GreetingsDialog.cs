using BestMatchDialog;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaifaBot.Dialogs
{
    [Serializable]
    public class GreetingsDialog : BestMatchDialog<bool>
    {
        [BestMatch(new string[] { "Hi", "Hi there", "Hello there", "Hey", "Hello",
            "Hey There", "ey up", "Greetings", "Good Morning", "Good Afternoon", "Good Evening", "Good Day", "g'day" })]
        public async Task HandleGreeting(IDialogContext context)
        {
            await context.PostAsync("Well hello there. What can I do for you today?");
            context.Done(true);
        }

        [BestMatch(new string[] { "how goes it", "how do", "hows it going", "how are you",
            "how do you feel", "whats up", "sup", "hows things" })]
        public async Task HandleStatusRequest(IDialogContext context)
        {
            await context.PostAsync("I am great, thanks for asking!");
            context.Done(true);
        }

        [BestMatch(new string[] { "bye", "bye bye", "got to go",
            "see you later", "laters", "adios" })]
        public async Task HandleGoodbye(IDialogContext context)
        {
            await context.PostAsync("Bye. Looking forward to our next awesome conversation already.");
            context.Done(true);
        }

        [BestMatch(new string[] { "thank you", "thanks", "much appreciated", "thanks very much", "thanking you" })]
        public async Task HandleThanks(IDialogContext context)
        {
            await context.PostAsync("You're welcome.");
            context.Done(true);
        }

        public override async Task NoMatchHandler(IDialogContext context, string messageText)
        {
            context.Done(false);
        }
    }
}