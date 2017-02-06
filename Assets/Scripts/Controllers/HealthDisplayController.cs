using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayController : MonoBehaviour
{
    public GameObject WorldSpaceCanvasPrefab;
    public GameObject HealthCirclePrefab;

    protected GameObject WorldSpaceCanvas;

    private void Start()
    {
        WorldSpaceCanvas = Instantiate(WorldSpaceCanvasPrefab) as GameObject;
        RectTransform rect = WorldSpaceCanvas.transform as RectTransform;

        rect.sizeDelta = new Vector2(DungeonController.instance.width, DungeonController.instance.height);
        // HACK: We want the prefabs to be as 'default' as possible, so the canvas pivot is in the middle
        rect.position = new Vector2(DungeonController.instance.width/2, DungeonController.instance.height/2);

        StartCoroutine(WaitUntilGuardGenerationComplete());
        InvokeRepeating("TakeDamageRepeatedly", 5, 1.2f);
    }

    // FIXME: Create a Controller Base Class which has an on complete callback!
    IEnumerator WaitUntilGuardGenerationComplete()
    {
        // FIXME: This is not perfect! 
        while (GuardController.instance == null || GuardController.instance.guardGameObjectMap == null)
            yield return null;
        CreateHealthCircles();
    }

    protected Dictionary<Damageable, Image> DamageableImageMap = new Dictionary<Damageable, Image>();

    protected void CreateHealthCircles()
    {

        Guard[] guards = GuardController.instance.guardGameObjectMap.Keys.ToArray<Guard>();
        for (int i = 0; i < guards.Length; i++)
        {
            GameObject healthCircle = Instantiate(HealthCirclePrefab, guards[i].Position, Quaternion.identity, WorldSpaceCanvas.transform);
            DamageableImageMap.Add(guards[i], healthCircle.GetComponent<Image>());
        }
    }


    void TakeDamageRepeatedly()
    {
        foreach (Damageable d in DamageableImageMap.Keys)
        {
            d.TakeDamage(5);
        }
    }

    private void Update()
    {
        foreach (Damageable d in DamageableImageMap.Keys)
        {
            DamageableImageMap[d].fillAmount = (float)d.Health / (float)d.MaxHealth;
            DamageableImageMap[d].color = Color.Lerp(Color.red, Color.green, (float)d.Health / (float)d.MaxHealth);
        }
        // FIXME: Why don't damageables need a position?
        foreach (Guard g in DamageableImageMap.Keys)
        {
            DamageableImageMap[g].transform.position = g.Position;
        }
    }
}
