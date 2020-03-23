﻿namespace Zilon.Core.World.GlobeDrafting
{
    /// <summary>
    /// Генератор черновика мира.
    /// </summary>
    public sealed class GlobeDraftGenerator
    {

        private int WorldSize { get; } = 40;

        /// <summary>
        /// Создаёт черновик.
        /// </summary>
        /// <returns></returns>
        public GlobeDraft Generate()
        {
            return new GlobeDraft
            {
                Size = WorldSize,
                StartLocalities = GenerateStartRealmLocalities(),
                StartDungeons = new[] {
                    new DungeonDraft{
                        SchemeSid = "intro",
                        SchemeLevelSid = "intro",
                        StartTerrainCoords = new OffsetCoords(7, 5)
                    }
                }
            };
        }

        private static RealmLocalityDraft[] GenerateStartRealmLocalities()
        {
            return new[] {
                    new RealmLocalityDraft{
                        StartTerrainCoords = new OffsetCoords(5, 5),
                        Population = 40
                    },

                    new RealmLocalityDraft{
                        StartTerrainCoords = new OffsetCoords(5, 15),
                        Population = 40
                    },

                    new RealmLocalityDraft{
                        StartTerrainCoords = new OffsetCoords(15, 15),
                        Population = 40
                    },
                };
        }
    }
}
