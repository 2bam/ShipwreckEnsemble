using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{


    Config _cfg;
    public Module[] modulePrefabs;
    MainBuilding _main;
    public float modSpeed;
    int _index = 0;

    ShuffleBag<Module> _moduleBag;

    // Start is called before the first frame update
    void Awake()
    {
        _cfg = FindObjectOfType<Config>();
        _main = FindObjectOfType<MainBuilding>();
        _moduleBag = new ShuffleBag<Module>(modulePrefabs);
    }


    private void UpdateSpawnerPosition()
    {
        var p = transform.position;
        float objPadding = 5;
        float camPosY = Camera.main.transform.position.y;
        float camSizeY = Camera.main.orthographicSize;
        float spawnPosY = camPosY + camSizeY + objPadding;

			p.y = spawnPosY;
			transform.position = p;
    }


    void Spawn() {
		var bounds = _main.bounds;

        Debug.Log("Bounds " + bounds);
        var sx = Random.Range(bounds.min.x, bounds.max.x);
        var mod = Instantiate(_moduleBag.Get());
        mod.name = "MOD" + _index;
        mod.transform.position = new Vector2(sx, this.transform.position.y);
        mod.initialVelocity = Vector2.down * modSpeed;

        var npc = Instantiate(_cfg.npcPrefab);
        npc.name = "NPC" + _index;
        npc.SetNodeAt(mod.innerNodes.Choice());
		if(IconsController.Instance != null)
			IconsController.Instance.SpawnNewElement(npc.gameObject);

        _index++;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Change for timer
        if (Input.GetKeyDown(KeyCode.Space))
            Spawn();
    }

    private void LateUpdate()
    {
   		UpdateSpawnerPosition();

    }
}
