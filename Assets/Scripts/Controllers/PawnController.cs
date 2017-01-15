using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PawnController : MonoBehaviour
{

    public GameObject PawnCardPrefab;
    public Transform CardArea;
    public PawnCardData[] PawnData;
    [System.NonSerialized]
    public PawnCardData Selected;
    //private PawnCardData selected;

    private void Awake()
    {
        pawnGameObjectMap = new Dictionary<Pawn, GameObject>();
        pawnCardMap = new Dictionary<PawnCardData, UI_PawnCard>();
    }


    private void Start()
    {

        for (int i = 0; i < PawnData.Length; i++)
        {
            PawnCardData data = PawnData[i];
            GameObject card_obj = Instantiate(PawnCardPrefab, CardArea);
            UI_PawnCard pawnCard = card_obj.GetComponent<UI_PawnCard>();
            pawnCardMap.Add(data, pawnCard);
            pawnCard.sprite = Resources.Load<Sprite>(data.cardSpritePath);
            pawnCard.data = data;
        }
        Selected = null;
    }
    Dictionary<PawnCardData, UI_PawnCard> pawnCardMap;

    Dictionary<Pawn, GameObject> pawnGameObjectMap;
    public Pawn[] Pawns
    {
        get
        {
            return pawnGameObjectMap.Keys.ToArray();
        }
    }

    /// <summary>
    /// This is not a callback function. If the mouse was pressed, it checks a bunch of stuff, then creates a new pawn
    /// </summary>
    private void OnLeftMouseButtonUp()
    {
        // We can't place something if we haven't selected something! TODO: Display a message on screen: NO CARD SELECTED
        if (Selected == null)
            return;

        // The left mouse button was released!
        // The environment does not have colliders, so we cannot do a raycast
        // But we do know that each tile is 1 unit by 1 unit, and we can get the bottom area y coord
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            // The mouse was pressed and its not over a ui/eventsystem element!
            // FIXME: Create a MouseManager Class to handle all mouse stuff
            // TODO: We seem to be creating gameObjects like this alot. Maybe make a GameObject creator?
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Pawn pawn = new Pawn(Selected, mousePos.x, mousePos.y);
            if (pawn.Data.count <= 0)
            {
                Selected = null; // FIXME: Have to remember to do this every time we cancel an operation
                return;
            }
            pawn.Data.count--;
            GameObject pawn_obj = new GameObject();
            pawn_obj.transform.SetParent(this.transform);
            pawn_obj.name = "Pawn " + pawn_obj.transform.childCount;

            SpriteRenderer sr = pawn_obj.AddComponent<SpriteRenderer>();
            // FIXME: Should create an AssetManager
            // TODO: We should think about how animations will work.
            //      Will we have a regular animator component on all the gameObjects
            //      or should we have an animator controller script that manages animations?
            sr.sprite = Resources.Load<Sprite>(pawn.Data.cardSpritePath); // Eventually, this will be a separate sprite
            pawnGameObjectMap.Add(pawn, pawn_obj);

            Selected = null;
        }
    }

    private void Update()
    {


        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp();
        }

        foreach (Pawn p in pawnGameObjectMap.Keys)
        {
            p.Update(Time.deltaTime);
            pawnGameObjectMap[p].transform.position = new Vector3(p.X, p.Y);
        }
    }

}
