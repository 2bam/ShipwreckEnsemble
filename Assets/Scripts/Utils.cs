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

	public static Quaternion RotationFromNormalizedDirStep(Vector2 dir, float step) {
		float rz = RoundToStep(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, step);
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

	public static Vector2 RoundVec(Vector2 vec, float step) {
		vec.x = RoundToStep(vec.x, step);
		vec.y = RoundToStep(vec.y, step);
		return vec;
	}

	public static bool LineSegmentsIntersection2(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection) {
		intersection = Vector2.zero;
		float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;

		float x1lo, x1hi, y1lo, y1hi;



		Ax = p2.x - p1.x;

		Bx = p3.x - p4.x;



		// X bound box test/

		if(Ax < 0) {

			x1lo = p2.x; x1hi = p1.x;

		}
		else {

			x1hi = p2.x; x1lo = p1.x;

		}



		if(Bx > 0) {

			if(x1hi < p4.x || p3.x < x1lo) return false;

		}
		else {

			if(x1hi < p3.x || p4.x < x1lo) return false;

		}



		Ay = p2.y - p1.y;

		By = p3.y - p4.y;



		// Y bound box test//

		if(Ay < 0) {

			y1lo = p2.y; y1hi = p1.y;

		}
		else {

			y1hi = p2.y; y1lo = p1.y;

		}



		if(By > 0) {

			if(y1hi < p4.y || p3.y < y1lo) return false;

		}
		else {

			if(y1hi < p3.y || p4.y < y1lo) return false;

		}



		Cx = p1.x - p3.x;

		Cy = p1.y - p3.y;

		d = By * Cx - Bx * Cy;  // alpha numerator//

		f = Ay * Bx - Ax * By;  // both denominator//



		// alpha tests//

		if(f > 0) {

			if(d < 0 || d > f) return false;

		}
		else {

			if(d > 0 || d < f) return false;

		}



		e = Ax * Cy - Ay * Cx;  // beta numerator//



		// beta tests //

		if(f > 0) {

			if(e < 0 || e > f) return false;

		}
		else {

			if(e > 0 || e < f) return false;

		}



		// check if they are parallel

		if(f == 0) return false;

		// compute intersection coordinates //

		num = d * Ax; // numerator //

		//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

		//    intersection.x = p1.x + (num+offset) / f;
		intersection.x = p1.x + num / f;



		num = d * Ay;

		//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

		//    intersection.y = p1.y + (num+offset) / f;
		intersection.y = p1.y + num / f;



		return true;

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
