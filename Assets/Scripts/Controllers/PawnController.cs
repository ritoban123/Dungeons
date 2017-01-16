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

    Dictionary<PawnCardData, UI_PawnCard> pawnCardMap;

    Dictionary<Pawn, GameObject> pawnGameObjectMap;
    public Pawn[] Pawns
    {
        get
        {
            return pawnGameObjectMap.Keys.ToArray();
        }
    }

    public MouseManager mm
    {
        get
        {
            return MouseManager.instance;
        }
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
        mm.OnLeftMouseButtonUp += OnLeftMouseButtonUp;
        mm.OnRightMouseButtonUp += OnRightMouseButtonUp;

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
            // DONE
            // TODO: We seem to be creating gameObjects like this alot. Maybe make a GameObject creator?
            Vector2 mousePos = mm.WorldSpaceMousePosition;
            Pawn pawn = new Pawn(Selected, Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));
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

            BoxCollider2D bc2d = pawn_obj.AddComponent<BoxCollider2D>(); // NOTE: The default fit should work fine, because the sprites are 1x1
            pawn_obj.layer = PawnLayer;

            pawnGameObjectMap.Add(pawn, pawn_obj);

            Selected = null;
        }
    }

    public int PawnLayer = 8;

    private void OnRightMouseButtonUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;
        // Is the mouse over a pawn?
        RaycastHit2D hit = Physics2D.Raycast(mm.WorldSpaceMousePosition, Camera.main.transform.forward, 15, 1 << PawnLayer);
        if (hit.collider != null)
        {
            hit.collider.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        }
    }

    private void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //{
        //    OnLeftMouseButtonUp();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    OnLeftMouseButtonUp();
        //}

        foreach (Pawn p in pawnGameObjectMap.Keys)
        {
            p.Update(Time.deltaTime);
            pawnGameObjectMap[p].transform.position = new Vector3(p.X, p.Y);
        }
    }

}
