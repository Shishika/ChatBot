// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;

namespace TableReservation
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="EchoWithCounterBot"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>

    public class WelcomeUserState
    {
        public bool DidBotWelcomeUser { get; set; } = false;
    }

    public class TableReservationAccessors
    {
        public TableReservationAccessors(UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public static string CounterStateName { get; } = $"{nameof(TableReservationAccessors)}.CounterState";

        public IStatePropertyAccessor<CounterState> CounterState { get; set; }

     
        public static string WelcomeUserName { get; } = $"{nameof(TableReservationAccessors)}.WelcomeUserState";

        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }

       
        public UserState UserState { get; }
    }
}
