using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPress : Button
{
    private AddColor _addColor;

    protected override void Awake()
    {
        base.Awake();
        _addColor = GetComponent<AddColor>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _addColor.OnHoldDownLMB();
    }
}