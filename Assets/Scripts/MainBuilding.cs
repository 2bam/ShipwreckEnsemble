using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainBuilding : MonoBehaviour
{
	Config _cfg;
	bool _isRotating { get; set;  } = false;
	public float linearSpeed = 10f;
	//public float angularSpeed = 45f;
	public float angle = 0;
	[HideInInspector] public Bounds bounds = new Bounds();

	public List<Node> allNodes = new List<Node>();
	public SpriteRenderer doorPrefab;



    void Start()
    {
		_cfg = FindObjectOfType<Config>();
		// Annex starting children
		foreach(var m in GetComponentsInChildren<Module>()) {
			Annex(null, m);
			var npc = Instantiate(_cfg.npcPrefab);
			npc.SetNodeAt(m.innerNodes.Choice());
		}


    }

	public void OnAnnexCollisionEnter2D(Module module, Collision2D collision) {
		Debug.Log($"Annex collision enter {module}: {collision.collider} {collision.otherCollider} ");
		var otherMod = collision.collider.GetComponentInParent<Module>();
		//collision.GetContact(0).
		if(otherMod != null && otherMod.owner != this)
			Annex(module, otherMod);
	}

	void PatchMainPos(Module module) {
		// Patch positions to "main building relative" positions
		foreach(var node in module.innerNodes) {
			var wPos = module.transform.localToWorldMatrix.MultiplyPoint(node.localPos);
			node.mainPos = this.transform.worldToLocalMatrix.MultiplyPoint(wPos);
		}
		
	}

	void Annex(Module ourMod, Module newMod) {
		Debug.Log($"Annexing new module {newMod} collided with our {ourMod}");
		if(_isRotating) {
			Debug.Log("Can't annex while rotating");
			return;
		}

		//Make static
		newMod.Lock();

		allNodes.AddRange(newMod.innerNodes);

		newMod.owner = this;
		newMod.transform.SetParent(this.transform);

		// Patch positions to "main building relative" positions
		PatchMainPos(newMod);
		//foreach(var node in newMod.innerNodes) {
		//	var wPos = newMod.transform.localToWorldMatrix.MultiplyPoint(node.localPos);
		//	node.mainPos = this.transform.worldToLocalMatrix.MultiplyPoint(wPos);
		//}
		
		//Snap rotation
		var angs = newMod.transform.eulerAngles;
		angs.z = Utils.RoundToStep(angs.z, 90f);
		newMod.transform.eulerAngles = angs;

		//Align position
		//TODO: Make sure no intersection occurs (relocate according to bounds and centers)
		if(ourMod != null) {
			//TODO:Check other collisions or nearby (<1 delta y distance) module to add doors

			var pairs = ourMod.innerNodes
				.SelectMany(ourNode =>
					newMod.innerNodes.Select(newNode => (ourNode, newNode))
				);

			//TODO IMPORTANT: Detect if |x|>|y|
			var candidates = pairs
				.Select(t => (delta: t.Item1.mainPos - t.Item2.mainPos, mag: (t.Item1.mainPos - t.Item2.mainPos).sqrMagnitude, t.ourNode, t.newNode))
				//.Where(t => Mathf.Abs(t.delta.x) < 0.5f)
				.Where(t => t.delta.sqrMagnitude < 1.41f)
				.OrderBy(t => t.mag)
				.Select(t => (t.ourNode, t.newNode))
				;

			//FIXME:
			//candidates = candidates.Take(1);
			bool snap = true;

			foreach(var good in candidates) {

				var wp1 = transform.localToWorldMatrix.MultiplyPoint(good.ourNode.mainPos);
				var wp2 = transform.localToWorldMatrix.MultiplyPoint(good.newNode.mainPos);

				Vector2 intersection1, intersection2;
				Module.LineData ld1, ld2;
				if(!good.newNode.module.GetOkIntersection(wp1, wp2, out intersection1, out ld1)) {
					Debug.DrawLine(wp1, wp2, Color.red, 5f);
					continue;
				}
				if(!good.ourNode.module.GetOkIntersection(wp1, wp2, out intersection2, out ld2)) {
					Debug.DrawLine(wp1, wp2, Color.magenta, 5f);
					continue;
				}
				Debug.DrawLine(intersection1, intersection1+Vector2.up, Color.blue, 5f);
				Debug.DrawLine(intersection2, intersection2+Vector2.up, Color.cyan, 5f);

				var intersection = (intersection1 + intersection2) * .5f;

				//var intersection = (wp1 + wp2) * .5f;

				var newXf = good.newNode.module.transform;

				if(snap) {
					snap = false; //Only snap to best candidate
								  //FIXME: Using X depends on rotation!!
					newXf.SetX(newXf.GetX()
						+ wp1.x
						- wp2.x
						);

					//Patch positions
					PatchMainPos(newMod);
					wp2 = transform.localToWorldMatrix.MultiplyPoint(good.newNode.mainPos);
				}

				var door = Instantiate(doorPrefab, this.transform);
				door.transform.rotation = Utils.RotationFromNormalizedDir((wp1 - wp2).normalized);
				var pi = (Vector3)intersection;
				pi.z = -0.9f;
				door.transform.position = pi;

				good.ourNode.neighborsSet.Add(good.newNode);
				good.newNode.neighborsSet.Add(good.ourNode);
			}
		}

	}

	private void LateUpdate() {
	
	}

	void Update()
    {
		// Update main building bounds
		var colliders = GetComponentsInChildren<Collider2D>();
		bounds = new Bounds();
		foreach(var col in colliders)
			bounds.Encapsulate(col.bounds);	

		var dir = Vector2.zero;
		if(Input.GetKey(KeyCode.LeftArrow))
			dir = Vector2.left;
		if(Input.GetKey(KeyCode.RightArrow))
			dir = Vector2.right;


		if(Input.GetKeyDown(KeyCode.A))
			angle -= 90;
		if(Input.GetKeyDown(KeyCode.D))
			angle += 90;


		transform.position += (Vector3)dir * Time.deltaTime * linearSpeed;
		transform.rotation = Quaternion.Euler(0, 0, angle);


		//if(Input.GetKeyDown(KeyCode.B)) {
		//	var mods = GetComponentsInChildren<Module>();
		//	var l = mods
		//		.OrderBy(m => m.transform.position.y)
		//		.ToList();

		//	var path = Pathfinder.BFS(l[0].innerNodes[0], l[l.Count - 1].innerNodes[0]);
		//	if(path != null) {
		//		_npath = path.Cast<Node>().ToList();
		//	}

		//}
    }

	private void OnDrawGizmos() {
		//Gizmos.color = Color.yellow;
		//foreach(var m in GetComponentsInChildren<Module>())
		//	if(m.innerNodes != null)
		//		foreach(var n in m.innerNodes)
		//			Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(n.mainPos), 0.12f);

		Gizmos.color = Color.magenta;
		Gizmos.matrix = transform.localToWorldMatrix;
		//if(_npath != null)
		//	for(int i = 0; i < _npath.Count - 1; i++)
		//		Gizmos.DrawLine(_npath[i].mainPos, _npath[i + 1].mainPos);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log($"MainBuilding collision enter {collision}");
	}

	private void OnCollisionExit2D(Collision2D collision) {
		
	}
}
