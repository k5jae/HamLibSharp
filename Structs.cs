//
//  Structs.cs
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
using System.Runtime.InteropServices;

namespace HamLibSharp
{
	[StructLayout (LayoutKind.Explicit)]
	public struct Value
	{
		// Signed integer
		[FieldOffset (0)]
		int i;
		// Single precision float
		[FieldOffset (0)]
		float f;
		// Pointer to char string
		[MarshalAs (UnmanagedType.LPStr)]
		[FieldOffset (0)]
		string s;
		// Pointer to constant char string
		[MarshalAs (UnmanagedType.LPStr)]
		[FieldOffset (0)]
		string cs;
	}

	/// Numeric type
	[StructLayout (LayoutKind.Sequential)]
	struct paramN
	{
		/// Minimum value
		float min;
		/// Maximum value
		float max;
		/// Step
		float step;
	}

	/// Combo type
	[StructLayout (LayoutKind.Sequential)]
	struct paramC
	{
		/// Combo list
		[MarshalAs (UnmanagedType.LPStr)]
		string combostr;
	}

	[StructLayout (LayoutKind.Explicit)]
	struct paramU
	{
		[FieldOffset (0)]
		[MarshalAs (UnmanagedType.Struct)]
		paramN n;
		[FieldOffset (0)]
		[MarshalAs (UnmanagedType.Struct)]
		paramC c;
	}

	// TODO: ConfigurationParameter still needs to attention due to C unions ///

	// TODO: the value_t union makes this difficult
	[StructLayout (LayoutKind.Sequential)]
	public struct Granularity
	{
		// Minimum value
		IntPtr min;
		// Maximum value
		IntPtr max;
		// Step
		IntPtr step;
	}

	// Frequency range
	// Put together a group of this struct in an array to define
	// what frequencies your rig has access to.
	[StructLayout (LayoutKind.Sequential)]
	public struct FrequencyRange
	{
		public double Start { get { return start; } }

		public double End { get { return end; } }

		public RigMode Modes { get { return modes; } }

		public int LowPower { get { return low_power; } }

		public int HighPower { get { return high_power; } }

		public int Vfo { get { return vfo; } }

		public int Antenna { get { return ant; } }

		// Start frequency
		double start;
		// End frequency
		double end;
		// Bit field of RIG_MODE's
		RigMode modes;
		// Lower RF power in mW, -1 for no power (ie. rx list)
		int low_power;
		// Higher RF power in mW, -1 for no power (ie. rx list)
		int high_power;
		// VFO list equipped with this range
		int vfo;
		// Antenna list equipped with this range, 0 means all
		int ant;
	}

	// Calibration table struct
	[StructLayout (LayoutKind.Sequential)]
	public class CalibrationTable
	{
		[StructLayout (LayoutKind.Sequential)]
		public struct TableDef
		{
			// raw (A/D) value, as returned by \a RIG_LEVEL_RAWSTR
			int raw;
			// associated value, basically the measured dB value
			int val;
		}

		// number of plots in the table
		int size;

		// table of plots
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.MAX_CAL_LENGTH)]
		TableDef[] table = new CalibrationTable.TableDef[Rig.MAX_CAL_LENGTH];
	};
}
