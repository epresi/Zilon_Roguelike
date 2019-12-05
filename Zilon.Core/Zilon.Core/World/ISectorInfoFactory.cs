﻿using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;

namespace Zilon.Core.World
{
    public interface ISectorInfoFactory
    {
        SectorInfo Create(GlobeRegion globeRegion,
            GlobeRegionNode globeRegionNode,
            SectorStorageData sectorStorageData,
            IEnumerable<ActorStorageData> actors,
            IDictionary<string, IPerson> personDict);
    }
}