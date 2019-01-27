using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconType : MonoBehaviour
{
    [SerializeField] private GameObject[] icons;

    public void ActivarIcono(int value)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] != null)
            {
                icons[i].SetActive(false);
            }
        }
        if (value == -1)
        {
            return;
        }
        icons[value].SetActive(true);

    }

}
