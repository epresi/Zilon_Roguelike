﻿using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    [Category("Real")]
    public class SectorRealTests
    {
        private const int IDLE_DURATION = 1;

        /// <summary>
        /// Тест проверяет выполнение обновления состояния сектора.
        /// Есть квадратная карта. В произвольных местах расположены два монстра.
        /// Монстры должны выполнять логику патрулирования.
        /// Это длится 100 ходов. Не должно быть выбрасываться NRE.
        /// Стартовое состояние взято с клиента в момент разработки.
        /// </summary>
        [Test(Description = "Ttc1")] // Tactic test case 1
        public void Update_2MonsterActorsPatrols2RoutesDuring100SectorUpdates_NoNRE()
        {
            // ARRANGE
            var map = new TestGrid15GenMap();

            var actorManager = new ActorList();

            var sector = new Sector(map, actorManager);
            GenerateSectorTtc1Content(sector, actorManager, map);



            // ACT
            for (var round = 0; round <= 100; round++)
            {
                sector.Update();
            }



            // ASSERT
            // Если не было исключений, то тест считается пройденным.
            // Иначе теряем читаемый стек вызовов, оборачивая Update в делегат.
        }

        private void GenerateSectorTtc1Content(Sector sector, IActorManager actorManager, IMap map)
        {
            // Подготовка карты
            map.Edges.RemoveAt(10);
            map.Edges.RemoveAt(20);
            map.Edges.RemoveAt(30);


            // Подготовка игроков
            var botPlayer = new BotPlayer();


            // Подготовка актёров
            var enemy1StartNode = map.Nodes.Cast<HexNode>().SelectBy(5, 5);
            var enemy1Actor = CreateActor(botPlayer, actorManager, enemy1StartNode);

            var enemy2StartNode = map.Nodes.Cast<HexNode>().SelectBy(9, 9);
            var enemy2Actor = CreateActor(botPlayer, actorManager, enemy2StartNode);


            // Подготовка маршрутов патрулирования
            var patrolMapNodes1 = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 2),
                map.Nodes.Cast<HexNode>().SelectBy(2, 10)
            };
            var patrolRoute1 = CreateRoute(patrolMapNodes1);

            var patrolMapNodes2 = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(10, 2),
                map.Nodes.Cast<HexNode>().SelectBy(10, 10)
            };
            var patrolRoute2 = CreateRoute(patrolMapNodes2);

            var routeDictionary = new Dictionary<IActor, IPatrolRoute>
            {
                { enemy1Actor, patrolRoute1 },
                { enemy2Actor, patrolRoute2 }
            };


            // Подготовка дополнительных сервисов
            var dice = new Dice();
            var decisionSource = new DecisionSource(dice);


            // Подготовка источника поведения ботов
            var botTaskSource = new MonsterActorTaskSource(botPlayer, routeDictionary, decisionSource);


            sector.BehaviourSources = new IActorTaskSource[]
            {
                botTaskSource
            };
        }

        private IPatrolRoute CreateRoute(IMapNode[] mapNodes)
        {
            var patrolRoute = new PatrolRoute(mapNodes);

            return patrolRoute;
        }

        private IActor CreateActor([NotNull] IPlayer player,
            [NotNull] IActorManager actorManager,
            [NotNull] IMapNode startNode)
        {
            var person = new Person()
            {
                Hp = 1
            };

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            
            return actor;
        }
    }
}