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

	public static Quaternion RotationFromNormalizedDir(Vector2 dir) {
		float rz = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		return Quaternion.Euler(0f, 0f, rz - 90);
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

	// https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
	public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection) {
		intersection = Vector2.zero;

		var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

		if(d == 0.0f) {
			return false;
		}

		var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
		var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

		if(u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f) {
			return false;
		}

		intersection.x = p1.x + u * (p2.x - p1.x);
		intersection.y = p1.y + u * (p2.y - p1.y);

		return true;
	}
}
