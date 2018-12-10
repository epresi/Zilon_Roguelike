﻿using System;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Client;
using Zilon.Core.Props;

public class PropItemVm : MonoBehaviour, IPropItemViewModel
{
    public Text CountText;
    public Image IconImage;
    public Image SelectedBorder;

    public string Sid;

    public IProp Prop { get; private set; }

    public event EventHandler Click;

    public void Init(IProp prop)
    {
        Prop = prop;

        if (prop is Resource resource)
        {
            CountText.gameObject.SetActive(true);
            CountText.text = $"x{resource.Count}";
        }
        else
        {
            CountText.gameObject.SetActive(false);
        }

        Sid = prop.Scheme.Sid;

        var iconSprite = CalcIcon(prop);

        IconImage.sprite = iconSprite;
    }

    private Sprite CalcIcon(IProp prop)
    {
        var iconSprite = Resources.Load<Sprite>($"Icons/props/{prop.Scheme.Sid}");
        return iconSprite;
    }

    public void SetSelectedState(bool value)
    {
        SelectedBorder.gameObject.SetActive(value);
    }

    public void Click_Handler()
    {
        Click?.Invoke(this, new EventArgs());
    }
}
