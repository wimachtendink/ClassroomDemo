using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps XR rig alive and consistent between scenes
/// </summary>
public class XRRig_Singleton : MonoBehaviour
{
	public static XRRig_Singleton instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			//DontDestroyOnLoad(gameObject);
		}
		else
		{
			//Destroy(gameObject);
			Debug.LogError($"Multiple Singleton Exception - {this}");
		}
	}

	//dictionary property drawers are doable but annoying
	[Header("0 = none, 1 = CenterEye, 2 = LeftHand, 3 = RightHand")]
	public List<Transform> followables;



	public GameObject NetworkFailedMessage;

	IEnumerator NetworkMessageTimer()
	{
		NetworkFailedMessage.SetActive(true);
		yield return new WaitForSeconds(6f);
		NetworkFailedMessage.SetActive(false);
	}

	public void ShowNetworkErrorOptions()
	{
		StartCoroutine(NetworkMessageTimer());
	}

}
