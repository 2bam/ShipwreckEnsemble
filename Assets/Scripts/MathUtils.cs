using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathUtils {
	public static float Modulo(float a, float m) {
		return ((a % m) + m) % m;
	}

	public static float RoundToStep(float x, float step) {
		return Mathf.Round(x / step) * step;
	}
}
