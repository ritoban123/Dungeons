using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TooltipPanel : MonoBehaviour
{
    public GameObject basicTextObject;

    private void Start()
    {
        img = GetComponent<Image>();
    }


    private void Update()
    {
        SetGuard(guard);
    }

    //public GameObject obj;
    Guard guard;

    Image img;

    /// <summary>
    /// Adds text components for each of the parameters in the guard
    /// </summary>
    /// <param name="g">The Guard. If null, hide the tooltip panel</param>
    public void SetGuard(Guard g)
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            transform.GetChild(0).SetParent(null);
        }
        if (g == null)
        {
            guard = null;
            // TODO: Add in a fade out animation (possibly using a canvas group)
            img.enabled = false;
            return;
        }
        guard = g;
        img.enabled = true;

        foreach (string param in g.GuardParameters.Keys)
        {
            Text t = Instantiate(basicTextObject, transform).GetComponent<Text>();
            t.text = string.Format("{0}: {1}%", param, Math.Round(g.GetParameter(param) * 100, 2)); // FIXME: For now, we are assuming all parameters are percents and should be displayed
        }
    }
}
