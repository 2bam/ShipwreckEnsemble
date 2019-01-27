using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconsController : MonoBehaviour
{
    public static IconsController Instance;


    public List<GameObject> Targets = new List<GameObject>();
    public Camera mCamera;
    public List<RectTransform> CanvasElements = new List<RectTransform>();
    [SerializeField] private GameObject CanvasElementToClone;

    public GameObject Testgo;

    public GameObject CanvasParent;

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
    private void Start()
    {

    }

    public void SpawnNewElement(GameObject Target)
    {
        Targets.Add(Target);
        GameObject tempicon = Instantiate(CanvasElementToClone, new Vector3(0, 0, 0), Quaternion.identity);
        tempicon.transform.SetParent(CanvasParent.transform);
        CanvasElements.Add(tempicon.GetComponent<RectTransform>());
    }
    public void DestroyElement()
    {

    }

    void LateUpdate()
    {

        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i] != null)
            {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mCamera, Targets[i].transform.position);

                CanvasElements[i].position = pos;
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
