using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils {
	public static float Modulo(float a, float m) {
		return ((a % m) + m) % m;
	}

	public static float RoundToStep(float x, float step) {
		return Mathf.Round(x / step) * step;
	}

	public static T Choice<T>(this IList<T> collection) {
		return collection[UnityEngine.Random.Range(0, collection.Count)];
	}


	public static void SetX(this Component comp, float x) {
		var p = comp.transform.position;
		p.x = x;
		comp.transform.position = p;
	}

	public static float GetX(this Component comp) {
		return comp.transform.position.x;
	}
}
