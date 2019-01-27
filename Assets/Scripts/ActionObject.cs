using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionObject : MonoBehaviour
{
	public NodeType type;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Assert(type != NodeType.Door, "Action type can't be door " + this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
