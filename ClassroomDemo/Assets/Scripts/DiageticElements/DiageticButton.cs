using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiageticButton : MonoBehaviour
{

	public Vector3 extents;

	public UnityEvent OnUp;
	public UnityEvent OnDown;
	public UnityEvent OnButtonDownUp;
	public UnityEvent OnHover;
	public UnityEvent OnEnter;
	public UnityEvent OnExit;

	[Tooltip("OnHover tooltip will pop up to provide user with more information - e.g. you are currently reading a tooltip")]
	[Multiline]
	public string diageticTooltip;

	bool down;

	

	//drag might break here... so I guess anything dragable just don't add downup, we can't account for all the creativity of a half-understood interaction
	void AttemptDownUp()
	{
		if(down)
		{
			OnButtonDownUp.Invoke();
		}
	}

	private void OnEnable()
	{
		OnDown.AddListener(SetDown);
		OnExit.AddListener(UnsetDown);
		OnUp.AddListener(AttemptDownUp);//on up, if down (meaning we haven't left) then we should do a downup!

		var colorer = GetComponent<ButtonColorer>();

		if(colorer != null)
		{
			colorer.SetColor_Normal();
		}
	}


	void SetDown()
	{
		down = true;
	}

	void UnsetDown()
	{
		down = false;
	}

	//Unity doesn't allow cross-scene references so for thing outside this scene we unfirtunately need to make local references to singletons (or scriptable objects)

	public void SignInAsStudent()
	{
		SceneOrchestration.instance.LoadServerScene();
	}

	public void SignInAsProfessor()
	{
		//I guess you're just in the position of the professor instead of the student
	}

	public void ToggleMuteStudent()
	{
		//send config message to server with new MuteList
	}

	public void ReconnectToServer()
	{
		//unload and reload ServerScene
		SceneOrchestration.instance.LoadServerScene();
	}
}
