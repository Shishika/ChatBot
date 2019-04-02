// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace TableReservation
{
    public class TableReservationBot : IBot
    {
        private readonly TableReservationAccessors _accessors;
        private readonly ILogger _logger;

        private const string WelcomeMessage = @"Welcome user!";

        public TableReservationBot(TableReservationAccessors statePropertyAccessor)
        {
            _accessors = statePropertyAccessor ?? throw new System.ArgumentNullException("state accessor can't be null");
        }

        public TableReservationBot(ILoggerFactory loggerFactory, UserState userState, ConversationState conversationState)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }
            _logger = loggerFactory.CreateLogger<TableReservationBot>();
            _logger.LogTrace("Turn start.");
        }
       
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var didBotWelcomeUser = await _accessors.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState());

            //await turnContext.SendActivityAsync($"Welcome!");
            //var rMessage = turnContext.Activity.Text;
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (didBotWelcomeUser.DidBotWelcomeUser == false)
                {
                    didBotWelcomeUser.DidBotWelcomeUser = true;
                    // Update user state flag to reflect bot handled first user interaction.
                    await _accessors.WelcomeUserState.SetAsync(turnContext, didBotWelcomeUser);
                    await _accessors.UserState.SaveChangesAsync(turnContext);

                    // the channel should sends the user name in the 'From' object
                    var userName = turnContext.Activity.From.Name;

                    await turnContext.SendActivityAsync($"May I have your name?", cancellationToken: cancellationToken);
                }
                else
                {
                    // This example hardcodes specific utterances. You should use LUIS or QnA for more advance language understanding.
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "hello":
                        case "hi":
                            await turnContext.SendActivityAsync($"You said {text}.", cancellationToken: cancellationToken);
                            break;
                        default:
                            await turnContext.SendActivityAsync(WelcomeMessage, cancellationToken: cancellationToken);
                            break;
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        if (member.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync($"Hi, {WelcomeMessage}", cancellationToken: cancellationToken);
                        }
                    }
                }
            }

            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
            }
            await _accessors.UserState.SaveChangesAsync(turnContext);

            
        }
    }
}
