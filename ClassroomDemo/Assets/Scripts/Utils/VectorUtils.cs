using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtils
{ 
	public static Vector3 Reciprocal(Vector3 v)
	{
		Vector3 output = Vector3.zero;

		for (int i = 0; i < 3; i++)
		{
			output[i] = 1f / v[i];
		}

		return output;
	}
}
