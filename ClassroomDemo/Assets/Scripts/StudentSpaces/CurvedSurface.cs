using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// describes a curved surface, x,y are primary variables, adding z doesn't offset along normal of surface
/// </summary>
public class CurvedSurface : MonoBehaviour, IStudentSpace
{
	public float CurveFactor;
	public AnimationCurve ZPositionCurve;
	public Vector3 extents;

	/// <summary>
	/// Transforms point in range -1..1 to -extents..extents and warped as apropriate
	/// </summary>
	/// <param name="inputPoint">point to transform</param>
	/// <returns>transformed point</returns>
	public Vector3 TransformToLocalPoint(Vector3 inputPoint)
	{
		Vector3 outputPoint = inputPoint;

		outputPoint.z = CurveFactor * ZPositionCurve.Evaluate(outputPoint.x + 1);
		outputPoint.Scale(extents);

		return outputPoint;
	}
}
