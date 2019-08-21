﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public Locality()
        {
            Regions = new List<LocalityRegion>();
            Stats = new LocalityStats();
            CurrentPopulation = new List<Population>();
        }

        public string Name { get; set; }

        public TerrainCell Cell { get; set; }

        public Realm Owner { get; set; }

        /// <summary>
        /// Текущее население города.
        /// Каждый объект в списке - это единица населения.
        /// Суммарно единиц популяции не должно быть больше, чем места для проживания.
        /// Иначе начнётся перенаселение. Тогда жители могут организовать миграционнуб группу и покинуть город.
        /// </summary>
        public List<Population> CurrentPopulation { get; }

        public Dictionary<BranchType, int> Branches { get; set; }

        /// <summary>
        /// Текущие районы города.
        /// Каждый район занимает один узел в провинции.
        /// Каждый район сначала должен быть разработан.
        /// После разработки в районе можно возводить структуры.
        /// </summary>
        public List<LocalityRegion> Regions { get; }

        /// <summary>
        /// Текущее состояние в городе. Харатектиристики города.
        /// </summary>
        public LocalityStats Stats { get; private set; }

        public override string ToString()
        {
            return $"{Name} [{Owner}] ({Branches.First().Key})";
        }

        /// <summary>
        /// Обновление состояния города.
        /// </summary>
        public void Update()
        {
            UpdatePopulation();

            foreach (var region in Regions)
            {
                // Для жилых мест отдельная логика.
                // Их потребляет только население, а производят структуры.
                // Поэтому зануляем перед обработкой структур города. Далее структуры выставят текущее значение.
                Stats.Resources[LocalityResource.LivingPlaces] = 0;

                var suppliedStructures = SupplyStructures(region.Structures);
                ProduceResources(suppliedStructures, Stats);
            }
        }

        private void UpdatePopulation()
        {
            // Изымаем столько товаров и еды, сколько населения в городе.
            var populationCount = CurrentPopulation.Count();

            RemoveResource(LocalityResource.Food, populationCount);
            RemoveResource(LocalityResource.Goods, populationCount);
        }

        private static void ProduceResources(List<ILocalityStructure> structures, LocalityStats stats)
        {
            // Все структуры, которые получили обеспечение, производят ресурс
            foreach (var structure in structures)
            {
                foreach (var productResource in structure.ProductResources)
                {
                    if (!stats.Resources.ContainsKey(productResource.Key))
                    {
                        stats.Resources[productResource.Key] = 0;
                    }

                    stats.Resources[productResource.Key] += productResource.Value;
                }
            }
        }

        private List<ILocalityStructure> SupplyStructures(List<ILocalityStructure> structures)
        {
            // Структуры, которые получили обеспечение.
            var suppliedStructures = new List<ILocalityStructure>();

            // Изымаем все ресурсы текущй структурой.
            // Струкруты, которые получили обеспечение, затем производят ресурсы.
            foreach (var structure in structures)
            {
                // Проверка наличия необходимых ресурсов.
                foreach (var requiredResource in structure.RequiredResources)
                {
                    var requiredResourceType = requiredResource.Key;
                    if (Stats.Resources.ContainsKey(requiredResourceType))
                    {
                        if (Stats.Resources[requiredResourceType] >= requiredResource.Value)
                        {
                            suppliedStructures.Add(structure);
                            Stats.Resources[requiredResourceType] -= requiredResource.Value;
                        }
                    }

                }
            }

            return suppliedStructures;
        }

        private void RemoveResource(LocalityResource resource, int count)
        {
            if (!Stats.Resources.ContainsKey(resource))
            {
                Stats.Resources[resource] = 0;
            }

            Stats.Resources[resource] -= count;
        }
    }
}
