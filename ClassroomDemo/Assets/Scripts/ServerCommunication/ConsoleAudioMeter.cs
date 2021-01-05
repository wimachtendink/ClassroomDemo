﻿using System;
using UnityEngine;

public class ConsoleAudioMeter
{
	public static void PrintAudioMeter_Dashes(sbyte[] data, int meterWidth, int offset, int length, int scale)
	{

		//we need to read as ring buffer

		for (int i = 0; i < length; i += scale)
		{
			string s = $"|";
			int dashes = ((int)Mathf.Abs(data[ (i + offset) % data.Length])) / meterWidth;
			int preDashes = meterWidth - dashes;

			//lets just put a . at the correct position?

			if (data[(i + offset) % data.Length] < 0)
			{
				for (int stringIdx = 0; stringIdx < preDashes; stringIdx++) { s += " "; }
				for (int stringIdx = 0; stringIdx < dashes; stringIdx++) { s += "-"; }
			}
			else
			{
				for (int stringIdx = 0; stringIdx < meterWidth; stringIdx++) { s += " "; }
				for (int stringIdx = 0; stringIdx < dashes; stringIdx++) { s += "-"; }
			}

			Console.WriteLine(s);
		}
	}
}