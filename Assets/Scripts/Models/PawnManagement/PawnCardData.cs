using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PawnCardData
{
    public string cardSpritePath = ""; // FIXME: Resources.Load does not work with spritesheets/texture atlases
    public int count = 1;
    public float movementSpeed;
    public int maxHealth = 100;


    public PawnCardData(string cardSpritePath, int count, float movementSpeed, int maxHealth)
    {
        this.cardSpritePath = cardSpritePath;
        this.count = count;
        this.movementSpeed = movementSpeed;
        this.maxHealth = maxHealth;
    }

}
