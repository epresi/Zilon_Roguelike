﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public class GlobeStorageData
    {
        /// <summary>
        /// Полная информация о ландшафте мира.
        /// </summary>
        public TerrainStorageData Terrain { get; set; }

        /// <summary>
        /// Информация о текущих государствах мира.
        /// </summary>
        public RealmStorageData[] Realms { get; set; }

        /// <summary>
        /// Информация о текущих населённых пунктах мира.
        /// </summary>
        public LocalityStorageData[] Localities { get; set; }

        public SectorStorageData[] Sectors { get; set; }

        public HumanPersonStorageData[] Persons { get; set; }

        public ActorStorageData[] Actors { get; set; }

        /// <summary>
        /// Текущая итерация генерация мира.
        /// </summary>
        public int Iteration { get; set; }

        public static GlobeStorageData Create(Globe globe)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            var storageData = new GlobeStorageData();

            storageData.Iteration = globe.Iteration;

            FillTerrainStorageData(globe, storageData);

            var realmDict = FillRealmsStorageData(globe, storageData);
            FillLocalities(globe, storageData, realmDict);

            var personDict = FillPersons(globe, storageData);

            var sectorStorageDict = new Dictionary<ISector, SectorStorageData>();
            FillSectors(globe, storageData, personDict, sectorStorageDict);

            FillActors(globe, storageData, personDict, sectorStorageDict);

            return storageData;
        }

        private static void FillActors(Globe globe,
            GlobeStorageData storageData,
            Dictionary<IPerson, string> personDict,
            IDictionary<ISector, SectorStorageData> sectorStorageDict)
        {
            var actors = new List<ActorStorageData>();

            foreach (var sectorInfo in globe.SectorInfos)
            {
                foreach (var actor in sectorInfo.ActorManager.Items)
                {
                    var actorStorageData = ActorStorageData.Create(actor, sectorInfo.Sector, sectorStorageDict, personDict);
                    actors.Add(actorStorageData);
                }
            }

            storageData.Actors = actors.ToArray();
        }

        private static void FillSectors(Globe globe, GlobeStorageData storageData,
            Dictionary<IPerson, string> personDict,
            IDictionary<ISector, SectorStorageData> sectorStorageDict)
        {
            var sectorStorageDataList = new List<SectorStorageData>();
            foreach (var sectorInfo in globe.SectorInfos)
            {
                var sectorStorageData = SectorStorageData.Create(sectorInfo.Region,
                    sectorInfo.RegionNode,
                    sectorInfo.Sector,
                    personDict);

                sectorStorageDataList.Add(sectorStorageData);

                sectorStorageDict.Add(sectorInfo.Sector, sectorStorageData);
            }

            storageData.Sectors = sectorStorageDataList.ToArray();
        }

        private static Dictionary<IPerson, string> FillPersons(Globe globe, GlobeStorageData storageData)
        {
            var personStorageDataList = new List<HumanPersonStorageData>();
            var personDict = new Dictionary<IPerson, string>();
            foreach (var sectorInfo in globe.SectorInfos)
            {
                foreach (var actor in sectorInfo.ActorManager.Items)
                {
                    var personStorageData = HumanPersonStorageData.Create(actor.Person as HumanPerson);
                    personStorageDataList.Add(personStorageData);
                    personDict.Add(actor.Person, personStorageData.Id);
                }
            }
            storageData.Persons = personStorageDataList.ToArray();
            return personDict;
        }

        private static void FillLocalities(Globe globe, GlobeStorageData storageData, Dictionary<Realm, RealmStorageData> realmDict)
        {
            var localityDict = globe.Localities.ToDictionary(locality => locality,
                            locality => new LocalityStorageData
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = locality.Name,
                                RealmId = realmDict[locality.Owner].Id,
                            });

            storageData.Localities = localityDict.Select(x => x.Value).ToArray();
        }

        private static Dictionary<Realm, RealmStorageData> FillRealmsStorageData(Globe globe, GlobeStorageData storageData)
        {
            var realmDict = globe.Realms.ToDictionary(realm => realm, realm => new RealmStorageData
            {
                Id = Guid.NewGuid().ToString(),
                MainColor = realm.Banner.MainColor,
                Name = realm.Name
            });

            storageData.Realms = realmDict.Select(x => x.Value).ToArray();
            return realmDict;
        }

        private static void FillTerrainStorageData(Globe globe, GlobeStorageData storageData)
        {
            var terrainStorageData = TerrainStorageData.Create(globe.Terrain);

            storageData.Terrain = terrainStorageData;
        }

        public Globe Restore(ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            ISectorInfoFactory sectorInfoFactory)
        {
            if (sectorInfoFactory is null)
            {
                throw new ArgumentNullException(nameof(sectorInfoFactory));
            }

            var globe = new Globe();
            globe.Iteration = Iteration;

            RestoreTerrain(globe, schemeService);

            var realmDict = RestoreRealms(globe);

            RestoreLocalities(out globe.Localities, Localities, globe.Terrain, realmDict);

            var personDict = RestorePersons(globe, schemeService, survivalRandomSource, propFactory);

            RestoreSectors(globe, sectorInfoFactory, personDict);

            return globe;
        }

        private Dictionary<string, Realm> RestoreRealms(Globe globe)
        {
            var realmDict = Realms.ToDictionary(storedRealm => storedRealm.Id, storedRealm => new Realm
            {
                Name = storedRealm.Name,
                Banner = new RealmBanner { MainColor = storedRealm.MainColor }
            });

            globe.Realms = realmDict.Select(x => x.Value).ToList();
            return realmDict;
        }

        private void RestoreTerrain(Globe globe, ISchemeService schemeService)
        {
            var terrain = Terrain.Restore(schemeService);
            globe.Terrain = terrain;
        }

        /// <summary>
        /// Восстанавливает нас.пункты в указанные коллекции.
        /// </summary>
        /// <param name="localities"> Целевая коллекция населённых пунктов. </param>
        /// <param name="localityCells"> Соответствующая целевая коллекция кеша узлов населённых пунктов. </param>
        /// <param name="storedLocalities"> Данные сохранения по нас.пунктам. </param>
        /// <param name="terrain"> Территория мира. </param>
        /// <param name="realmsDict"> Словарь государств. Нужен, чтобы знать id государств, которые были в файле сохранения. </param>
        private static void RestoreLocalities(out List<Locality> localities,
            LocalityStorageData[] storedLocalities,
            Terrain terrain,
            Dictionary<string, Realm> realmsDict)
        {
            localities = new List<Locality>(storedLocalities.Length);

            foreach (var storedLocality in storedLocalities)
            {
                var locality = new Locality()
                {
                    Name = storedLocality.Name,
                    Owner = realmsDict[storedLocality.RealmId],
                };

                localities.Add(locality);
            }
        }

        private Dictionary<string, IPerson> RestorePersons(Globe globe, ISchemeService schemeService, ISurvivalRandomSource survivalRandomSource, IPropFactory propFactory)
        {
            var personsTemp = Persons.Select(x => new
            {
                Person = (IPerson)x.Restore(schemeService, survivalRandomSource, propFactory),
                x.Id
            });
            globe.Persons = personsTemp.Select(x => x.Person).ToList();

            return personsTemp.ToDictionary(x => x.Id, x => x.Person);
        }

        private Dictionary<string, ISector> RestoreSectors(
            Globe globe,
            ISectorInfoFactory sectorInfoFactory,
            Dictionary<string, IPerson> personDict)
        {
            var infos = new List<SectorInfo>();
            var sectorDict = new Dictionary<string, ISector>();
            foreach (var sectorInfoStorageData in Sectors)
            {
                var terrainCell = globe.Terrain.Cells.SelectMany(x => x).Single(x => x.Coords == sectorInfoStorageData.TerrainCoords);
                var globeRegion = globe.Terrain.Regions.Single(x => x.TerrainCell == terrainCell);
                var coordX = sectorInfoStorageData.GlobeRegionNodeCoords.X;
                var coordY = sectorInfoStorageData.GlobeRegionNodeCoords.Y;
                var globeRegionNode = globeRegion.RegionNodes.Single(x => x.OffsetX == coordX && x.OffsetY == coordY);

                var actorStorageDatas = Actors.Where(x => x.SectorId == sectorInfoStorageData.Id).ToArray();

                var info = sectorInfoFactory.Create(globeRegion,
                    globeRegionNode,
                    sectorInfoStorageData,
                    actorStorageDatas,
                    personDict
                    );

                infos.Add(info);

                sectorDict.Add(sectorInfoStorageData.Id, info.Sector);
            }

            globe.SectorInfos = infos;

            return sectorDict;
        }
    }
}
