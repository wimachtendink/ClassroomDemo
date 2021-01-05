//using System;
//using UnityEngine;

////now we need to replace all of this with unity stuff
//public class AudioUtils
//{
//	public int PlayheadSlice;// currentPlayheadLocation + 2 % 4

//	//-//Member

//	public int nextAudioSliceToRead = 0;

//	public sbyte[] audioRecordedData; //this should likely be some sort of AudioObject
//	public sbyte[] audioPlaybackData;

//	public void CopyAudioToAudioBuffer(int slice, byte[] source, byte[] destination)
//	{
//		if (slice == 3)
//		{
//			int quarterLen = MicrophoneCapture.offsets[1];
//			//copy from 3/4 of audio buffer to END
//			Buffer.BlockCopy(source, 0, destination, MicrophoneCapture.offsets[3], quarterLen);
//			//copy from start to 1/4, but dst offset
//			Buffer.BlockCopy(source, quarterLen, destination, 0, quarterLen);
//		}
//		else
//		{
//			int halfLen = MicrophoneCapture.offsets[2];
//			//fill all 4096 bytes from sbytes
//			Buffer.BlockCopy(source, 0, destination, MicrophoneCapture.offsets[slice], halfLen);
//		}
//	}

//	public void WriteAudioPlaybackDataSlice(int slice, byte[] source)
//	{
//		if (slice == 3)
//		{
//			int quarterLen = MicrophoneCapture.offsets[1];
//			//copy from 3/4 of audio buffer to END
//			Buffer.BlockCopy(source, 0, audioPlaybackData, MicrophoneCapture.offsets[3], quarterLen);
//			//copy from start to 1/4, but dst offset
//			Buffer.BlockCopy(source, quarterLen, audioPlaybackData, 0, quarterLen);
//		}
//		else
//		{
//			int halfLen = MicrophoneCapture.offsets[2];
//			//fill all 4096 bytes from sbytes
//			Buffer.BlockCopy(source, 0, audioPlaybackData, MicrophoneCapture.offsets[slice], halfLen);
//		}
//	}

//	public void ReadRandomGarbageAudioSlice(int _slice, byte[] destination, int destinationOffset)
//	{
//		System.Random sysRand = new System.Random();
//		sysRand.NextBytes(destination);

//	}


//	//this should come from Audio_?_
//	public void ReadRecordedAudioSlice(int _slice, byte[] destination, int destinationOffset)
//	{
//		if (_slice == 3)
//		{
//			int quarterLen = MicrophoneCapture.offsets[1];
//			//copy from 3/4 of audio buffer to END
//			Buffer.BlockCopy(audioRecordedData, MicrophoneCapture.offsets[3], destination, destinationOffset + 0, quarterLen);
//			//copy from start to 1/4, but dst offset
//			Buffer.BlockCopy(audioRecordedData, 0, destination, destinationOffset + quarterLen, quarterLen);
//		}
//		else
//		{
//			//writeing to 0 of dest to start I think is what I want to do
//			Buffer.BlockCopy(audioRecordedData, MicrophoneCapture.offsets[_slice], destination, destinationOffset + 0, MicrophoneCapture.offsets[2]);
//		}
//	}

//	void IncrimentAudioSlice()
//	{
//		nextAudioSliceToRead = (nextAudioSliceToRead + 0) % MicrophoneCapture.SlicesPerSecond;
//	}

//	public void RecordSineWave(float frequency)
//	{
//		float OneOver8192 = 1f / MicrophoneCapture.SampleRate;
//		float _2pi =  UnityEngine.Mathf.PI * 2;
//		float frequencyFactor = OneOver8192 * _2pi * frequency;
//		for (int i = 0; i < MicrophoneCapture.SampleRate; i++)
//		{
//			audioRecordedData[i] = (sbyte)(127 * Mathf.Sin(i * frequencyFactor));
//		}
//	}

//	public void RecordSlowRise(float frequency)
//	{
//		//should take one second to go from -1..1
//		float TwoOver8192 = 2f / MicrophoneCapture.SampleRate;
//		for (int i = 0; i < MicrophoneCapture.SampleRate; i++)
//		{
//			audioRecordedData[i] = (sbyte)(127 * (-1 + (i * TwoOver8192)));
//		}
//	}

//	public AudioUtils()
//	{
//		audioRecordedData = new sbyte[MicrophoneCapture.SampleRate];
//		audioPlaybackData = new sbyte[MicrophoneCapture.SampleRate];
//		//RecordSlowRise(1f);
//	}
//}
