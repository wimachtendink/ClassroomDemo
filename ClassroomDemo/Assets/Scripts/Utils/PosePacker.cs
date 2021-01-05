using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//creates NeighbourDescription from current Pose
public class PosePacker : MonoBehaviour
{
	public static PosePacker Instance;
	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Instantiating multiple Singletons is not allowed");
		}
	}

	//we're just going to do this real simple at first
	public GameObject Head;
	public GameObject LeftHand;
	public GameObject RightHand;

	public sbyte QuantizeQuaternionElement_FloatToByte(float floatValue)
	{
		/*
		in |out
		 1 | 127
		 0 | 0
		-1 |-127
		multiply by 127
		 */

		return (sbyte)(127f * floatValue);
	}

	public sbyte QuantizePosition(float floatValue)
	{

		/*
		  in  |	out
		1.27  |	127
		-1.27 |-127
		clamp values to 1.27 meters then multiply by 100
		gives us an arm span of about 4.5ft which seems like plenty unless soneome's 9ft tall
		*/

		return (sbyte)Mathf.RoundToInt(100 * Mathf.Clamp(floatValue, -1.27f, 1.27f));
	}

	public PoseDescription PackPose()
	{
		var nd = new PoseDescription().FakeConstructor();

		//todo: establish how this will work, this is position in playerSpace... undecided at the moment

		nd.globalPosition_x = 0;
		nd.globalPosition_y = 0;

		Vector3 headPosition		= Head.transform.position;
		Vector3 leftHandPosition	= LeftHand.transform.position  - headPosition;
		Vector3 rightHandPosition	= RightHand.transform.position - headPosition;

		//-//Positions
		//head
		nd.head_Position_x		 = QuantizePosition(headPosition.x);
		nd.head_Position_y		 = QuantizePosition(headPosition.y);
		nd.head_Position_z		 = QuantizePosition(headPosition.z);
		//left hand				 
		nd.hand_Position_Left_x	 = QuantizePosition(leftHandPosition.x);
		nd.hand_Position_Left_y	 = QuantizePosition(leftHandPosition.y);
		nd.hand_Position_Left_z	 = QuantizePosition(leftHandPosition.z);
		//right hand			 
		nd.hand_Position_Right_x = QuantizePosition(rightHandPosition.x);
		nd.hand_Position_Right_y = QuantizePosition(rightHandPosition.y);
		nd.hand_Position_Right_z = QuantizePosition(rightHandPosition.z);
		
		//-//Rotations
		//head
		nd.head_Rotation_x		 = QuantizeQuaternionElement_FloatToByte(Head.transform.rotation.x);
		nd.head_Rotation_y		 = QuantizeQuaternionElement_FloatToByte(Head.transform.rotation.y);
		nd.head_Rotation_z		 = QuantizeQuaternionElement_FloatToByte(Head.transform.rotation.z);
		nd.head_Rotation_w		 = QuantizeQuaternionElement_FloatToByte(Head.transform.rotation.w);
		//left hand				 
		nd.hand_Rotation_Left_x	 = QuantizeQuaternionElement_FloatToByte(LeftHand.transform.rotation.x);
		nd.hand_Rotation_Left_y	 = QuantizeQuaternionElement_FloatToByte(LeftHand.transform.rotation.y);
		nd.hand_Rotation_Left_z	 = QuantizeQuaternionElement_FloatToByte(LeftHand.transform.rotation.z);
		nd.hand_Rotation_Left_w	 = QuantizeQuaternionElement_FloatToByte(LeftHand.transform.rotation.w);
		//right hand			 
		nd.hand_Rotation_Right_x = QuantizeQuaternionElement_FloatToByte(RightHand.transform.rotation.x);
		nd.hand_Rotation_Right_y = QuantizeQuaternionElement_FloatToByte(RightHand.transform.rotation.y);
		nd.hand_Rotation_Right_z = QuantizeQuaternionElement_FloatToByte(RightHand.transform.rotation.z);
		nd.hand_Rotation_Right_w = QuantizeQuaternionElement_FloatToByte(RightHand.transform.rotation.w);

//		Debug.Log($"packing current pose as {nd}");

		return nd;
	}
}
