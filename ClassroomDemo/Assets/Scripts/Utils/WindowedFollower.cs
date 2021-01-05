using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowedFollower : MonoBehaviour
{
	public Transform transformToFollow;
	public float maxDistance;

	private void Awake()
	{
		transform.position = transformToFollow.position;
		transform.position = new Vector3(transformToFollow.position.x, 0, transformToFollow.position.z);
	}

	private void Update()
	{		
		Vector2 mypos_X_Z = new Vector2(transform.position.x, transform.position.z);
		Vector2 toFollow_X_Z = new Vector2(transformToFollow.position.x, transformToFollow.position.z);

		float distance = Vector2.Distance(mypos_X_Z, toFollow_X_Z);

		if(distance > maxDistance)
		{
			if(distance > 1_000_000_000)
			{
				//"lost tracking" breaks everything
				transform.position = new Vector3(transformToFollow.position.x, 0, transformToFollow.position.x);
				
			}
			else
			{
				float distOver = distance - maxDistance;

				Vector3 directionToMove = transformToFollow.position - transform.position;

				directionToMove.y = 0;
				Vector3.Normalize(directionToMove.normalized);

				transform.Translate(directionToMove * distOver);
			}
		}
	}
}
