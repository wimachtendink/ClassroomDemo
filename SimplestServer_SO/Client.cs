using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Client : IDisposable
{
	//-//Static Elements

	public static int MaxClients = 41;//1 prof+40 students

	//a lot of this is audio stuff...
	public static void Init(int MaxClients, int SampleRate, byte slicesPerSecond)
	{
		Client.SampleRate = SampleRate;
		Client.ClientsList = new List<Client>();
		Client.agregatedPose = new PoseMessage_Server();
		Client.ChunksPerSecond = slicesPerSecond;

		SetAudioSliceOffsets(SampleRate, slicesPerSecond);
	}

	public static void SetAudioSliceOffsets(int audioBufferLength, byte slicesPerSecond)
	{
		AudioChunkOffsets = new int[slicesPerSecond];

		int smallestSlice = SampleRate / slicesPerSecond;
		//I want to make N offsets 0 - end
		for (int i = 0; i < slicesPerSecond; i++)
		{
			AudioChunkOffsets[i] = i * smallestSlice;
			Console.WriteLine($"slice_{i}:{AudioChunkOffsets[i]}");
		}
	}

	public static int[] AudioChunkOffsets = { 0, 2048, 4096, 6144 }; //if sample rate is default 8192

	public static byte ChunksPerSecond;

	public static int SampleRate;

	public static Dictionary<int, bool> clientIDs = new Dictionary<int, bool>(MaxClients);
	public static List<Client> ClientsList;

	public static PoseMessage_Server agregatedPose;

	public static bool AssignID(out int clientId)
	{
		clientId = int.MaxValue;

		for (int i = 0; i < 40; i++)
		{
			if (clientIDs.ContainsKey(i))
			{
				if (clientIDs[i] == false)
				{
					clientId = i;
					clientIDs[i] = true;
					return true;
				}
			}
			else
			{
				clientIDs.Add(i, true);
				clientId = i;
				return true;
			}
		}

		return false;

	}

	//-//-//Member elements

	int ClientID;
	NetworkStream networkStream;

	Dictionary<int, bool> muteList;

	int NextSliceToWrite = 0;
	public bool HasMessages => networkStream.DataAvailable;
	public bool NeedsAudio;
	public bool NeedsPoseData;
	public bool NeedsConfig;
	public int MostRecentMessageTime = 0;

	//the entire buffer of Client Audio Data
	public byte[] memberAudioData;
	public float[] audioData_Float;

	PoseDescription MostRecentPoseDescription;


	//-//Handle Messages
	float[] myfloats;

	public void HandleMessage_Config()
	{

		int derp = 0;
		byte[] bytes = new byte[4];

		derp = networkStream.Read(bytes, 0, ConfigMessage.DATA_SIZE);

		myfloats = new float[1];

		Buffer.BlockCopy(bytes, 0, myfloats, 0, sizeof(float));

		Console.WriteLine($"received float: {myfloats[0]}");
		
		NeedsConfig = true;
	}

	public void HandleMessage_Pose()
	{
		NeedsPoseData = true;
		//lol, hacky, not sure what's going to happen
		_ = networkStream.Read(ByteTypeConverter.SbytesTobytes(MostRecentPoseDescription.data), 0, PoseDescription.DATA_SIZE);
	}

	
	public void HandleMessage_Audio()
	{
		var data = new byte[AudioMessage.AudioSliceSize * sizeof(float)];
		int bytesRead = 0;
		bytesRead = networkStream.Read(data, 0, AudioMessage.AudioSliceSize * sizeof(float));

		if(bytesRead < data.Length)
		{
			Console.WriteLine($"data not big enough it seems: {bytesRead} vs {data.Length}");
		}

		Buffer.BlockCopy(data, 0, audioData_Float, NextSliceToWrite * AudioChunkOffsets[1] * sizeof(float), data.Length);

		//WriteAudioDataSlice(NextSliceToWrite, audioMessageFromClient.data);
		NextSliceToWrite = (NextSliceToWrite + 1) % ChunksPerSecond;
		NeedsAudio = true;
	}

	public void HandleMessage_Error(MessageHeader header)
	{
		//error state recovery
		var dump = new byte[8192];
		int errorBytes = 0;
		int totalBytes = 0;
		while (networkStream.DataAvailable)
		{
			errorBytes = networkStream.Read(dump, 0, 8192);

			totalBytes += errorBytes;

			for (int i = 0; i < errorBytes; i++)
			{
				Console.Write($"{dump[i]}, ");
			}
		}

		Console.Write($"\n\t totalErrorBytes: {totalBytes}\n");

	}

	public void HandleMessages()
	{
		var headerBuffer = new byte[MessageHeader.HEADER_SIZE];

		while (networkStream.DataAvailable)
		{
			int headerSizeIn = networkStream.Read(headerBuffer, 0, MessageHeader.HEADER_SIZE);

			var header = new MessageHeader(headerBuffer);

			Console.WriteLine($"header: type:{header.messageType}, ClientID:{header.ClientID}, DataByte:{header.dataByte}");

			switch (header.messageType)
			{
				case MessageHeader.MessageType.configMessage: HandleMessage_Config(); break;
				case MessageHeader.MessageType.poseMessage: HandleMessage_Pose(); break;
				case MessageHeader.MessageType.audioMessage: HandleMessage_Audio(); break;
				default: HandleMessage_Error(header); break;
			}
		}

		MostRecentMessageTime = Environment.TickCount;
	}

//-//Audio Utils
	public void WriteAudioDataSlice(int slice, byte[] source)
	{
		Buffer.BlockCopy(source, 0, memberAudioData, AudioChunkOffsets[slice], AudioMessage.AudioSliceSize);
	}

	public void ReadAudioDataSlice(int slice, byte[] destination, int destinationOffset)
	{
		Buffer.BlockCopy(memberAudioData, AudioChunkOffsets[slice], destination, 0 + destinationOffset, AudioMessage.AudioSliceSize);
	}


	public void WriteBytesToStream(byte[] bytesToWrite, NetworkStream networkStream)
	{
		
		//there's probably a better way to do this... 
		try
		{
			networkStream.Write(bytesToWrite, 0, bytesToWrite.Length);
		}
		catch (Exception)
		{
			Dispose();
		}

	}

	public void SendMessage_Config()
	{
		var configMessage = new ConfigMessage();
		configMessage.ClientId = (byte)ClientID;
		configMessage.SampleRate = Client.SampleRate;
		configMessage.SlicesPerSecond = Client.ChunksPerSecond;

		WriteBytesToStream(configMessage.GetBytes(), networkStream);

		NeedsConfig = false;
	}

	public void SendMessage_Pose()
	{
		PoseMessage_Server poseMessageOut = new PoseMessage_Server();

		for (int i = 0; i < MaxClients; i++)
		{
			//lol, what a crazy thing to do...
			poseMessageOut.poseDescriptions.Add(ClientsList[i % ClientsList.Count].MostRecentPoseDescription);
		}

		poseMessageOut.header.dataByte = (byte)MaxClients;//if there's ever more than 256 clients, update this

		WriteBytesToStream(poseMessageOut.GetBytes(), networkStream);

		NeedsPoseData = false;
	}

	public void SendMessage_Audio()
	{
		NeedsAudio = false;

		float[] AudioSignalOut = new float[AudioMessage.AudioSliceSize];

		//Aggregate NonMuted Classmates
		{ 
			foreach (var client in ClientsList)
			{
				int offset = (client.NextSliceToWrite + (ChunksPerSecond / 2)) % ChunksPerSecond;

				for (int i = 0; i < AudioMessage.AudioSliceSize; i++)
				{
					AudioSignalOut[i] += client.audioData_Float[(i + AudioChunkOffsets[offset]) % SampleRate];
				}
			}
		}

		var audioMessageToClient = new AudioMessage();

		int samplesPerChunk = SampleRate / ChunksPerSecond; //todo: this should be static like in client for consistency

		audioMessageToClient.data = new byte[samplesPerChunk * sizeof(float)];

		Buffer.BlockCopy(AudioSignalOut, 0, audioMessageToClient.data, 0, samplesPerChunk * sizeof(float));

		audioMessageToClient.header.messageType = MessageHeader.MessageType.audioMessage;//audio data = we need an enum I guess
		audioMessageToClient.header.ClientID = 255; //null userid
		audioMessageToClient.header.dataByte = (byte)NextSliceToWrite; //which audio slice we're using

		WriteBytesToStream(audioMessageToClient.GetBytes(), networkStream);
	}

#region ctor and Dispose

	//Ctor stuff
	public Client(NetworkStream _networkStream)
	{
		audioData_Float = new float[SampleRate];//one second of float audio to kick this bad boy off!

		MostRecentMessageTime = Environment.TickCount + 10000;

		if (!AssignID(out ClientID))
		{
			throw new Exception("Server Full");
		}

		Console.WriteLine($"client_{ClientID} connected");

		networkStream = _networkStream;

		//member audio data should be a full second of data I think
		memberAudioData = new byte[SampleRate];

		MostRecentPoseDescription = new PoseDescription();
	}
	public void Dispose()
	{
		ClientsList.Remove(this);
		clientIDs[ClientID] = false;
		Console.WriteLine($"client_{ClientID} disconected");
	}
#endregion //ctor and Dispose
}