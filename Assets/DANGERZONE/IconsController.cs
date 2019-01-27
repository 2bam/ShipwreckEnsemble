using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IconsController : MonoBehaviour
{
    public static IconsController Instance;


    public List<NPC> Targets = new List<NPC>();
    public Camera mCamera;
    public List<RectTransform> CanvasElements = new List<RectTransform>();
    [SerializeField] private GameObject CanvasElementToClone;

    public GameObject Testgo;

    public GameObject CanvasParent;

    private IconType tempicon;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SpawnNewElement(NPC Target)
    {
        Targets.Add(Target);
        GameObject tempicon = Instantiate(CanvasElementToClone, new Vector3(0, 0, 0), Quaternion.identity);
        tempicon.transform.SetParent(CanvasParent.transform);
        CanvasElements.Add(tempicon.GetComponent<RectTransform>());
    }

    void LateUpdate()
    {

        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i] != null)
            {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mCamera, Targets[i].gameObject.transform.position);
                CanvasElements[i].position = Vector3.Lerp(CanvasElements[i].position, pos, 5 * Time.deltaTime);
                tempicon = CanvasElements[i].GetComponent<IconType>();
                //if (Targets[i].max > 0f)
                //{

                if (Targets[i].currentNeed == NeedType.None)
                {
                    tempicon.ActivarIcono(-1);

                    //no activa ninguno
                }
                else if (Targets[i].currentNeed == NeedType.Fish)
                {
                    tempicon.ActivarIcono(0);
                }
                else if (Targets[i].currentNeed == NeedType.Toilet)
                {
                    tempicon.ActivarIcono(4);
                }
                else if (Targets[i].currentNeed == NeedType.Sleep)
                {
                    tempicon.ActivarIcono(2);
                }
                //else { tempicon.ActivarIcono(0);
                //}



                    //Transform[] children = GetComponentsInChildren<Transform>();
                    //Transform[] allChildTransforms =   transform.Cast<Transform>().ToList().ConvertAll(t => t.gameObject);

                //}
                else
                {
                    tempicon.ActivarIcono(-1);
                }
                //Targets[i]
            }
            else
            {
                Targets.RemoveAt(i);
                Destroy(CanvasElements[i].gameObject);
                CanvasElements.RemoveAt(i);
            }
        }
    }
}
