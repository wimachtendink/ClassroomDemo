using System;
using System.Net.Sockets;
using UnityEngine;

public class AudioMessage
{
	//-//Member
	public  MessageHeader header;
	public byte[] data;

	public byte[] GetBytes()
	{
		int bufferSize =  AudioManager.SamplesPerChunk * sizeof(float);
		
		byte[] output = new byte[MessageHeader.HEADER_SIZE + bufferSize];
		header.GetBytes().CopyTo(output, 0);

		Buffer.BlockCopy(data, 0, output, MessageHeader.HEADER_SIZE, bufferSize);
		return output;
	}

	public void ReadDataFromNetworkStream(NetworkStream networkStream, MessageHeader _header)
	{
		header = _header;
		networkStream.Read(data, 0, AudioManager.SamplesPerChunk * sizeof(float));
	}

	public AudioMessage()
	{
		header = new MessageHeader(MessageHeader.MessageType.audioMessage, 255);
		data = new byte[AudioManager.SamplesPerChunk * sizeof(float)];
	}

	public AudioMessage(NetworkStream networkStream, MessageHeader _header)
	{
		data = new byte[AudioManager.SamplesPerChunk * sizeof(float)];
		ReadDataFromNetworkStream(networkStream, _header);
	}
}