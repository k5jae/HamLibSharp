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

	[StructLayout (LayoutKind.Sequential)]
	struct paramN
	{
		/*!< Minimum value */
		float min;
		/*!< Maximum value */
		float max;
		/*!< Step */
		float step;
		/*!< Numeric type */
	}

	[StructLayout (LayoutKind.Sequential)]
	struct paramC
	{
		/*!< Combo list */
		[MarshalAs (UnmanagedType.LPStr)]
		string combostr;
		/*!< Combo type */
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

	/// Configuration parameter structure.
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class ConfigurationParameter
	{
		public int Token { get { return token.Val; } }

		public string Name { get { return name; } }

		public string Label { get { return label; } }

		public string Tooltip { get { return tooltip; } }

		public string Default { get { return dflt; } }

		public float Min { get { return min; } }

		public float Max { get { return max; } }

		public float Step { get { return step; } }

		/*!< Conf param token ID */
		Long token;
		/*!< Param name, no spaces allowed */
		[MarshalAs (UnmanagedType.LPStr)]
		string name;
		/*!< Human readable label */
		[MarshalAs (UnmanagedType.LPStr)]
		string label;
		/*!< Hint on the parameter */
		[MarshalAs (UnmanagedType.LPStr)]
		string tooltip;
		/*!< Default value */
		[MarshalAs (UnmanagedType.LPStr)]
		string dflt;
		/*!< Type of the parameter */
		RigConf type;
		/*!< */
		//[MarshalAs (UnmanagedType.Struct)]
		//paramU u;
		/*!< Minimum value */
		float min;
		/*!< Maximum value */
		float max;
		/*!< Step */
		float step;
		/*!< Numeric type */
	}

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


	// Tuning step definition
	//
	// Lists the tuning steps available for each mode.
	//
	// If a ts field in the list has RIG_TS_ANY value,
	// this means the rig allows its tuning step to be
	// set to any value ranging from the lowest to the
	// highest (if any) value in the list for that mode.
	// The tuning step must be sorted in the ascending
	// order, and the RIG_TS_ANY value, if present, must
	// be the last one in the list.
	//
	// Note also that the minimum frequency resolution
	// of the rig is determined by the lowest value
	// in the Tuning step list.
	[StructLayout (LayoutKind.Sequential)]
	public struct TuningStep
	{
		public RigMode Modes { get { return modes; } }

		public int Step { get { return ts.Val; } }

		public const int Any = 0;

		// Bit field of RIG_MODE's
		RigMode modes;
		// Tuning step in Hz
		Long ts;
	};


	// Filter definition
	//
	// Lists the filters available for each mode.
	//
	// If more than one filter is available for a given mode,
	// the first entry in the array will be the default
	// filter to use for the normal passband of this mode.
	// The first entry in the array below the default normal passband
	// is the default narrow passband and the first entry in the array
	// above the default normal passband is the default wide passband.
	// Note: if there's no lower width or upper width, then narrow or
	// respectively wide passband is equal to the default normal passband.
	//
	// If a width field in the list has RIG_FLT_ANY value,
	// this means the rig allows its passband width to be
	// set to any value ranging from the lowest to the
	// highest value (if any) in the list for that mode.
	// The RIG_FLT_ANY value, if present, must
	// be the last one in the list.
	//
	// The width field is the narrowest passband in a transmit/receive chain
	// with regard to different IF.
	[StructLayout (LayoutKind.Sequential)]
	public struct FilterList
	{
		public RigMode Modes { get { return modes; } }

		public int Width { get { return width.Val; } }

		public const int Any = 0;

		// Bit field of RIG_MODE's
		RigMode modes;
		// Passband width in Hz
		Long width;
	};

	[StructLayout (LayoutKind.Sequential)]
	public  struct ChannelList
	{
		public int Start { get { return start; } }

		public int End { get { return end; } }

		public RigMemoryChannel Type { get { return type; } }

		public ChannelCapability MemCaps { get { return mem_caps; } }

		// Starting memory channel \b number
		int start;
		// Ending memory channel \b number
		int end;
		// Memory type. see chan_type_t
		RigMemoryChannel type;
		// Definition of attributes that can be stored/retrieved
		ChannelCapability mem_caps;
	};

	// Channel capability definition
	//
	// Definition of the attributes that can be stored/retrieved in/from memory
	[StructLayout (LayoutKind.Sequential)]
	public struct ChannelCapability
	{
		private short raw0;

		// Bank number
		public bool BankNumber { get { return ((byte)((raw0 >> 0) & 0x01)) != 0; } }
		// VFO
		public bool Vfo{ get { return ((byte)((raw0 >> 1) & 0x01)) != 0; } }
		// Selected antenna
		public bool  Antenna{ get { return ((byte)((raw0 >> 2) & 0x01)) != 0; } }
		// Receive frequency
		public bool  RXFrequency{ get { return ((byte)((raw0 >> 3) & 0x01)) != 0; } }
		// Receive mode
		public bool  RXMode{ get { return ((byte)((raw0 >> 4) & 0x01)) != 0; } }
		// Receive passband width associated with mode
		public bool  RXWidth{ get { return ((byte)((raw0 >> 5) & 0x01)) != 0; } }
		// Transmit frequency
		public bool  TXFrequency{ get { return ((byte)((raw0 >> 6) & 0x01)) != 0; } }
		// Transmit mode
		public bool  TXMode{ get { return ((byte)((raw0 >> 7) & 0x01)) != 0; } }
		// Transmit passband width associated with mode

		public bool  TXWidth{ get { return ((byte)((raw0 >> 8) & 0x01)) != 0; } }
		// Split mode
		public bool  Split{ get { return ((byte)((raw0 >> 9) & 0x01)) != 0; } }
		// Split transmit VFO
		public bool  TXVfo{ get { return ((byte)((raw0 >> 10) & 0x01)) != 0; } }
		// Repeater shift
		public bool  RepeaterShift{ get { return ((byte)((raw0 >> 11) & 0x01)) != 0; } }
		// Repeater offset
		public bool  RepeaterOffset{ get { return ((byte)((raw0 >> 12) & 0x01)) != 0; } }
		// Tuning step
		public bool  TuningStep{ get { return ((byte)((raw0 >> 13) & 0x01)) != 0; } }
		// RIT
		public bool  Rit{ get { return ((byte)((raw0 >> 14) & 0x01)) != 0; } }
		// XIT
		public bool  Xit{ get { return ((byte)((raw0 >> 15) & 0x01)) != 0; } }

		// Function status
		ULong funcs;
		// Level values
		ULong levels;

		public uint Functions { get { return funcs.Val; } }

		public uint Levels { get { return levels.Val; } }


		private short raw2;

		// CTCSS tone
		public bool  CtcssTone { get { return ((byte)((raw2 >> 0) & 0x01)) != 0; } }
		// CTCSS squelch tone
		public bool  CtcssSquelch{ get { return ((byte)((raw2 >> 1) & 0x01)) != 0; } }
		// DCS code
		public bool  DcsCode{ get { return ((byte)((raw2 >> 2) & 0x01)) != 0; } }
		// DCS squelch code
		public bool  DcsSquelch{ get { return ((byte)((raw2 >> 3) & 0x01)) != 0; } }
		// Scan group
		public bool  ScanGroup{ get { return ((byte)((raw2 >> 4) & 0x01)) != 0; } }
		// Channel flags
		public bool  ChannelFlags{ get { return ((byte)((raw2 >> 5) & 0x01)) != 0; } }
		// Name
		public bool  ChannelName{ get { return ((byte)((raw2 >> 6) & 0x01)) != 0; } }
		// Extension level value list
		public bool  ExtensionLevels{ get { return ((byte)((raw2 >> 7) & 0x01)) != 0; } }
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

		int size;
		// number of plots in the table
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.MAX_CAL_LENGTH)]
		// table of plots
	TableDef[] table = new CalibrationTable.TableDef[Rig.MAX_CAL_LENGTH];
	};

	[StructLayout (LayoutKind.Sequential)]
	public struct Long
	{
		IntPtr val;

		public int Val {
			get {
				return val.ToInt32 ();
			}
			set {
				val = new IntPtr (value);
			}
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct ULong
	{
		IntPtr val;

		public uint Val {
			get {
				return (uint)val.ToInt32 ();
			}
			set {
				val = new IntPtr (value);
			}
		}
	}
}
