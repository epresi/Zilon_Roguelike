﻿using System;
using System.Linq;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;

public class PersonCreateModalBody : MonoBehaviour, IModalWindowHandler
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public Text DescriptionText;

    public string Caption
    {
        get
        {
            var currentLanguage = _uiSettingService.CurrentLanguage;
            var caption = StaticPhrases.GetValue("caption-person-create", currentLanguage);

            return caption;
        }
    }

    public event EventHandler Closed;

    public void Init(IPerson playerPerson)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        var backstoryText = GetLocalizedBackstoryText(currentLanguage, "main");
        DescriptionText.text = backstoryText + Environment.NewLine + Environment.NewLine;

        var className = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, (playerPerson as HumanPerson).PersonEquipmentTemplate);
        var classDescriptonText = GetLocalizedBackstoryText(currentLanguage, "class") + " " + className;
        DescriptionText.text += classDescriptonText + Environment.NewLine + Environment.NewLine;

        DescriptionText.text += GetLocalizedBackstoryText(currentLanguage, "trait") + Environment.NewLine;

        var buildInTraits = playerPerson.GetModule<IEvolutionModule>().Perks.Where(x => x.Scheme.IsBuildIn).ToArray();
        foreach (var startTrait in buildInTraits)
        {
            var traitName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, startTrait.Scheme.Name);
            DescriptionText.text += " - " + traitName + Environment.NewLine;
        }

        DescriptionText.text += Environment.NewLine + Environment.NewLine;

        DescriptionText.text += GetLocalizedBackstoryText(currentLanguage, "props") + Environment.NewLine;
        foreach (var prop in playerPerson.GetModule<IEquipmentModule>())
        {
            if (prop is null)
            {
                continue;
            }

            var propName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, prop.Scheme.Name);
            DescriptionText.text += " - " + propName + Environment.NewLine;
        }

        foreach (var prop in playerPerson.GetModule<IInventoryModule>().CalcActualItems())
        {
            var propName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, prop.Scheme.Name);

            if (prop is Resource resource)
            {
                propName += " x " + resource.Count;
            }

            DescriptionText.text += " - " + propName + Environment.NewLine;
        }
    }

    private string GetLocalizedBackstoryText(Language currentLanguage, string mainKey)
    {
        string langKey;
        switch (currentLanguage)
        {
            case Language.Russian:
                langKey = "ru";
                break;

            default:
            case Language.English:
                langKey = "en";
                break;

            case Language.Undefined:
                throw new ArgumentException($"Некоректное значение языка {currentLanguage}.");
        }
        var text = Resources.Load<TextAsset>($@"Backstory\{mainKey}-{langKey}");

        return text.text;
    }

    public void ApplyChanges()
    {
        // Ничего не делаем при закрытии окна.
        // Окно только читает данные. Ничего не изменяет.
    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
    }
}
