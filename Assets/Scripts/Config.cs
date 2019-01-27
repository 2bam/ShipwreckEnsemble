using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class NeedData {
	public NeedType type = NeedType.None;
	[Range(1, 10)] public int priority = 5;
	[Range(0f, 1f)] public float rateRise = 0.1f;
	[Range(0f, 1f)] public float rateFall = 0.1f;
	public bool blocksModule = false;
	public bool knowsAvail = true;
	public float activityDuration = 1f;
}

public class Config : MonoBehaviour {


	public Dictionary<NeedType, NeedData> needDict;

	public NPC npcPrefab;

	public float secondsPerGameHour = 10f;

	public float npcSpeedNormal = 2f;
	public float npcSpeedFast = 4f;
	public float beingUsedCooldownHours = 1.2f;

	public NeedData[] needDatas = new NeedData[0];

	void Awake() {
		needDict = needDatas.ToDictionary(nd => nd.type, nd => nd);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
