﻿using System.Linq;
using Zilon.Core.CommonServices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.CommonServices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DropRollerTests
    {
        /// <summary>
        ///     1. В системе есть две записи дропа с весами 16 и 64. Ролл на 16.
        ///     2. Рассчитываем дроп.
        ///     3. Получаем дроп из первой записи.
        /// </summary>
        [Test]
        public void GetRecord_RollIn1stBorder_Has1stRecord()
        {
            // ARRANGE
            DropTableRecordSubScheme[] records =
            {
                new DropTableRecordSubScheme("trophy1", 16), new DropTableRecordSubScheme("trophy2", 64)
            };

            int roll = 16;

            DropTableModRecord[] recMods = records.Select(x => new DropTableModRecord
            {
                Record = x, ModifiedWeight = x.Weight
            }).ToArray();


            // ACT
            DropTableModRecord recordMod = DropRoller.GetRecord(recMods, roll);


            // ASSERT
            recordMod.Record.SchemeSid.Should().Be("trophy1");
        }
    }
}