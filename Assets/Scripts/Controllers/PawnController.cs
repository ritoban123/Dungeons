using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum State { Normal, PlacingPawn, SettingTP /* Target Position*/}
// TODO: We should probably impement this as delegates, and call different dlegates based on the state

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
        gameOjbectPawnMap = new Dictionary<GameObject, Pawn>(); // FIXME: Create a 2-way dictionary class!
        pawnCardMap = new Dictionary<PawnCardData, UI_PawnCard>();
    }

    Dictionary<PawnCardData, UI_PawnCard> pawnCardMap;

    Dictionary<Pawn, GameObject> pawnGameObjectMap; // FIXME: I do not like having to remember to add to both dictionaries
    Dictionary<GameObject, Pawn> gameOjbectPawnMap; // FIXME: We should create a data structure to manage this two way-dictionary

    public State state = State.Normal;
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

        GameObject cursor = new GameObject("Cursor");
        cursorSr = cursor.AddComponent<SpriteRenderer>();
    }
    SpriteRenderer cursorSr;

    /// <summary>
    /// This is not a callback function. If the mouse was pressed, it checks a bunch of stuff, then creates a new pawn
    /// </summary>
    private void OnLeftMouseButtonUp()
    {
        switch (state)
        {
            case State.PlacingPawn:
                LeftClick_PlacingPawn();
                break;
            case State.SettingTP:
                LeftClick_SetTP();
                break;
        }
    }

    private void LeftClick_SetTP()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        // TODO: Create a function to get rounded mouse position
        currentSelectedPawn.TargetPosition = new Vector3(Mathf.Round(mm.WorldSpaceMousePosition.x), Mathf.Round(mm.WorldSpaceMousePosition.y));
        state = State.Normal;
    }

    private void LeftClick_PlacingPawn()
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
            gameOjbectPawnMap.Add(pawn_obj, pawn); // FIXME: repeated code for two way dictionaries!
            Selected = null;
            state = State.Normal; // FIXME: I don't like having to remember to set the states manually!
        }
    }

    public int PawnLayer = 8;
    public GameObject rightClickMenu;

    Sprite CursorSprite;

    // FIXME: This is the only state where we have a special function. We need to be consistent!
    public void EnterSettingTPState()
    {
        CursorSprite = Resources.Load<Sprite>("Sprites/UI/BaseCursor");
        state = State.SettingTP;
    }

    Pawn currentSelectedPawn;
    private void OnRightMouseButtonUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;
        // Is the mouse over a pawn?
        RaycastHit2D hit = Physics2D.Raycast(mm.WorldSpaceMousePosition, Camera.main.transform.forward, 15, 1 << PawnLayer);
        if (hit.collider != null && gameOjbectPawnMap.ContainsKey(hit.collider.gameObject))
        {
            // We hit a pawn!
            currentSelectedPawn = gameOjbectPawnMap[hit.collider.gameObject];
            rightClickMenu.SetActive(true);
            rightClickMenu.transform.position = Input.mousePosition;
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
        if (state == State.PlacingPawn)
        {
            cursorSr.color = new Color(1, 1, 1, 0.5f);
            CursorSprite = Resources.Load<Sprite>(Selected.cardSpritePath); // FIXME: WE NEED AN ASSET MANAGER!
        }
        else if (state == State.Normal)
        {
            CursorSprite = null;
        }
        else
        {
            // Setting TP
            cursorSr.color = new Color(1, 1, 1, 1f);
        }
        if (cursorSr.sprite != CursorSprite)
        {
            cursorSr.sprite = CursorSprite;
        }

        cursorSr.transform.position = new Vector3(Mathf.Round(mm.WorldSpaceMousePosition.x), Mathf.Round(mm.WorldSpaceMousePosition.y));

        foreach (Pawn p in pawnGameObjectMap.Keys)
        {
            p.Update(Time.deltaTime);
            pawnGameObjectMap[p].transform.position = new Vector3(p.X, p.Y);
        }
    }

}
