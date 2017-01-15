using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PawnCardData
{
    public string cardSpritePath = ""; // FIXME: Resources.Load does not work with spritesheets/texture atlases
    public int count = 1;

    public PawnCardData(string cardSpritePath, int count = 1)
    {
        this.cardSpritePath = cardSpritePath;
        this.count = count;
    }

}
