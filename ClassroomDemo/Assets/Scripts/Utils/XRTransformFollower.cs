using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class XRTransformFollower : MonoBehaviour
{

	public bool FollowPosition;
	public bool FollowRotation;
	public bool OffsetPosition;
	public bool OffsetRotation;

	public enum PartToFollow
	{
		none = 0,
		centerEye,
		leftHand,
		rightHand
	}

	public PartToFollow partToFollow;

	private void Start()
	{
		if(XRRig_Singleton.instance != null)
		{
			//this isn't working for some reason...
			transformToFollow = XRRig_Singleton.instance.followables[(int)partToFollow];
		}
	}

	public Vector3 positionOffset;
	
	public Vector3 rotationOffset;

	public Transform transformToFollow;// => XRRig_Singleton.instance.followables[(int)partToFollow];

	delegate void UpdateDelegate();
	UpdateDelegate OnUpdate;

	private void OnEnable()
	{
		if(FollowPosition)
		{
			if(OffsetPosition)
			{
				OnUpdate += OffsetPosition_Offset_Action;
			}
			else
			{

				OnUpdate += FollowPosition_Action;
			}
		}

		if(FollowRotation)
		{
			if(OffsetRotation)
			{
				OnUpdate += FollowRotation_Offset_Action;
			}
			else
			{
				OnUpdate += FollowRotation_Action;
			}
		}
	}

	void FollowPosition_Action()
	{
		transform.position = transformToFollow.position;
	}

	void OffsetPosition_Offset_Action()
	{
		transform.position = (transformToFollow.position + transformToFollow.TransformDirection(positionOffset)); //need to make sure this happens after setting position...
	}

	void FollowRotation_Action()
	{
		transform.rotation = transformToFollow.rotation;
	}

	void FollowRotation_Offset_Action()
	{
		transform.rotation = transformToFollow.rotation;
		transform.rotation = transform.rotation * Quaternion.Euler(rotationOffset);//this might be backwards
	}

	private void Update()
	{
		Debug.Log(OnUpdate);
		OnUpdate.Invoke();
	}

	private void OnDisable()
	{
		if (FollowPosition)
		{
			if (OffsetPosition)
			{
				OnUpdate -= OffsetPosition_Offset_Action;
			}
			else
			{

				OnUpdate -= FollowPosition_Action;
			}
		}

		if (FollowRotation)
		{
			if (OffsetRotation)
			{
				OnUpdate -= FollowRotation_Offset_Action;
			}
			else
			{
				OnUpdate -= FollowRotation_Action;
			}
		}
	}

}
