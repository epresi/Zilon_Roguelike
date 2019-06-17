﻿using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Карточка, когда один агент поддерживает другого.
    /// </summary>
    /// <remarks>
    /// Поддержка выражается в увеличение количества ХП целевого агента.
    /// </remarks>
    public sealed class AgentSupport : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            return true;
        }

        public string Use(Agent agent, Globe globe, IDice dice)
        {
            string history = null;

            var availableTargets = globe.Agents.Where(x => x != agent && x.Hp >= 0 && x.Hp <= 2).ToArray();
            if (availableTargets.Any())
            {
                var agentRollIndex = dice.Roll(0, availableTargets.Count() - 1);
                var targetAgent = availableTargets[agentRollIndex];
                targetAgent.Hp++;

                history = $"{agent} supported {targetAgent}.";
            }

            return history;
        }
    }
}
