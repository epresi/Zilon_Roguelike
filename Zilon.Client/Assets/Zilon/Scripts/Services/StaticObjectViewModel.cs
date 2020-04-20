﻿using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class StaticObjectViewModel : MonoBehaviour, IContainerViewModel
{
    public event EventHandler Selected;
    public event EventHandler MouseEnter;

    public virtual IStaticObject StaticObject { get; set; }

    public Vector3 WorldPosition { get; set; }

    //TODO Убрать, т.к. оставлено для совместимости со старым кодом.
    // Не убрал, потому что работал над другой задачей.
    public IStaticObject Container
    {
        get => StaticObject;
        set
        {
            StaticObject = value;
        }
    }

    public virtual void Start()
    {
        var hexNode = (HexNode)StaticObject.Node;
        //TODO -0.26 вынести в отдельную константу или вообще сервис.
        //https://answers.unity.com/questions/598492/how-do-you-set-an-order-for-2d-colliders-that-over.html
        // Статья, в которой подтверждается, что коллайдеры, расположенные на одной z-координате,
        // срабатывают в произвольном порядке.
        // Поэтому сундуки рендерятся ближе к камере и поднимают свой коллайдер.
        transform.position = new Vector3(WorldPosition.x, WorldPosition.y, hexNode.OffsetCoords.Y - 0.26f);
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        DoSelected();
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        DoMouseEnter();
    }

    private void DoSelected()
    {
        Selected?.Invoke(this, new EventArgs());
    }

    private void DoMouseEnter()
    {
        MouseEnter?.Invoke(this, new EventArgs());
    }
}