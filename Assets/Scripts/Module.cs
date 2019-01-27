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

	public NeedType needType;
	public NPC inUseBy;
	public Module module;
	public Vector2 localPos;
	public Vector2 mainPos;
	public HashSet<Node> neighborsSet = new HashSet<Node>();
	public Node(NeedType needType, Module module, Vector2 position) {
		this.needType = needType;
		this.module = module;
		this.localPos = position;
		this.mainPos = position;		//Workaround until annexed to main building
	}
}

// Module component - add to top hierarchy class for modules.
public class Module : MonoBehaviour
{
	public Vector2 initialVelocity;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public MainBuilding owner;
	[HideInInspector] public Matrix4x4 ownerLocalToWorld {
		get => owner == null ? transform.localToWorldMatrix : owner.transform.localToWorldMatrix;
	}
	public Material matLine;

	public Transform actionPointsObject;
	public Transform doorPointsObject;


	public bool[] walls = new bool[0];

	public List<Node> innerNodes = new List<Node>();

	public class LineData {
		public LineRenderer rend;
		public Vector2 localP1, localP2;
		public bool isWall;
	}
	public LineData[] lineDatas;

	public bool GetOkIntersection(Vector2 a1, Vector2 a2, out Vector2 intersection, out LineData lineData) {
		foreach(var line in lineDatas) {
			if(line.isWall)
				continue;

			var b1 = transform.localToWorldMatrix * line.localP1;
			var b2 = transform.localToWorldMatrix * line.localP2;
			if(Utils.LineSegmentsIntersection(
				a1, a2
				, b1, b2
				, out intersection
			)) {
				lineData = line;
				return true;

			}

		}
		intersection = Vector2.zero;
		lineData = null;
		return false;
	}

	void ExtractPoints() {
		var o = actionPointsObject;
		Debug.Assert(o.localPosition == Vector3.zero && o.localRotation == Quaternion.identity && o.localScale == Vector3.one);
		o = doorPointsObject;
		Debug.Assert(o.localPosition == Vector3.zero && o.localRotation == Quaternion.identity && o.localScale == Vector3.one);

		innerNodes = doorPointsObject
					.Cast<Transform>()
					.Select(xf => new Node(NeedType.None, this, xf.transform.localPosition))
					.ToList();

		innerNodes.AddRange(
			actionPointsObject
					.Cast<Transform>()
					.Select(xf => {
						var ao = xf.GetComponent<ActionObject>();
						return new Node(ao != null ? ao.needType : NeedType.None, this, xf.transform.localPosition);
					})
					.ToList()
		);
		

		//DestroyImmediate(actionPointsObject.gameObject);
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

	LineData CreateLine(int index, Vector2 p1, Vector2 p2) {
		var line = new GameObject("line").AddComponent<LineRenderer>();
		line.material = matLine;
		line.transform.parent = this.transform;
		line.transform.localPosition = new Vector3(0, 0, -1);
		line.useWorldSpace = false;
		line.startWidth = line.endWidth = 0.1f;
		var color = index < walls.Length && walls[index] ? Color.red : Color.green;
		color.a = 0.5f;
		line.startColor = line.endColor = color;
		line.SetPosition(0, p1);
		line.SetPosition(1, p2);

		var lineData = new LineData();
		lineData.rend = line;
		lineData.localP1 = p1;
		lineData.localP2 = p2;
		lineData.isWall = index < walls.Length && walls[index];

		return lineData;
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
		lineDatas = new LineData[count];
		for(int i = 0; i < count; i++) {
			lineDatas[i] = CreateLine(i, poly.points[i], poly.points[(i + 1) % count]);
		}
    }

	private void OnDrawGizmos() {
		var poly = GetComponent<PolygonCollider2D>();
		int count = poly.GetTotalPointCount();
		for(int s = -1; s <= 1; s++) {
			Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Scale(Vector3.one * (1+s*.05f));
			for(int i = 0; i < count; i++) {
				Gizmos.color = i < walls.Length && walls[i] ? Color.red : Color.green;
				Gizmos.DrawLine(poly.points[i], poly.points[(i + 1) % count]);
			}
		}

		if(!Application.isPlaying)
			return;

		Gizmos.matrix = Matrix4x4.identity;

		foreach(var node in innerNodes) {
			Gizmos.color = Color.green;
		

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
		Debug.Log($"Module collision enter {this}: {collision.collider} {collision.otherCollider}");
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
