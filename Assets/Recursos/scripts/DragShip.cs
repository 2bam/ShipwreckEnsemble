using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragShip : MonoBehaviour
{
    
    private bool dragging = false;
    private float distance;
    

    [SerializeField]private float LimitLeft;
    [SerializeField]private float LimitRight;

    [SerializeField]private GameObject Player;


    void OnMouseEnter()
    {
        //renderer.material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        // renderer.material.color = originalColor;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            if (rayPoint.x >LimitRight)
            {
                rayPoint = new Vector3(LimitRight, rayPoint.y, rayPoint.z);
            }
            else if (rayPoint.x <LimitLeft)
            {
                rayPoint = new Vector3(LimitLeft, rayPoint.y, rayPoint.z);
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(rayPoint.x, transform.position.y, transform.position.z), 5*Time.deltaTime);
        }
        else
        {
            //transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }

}
