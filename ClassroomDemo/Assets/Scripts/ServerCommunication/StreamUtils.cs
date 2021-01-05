
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class StreamUtils
{

	#region writing
	public static void WriteBytesToStream(byte[] bytes, NetworkStream networkStream)
	{
		try
		{
			networkStream.Write(bytes, 0, bytes.Length);
			networkStream.Flush();
		}
		catch (Exception)
		{
			//here we need to handle and stop checking or try to reconnect?
			throw;
		}
	}

	public static void WriteMessageToStream(AudioMessage audioMessage, NetworkStream networkStream)
	{
		WriteBytesToStream(audioMessage.GetBytes(), networkStream);
	}

	public static void WriteMessageToStream(PoseMessage_Client poseMessage, NetworkStream networkStream)
	{
		WriteBytesToStream(poseMessage.GetBytes(), networkStream);
	}

	public static void WriteMessageToStream(ConfigMessage configMessage, NetworkStream networkStream)
	{
		WriteBytesToStream(configMessage.GetBytes(), networkStream);
	}

	#endregion //writing

	#region reading

	public static MessageHeader ReadHeaderFromStream(NetworkStream networkStream)
	{
		byte[] headerBuffer = new byte[MessageHeader.HEADER_SIZE];

		int bytesRead = networkStream.Read(headerBuffer, 0, MessageHeader.HEADER_SIZE);

		return new MessageHeader(headerBuffer);
	}

	#endregion //reading

	public static void ParseMessages
	(
		NetworkStream networkStream, 
		ref List<AudioMessage> audioMessages, 
		ref List<PoseMessage_Server> poseMessages,
		ref List<ConfigMessage> configMessages
	)
	{
		while (networkStream.DataAvailable)
		{
			MessageHeader header = ReadHeaderFromStream(networkStream);

//			Debug.Log(header);

			switch (header.messageType)
			{
				case MessageHeader.MessageType.configMessage:
					configMessages.Add(new ConfigMessage(networkStream, header));
					break;
				case MessageHeader.MessageType.poseMessage:
					poseMessages.Add(new PoseMessage_Server(networkStream, header)); 
					break;
				case MessageHeader.MessageType.audioMessage:
					audioMessages.Add(new AudioMessage(networkStream, header));
					break;
				default:
					Console.WriteLine($"[ERROR] Message type {header.messageType})");
					break;
			}
		}
	}
}