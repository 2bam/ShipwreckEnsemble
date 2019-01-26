using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainBuilding : MonoBehaviour
{
	bool _isRotating { get; set;  } = false;
	public float linearSpeed = 10f;
	//public float angularSpeed = 45f;
	public float angle = 0;

    void Start()
    {
		// Annex starting children
		foreach(var m in GetComponentsInChildren<Module>())
			Annex(null, m);
    }

	public void OnAnnexCollisionEnter2D(Module module, Collision2D collision) {
		Debug.Log($"Annex collision enter {module} {collision.collider} {collision.otherCollider} ");
		var otherMod = collision.collider.GetComponentInParent<Module>();
		if(otherMod != null)
			Annex(module, otherMod);
	}

	void Annex(Module collidedWith, Module newMod) {
		Debug.Log($"Annexing new module {newMod} collided with {collidedWith}");
		if(_isRotating) {
			Debug.Log("Can't annex while rotating");
			return;
		}

		var nearest = GetComponentsInChildren<Module>()
				.OrderBy(m => (m.transform.position - newMod.transform.position).sqrMagnitude)
				.First();

		newMod.owner = this;
		newMod.transform.SetParent(this.transform);
		
		//Snap rotation
		var angs = newMod.transform.eulerAngles;
		angs.z = MathUtils.RoundToStep(angs.z, 90f);
		newMod.transform.eulerAngles = angs;
		
		//Align position
		//TODO: Make sure no intersection occurs (relocate according to bounds and centers)
		if(collidedWith != null) {
			collidedWith = nearest;
			var np = newMod.transform.position;
			np.x = collidedWith.transform.position.x;
			newMod.transform.position = np;
		}
		//Make static
		newMod.Lock();
	}

    // Update is called once per frame
    void Update()
    {
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
    }

	private void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log($"MainBuilding collision enter {collision}");
	}

	private void OnCollisionExit2D(Collision2D collision) {
		
	}
}
