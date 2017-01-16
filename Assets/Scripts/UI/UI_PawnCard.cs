using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_PawnCard : MonoBehaviour
{
    [HideInInspector]
    public PawnCardData data;
    [HideInInspector]
    public Sprite sprite;

    public Image backImg;
    public Text countText;

    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (backImg.sprite != sprite)
            backImg.sprite = sprite;
        countText.text = data.count.ToString();

        if(data.count <= 0)
        {
            button.interactable = false;    
        }
    }

    Button button;
    PawnController pc;

    public void OnButtonClicked()
    {
        if (pc == null)
            pc = FindObjectOfType<PawnController>();
        pc.Selected = data;
        pc.mode = Mode.PlacingPawn;
        Debug.Log(sprite.name);
    }
}
