using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ClientComponent : MonoBehaviour
{
	Client client;

	public AudioManager microphoneCapture;

	private void Awake()
	{
		client = new Client();

		try
		{
			client.Init(440);
		}
		catch (System.Net.Sockets.SocketException)
		{
			XRRig_Singleton.instance.ShowNetworkErrorOptions();
			SceneOrchestration.instance.UnloadServerScene();
			throw;
		}

	}

	private void FixedUpdate()
	{
		client.MainLoop();
	}
}
