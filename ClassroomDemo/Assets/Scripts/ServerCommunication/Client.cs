using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

using UnityEngine;

//It's slightly unfortunate that UnityApp.Client and Server.Client are both called client, but it doesn't really make sense to be more specific, does it?
public class Client
{
	//todo: put this in a message manager or something
	public List<AudioMessage> audioMessagesFromServer;
	public List<PoseMessage_Server> poseMessagesFromServer;
	public List<ConfigMessage> configMessagesFromServer;

	TcpClient tcpClient;
	NetworkStream networkStream;

	int ClientID;
	int ReadHeadChunk = 0;
	int nextChunkToWrite = 0;
	int sliceToSend = 0;

	void InitNetworking()
	{
		//I'm not sure I need to keep a reference to this, but it seems safer?
		tcpClient = new TcpClient();
		tcpClient.NoDelay = true;
		tcpClient.Client.NoDelay = true;

		tcpClient.Connect("127.0.0.1", 8080);


		networkStream = tcpClient.GetStream();
	}

	void ConfigureAudioStream()
	{
		ConfigMessage m = new ConfigMessage();

		Buffer.BlockCopy(new float[1] { 187.420f }, 0, m.data, 0, sizeof(float));

		StreamUtils.WriteMessageToStream(m, networkStream);

		var start = Environment.TickCount;

		while (configMessagesFromServer.Count == 0)
		{
			StreamUtils.ParseMessages
			(
				networkStream,
				ref audioMessagesFromServer,
				ref poseMessagesFromServer,
				ref configMessagesFromServer
			);

			if( (Environment.TickCount - start) > 10_000)//wait 10 seconds
			{
				Debug.LogError("server failed");
				return;
			}
		}

		//I *think* this should never fail at this point
		try
		{
			var configMessageFromServer = configMessagesFromServer[configMessagesFromServer.Count - 1];
			AudioManager.Init(0, configMessageFromServer.SampleRate, configMessageFromServer.SlicesPerSecond, 1);
		}
		catch (Exception e)
		{

			throw e;
		}
		
	}

	public void Init(int args)
	{

		poseMessagesFromServer = new List<PoseMessage_Server>();
		audioMessagesFromServer = new List<AudioMessage>();
		configMessagesFromServer = new List<ConfigMessage>();
		
		InitNetworking();

		ConfigureAudioStream();
	}

	public bool ServerMessagesAvailable()
	{
		return networkStream.DataAvailable;
	}

	public void GetMessages()
	{
		audioMessagesFromServer.Clear();
		poseMessagesFromServer.Clear();
		configMessagesFromServer.Clear();

		StreamUtils.ParseMessages
		(
			networkStream,
			ref audioMessagesFromServer,
			ref poseMessagesFromServer,
			ref configMessagesFromServer
		);

		if (audioMessagesFromServer.Count > 0)
		{
//			Debug.Log($"[AudioData] received {audioMessagesFromServer.Count} messages");
			AudioManager.instance.SetAudioFromServerData_FloatBytes
			(
				nextChunkToWrite,
				audioMessagesFromServer[audioMessagesFromServer.Count - 1].data
			);

			nextChunkToWrite = (nextChunkToWrite + 1) % AudioManager.ChunksPerSecond;
		}

		if(poseMessagesFromServer.Count > 0)
		{
			NeighboursRenderer.instance.UpdateNeighbours(poseMessagesFromServer[0].poseDescriptions);
		}

		if(configMessagesFromServer.Count > 0)
		{
			//handle config messages

			//todo: best solution?
			//-//scenemanager.load("reconnectingToServerScene");
		}
	}

	public void SendMessage_Pose()
	{
		PoseMessage_Client poseMessage_Client = new PoseMessage_Client();

		poseMessage_Client.poseDescription = PosePacker.Instance.PackPose();

		StreamUtils.WriteMessageToStream(poseMessage_Client, networkStream);
	}

	public bool AudioMessageAvailable()
	{
		var mostRecentChunkFinished = AudioManager.instance.GetMostRecentlyCompletedChunk();

		if(mostRecentChunkFinished != ReadHeadChunk)
		{
			ReadHeadChunk = mostRecentChunkFinished;
			return true;
		}

		return false;
	}
	public void SendMessage_Audio()
	{
		//todo: science - allocating every time is surely not performance-friendly, but sitting setting and resetting the same data seems error-prone
		var audioMessageToServer = new AudioMessage();

		audioMessageToServer.data = AudioManager.instance.GetChunk_FloatBytes();
		audioMessageToServer.header.messageType = MessageHeader.MessageType.audioMessage;

		StreamUtils.WriteMessageToStream(audioMessageToServer, networkStream);
	}

	public void MainLoop()
	{

		SendMessage_Pose();

		if (AudioMessageAvailable())
		{
			SendMessage_Audio();
		}

		if (ServerMessagesAvailable())
		{
			GetMessages();
		}
	}

	/// <summary>
	/// customizeable main loop
	/// </summary>
	/// <param name="secondsPerLoop">How many seconds between each loop (probably less than 1.0f)</param>
	/// <param name="cancelCondition">Method to be checked every loop to see if it should continue</param>
	/// <returns></returns>
	public IEnumerator MainLoop_UnscaledTime(float secondsPerLoop, Func<bool> cancelCondition)
	{
		//runs until object destroyed, currently
		while (cancelCondition())
		{
			MainLoop();
			yield return new WaitForSecondsRealtime(secondsPerLoop);
		}
	}
}