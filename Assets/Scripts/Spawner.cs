using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Module[] modulePrefabs;
	MainBuilding _main;
	public float modSpeed;

    // Start is called before the first frame update
    void Start()
    {
		_main = FindObjectOfType<MainBuilding>();

    }

	void Spawn() {
		var cols = _main.GetComponentsInChildren<Collider2D>();
		var bounds = new Bounds();
		foreach(var col in cols)
			bounds.Encapsulate(col.bounds);

		Debug.Log("Bounds " + bounds);
		var sx = Random.Range(bounds.min.x, bounds.max.x);
		var mod = Instantiate(modulePrefabs[Random.Range(0, modulePrefabs.Length)]);
		mod.transform.position = new Vector2(sx, this.transform.position.y);
		mod.initialVelocity = Vector2.down * modSpeed;
	}

    // Update is called once per frame
    void Update()
    {
		//TODO: Change for timer
		if(Input.GetKeyDown(KeyCode.Space))
			Spawn();
    }
}
