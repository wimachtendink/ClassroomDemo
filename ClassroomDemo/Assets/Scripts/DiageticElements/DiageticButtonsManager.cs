using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiageticButtonsManager : MonoBehaviour
{

	public static DiageticButtonsManager instance;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;

			DiageticButtons = new List<DiageticButton>(GetComponentsInChildren<DiageticButton>(true));
		}
		else
		{
			throw new System.Exception($"Singleton Multiple Instance Exception {this}");
		}
	}


	public Transform PrimaryHandCursor;

	public List<DiageticButton> DiageticButtons;

	DiageticButton CurrentCursorTarget;

	float HoverTime = 1;

	IEnumerator HoverTimer(float HoverTimeSeconds, int currentHoverInstanceId)
	{
		var t = 0f;
		while(t < HoverTimeSeconds)
		{
			t += Time.deltaTime;

			yield return new WaitForEndOfFrame();

			if (CurrentCursorTarget == null || CurrentCursorTarget.GetInstanceID() != currentHoverInstanceId)
			{
				yield break;
			}
		}

		CurrentCursorTarget.OnHover.Invoke();
	}

	bool down;

	//why not just "onTriggerExit"?  - because it comes with an entire physics engine
	private void Update()
	{
		if(CurrentCursorTarget == null)//trying to 
		{

			foreach (var button in DiageticButtons)
			{
				if (!button.isActiveAndEnabled)
				{
					continue;
				}
				//CheckForHover
				if (button.transform.InverseTransformPoint(PrimaryHandCursor.position).magnitude < 1f)//might be really slow...
				{
					Debug.LogWarning($"Entering {button}");
					button.OnEnter.Invoke();

					CurrentCursorTarget = button;
					break;
				}
			}
		}
		else
		{

			if (CurrentCursorTarget.transform.InverseTransformPoint(PrimaryHandCursor.position).magnitude > 1f)//does this work? not sure
			{
				Debug.LogWarning($"Exiting {CurrentCursorTarget}");
				CurrentCursorTarget.OnExit.Invoke();
				CurrentCursorTarget = null;
			}

		}

		//I need a fake OpenXR Changed + value
		if(!down)
		{
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > .8f)
			{

				Debug.LogWarning("PrimaryHandTrigger Down");

				if (CurrentCursorTarget != null)
				{
					CurrentCursorTarget.OnDown.Invoke();
				}

				down = true;
			}
		}
		else
		{
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) < .05f)
			{
				if (CurrentCursorTarget != null)
				{
					CurrentCursorTarget.OnUp.Invoke();
				}

				down = false;
			}
		}
	}

}
