using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// FIXME: This should be a view instead!
public class HealthDisplayController : MonoBehaviour
{
    // FIXME: Create a base controller class that already implements instancing
    public static HealthDisplayController instance;


    public GameObject WorldSpaceCanvasPrefab;
    public GameObject HealthCirclePrefab;

    protected GameObject WorldSpaceCanvas;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        WorldSpaceCanvas = Instantiate(WorldSpaceCanvasPrefab) as GameObject;
        RectTransform rect = WorldSpaceCanvas.transform as RectTransform;

        rect.sizeDelta = new Vector2(DungeonController.instance.width, DungeonController.instance.height);
        // HACK: We want the prefabs to be as 'default' as possible, so the canvas pivot is in the middle
        rect.position = new Vector2(DungeonController.instance.width/2, DungeonController.instance.height/2);

        StartCoroutine(WaitUntilGenerationComplete());
        //InvokeRepeating("TakeDamageRepeatedly", 5, 1.2f);
    }

    // FIXME: Create a Controller Base Class which has an on complete callback!
    IEnumerator WaitUntilGenerationComplete()
    {
        // FIXME: This is not perfect! 
        while (GuardController.instance == null || GuardController.instance.guardGameObjectMap == null)
            yield return null;
        CreateGuardHealthCircles();
    }

    protected Dictionary<Damageable, Image> DamageableImageMap = new Dictionary<Damageable, Image>();

    protected void CreateGuardHealthCircles()
    {

        Guard[] guards = GuardController.instance.guardGameObjectMap.Keys.ToArray<Guard>();
        for (int i = 0; i < guards.Length; i++)
        {
            //GameObject healthCircle = Instantiate(HealthCirclePrefab, guards[i].Position, Quaternion.identity, WorldSpaceCanvas.transform);
            //DamageableImageMap.Add(guards[i], healthCircle.GetComponent<Image>());
            AddHealthCircle(guards[i]);
        }
    }


    void TakeDamageRepeatedly()
    {
        // HACK to ensure that we are not iterating over the dictionary while modifying it in another method
        List<Damageable> keys = new List<Damageable>(DamageableImageMap.Keys);
        foreach (Damageable d in keys)
        {
            d.TakeDamage(5);
        }
    }

    public void AddHealthCircle(Damageable d)
    {
        GameObject healthCircle = Instantiate(HealthCirclePrefab, d.Position, Quaternion.identity, WorldSpaceCanvas.transform);
        DamageableImageMap.Add(d, healthCircle.GetComponent<Image>());
    }

    public void RemoveHealthCircle(Damageable d)
    {
        //GameObject healthCircle = Instantiate(HealthCirclePrefab, d.Position, Quaternion.identity, WorldSpaceCanvas.transform);
        //DamageableImageMap.Add(d, healthCircle.GetComponent<Image>());
        Destroy(DamageableImageMap[d].gameObject);
        DamageableImageMap.Remove(d);
    }

    private void Update()
    {
        foreach (Damageable d in DamageableImageMap.Keys)
        {
            DamageableImageMap[d].fillAmount = (float)d.Health / (float)d.MaxHealth;
            DamageableImageMap[d].color = Color.Lerp(Color.red, Color.green, (float)d.Health / (float)d.MaxHealth);
            DamageableImageMap[d].transform.position = d.Position;
        }

    }
}
