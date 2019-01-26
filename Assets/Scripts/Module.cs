using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Module component - add to top hierarchy class for modules.
public class Module : MonoBehaviour
{
	public Vector2 initialVelocity;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public MainBuilding owner;
	public Material matLine;

	public Transform actionPointsObject;
	public Transform doorPointsObject;

	public List<Vector2> actionPos;
	public List<Vector2> doorPos;

	void ExtractPoints() {
		actionPos = actionPointsObject
			.Cast<Transform>()
			.Select(xf => (Vector2) xf.transform.localPosition)
			.ToList();

		doorPos = doorPointsObject
			.Cast<Transform>()
			.Select(xf => (Vector2) xf.transform.localPosition)
			.ToList();

		DestroyImmediate(actionPointsObject.gameObject);
		DestroyImmediate(doorPointsObject.gameObject);
	}


	LineRenderer CreateLine(Vector2 p0, Vector2 p1) {
		var line = new GameObject("line").AddComponent<LineRenderer>();
		line.material = matLine;
		line.transform.parent = this.transform;
		line.transform.localPosition = new Vector3(0, 0, -1);
		line.useWorldSpace = false;
		line.startWidth = line.endWidth = 0.1f;
		line.startColor = line.endColor = Color.red;
		line.SetPosition(0, transform.localToWorldMatrix * p0);
		line.SetPosition(1, transform.localToWorldMatrix * p1);

		return line;
	}

    // Start is called before the first frame update
    void Start()
    {
		ExtractPoints();

		body = GetComponent<Rigidbody2D>();
		body.velocity = initialVelocity;

		var poly = GetComponent<PolygonCollider2D>();
		int count = poly.GetTotalPointCount();
		for(int i = 0; i < count; i++) {
			CreateLine(poly.points[i], poly.points[(i + 1) % count]);
		}
    }

	private void OnDrawGizmos() {
		//var poly = GetComponent<PolygonCollider2D>();
		//int count = poly.GetTotalPointCount();
		//for(int i = 0; i < count; i++) {
		//	Gizmos.DrawLine(poly.points[i], poly.points[(i + 1) % count]);
		//}		
		Gizmos.matrix *= transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		foreach(var p in doorPos)
			Gizmos.DrawSphere(p, 0.05f);

		Gizmos.color = Color.blue;
		foreach(var p in actionPos)
			Gizmos.DrawSphere(p, 0.05f);
	}

	public void Lock() {
		//body.velocity = Vector2.zero;
		body.bodyType = RigidbodyType2D.Static;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log($"Moduel collision enter {collision}");
		if(owner)
			owner.OnAnnexCollisionEnter2D(this, collision);
	}

	private void OnCollisionExit2D(Collision2D collision) {
		
	}

	private void FixedUpdate() {
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
