using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOrchestration : MonoBehaviour
{
	//this scene will load other scenes based on various data and whatnot

	public static SceneOrchestration instance;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
			UnityEditor.SceneManagement.EditorSceneManager.preventCrossSceneReferences = false;
#endif
		}
	}

	public void Start()
	{
		SceneManager.LoadScene("XRRigScene", LoadSceneMode.Additive);
	}



	IEnumerator spin(AsyncOperation asyncOperation)
	{
		while(!asyncOperation.isDone)
		{
			yield return new WaitForSeconds(2);
		}

		SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
	}

	public void UnloadServerScene()
	{

		int qtyScenes = SceneManager.sceneCount;
		AsyncOperation unloading = null;

		for (int i = 0; i < qtyScenes; i++)
		{
			var scene = SceneManager.GetSceneAt(i);
			if (scene.isLoaded && scene.name == "ServerScene")
			{
				unloading = SceneManager.UnloadSceneAsync(scene);
				//not breaking... might as well kill them all if there are multiple - which there shuldn't be
			}
		}

		if (unloading != null)
		{
			StartCoroutine(spin(unloading));
		}

	}

	public void LoadServerScene()
	{
		//check if server scene is currently loaded
		//if it is, unload it

		int qtyScenes = SceneManager.sceneCount;

		AsyncOperation unloading = null;

		for (int i = 0; i < qtyScenes; i++)
		{
			var scene = SceneManager.GetSceneAt(i);
			if (scene.isLoaded && scene.name == "ServerScene")
			{
				unloading = SceneManager.UnloadSceneAsync(scene);
			}
		}

		int safetyCounter = 0;
		if(unloading != null)
		{
			StartCoroutine(spin(unloading));
		}
		else
		{
			SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
		}

		//load scene
	}
}
