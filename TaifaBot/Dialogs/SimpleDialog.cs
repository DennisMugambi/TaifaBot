using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaifaBot
{
    [Serializable]
    public class SimpleDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ActivityReceivedAsync);
        }

        private async Task ActivityReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            if (activity.Text.Contains("hello"))
            {

                await context.PostAsync($"Hi There, I am the taifa bot");
            }
            else if (activity.Text.StartsWith("Tasks"))
            {

                await context.PostAsync("The Tasks that I can do include:");
            }
            else if (activity.Text.StartsWith("help"))
            {

                List<CardImage> images = new List<CardImage>();
                CardImage ci = new CardImage("http://www.iccnetwork.net/images/customercare.jpg");
                images.Add(ci);
                CardAction ca = new CardAction()
                {
                    Title = "Visit Support",
                    Type = "openUrl",
                    Value = "http://bespokefusion.com"
                };
                ThumbnailCard tc = new ThumbnailCard()
                {
                    Title = "Need Help?",
                    Subtitle = "Go to our main site support",
                    Images = images,
                    Tap = ca
                };
                reply.Attachments.Add(tc.ToAttachment());
            }
            else
            {
                await context.PostAsync("Sorry but my responses are limited. Please ask the right questsions.");
            }
            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }
    }
}
