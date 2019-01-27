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
	public Dictionary<Node, float> nodeCooldownSecs = new Dictionary<Node, float>();

	public Node nodeAt;
	Node _lastNode;
	public event Action<NPC> onNPCDied = delegate { };
	float _speed;
	NeedType currentNeed;
	NeedType maxNeed;
	public float max;
	string _coroutineStatus = "";

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

	bool IsNodeWalkable(INode abstractNode) {
		var node = (Node)abstractNode;

		//Don't allow to enter blocked, but do allow exit if we're at it.
		if(node.module.blocked && nodeAt.module != node.module)	
			return false;

		//If in use but not by us
		if(node.inUseBy != null && node.inUseBy != this)
			return false;

		//If in cooldown (tried to enter and failed)
		if(nodeCooldownSecs.ContainsKey(node))
			return false;

		return true;
	}

	IEnumerator Think() {
		Node target = null;
		while(true) {
			// Refresh nodes (main building or "raft")
			var nodes = nodeAt.module.owner != null ? nodeAt.module.owner.allNodes : nodeAt.module.innerNodes;

			//Change need outside working loop...
			if(maxNeed != currentNeed) {
				Debug.Log($"Need changed from {currentNeed} to {maxNeed} for {this}");
				currentNeed = maxNeed;
				target = null;
				path = null;
			}

			//If we have a target and we are at it.
			if(target == nodeAt) {
				
				if(currentNeed != NeedType.None && currentNeed == nodeAt.needType) {

					//If empty (free to use), occupy it and set relevant flags.
					if(nodeAt.inUseBy == null) {
						bool blocksModule = _cfg.needDict[currentNeed].blocksModule;
						if(blocksModule)
							nodeAt.module.blocked = true;

						nodeAt.inUseBy = this;  //USE IT
						float needValue;
						while(needs.TryGetValue(currentNeed, out needValue) && needValue > 0f) { //TODO: Or sleep interrupt
							_coroutineStatus = "working";
							yield return null;  //Wait (decrease in Update)
						}
						nodeAt.inUseBy = null;

						if(blocksModule)
							nodeAt.module.blocked = false;
					}
					else {  //If in use, cool down and recalc path
						nodeCooldownSecs[nodeAt] = _cfg.beingUsedCooldownHours * Time.deltaTime / _cfg.secondsPerGameHour;
						path = null;
						target = null;
					}


				}
				else {  //Was idle walking, reset target
					path = null;
					target = null;
				}
			}

			// If has target and not a path, rebuild path and walk it
			// (until reach or path set to null from Update)
			if(target != null && path == null) {
				if(path == null) {
					path = Pathfinder.BFS(nodeAt, target, IsNodeWalkable)
						.Cast<Node>().ToList();
					pathIndex = 0;

					if(path.Count == 0) {
						path = null;
						_coroutineStatus = "Path error";
						//Error? This shouldn't happen. At least should find nodeAt.
					}
					else
						_coroutineStatus = "Path found";

					yield return null;
					continue;
				}
			}

			if(target != null && path != null) {
				// Interpolate position until reached
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

				// Remember node we reached
				if(path != null) {
					SetNodeAt(path[pathIndex]);
					pathIndex++;
					if(pathIndex == path.Count)
						path = null;	//No more path.
				}
			}
			
			// If without target, find a target
			if(target == null) {
				if(currentNeed != nodeAt.needType) {
					target = nodes
						.Where(n => n.needType == currentNeed)
						//TODO: Filter by cooldown? or IsWalkable??
						.OrderBy(n => (n.mainPos - (Vector2)transform.localPosition).sqrMagnitude)
						.FirstOrDefault()
						;

					_coroutineStatus = "Walk purpose";
					
				}
				//If still null, find any node to walk to.
				if(target == null) {
					target = nodes.Choice();
					_coroutineStatus = "Walk idle";

				}

			}

			yield return null;
		}
	}

    void Update()
    {
		max = needs.Values.Max();
		_debug = $"{currentNeed} max={max:F2}\n{_coroutineStatus}\n";
		_debug += string.Join("\n", needs.OrderBy(kv => kv.Key.ToString()).Select(kv => $"{kv.Key,15}: {kv.Value:F2}"));
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

		foreach(var nodeKey in nodeCooldownSecs.Keys.ToArray()) {
			var val = nodeCooldownSecs[nodeKey] - Time.deltaTime;
			if(val > 0f)
				nodeCooldownSecs[nodeKey] = val;
			else
				nodeCooldownSecs.Remove(nodeKey);
		}

		maxNeed = needs
			.Where(kv => kv.Value > 0.6f)
			.OrderBy(kv => _cfg.needDict[kv.Key].priority)
			.Select(kv => kv.Key)
			.FirstOrDefault();

		//if(maxNeed != currentNeed) {
		//	Debug.Log($"Need changed from {currentNeed} to {newNeed} for {this}");
		//	currentNeed = maxNeed;

		//	//TODO: Only reset path if sleeping
		//	//path = null;    //Reset path
		//	//if(nodeAt.inUseBy == this)
		//	//	nodeAt.inUseBy = null;
		//}

    }
}
