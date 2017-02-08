using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Mode { Normal, PlacingPawn, SettingTP /* Target Position*/, RightClickMenu }
// TODO: We should probably impement this as delegates, and call different dlegates based on the state

public class PawnController : MonoBehaviour
{
    public static PawnController instance;

    public GameObject PawnCardPrefab;
    public Transform CardArea;
    public PawnCardData[] PawnData;
    [System.NonSerialized]
    public PawnCardData Selected;
    //private PawnCardData selected;

    /// <summary>
    /// Called at the start of the program. Created the Dictionaries
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        pawnGameObjectMap = new Dictionary<Pawn, GameObject>();
        gameObjectPawnMap = new Dictionary<GameObject, Pawn>(); // FIXME: Create a 2-way dictionary class!
        pawnCardMap = new Dictionary<PawnCardData, UI_PawnCard>();


        OnNewPawnCreated += PawnController_OnNewPawnCreated;
    }

    private void PawnController_OnNewPawnCreated(Pawn p)
    {
        //foreach (Pawn p in pawnGameObjectMap.Keys)
        //{
        //    p.Update(Time.deltaTime);
        //    pawnGameObjectMap[p].transform.position = p.Position;
        //}
        HealthDisplayController.instance.AddHealthCircle(p);

        Action<float> updateAction = (float deltaTime) =>
        {
            p.Update(deltaTime);
            pawnGameObjectMap[p].transform.position = p.Position;
        };

        GameManager.instance.OnUpdate += updateAction;
        pawnUpdateActionMap.Add(p, updateAction);

        p.OnDeath += OnPawnDath;
    }

    Dictionary<Pawn, Action<float>> pawnUpdateActionMap = new Dictionary<Pawn, Action<float>>();


    private void OnPawnDath(Pawn p)
    {
        // TODO: Play a beautiful death animation with funeral music!
        // For now, just delete the pawn from the 2 dictionaries
        GameObject obj = pawnGameObjectMap[p];
        pawnGameObjectMap.Remove(p);
        gameObjectPawnMap.Remove(obj);
        Destroy(obj);
        GameManager.instance.OnUpdate -= pawnUpdateActionMap[p]; // HACK to get a copy of the update function!
        HealthDisplayController.instance.RemoveHealthCircle(p);
    }

    public event Action<Pawn> OnNewPawnCreated;

    Dictionary<PawnCardData, UI_PawnCard> pawnCardMap;

    Dictionary<Pawn, GameObject> pawnGameObjectMap; // FIXME: I do not like having to remember to add to both dictionaries
    Dictionary<GameObject, Pawn> gameObjectPawnMap; // FIXME: We should create a data structure to manage this two way-dictionary

    public Mode mode = Mode.Normal;
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

    public Pawn GetPawn(GameObject obj)
    {
        if (IsGameObjectPawn(obj) == false)
        {
            Debug.LogError(obj.name + " is not a pawn gameObject");
            return null;
        }
        return gameObjectPawnMap[obj];
    }

    /// <summary>
    /// Returns If a gameObject is a pawn (parented to PawnController)
    /// </summary>
    /// <param name="obj">The object that is possibly a pawn</param>
    /// <returns>True if the object is a pawn gameobject</returns>
    public bool IsGameObjectPawn(GameObject obj)
    {
        // FIXME: This relies on the fact that pawn gameobjects are parented to this. 
        // If we end up having to change the pawn parent at some point, we should check if the gameobject is in the gameobjectpawnmap

        if (obj.transform.parent == this.transform)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Creates a PawnCardPrefab for each PawnCardData, Sets Selected to null, Registers the MouseManager callbacks, and creates a cursor gameobject
    /// </summary>
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
    /// Callback registered to the mouse manager's OnLeftMouseButtonUp Action
    /// </summary>
    private void OnLeftMouseButtonUp()
    {
        // Depending on the state call, the appropriate LeftClick function
        switch (mode)
        {
            case Mode.PlacingPawn:
                LeftClick_PlacingPawn();
                break;
            case Mode.SettingTP:
                LeftClick_SetTP();
                break;
            case Mode.RightClickMenu:
                LeftClick_RightClickMenu();
                break;
        }
    }

    private void LeftClick_RightClickMenu()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        currentSelectedPawn = null; // We shouldn't need to do this, since we are exiting setting tp mode, but just to be safe
        rightClickMenu.SetActive(false);
        mode = Mode.Normal;
    }

    /// <summary>
    /// We were SettingTP Mode, and we clicked. If we are not over a ui element, set the pawns target position to the mouse position
    /// </summary>
    private void LeftClick_SetTP()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        // TODO: Create a function to get rounded mouse position
        Tile t = DungeonController.instance.dungeon.GetTileAt(Mathf.RoundToInt(mm.WorldSpaceMousePosition.x), Mathf.RoundToInt(mm.WorldSpaceMousePosition.y));
        if (t.IsWall)
        {
            mode = Mode.Normal; // TODO: Display a message to the user!
            return;
        }
        currentSelectedPawn.TargetPosition = new Vector3(t.X, t.Y);
        currentSelectedPawn = null; // We shouldn't need to do this, since we are exiting setting tp mode, but just to be safe
        mode = Mode.Normal;
    }

    /// <summary>
    /// If we are placing a pawn, create the appropriate game and model objects, add the components, and assign the sprite
    /// </summary>
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
            Tile t = DungeonController.instance.dungeon.GetTileAt(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

            if (t == null || t.room == null)
            {
                // We are in a corridor or a wall!
                // TODO: Add predefined deploy points (where you have dug tunnels)
                return;
            }
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
            sr.sortingLayerName = "Pawns";

            BoxCollider2D bc2d = pawn_obj.AddComponent<BoxCollider2D>(); // NOTE: The default fit should work fine, because the sprites are 1x1
            bc2d.isTrigger = true; // NOTE: Raycasts must be blocked by triggers
            pawn_obj.layer = PawnLayer;

            pawnGameObjectMap.Add(pawn, pawn_obj);
            gameObjectPawnMap.Add(pawn_obj, pawn); // FIXME: repeated code for two way dictionaries!
            Selected = null;
            mode = Mode.Normal; // FIXME: I don't like having to remember to set the states manually!


            OnNewPawnCreated(pawn);
        }
    }

    public int PawnLayer = 8;
    public GameObject rightClickMenu;

    Sprite CursorSprite;

    // FIXME: This is the only state where we have a special function. We need to be consistent!
    /// <summary>
    /// Called by right-click menu button when entering tp setting mode
    /// </summary>
    public void EnterSettingTPState()
    {
        CursorSprite = Resources.Load<Sprite>("Sprites/UI/BaseCursor");
        mode = Mode.SettingTP;
    }

    Pawn currentSelectedPawn;
    /// <summary>
    /// Callback registered to the mouse manager when the right mouse button is released
    /// Checks if we were over a pawn and opens the right-click menu
    /// </summary>
    private void OnRightMouseButtonUp()
    {

        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;
        // Is the mouse over a pawn?
        GameObject hit = mm.GameObjectUnderMouse;
        //RaycastHit2D hit = Physics2D.Raycast(mm.WorldSpaceMousePosition, Camera.main.transform.forward, 15, 1 << PawnLayer);
        if (hit != null && gameObjectPawnMap.ContainsKey(hit))
        {
            // We hit a pawn!
            currentSelectedPawn = gameObjectPawnMap[hit];
            rightClickMenu.SetActive(true);
            rightClickMenu.transform.position = Input.mousePosition;
            mode = Mode.RightClickMenu;
        }
    }

    /// <summary>
    /// Called every frame. Updates the pawn position, calls their update methodsd, and makes sure the cursor is set appropriatly
    /// </summary>
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
        // FIXME: Why is the PawnController Managing the cursor stuff!
        if (mode == Mode.PlacingPawn)
        {
            cursorSr.color = new Color(1, 1, 1, 0.5f);
            CursorSprite = Resources.Load<Sprite>(Selected.cardSpritePath); // FIXME: WE NEED AN ASSET MANAGER!
        }
        else if (mode == Mode.Normal)
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



        //foreach (Pawn p in pawnGameObjectMap.Keys)
        //{
        //    p.Update(Time.deltaTime);
        //    pawnGameObjectMap[p].transform.position = p.Position;
        //}
    }


    void OnDrawGizmos()
    {
        if (pawnGameObjectMap == null)
            return;
        foreach (Pawn p in Pawns)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(p.TargetPosition, Vector3.one * 0.3f);
            //if (p.aStar == null)
            //{
            //    continue;
            //}

            //Queue<IPath_Node> path = new Queue<IPath_Node>(p.aStar.path);

            //int index = 0;
            //while (path.Count > 0)
            //{
            //    IPath_Node current = path.Dequeue();
            //    index+= 20;
            //    Gizmos.color = new Color(index/255, index/255, index/255);
            //    Gizmos.DrawCube(new Vector3(current.X, current.Y), Vector3.one * 0.3f);
            //}
        }
    }
}
