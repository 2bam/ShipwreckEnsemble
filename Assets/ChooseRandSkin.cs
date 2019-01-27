using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRandSkin : MonoBehaviour
{

    public Sprite[] allSkins;
    public SpriteRenderer myRenderer; //I know this is not cool but whatever.

    // Start is called before the first frame update
    void Start()
    {
        myRenderer.sprite = allSkins.Choice();
    }
}
