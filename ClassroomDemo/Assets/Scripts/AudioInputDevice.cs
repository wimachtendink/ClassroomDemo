using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AudioInputDevice
{
	//seems safer that we only make them from query
	[SerializeField]
	private string _deviceName;

	//personal preference, when you have two things which are basically the same, I prefer this naming convention for the sake of intellisense suggestion ordering
	[SerializeField]
	private int _frequency_Min;
	[SerializeField]
	private int _frequency_Max;

	public string DeviceName { get => _deviceName; }// set => _deviceName = value; }
	public int Frequency_Min { get => _frequency_Min; }// set => _frequency_Min = value; }
	public int Frequency_Max { get => _frequency_Max; }// set => _frequency_Max = value; }

	//possibly not needed...
	public AudioInputDevice(string _DeviceName, int _Frequency_Min, int _Frequency_Max)
	{
		_deviceName = _DeviceName;
		_frequency_Max = _Frequency_Max;
		_frequency_Min = _Frequency_Min;
	}
}
