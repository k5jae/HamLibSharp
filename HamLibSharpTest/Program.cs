//
//  Program.cs
//
//  Author:
//       Jae Stutzman <jaebird@gmail.com>
//
//  Copyright (c) 2016 Jae Stutzman
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HamLibSharp;

namespace HamLibSharpTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//var rigName = "Dummy";
			//var port = string.Empty;

			//var rigName = "FT-857";
			//var port = "/dev/ttyUSB0";

			var rigName = "PiHPSDR";
			var port = "127.0.0.1:19090";

			Console.WriteLine("HamLib Native Library Version: {0}", HamLib.NativeVersion);
			Console.WriteLine("HamLib Managed Library Version: {0}", HamLib.ManagedVersion);

			Console.WriteLine ();
			Console.WriteLine ("WARNING: This will PTT your RIG!! Ensure proper dummy load");
			Console.WriteLine ("Press Enter to continue or CTRL+C to exit");
			Console.ReadLine ();

			Console.WriteLine ("Attempting to open: {0} at port: {1}", rigName, port);

			//This will output to console hamlib traces
			//HamLib.SetDebugLevel (RigDebugLevel.Trace);

			Rig rig = null;
			try {
				rig = new Rig (rigName);
				rig.Open (port);
			} catch (RigException e) {
				Console.WriteLine ("Unable to Open Rig: {0}", e.Message);
				return;
			}


			var freqHz = 28350125;
			Console.WriteLine ("Set Frequency to: {0}", Rig.FrequencyToString (freqHz));
			rig.SetFrequency (freqHz);

			Console.WriteLine ("Read Frequency is: {0}", Rig.FrequencyToString (rig.GetFrequency ()));

			Console.WriteLine ("Attempting to PTT Rig for 1 Sec");
			//rig.SetPtt (PttMode.On);

			System.Threading.Thread.Sleep (1000);
			Console.WriteLine(rig.GetPtt ());

			Console.WriteLine ("PTT OFF");
			rig.SetPtt (PttMode.Off);

			rig.SetLevel (RigLevel.Agc, (int)RigAgcLevel.Off);

			System.Threading.Thread.Sleep (1000);

			int level;
			rig.GetLevel (RigLevel.Agc, out level);
			Console.WriteLine ("AGC Level: {0}", level);

			rig.SetLevel (RigLevel.Agc, (int)RigAgcLevel.Superfast);
			System.Threading.Thread.Sleep (1000);
			rig.GetLevel (RigLevel.Agc, out level);
			Console.WriteLine ("AGC Level: {0}", level);

			rig.SetLevel (RigLevel.Agc, (int)RigAgcLevel.Fast);
			System.Threading.Thread.Sleep (1000);
			rig.GetLevel (RigLevel.Agc, out level);
			Console.WriteLine ("AGC Level: {0}", level);


			rig.SetLevel (RigLevel.Agc, (int)RigAgcLevel.Medium);
			System.Threading.Thread.Sleep (1000);
			rig.GetLevel (RigLevel.Agc, out level);
			Console.WriteLine ("AGC Level: {0}", level);

			rig.SetLevel (RigLevel.Agc, (int)RigAgcLevel.Slow);
			System.Threading.Thread.Sleep (1000);
			rig.GetLevel (RigLevel.Agc, out level);
			Console.WriteLine ("AGC Level: {0}", level);


			while (true) {
				Console.WriteLine ("Frequency is: {0}", Rig.FrequencyToString (rig.GetFrequency ()));
				System.Threading.Thread.Sleep (1000);
			}
		}
	}
}
