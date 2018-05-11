﻿namespace Zilon.Core.Services.CombatEvents
{
    using System;

    using Zilon.Core.Tactics.Events;

    public class CombatEventArgs : EventArgs
    {
        public ICommandEvent CommandEvent { get; set; }
        public event EventHandler<EventHandler> OnComplete;
    }
}
