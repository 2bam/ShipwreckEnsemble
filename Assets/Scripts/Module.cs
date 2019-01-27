using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public enum NodeType {
	//NEVER CHANGE THE ORDER OR REMOVE ANY OF THESE LABELS
	Way
	, Door
	, Kitchen
}

public class Node : INode {
	public Vector2 position { get => mainPos; }
	public IEnumerable<INode> neighbors { get => neighborsSet; }

	public NodeType type;
	public Module module;
	public Vector2 localPos;
	public Vector2 mainPos;
	public HashSet<Node> neighborsSet = new HashSet<Node>();
	public Node(NodeType type, Module module, Vector2 position) {
		this.type = type;
		this.module = module;
		this.localPos = position;
	}
}

// Module component - add to top hierarchy class for modules.
public class Module : MonoBehaviour
{
	public Vector2 initialVelocity;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public MainBuilding owner;
	public Material matLine;

	public Transform actionPointsObject;
	public Transform doorPointsObject;



	public List<Node> innerNodes;

	void ExtractPoints() {
		var o = actionPointsObject;
		Debug.Assert(o.localPosition == Vector3.zero && o.localRotation == Quaternion.identity && o.localScale == Vector3.one);
		o = doorPointsObject;
		Debug.Assert(o.localPosition == Vector3.zero && o.localRotation == Quaternion.identity && o.localScale == Vector3.one);

		innerNodes = doorPointsObject
					.Cast<Transform>()
					.Select(xf => new Node(NodeType.Door, this, xf.transform.localPosition))
					.ToList();

		DestroyImmediate(actionPointsObject.gameObject);
		DestroyImmediate(doorPointsObject.gameObject);
	}

	void InnerConnect() {
		//Connect all internal nodes between themselves.
		foreach(var x in innerNodes)
			foreach(var y in innerNodes)
				if(x != y) {
					x.neighborsSet.Add(y);
					y.neighborsSet.Add(x);
				}
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

	private void Awake() {
		ExtractPoints();
		InnerConnect();
	}
	// Start is called before the first frame update
	void Start()
    {
		Debug.Assert(innerNodes.Any(), "There are no door positions for module " + this);

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
		if(!Application.isPlaying)
			return;

		foreach(var node in innerNodes) {
			if(node.type == NodeType.Way)
				Gizmos.color = Color.gray;
			else if(node.type == NodeType.Door)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.blue;

			var wp0 = transform.localToWorldMatrix.MultiplyPoint(node.localPos);
			Gizmos.DrawSphere(wp0, 0.05f);
			foreach(var neighbor in node.neighborsSet) {
				var wp1 = neighbor.module.transform.localToWorldMatrix.MultiplyPoint(
					neighbor.localPos
				);
				Gizmos.DrawLine(wp0, wp1);
			}
		}

	}

	public void Lock() {
		//body.velocity = Vector2.zero;
		body.bodyType = RigidbodyType2D.Static;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log($"Module collision enter {collision}");
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
