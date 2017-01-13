using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_PawnCard : MonoBehaviour
{
    [HideInInspector]
    public PawnCardData data;
    [HideInInspector]
    public Sprite sprite;

    Image img;
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (img.sprite != sprite)
            img.sprite = sprite;
    }

    PawnController pc;

    public void OnButtonClicked()
    {
        if (pc == null)
            pc = FindObjectOfType<PawnController>();
        pc.Selected = data;
    }
}
