
using UnityEngine;

public class MouseDragTest : MonoBehaviour
{
    private bool isdrag = false;
    private Vector3 distance;
    private BoxCollider2D col;
    [SerializeField]private float LimitLeft = -0.5f;
    [SerializeField]private float LimitRight = 0.5f;
    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }
    private void OnMouseDown()
    {
        distance = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
    }
    void OnMouseDrag()
    {
        isdrag = true;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);

        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        objPosition = new Vector3(objPosition.x + distance.x, transform.position.y, transform.position.z);
        if (objPosition.x > LimitRight)
        {
            objPosition = new Vector3(LimitRight, Input.mousePosition.y, transform.position.z);
        }
        else if (objPosition.x < LimitLeft)
        {
            objPosition = new Vector3(LimitLeft, Input.mousePosition.y, transform.position.z);
        }
        transform.position = Vector3.Lerp(transform.position, new Vector3(objPosition.x, transform.position.y, transform.position.z), 10 * Time.deltaTime);
    }

    private void OnMouseUp()
    {
        isdrag = false;
    }

    void Update()
    {
            //if (!isdrag)
            //{
            //    transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, 0), 12 * Time.deltaTime);
            //}
    }
}
