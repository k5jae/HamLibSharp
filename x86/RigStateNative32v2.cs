//
//  RigStateNative32v2.cs
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

namespace HamLibSharp.x86
{
	// TODO: Primary interest is to get the vfo_list and mode_list. Everything else untested.
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct RigStateNative32v2 : IRigStateNative
	{
		// max mode/filter list size, zero ended
		// NOTE: This was changed from 42 to 60 in version 3.0.1
		internal const int FLTLSTSIZ = 42;

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
		internal ModeValue32[] tuning_steps;

		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = FLTLSTSIZ)]
		internal ModeValue32[] filters;

		// S-meter calibration table
		internal CalibrationTable str_cal;

		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.CHANLSTSIZ)]
		internal ChannelList32[] chan_list;

		/// max absolute
		internal int max_rit;
		/// max absolute XIT
		internal int max_xit;
		/// max absolute IF-SHIFT
		internal int max_ifshift;

		/// Announces bit field list
		internal RigAnnounce announces;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] preamp;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] attenuator;

		// List of get functions
		internal uint has_get_func;
		// List of set functions
		internal uint has_set_func;
		// List of get level
		internal uint has_get_level;
		// List of set level
		internal uint has_set_level;
		// List of get parm
		internal uint has_get_parm;
		// List of set parm
		internal uint has_set_parm;

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
		internal int current_width;
		/// Tx VFO currently set
		internal int tx_vfo;
		/// Complete list of modes for this rig
		internal RigMode mode_list;

		// interface properties

		public HamLibPortNative[] Ptt_dcd_ports { get { return ptt_dcd_ports; } }
		public double Vfo_comp { get { return vfo_comp; } }
		public int Itu_region { get { return itu_region; } }
		public FrequencyRange[] Rx_range_list { get { return rx_range_list; } }
		public FrequencyRange[] Tx_range_list { get { return tx_range_list; } }
		public IList<IModeValue> Tuning_steps { get { return tuning_steps.CastList<ModeValue32, IModeValue>(); } }
		public IList<IModeValue> Filters { get { return filters.CastList<ModeValue32, IModeValue>(); } }
		public CalibrationTable Str_cal { get { return str_cal; } }
		public IList<IChannelList> Chan_list { get { return chan_list.CastList<ChannelList32, IChannelList>(); } }
		public int Max_rit { get { return max_rit; } }
		public int Max_xit { get { return max_xit; } }
		public int Max_ifshift { get { return max_ifshift; } }
		public RigAnnounce Announces { get { return announces; } }
		public int[] Preamp { get { return preamp; } }
		public int[] Attenuator { get { return attenuator; } }
		public uint Has_get_func { get { return has_get_func; } }
		public uint Has_set_func { get { return Has_set_func; } }
		public uint Has_get_level { get { return has_get_level; } }
		public uint Has_set_level { get { return has_set_level; } }
		public uint Has_get_parm { get { return has_get_parm; } }
		public uint Has_set_parm { get { return has_set_parm; } }
		public Granularity[] Level_gran { get { return level_gran; } }
		public Granularity[] Parm_gran { get { return parm_gran; } }
		public int Hold_decode { get { return hold_decode; } }
		public int Current_vfo { get { return current_vfo; } }
		public int Vfo_list { get { return vfo_list; } }
		public int Comm_state { get { return comm_state; } }
		public IntPtr Priv { get { return priv; } }
		public IntPtr Obj { get { return obj; } }
		public int Transceive { get { return transceive; } }
		public int Poll_interval { get { return poll_interval; } }
		public double Current_freq { get { return current_freq; } }
		public RigMode Current_mode { get { return current_mode; } }
		public int Current_width { get { return current_width; } }
		public int Tx_vfo { get { return tx_vfo; } }
		public RigMode Mode_list { get { return mode_list; } }
	}
}
