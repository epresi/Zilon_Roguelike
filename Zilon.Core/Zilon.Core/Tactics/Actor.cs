﻿using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class Actor : IActor
    {
        public event EventHandler Moved;
        public event EventHandler<OpenContainerEventArgs> OpenedContainer;
        public event EventHandler<UsedActEventArgs> UsedAct;
        public event EventHandler<DefenceEventArgs> OnDefence;
        public event EventHandler<DamageTakenEventArgs> DamageTaken;
        public event EventHandler<ArmorEventArgs> OnArmorPassed;

        /// <inheritdoc />
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public IMapNode Node { get; private set; }

        public IPlayer Owner { get; }

        [ExcludeFromCodeCoverage]
        public Actor([NotNull] IPerson person, [NotNull]  IPlayer owner, [NotNull]  IMapNode node)
        {
            Person = person ?? throw new ArgumentNullException(nameof(person));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public bool CanBeDamaged()
        {
            return !Person.Survival.IsDead;
        }

        public void MoveToNode(IMapNode targetNode)
        {
            Node = targetNode;
            Moved?.Invoke(this, new EventArgs());
        }

        public void OpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            var openResult = method.TryOpen(container);

            DoOpenContainer(openResult);
        }

        public void UseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            DoUseAct(target, tacticalAct);
        }

        public void UseProp(IProp usedProp)
        {
            var useData = usedProp.Scheme.Use;

            foreach (var rule in useData.CommonRules)
            {
                switch (rule.Type)
                {
                    case ConsumeCommonRuleType.Satiety:
                        Person.Survival.RestoreStat(SurvivalStatType.Satiety, 51);
                        break;

                    case ConsumeCommonRuleType.Thrist:
                        Person.Survival.RestoreStat(SurvivalStatType.Water, 51);
                        break;

                    case ConsumeCommonRuleType.Health:
                        Person.Survival.RestoreStat(SurvivalStatType.Health, 4);
                        break;
                }
            }

            if (useData.Consumable)
            {
                switch (usedProp)
                {
                    case Resource resource:
                        var removeResource = new Resource(resource.Scheme, 1);
                        Person.Inventory.Remove(removeResource);
                        break;
                }
            }
        }

        public void TakeDamage(int value)
        {
            Person.Survival.DecreaseStat(SurvivalStatType.Health, value);
            DoDamageTaken(value);
        }

        [ExcludeFromCodeCoverage]
        private void DoDamageTaken(int value)
        {
            DamageTaken?.Invoke(this, new DamageTakenEventArgs(value));
        }

        [ExcludeFromCodeCoverage]
        public void ProcessDefence(PersonDefenceItem prefferedDefenceItem, int successToHitRoll, int factToHitRoll)
        {
            var eventArgs = new DefenceEventArgs(prefferedDefenceItem,
                successToHitRoll,
                factToHitRoll);

            OnDefence?.Invoke(this, eventArgs);
        }

        [ExcludeFromCodeCoverage]
        private void DoOpenContainer(IOpenContainerResult openResult)
        {
            var e = new OpenContainerEventArgs(openResult);
            OpenedContainer?.Invoke(this, e);
        }

        [ExcludeFromCodeCoverage]
        private void DoUseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            var args = new UsedActEventArgs(target, tacticalAct);
            UsedAct?.Invoke(this, args);
        }

        [ExcludeFromCodeCoverage]
        public void ProcessArmor(int armorRank, int successRoll, int factRoll)
        {
            OnArmorPassed?.Invoke(this, new ArmorEventArgs(armorRank, successRoll, factRoll));
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Person}";
        }
    }
}
