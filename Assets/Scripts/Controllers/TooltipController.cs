using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    public UI_TooltipPanel tooltip;

    void Start()
    {
        MouseManager.instance.OnLeftMouseButtonUp += DisplayGuardTooltipClicked;
        MouseManager.instance.OnMouseOverLayer += DisplayGuardTooltip;
    }

    private void DisplayGuardTooltip(int layer, GameObject obj)
    {
        if (layer != 10 && locked == false)
        {
            tooltip.SetGuard(null); // FIXME: Calling this every frame doesn't seem to be very efficient
            return;
        }
        if(locked == false)
            tooltip.SetGuard(GuardController.instance.GetGuardForGameObject(obj));

    }

    bool locked = false;

    private void DisplayGuardTooltipClicked()
    {
        // BUG: We might be setting the target position for a pawn! Maybe let other things 'claim' clicks?
        locked = !locked;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
