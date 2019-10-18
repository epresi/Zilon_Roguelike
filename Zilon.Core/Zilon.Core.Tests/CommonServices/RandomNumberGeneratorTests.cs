﻿using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices;

namespace Zilon.Core.Tests.CommonServices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class RandomNumberGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что при разных зернах генерации не происходит ошибки получения случайного числа.
        /// </summary>
        /// <param name="seed"></param>
        [Test]
        public void NextTest([Values(uint.MinValue, (uint)100, (uint)300, uint.MaxValue)]uint seed,
            [Values(1, 10, 100, 1000)] int count)
        {
            // ARRANGE
            var rng = new RandomNumberGenerator(seed);

            // ACT
            Action act = () =>
            {
                for (var i = 0; i < count; i++)
                {
                    var randomValue = rng.Next();
                }
            };

            // ASSERT
            act.Should().NotThrow();
        }
    }
}