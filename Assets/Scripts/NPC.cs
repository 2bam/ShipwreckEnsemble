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
	Node _lastNode;
	public event Action<NPC> onNPCDied = delegate { };
	float _speed;
	NeedType currentNeed;
	public float max;

	NeedType[] raiseKeys;

	public string _debug;
	TextMesh _tm;

	List<Node> path;
	int pathIndex;

	public void SetNodeAt(Node node) {
		_lastNode = (nodeAt == null ? node : nodeAt);
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
			Node target = null;
			var nodes = nodeAt.module.owner != null ? nodeAt.module.owner.allNodes : nodeAt.module.innerNodes;

			if(path == null && currentNeed != NeedType.None && currentNeed == nodeAt.needType) {
				if(nodeAt.inUseBy != null && nodeAt.inUseBy != this) {
					path = new List<Node>() { _lastNode };
					pathIndex = 0;
				}
				else {
					nodeAt.inUseBy = this;

					if(!needs.ContainsKey(currentNeed) || needs[currentNeed] == 0f) {
						nodeAt.inUseBy = null;
						currentNeed = NeedType.None;
					}
				}
				yield return null;
				continue;
			}

			if(currentNeed != nodeAt.needType) {
				target = nodes
					.Where(n => n.needType == currentNeed)
					.OrderBy(n => (n.mainPos - (Vector2)transform.localPosition).sqrMagnitude)
					.FirstOrDefault()
					;
			}
			if(target == null) {
				target = nodes.Choice();
			}

			if(target != null) {

				if(path == null) {
					path = Pathfinder.BFS(nodeAt, target)
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

					transform.rotation = Utils.RotationFromNormalizedDir(delta);
					transform.position += delta * _speed * Time.deltaTime;
					yield return null;
				}
				while(path!=null && delta.sqrMagnitude > 0.005f);

				if(path != null) {
					SetNodeAt(path[pathIndex]);
					pathIndex++;
					if(pathIndex == path.Count)
						path = null;
				}
			}
			yield return null;
		}
	}

    void Update()
    {
		max = needs.Values.Max();
		_debug = $"{currentNeed} max={max:F2}";
		_tm.text = _debug;

		//TODO: Maybe refresh slower than each frame (every 1sec?)
		foreach(var k in raiseKeys) {
			if(nodeAt.needType == k)
				needs[k] = Mathf.Max(0, needs[k] - _cfg.needDict[k].rateFall * Time.deltaTime / _cfg.secondsPerGameHour);
			else
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
			Debug.Log($"Need changed from {currentNeed} to {newNeed} for {this}");
			currentNeed = newNeed;
			path = null;    //Reset path
			if(nodeAt.inUseBy == this)
				nodeAt.inUseBy = null;
		}

    }
}
