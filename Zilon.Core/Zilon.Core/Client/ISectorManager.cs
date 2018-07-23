﻿using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    /// <summary>
    /// Интерфейс, который предоставляет доступ к общей информации о текущем секторе.
    /// </summary>
    internal interface ISectorManager
    {
        Sector CurrentSector { get; set; }
    }
}