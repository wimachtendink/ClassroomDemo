using System.Collections;
using System.Collections.Generic;

using UnityEngine.Audio;

using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
//-//Static
	
	//samples
	public static int SamplesPerSecond; //sample rate
	public static int BytesPerSample;	//bit depth

	//chunks
	public static int ChunksPerSecond;
	public static int SamplesPerChunk;
	public static int BytesPerChunk;

	//Can't instantiate a MonoBehaviour - could use Scriptable Objects - but that means making bushels of architecting choices
	public static void Init(int MaxClients, int SampleRate, byte chunksPerSecond, byte bytesPerSample)
	{
		AudioManager.SamplesPerSecond = SampleRate;
		AudioManager.ChunksPerSecond = chunksPerSecond;
		AudioManager.BytesPerSample = bytesPerSample;
		AudioManager.SamplesPerChunk = SamplesPerSecond / ChunksPerSecond;

		SetChunkOffsets();
	}

	public static void SetChunkOffsets()
	{
		chunkOffsets = new int[AudioManager.ChunksPerSecond];

		int smallestSlice = SamplesPerSecond / AudioManager.ChunksPerSecond;

		for (int i = 0; i < AudioManager.ChunksPerSecond; i++)
		{
			chunkOffsets[i] = i * smallestSlice;
			Debug.Log($"slice_{i}:{chunkOffsets[i]}");
		}
	}

	public static int[] chunkOffsets; //if sample rate is default 8192

	public static AudioManager instance;//perhaps it would be better to name this "Singleton"

//-//Member

//Unity audio
	public AudioSource audioSource;
	public AudioClip audioFromServer;
	public AudioClip microphoneClipIn;

	public LineRenderer audioDataLineRenderer;

	public AudioInputDevice CurrentActiveAudioInputDevice;
	//todo: property drawer to display known devices
	public List<AudioInputDevice> KnownAudioInputDevices;

	//slow but should only be once or twice ever so should be negligable
	public bool AudioDeviceKnown(string DeviceNameInQuestion)
	{
		foreach (AudioInputDevice audioInputDevice in KnownAudioInputDevices)
		{
			if(audioInputDevice.DeviceName == DeviceNameInQuestion)
			{
				return true;
			}
		}

		return false;
	}

	public void ScanForAudioInputDevices()
	{
		foreach (string deviceName in Microphone.devices)
		{
			if(!AudioDeviceKnown(deviceName))
			{
				int freqMax = 0;
				int freqMin = 0;
				Microphone.GetDeviceCaps(deviceName, out freqMin, out freqMax);
				AudioInputDevice tempAudioInputDevice = new AudioInputDevice(deviceName, freqMin, freqMax);

				//todo: put in some sort of console instead, inform user?
				Debug.Log($"Audio Input Device found: {deviceName}\n\tMinimum Frequency:{freqMin}\n\tMaximum Frequency:{freqMax}");

				KnownAudioInputDevices.Add(tempAudioInputDevice);
			}
		}

		//todo: actually pick one as apropriate - we can also just leave it null for default...
		CurrentActiveAudioInputDevice = KnownAudioInputDevices[0];
	}

	//so, we're just going to do floats end to end... 32b floats to boot!
	public void SetAudioFromServerData(int slice, sbyte[] source)
	{
		audioFromServer.GetData(audioChunk_float, 0);
		int offset = slice * AudioManager.SamplesPerChunk;
		
		for (int i = 0; i < AudioManager.SamplesPerChunk; i++)
		{
			audioChunk_float[(i + offset) % SamplesPerSecond] = (float)source[i] * ByteUtils.ONE_OVER_128_F;
		}

		audioFromServer.SetData(audioChunk_float, 0);
	}

	public void SetAudioFromServerData_FloatBytes(int chunk, byte[] source)
	{
		float[] audioDataFloats = new float[source.Length / sizeof(float)];

		Buffer.BlockCopy(source, 0, audioDataFloats, 0, source.Length);

		audioFromServer.SetData(audioDataFloats, AudioManager.SamplesPerChunk * chunk);
	}

	public void UpdateMeter(sbyte[] data)
	{
		for (int i = 0; i < chunkOffsets[2]; i++)
		{
			audioDataLineRenderer.SetPosition(i, new Vector3(i * 0.001f, data[i] * 0.5f, 0f));
		}
	}

	sbyte[] fullAudioTempBuffer;
	float[] audioChunk_float;
	sbyte[] audioChunk_sbyte;
	float[] audioChunk_float_from_byte;

	public int GetMostRecentlyCompletedChunk()
	{
		return ((Microphone.GetPosition(CurrentActiveAudioInputDevice.DeviceName) / AudioManager.SamplesPerChunk) + (ChunksPerSecond - 1)) % ChunksPerSecond;
	}


	public byte[] GetChunk_FloatBytes()
	{

		int audioChunkIndex = GetMostRecentlyCompletedChunk();
		microphoneClipIn.GetData(audioChunk_float, 0);

		byte[] output = new byte[AudioManager.SamplesPerChunk * sizeof(float)];

		Buffer.BlockCopy(audioChunk_float, SamplesPerChunk * audioChunkIndex * sizeof(float), output, 0, SamplesPerChunk * sizeof(float));

		return output;
	}

	public sbyte[] GetClipChunk()
	{
		int audioSliceIndex = GetMostRecentlyCompletedChunk();

		microphoneClipIn.GetData(audioChunk_float, 0);

		int offset = AudioManager.SamplesPerChunk * audioSliceIndex;

		for (int i = 0; i < SamplesPerChunk; i++)
		{
			audioChunk_sbyte[i] = (sbyte)(audioChunk_float[(i + offset) % SamplesPerSecond] * 127f);
		}

		for (int i = 0; i < SamplesPerChunk; i++)
		{
			audioChunk_sbyte[i] = (sbyte)(audioChunk_float[(i + offset) % SamplesPerSecond] * 127f);
		}

		return audioChunk_sbyte;
	}

	public void ConfigureRecordingData(int SampleRate)
	{
		AudioManager.SamplesPerSecond = SampleRate;

		microphoneClipIn = Microphone.Start("", true, 1, AudioManager.SamplesPerSecond);

		fullAudioTempBuffer = new sbyte[AudioManager.SamplesPerSecond];
		audioChunk_float = new float[AudioManager.SamplesPerSecond];
		audioChunk_float_from_byte = new float[AudioManager.BytesPerChunk];
		audioChunk_sbyte = new sbyte[AudioManager.BytesPerChunk];

		audioFromServer = AudioClip.Create("audioFromServer", AudioManager.SamplesPerSecond, 1, AudioManager.SamplesPerSecond, false);

		audioSource.clip = audioFromServer;
		audioSource.Play();

	}


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("Instantiating multiple Singleton instances not allowed");
		}

		audioSource = GetComponent<AudioSource>();

		if(SamplesPerSecond < 0)
		{
			ConfigureRecordingData(8192);
		}
		else
		{
			ConfigureRecordingData(SamplesPerSecond);
		}
	}

	private void Start()
	{
		ScanForAudioInputDevices();
	}

}
