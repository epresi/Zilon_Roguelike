﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация данных по развитию персонажа.
    /// </summary>
    public sealed class EvolutionModule : IEvolutionModule
    {
        private readonly List<IPerk> _buildInPerks;
        private readonly ISchemeService _schemeService;

        public EvolutionModule(ISchemeService schemeService)
        {
            IsActive = true;

            _schemeService = schemeService;

            _buildInPerks = new List<IPerk>();

            Stats = new[]
            {
                new SkillStatItem { Stat = SkillStatType.Ballistic, Value = 10 },
                new SkillStatItem { Stat = SkillStatType.Melee, Value = 10 }
            };

            UpdatePerks();
        }

        /// <summary>
        /// Этот метод форсированно устанавливает перки персонажа с их состоянием.
        /// Используется для восстановления персонажа из сохранения.
        /// </summary>
        /// <param name="perks"> Набор перков с их состоянием, который нужно восстановить. </param>
        public void SetPerksForced(IEnumerable<IPerk> perks)
        {
            Perks = perks.ToArray();

            UpdatePerks();
        }

        private void DoPerkArchieved(IPerk perk)
        {
            var eventArgs = new PerkEventArgs(perk);
            PerkLeveledUp?.Invoke(this, eventArgs);
        }

        private IList<IPerk> GetPerks()
        {
            var schemes = _schemeService.GetSchemes<IPerkScheme>()
                // Для развития годятся только те перки, которые не врождённые.
                // Врождённые перки даются только при генерации персонажа.
                .Where(x => !x.IsBuildIn)
                // Защиита от схем, в которых забыли прописать уровни.
                // По идее, такие перки либо должны быть врождёнными.
                // Следовательно, если они не отсеяны выше, то это ошибка.
                // Такие схемы лучше проверять в тестах на валидацию схем.
                .Where(x => x.Levels != null);

            var perks = new List<IPerk>(_buildInPerks);
            if (Perks != null)
            {
                perks.AddRange(Perks);
            }

            foreach (var perkScheme in schemes)
            {
                var existingPerk = Perks?.SingleOrDefault(x => x.Scheme == perkScheme);
                if (existingPerk != null)
                {
                    continue;
                }

                //TODO Сейчас можно качнуть только первый уровень перка. Должно быть полноценное развитие.
                var perk = new Perk
                {
                    Scheme = perkScheme,
                    CurrentLevel = null,
                    CurrentJobs = perkScheme.Levels[0].Jobs
                        .Select(x => (IJob)new PerkJob(x))
                        .ToArray()
                };

                perks.Add(perk);
            }

            return perks;
        }

        private void UpdatePerks()
        {
            var perks = GetPerks();

            Perks = perks.ToArray();
        }

        /// <inheritdoc />
        public SkillStatItem[] Stats { get; }

        /// <inheritdoc />
        public IPerk[] Perks { get; private set; }

        /// <inheritdoc />
        public string Key => nameof(IEvolutionModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public event EventHandler<PerkEventArgs> PerkLeveledUp;

        /// <inheritdoc />
        public event EventHandler<PerkEventArgs> PerkAdded;

        /// <inheritdoc />
        public void AddBuildInPerks(IEnumerable<IPerk> perks)
        {
            if (perks is null)
            {
                throw new ArgumentNullException(nameof(perks));
            }

            _buildInPerks.AddRange(perks);

            UpdatePerks();

            foreach (var perk in perks)
            {
                PerkAdded?.Invoke(this, new PerkEventArgs(perk));
            }
        }

        /// <inheritdoc />
        public void PerkLevelUp(IPerk perk)
        {
            if (perk is null)
            {
                throw new ArgumentNullException(nameof(perk));
            }

            var activePerkIsValid = Perks.Contains(perk);
            if (!activePerkIsValid)
            {
                throw new InvalidOperationException("Указанный перк не является активным для текущего актёра.");
            }

            var nextLevel = PerkHelper.GetNextLevel(perk.Scheme, perk.CurrentLevel);

            perk.CurrentLevel = nextLevel;

            UpdatePerks();

            DoPerkArchieved(perk);
        }
    }
}