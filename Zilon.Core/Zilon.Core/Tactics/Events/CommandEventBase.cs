﻿namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public abstract class CommandEventBase : ITacticEvent
    {
        protected string triggerName;
        protected TargetTriggerGroup[] targets;

        [JsonProperty("id")]
        public abstract string Id { get; }

        [JsonProperty("triggerName")]
        public string TriggerName => triggerName;

        [JsonProperty("targets")]
        public TargetTriggerGroup[] Targets => targets;

        public CommandEventBase(string triggerName, TargetTriggerGroup[] targets)
        {
            this.triggerName = triggerName;
            this.targets = targets;
        }
    }
}