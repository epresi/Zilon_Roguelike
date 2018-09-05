﻿using System;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на назначение экипировки.
    /// </summary>
    public class EquipCommand : SpecialActorCommandBase
    {
        private readonly IInventoryState _inventoryState;

        public int? SlotIndex { get; set; }

        public EquipCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            IPlayerState playerState,
            IInventoryState inventoryState) :
            base(gameLoop, sectorManager, playerState)
        {
            _inventoryState = inventoryState;
        }

        public override bool CanExecute()
        {
            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var propVm = _inventoryState.SelectedProp;
            if (propVm == null)
            {
                return false;
            }

            var equipment = propVm.Prop as Equipment;

            if (equipment == null)
            {
                return false;
            }

            //TODO Добавить проверку на то, что выбранный предмет может быть экипирован в указанный слот.

            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var propVm = _inventoryState.SelectedProp;
            var equipment = propVm.Prop as Equipment;
            if (equipment == null)
            {
                throw new InvalidOperationException("Попытка экипировать то, что не является экипировкой.");
            }

            var intention = new Intention<EquipTask>(a => new EquipTask(a, equipment, SlotIndex.Value));
            _playerState.TaskSource.Intent(intention);
        }
    }
}