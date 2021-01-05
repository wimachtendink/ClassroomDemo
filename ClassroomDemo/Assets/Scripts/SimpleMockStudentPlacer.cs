using UnityEngine;

//todo: this is a prime "ScriptableObject" candidate
public class SimpleMockStudentPlacer : MonoBehaviour
{

#if UNITY_EDITOR
	private void OnValidate()
	{
		//StudentSpace
		if (studentSpace_GameObject)
		{
			if (studentSpace_GameObject.GetComponent<IStudentSpace>() == null)
			{
				//invalid selection
				studentSpace = null;
				studentSpace_GameObject = null;
				Debug.LogError("studentSpace must implement IStudentSpace interface");

			}
			else
			{
				studentSpace = studentSpace_GameObject.GetComponent<IStudentSpace>();
			}
		}
		else
		{
			//revent lingering reference
			studentSpace = null;
		}
	}
#endif

	public static SimpleMockStudentPlacer instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

			//this should be based on the professor's position I suppose
			transform.position = new Vector3(transform.position.x, transform.position.y, SimpleMockStudentPlacer.instance.GetPositionInStudentSpace(Vector3.one * 0.5f).z * 0.5f);//should be the point right in the middle, as long as we only curve on one axis (z) we can offset by that much to make it a bit less distant

		}
		else
		{
			throw new System.Exception("Singleton multiple instantiation exception");
		}
	}

	//we can make other volumes into which to place students
	private IStudentSpace studentSpace;
	//todo: make a separate component type for this - not really necessary, but cleaner
	public GameObject studentSpace_GameObject;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="inputPosition">x and y should each be between -1..1</param>
	/// <returns></returns>
	public Vector3 GetPositionInStudentSpace(Vector3 inputPosition)
	{
		Vector3 outputPosition = inputPosition;

		if (studentSpace != null)
		{
			outputPosition = studentSpace.TransformToLocalPoint(inputPosition);
		}

		return outputPosition;
	}
}
