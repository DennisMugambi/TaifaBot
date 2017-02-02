using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;
using System.Configuration;
using TaifaBot.Internal;

namespace TaifaBot.Dialogs
{
    [LuisModel("c8580a5a-bd23-4115-ac0a-ccf97185a93c", "b65d3a8bd58541a49fca7c5981bab797")]
    [Serializable]
    public class TaifaLuisDialog : LuisDialog<object>
    {
        public TaifaLuisDialog(params ILuisService[] services) : base(services)
        {
        }
        [LuisIntent("Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.WelcomeMessage);
            context.Wait(MessageReceived);
        }
        [LuisIntent("AboutTaifa")]
        public async Task AboutTaifa(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(@"The TAIFA Brand Laptop is a product of the Nairobi Industrial and Technological Park (NITP)");
            await context.PostAsync(@"NITP is a Kenyan Vision 2030 flagship project between Jomo Kenyatta University of Agriculture & Technology (JKUAT) and the Ministry of Industrialization and Enterprise Development (MOIED).");
            context.Wait(MessageReceived);
        }
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.HelpMessage);
            context.Wait(MessageReceived);
        }
        [LuisIntent("Feedback")]
        public async Task Feedback(IDialogContext context, LuisResult result)
        {
            try
            {
                await context.PostAsync("That's great. You will need to provide few details about yourself before giving feedback.");
                var feedbackForm = new FormDialog<FeedbackForm>(new FeedbackForm(), FeedbackForm.BuildForm, FormOptions.PromptInStart);
                context.Call(feedbackForm, FeedbackFormComplete);
            }
            catch (Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
                context.Wait(MessageReceived);
            }
        }
        [LuisIntent("TaifaFeatures")]
        public async Task TaifaFeatures(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
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
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("TaifaSpecs")]
        public async Task TaifaSpecs(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetCardsAttachments();

            await context.PostAsync(reply);

            context.Wait(this.MessageReceived);
        }

        private static IList<Attachment> GetCardsAttachments()
        {
            return new List<Attachment>()
            {
                GetHeroCard(
                    "Display",
                    "14.0” WXGA HD (1366×768)",
                    "Crystal Clear display for your work and entertainement needs",
                    new CardImage(url: "http://nitp.ac.ke/wp-content/uploads/2016/11/user3.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "View More", value: "http://nitp.ac.ke/products/taifa/")),
                GetHeroCard(
                    "Processor Options",
                    "Intel® CoreTM i3 - 400M, Intel® Processors 2.4Ghz",
                    "Quality processor for your work and entertainement needs",
                    new CardImage(url: "http://nitp.ac.ke/wp-content/uploads/2016/11/user.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "View more", value: "http://nitp.ac.ke/products/taifa/")),
                GetHeroCard(
                    "Operating System and Hardware",
                    "Genuine Windows® 8.1 & Genuine Microsoft Office® 2013",
                    "Sufficient for your work and studies. 8.1 is upgradable to Windows 10",
                    new CardImage(url: "http://nitp.ac.ke/wp-content/uploads/2016/11/user2.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "View more", value: "http://nitp.ac.ke/products/taifa/")),
                GetHeroCard(
                    "Multimedia",
                    "High quality speakers, Stereo headphone & Microphone jack, HDMI Port, HD video webcam",
                    "Suitable for your entertainment needs and teleconferencing",
                    new CardImage(url: "http://nitp.ac.ke/wp-content/uploads/2016/11/user4.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "View more", value: "http://nitp.ac.ke/products/taifa/")),

            };
        }
        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I have no idea what you are talking about.");
            context.Wait(MessageReceived);
        }

  

        #region PrivateDeclarations
        private static string recipientEmail = ConfigurationManager.AppSettings["RecipientEmail"];
        private static string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];

        private async Task FeedbackFormComplete(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            try
            {
                var feedback = await result;
                string message = GenerateEmailMessage(feedback);
                var success = await EmailSender.SendEmail(recipientEmail, senderEmail, $"Email from {feedback.Name}", message);
                if (!success)
                    await context.PostAsync("I was not able to send your message. Something went wrong.");
                else
                {
                    await context.PostAsync($"Thank you {feedback.Name} for the feedback.");
                    await context.PostAsync("What else would you like to do?");
                }

            }
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to send feedback? That's ok. You can drop a comment below.");
            }
            catch (Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
            }
            finally
            {
                context.Wait(MessageReceived);
            }
        }

        private string GenerateEmailMessage(FeedbackForm feedback)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Message from: {feedback.Name}");
            sb.AppendLine($"Contact: {feedback.Contact}");
            sb.AppendLine($"Message: {feedback.Feedback}");
            return sb.ToString();
        }

        //private async Task GreetingDialogDone(IDialogContext context, IAwaitable<bool> result)
        //{
        //    var success = await result;
        //    if (!success)
        //        await context.PostAsync("I'm sorry. I didn't understand you.");

        //    context.Wait(MessageReceived);
        //}
    
        #endregion
    }


}