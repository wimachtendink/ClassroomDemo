using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// This class should take neighbourData and plug it into VFXGraph displaying the body parts at their apropriate positions
/// </summary>
public class NeighboursRenderer : MonoBehaviour
{
	public static NeighboursRenderer instance;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("Duplicate Singletons Not Allowed");
			Destroy(gameObject);
		}
	}

	int _qtyNeighbours;
	public int QtyNeighbours
	{
		get
		{
			return _qtyNeighbours;
		}
		set
		{
			//not sure how else to 
			int oldValue = _qtyNeighbours;
			
			_qtyNeighbours = value;

			if(oldValue < _qtyNeighbours)
			{
				PopulateListsWithPrefabs();
			}
		}
	}

	int QtyInstantiatedNeighbourAvatars = 0;

	public List<Transform> LeftHands_Pool;
	public List<Transform> RightHands_Pool;
	public List<Transform> Heads_Pool;

	public GameObject Heads_Prefab;
	public GameObject RightHands_Prefab;
	public GameObject LeftHands_Prefab;

	public Transform StudentAvatarsParent;

	int TestCounter = -127;

	public List<PoseDescription> CopyFromSingle(PoseDescription poseDescriptionToCopy)
	{
		var output = new List<PoseDescription>(40);

		for (int i = 0; i < 40; i++)
		{
			output.Add(new PoseDescription(poseDescriptionToCopy.data));
		}

		return output;
	}

	public void SetNeighbourAppearanceFromDescription(int poolsIdx, PoseDescription pd)
	{

		float positionDequantizationFactor = 0.01f;
		float rotationDequantizationFactor = 1f / 127f;

		//todo: remove this!
		float tempX = (2.5f - (poolsIdx % 6)) / 6f;
		float tempY = (2.5f - (poolsIdx / 6)) / 6f;

		//-//Positions
		//StudentSpace position
		Vector3 StudentGlobalPosition = SimpleMockStudentPlacer.instance.GetPositionInStudentSpace( new Vector3((pd.globalPosition_x * positionDequantizationFactor) + tempX, (pd.globalPosition_y * positionDequantizationFactor) + tempY, 0));
		//head
		Vector3 headPosition = 
			(new Vector3(pd.head_Position_x, pd.head_Position_y, pd.head_Position_z) * positionDequantizationFactor) + StudentGlobalPosition;

		Heads_Pool[poolsIdx].localPosition = headPosition;

		//left hand
		LeftHands_Pool[poolsIdx].localPosition = 
			(new Vector3(pd.hand_Position_Left_x, pd.hand_Position_Left_y, pd.hand_Position_Left_z) * positionDequantizationFactor) + headPosition;
		//right hand
		RightHands_Pool[poolsIdx].localPosition = 
			(new Vector3(pd.hand_Position_Right_x, pd.hand_Position_Right_y, pd.hand_Position_Right_z) * positionDequantizationFactor) + headPosition;

		//-//Rotations
		//left
		LeftHands_Pool[poolsIdx].localRotation = new Quaternion(
													pd.hand_Rotation_Left_x * rotationDequantizationFactor,
													pd.hand_Rotation_Left_y * rotationDequantizationFactor,
													pd.hand_Rotation_Left_z * rotationDequantizationFactor,
													pd.hand_Rotation_Left_w * rotationDequantizationFactor);


		//right
		RightHands_Pool[poolsIdx].localRotation = new Quaternion(
												pd.hand_Rotation_Right_x * rotationDequantizationFactor,
												pd.hand_Rotation_Right_y * rotationDequantizationFactor,
												pd.hand_Rotation_Right_z * rotationDequantizationFactor,
												pd.hand_Rotation_Right_w * rotationDequantizationFactor);

		//head
		Heads_Pool[poolsIdx].localRotation = new Quaternion(
												pd.head_Rotation_x * rotationDequantizationFactor,
												pd.head_Rotation_y * rotationDequantizationFactor,
												pd.head_Rotation_z * rotationDequantizationFactor,
												pd.head_Rotation_w * rotationDequantizationFactor);
	}

	/// <summary>
	/// I want to deal directly with the GFX stack for this because all transformComponents and everything else are totally unnecessary, I'll work on that sun-mon if there is time
	/// </summary>
	public void UpdateNeighbours(List<PoseDescription> neighbourDescriptions)
	{
		if(QtyNeighbours < neighbourDescriptions.Count)
		{
			QtyNeighbours = neighbourDescriptions.Count;
		}

		int counter = 0;

		for (int i = 0; i < neighbourDescriptions.Count; i++)
		{
			SetNeighbourAppearanceFromDescription(i, neighbourDescriptions[i]);
		}
	}

	void PopulateListsWithPrefabs()
	{
		if(LeftHands_Pool == null)
		{
			LeftHands_Pool = new List<Transform>();
		}

		if(RightHands_Pool == null)
		{
			RightHands_Pool = new List<Transform>();
		}

		if(Heads_Pool == null)
		{
			Heads_Pool = new List<Transform>();
		}

		while(QtyInstantiatedNeighbourAvatars < QtyNeighbours)
		{
			var tempLeftHand = GameObject.Instantiate(LeftHands_Prefab);
			tempLeftHand.transform.SetParent(StudentAvatarsParent);
			LeftHands_Pool.Add(tempLeftHand.transform);

			var tempRightHand = GameObject.Instantiate(RightHands_Prefab);
			tempRightHand.transform.SetParent(StudentAvatarsParent);
			RightHands_Pool.Add(tempRightHand.transform);

			var tempHead = GameObject.Instantiate(Heads_Prefab);
			tempHead.transform.SetParent(StudentAvatarsParent);
			Heads_Pool.Add(tempHead.transform);

			QtyInstantiatedNeighbourAvatars++;
		}
	}
}
