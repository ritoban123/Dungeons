using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : MonoBehaviour
{

    public GameObject PawnCardPrefab;
    public Transform CardArea;
    public PawnCardData[] PawnData;
    [HideInInspector]
    public PawnCardData Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            Debug.Log(selected.spritePath);
        }
    }

    private PawnCardData selected;


    private void Start()
    {

        for (int i = 0; i < PawnData.Length; i++)
        {
            PawnCardData data = PawnData[i];
            GameObject card_obj = Instantiate(PawnCardPrefab, CardArea);
            UI_PawnCard pawnCard = card_obj.GetComponent<UI_PawnCard>();
            pawnCard.sprite = Resources.Load<Sprite>(data.spritePath);
            pawnCard.data = data;
        }
    }
}
