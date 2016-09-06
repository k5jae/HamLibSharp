//
//  RigStateNative.cs
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
	// TODO: Primary interest is to get the vfo_list and mode_list. Everything else untested.

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct RigStateNative
	{
		//		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
		//		HamLibCommPortNative port;	/// Rig port (internal use).
	
		/// Rig port (internal use).
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 3)]
		internal HamLibPortNative[] ptt_dcd_ports;

		/// VFO compensation in PPM, 0.0 to disable
		internal double vfo_comp;

		/// ITU region to select among freq_range_t
		internal int itu_region;

		/// Receive frequency range list
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] rx_range_list;

		/// Transmit frequency range list
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] tx_range_list;

		// Tuning step list
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.TSLSTSIZ)]
		internal TuningStep[] tuning_steps;

		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FLTLSTSIZ)]
		internal FilterList[] filters;

		// S-meter calibration table
		internal CalibrationTable str_cal;

		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.CHANLSTSIZ)]
		internal ChannelList[] chan_list;

		/// max absolute
		internal Long max_rit;
		/// max absolute XIT
		internal Long max_xit;
		/// max absolute IF-SHIFT
		internal Long max_ifshift;

		/// Announces bit field list
		internal RigAnnounce announces;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] preamp;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] attenuator;

		// List of get functions
		internal ULong has_get_func;
		// List of set functions
		internal ULong has_set_func;
		// List of get level
		internal ULong has_get_level;
		// List of set level
		internal ULong has_set_level;
		// List of get parm
		internal ULong has_get_parm;
		// List of set parm
		internal ULong has_set_parm;

		// level granularity (i.e. steps)
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.RIG_SETTING_MAX)]
		internal Granularity[] level_gran;

		// parm granularity (i.e. steps)
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.RIG_SETTING_MAX)]
		internal Granularity[] parm_gran;

		// non overridable fields, internal use

		/// set to 1 to hold the event decoder (async) otherwise 0
		internal int hold_decode;
		/// VFO currently set
		internal int current_vfo;
		/// Complete list of VFO for this rig
		internal int vfo_list;
		/// Comm port state, opened/closed.
		internal int comm_state;
		/// Pointer to private rig state data.
		IntPtr priv;
		/// Internal use by hamlib++ for event handling.
		IntPtr obj;
		/// Whether the transceive mode is on
		internal int transceive;
		/// Event notification polling period in milliseconds
		internal int poll_interval;
		/// Frequency currently set
		internal double current_freq;
		/// Mode currently set
		RigMode current_mode;
		/// Passband width currently set
		internal Long current_width;
		/// Tx VFO currently set
		internal int tx_vfo;
		/// Complete list of modes for this rig
		internal RigMode mode_list;
	}
}
