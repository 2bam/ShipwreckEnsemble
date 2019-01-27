using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum NeedType {
	//NEVER CHANGE THE ORDER OR REMOVE ANY OF THESE LABELS
	None
	, Toilet
	, Eat
	, Bath
	, Sleep
	, Fish
	, Leisure
}

public class NPC : MonoBehaviour {
	Config _cfg;
	public Dictionary<NeedType, float> needs;

	public Node nodeAt;
	public event Action<NPC> onNPCDied = delegate { };
	float _speed;
	NeedType currentNeed;

	NeedType[] raiseKeys;

	public string _debug;
	TextMesh _tm;

	List<Node> path;
	int pathIndex;

	public void SetNodeAt(Node node) {
		nodeAt = node;
		var p = node.module.ownerLocalToWorld.MultiplyPoint(node.mainPos);
		p.z = -1.6f;
		transform.position = p;
		if(node.module.owner != null)
			transform.SetParent(node.module.owner.transform);
		else
			transform.SetParent(node.module.transform);
	}

	void Start() {
		_cfg = FindObjectOfType<Config>();
		needs = _cfg.needDatas.ToDictionary(nd => nd.type, nd => 0f);
		raiseKeys = _cfg.needDatas.Where(nd => nd.rateRise > 0f).Select(nd => nd.type).ToArray();
		StartCoroutine(Think());
		_tm = GetComponentInChildren<TextMesh>();
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		if(path != null) {
			var nextPosMain = path[pathIndex].mainPos;
			var nextPos = nodeAt.module.ownerLocalToWorld.MultiplyPoint(nextPosMain);
			Gizmos.DrawSphere(nextPos, 0.007f);
		}
	}
	IEnumerator Think() {
		while(true) {
			if(currentNeed == NeedType.None) {
				if(path == null) {
					var nodes = nodeAt.module.owner != null ? nodeAt.module.owner.allNodes : nodeAt.module.innerNodes;
					path = Pathfinder.BFS(nodeAt, nodes.Choice())
						.Cast<Node>().Skip(1).ToList();
					pathIndex = 0;
					if(path.Count == 0)
						path = null;
					yield return null;
					continue;
				}

				var delta = Vector3.zero;
				do {
					var nextPosMain = path[pathIndex].mainPos;
					var nextPos = nodeAt.module.ownerLocalToWorld.MultiplyPoint(nextPosMain);
					delta = nextPos - this.transform.position;
					delta.z = 0f;

					if(delta.sqrMagnitude > _speed*_speed*Time.deltaTime*Time.deltaTime)
						delta.Normalize();
					//transform.rotation = Quaternion.LookRotation(delta, Vector3.up);
					transform.position += delta * _speed * Time.deltaTime;
					yield return null;
				}
				while(delta.sqrMagnitude > 0.005f);

				
				SetNodeAt(path[pathIndex]);
				pathIndex++;
				if(pathIndex == path.Count)
					path = null;
			}
			yield return null;
		}
	}

    void Update()
    {
		var max = needs.Values.Max();
		_debug = $"{currentNeed} max={max:F2}";
		_tm.text = _debug;

		//TODO: Maybe refresh slower than each frame (every 1sec?)
		foreach(var k in raiseKeys) {
			needs[k] += _cfg.needDict[k].rateRise * Time.deltaTime / _cfg.secondsPerGameHour;
		}

		if(max >= 1f) {
			onNPCDied(this);
			Destroy(this.gameObject);
			return;
		}
		else if(max >= 0.8f) {
			_speed = _cfg.npcSpeedFast;
		}
		else {
			_speed = _cfg.npcSpeedNormal;
		}

		var newNeed = needs
			.Where(kv => kv.Value > 0.6f)
			.OrderBy(kv => _cfg.needDict[kv.Key].priority)
			.Select(kv => kv.Key)
			.FirstOrDefault();

		if(newNeed != currentNeed) {
			currentNeed = newNeed;
			Debug.Log($"Need changed from {currentNeed} to {newNeed} for {this}");
		}

    }
}
