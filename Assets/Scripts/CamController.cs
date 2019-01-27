using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{


	Camera _cam;
	MainBuilding _main;
	private void Start() {
		_cam = GetComponent<Camera>();
		_main = FindObjectOfType<MainBuilding>();
	}

	private void LateUpdate() {
		var mult = 1.5f;

		//https://answers.unity.com/questions/1231701/fitting-bounds-into-orthographic-2d-camera.html
		var bounds = _main.bounds;

		float screenRatio = (float)Screen.width / (float)Screen.height;

		float targetRatio = bounds.size.x / bounds.size.y;

		if(screenRatio >= targetRatio) {
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, mult * bounds.size.y / 2, 3 * Time.deltaTime);
		}
		else {
			float differenceInSize = targetRatio / screenRatio;
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, mult * bounds.size.y / 2 * differenceInSize, 3 * Time.deltaTime);
		}

		var newPosition = new Vector3(bounds.center.x, bounds.center.y, -1f);
        transform.position = Vector3.Lerp(transform.position, newPosition, 3 * Time.deltaTime);

    }

}
