﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Перечисление характеристик выживания для персонажа.
    /// Служит только для загрузки схемы из json.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PersonSurvivalStatType
    {
        /// <summary>
        /// Не определена. Скорее всего, ошибка.
        /// </summary>
        Undefined,

        /// <summary>
        /// Сытость.
        /// </summary>
        Satiety,

        /// <summary>
        /// Достаточность воды.
        /// </summary>
        Hydration,

        /// <summary>
        /// Интоксикация персонажа.
        /// </summary>
        Intoxication,

        /// <summary>
        /// Способность восстанвливать раны.
        /// </summary>
        Wound,

        /// <summary>
        /// Способность дишать обычным воздухом.
        /// </summary>
        Breath,

        /// <summary>
        /// Энергия для совершения действий.
        /// </summary>
        Energy
    }
}