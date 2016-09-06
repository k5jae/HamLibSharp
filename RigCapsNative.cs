//
//  RigCapsNative.cs
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
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class RigCapsNative
	{
		// Rig model
		internal int rig_model;
		//rig_model_t

		// Model name.
		[MarshalAs (UnmanagedType.LPStr)]
		internal string model_name;

		// Manufacturer.
		[MarshalAs (UnmanagedType.LPStr)]
		internal string mfg_name;

		// Driver version.
		[MarshalAs (UnmanagedType.LPStr)]
		internal string version;

		// Copyright info.
		[MarshalAs (UnmanagedType.LPStr)]
		internal string copyright;

		// Driver status.
		internal RigBackendStatus status;

		// Rig type.
		internal RigType rig_type;
		// Type of the PTT port.
		internal PttType ptt_type;
		// Type of the DCD port.
		internal RigDcd dcd_type;
		// Type of communication port.
		internal RigPort port_type;

		// Minimum serial speed.
		internal int serial_rate_min;
		// Maximum serial speed.
		internal int serial_rate_max;
		// Number of data bits.
		internal int serial_data_bits;
		// Number of stop bits.
		internal int serial_stop_bits;

		// Parity.
		internal RigSerialParity serial_parity;
		// Handshake.
		internal RigSerialHandshake serial_handshake;

		// Delay between each byte sent out, in mS
		internal int write_delay;
		// Delay between each commands send out, in mS
		internal int post_write_delay;
		// Timeout, in mS
		internal int timeout;

		// Maximum number of retries if command fails, 0 to disable
		internal int retry;

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

		// Extension parm list, \sa ext.c

		//[MarshalAs (UnmanagedType.Struct)]
		internal IntPtr extparms;
		// Extension level list, \sa ext.c
		//[MarshalAs (UnmanagedType.Struct)]
		internal IntPtr extlevels;

		// CTCSS tones list, zero ended
		internal IntPtr ctcss_list;
		// DCS code list, zero ended
		internal IntPtr dcs_list;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] preamp;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		internal int[] attenuator;

		// max absolute RIT
		internal Long max_rit;
		// max absolute XIT
		internal Long max_xit;
		// max absolute IF-SHIFT
		internal Long max_ifshift;

		// Announces bit field list
		internal RigAnnounce announces;

		// VFO op bit field list
		internal RigVfoOperation vfo_ops;
		// Scan bit field list
		internal RigScanOperation scan_ops;
		// Bit field list of direct VFO access commands
		internal int targetable_vfo;
		// Supported transceive mode
		internal RigTransceive transceive;

		// Number of banks
		internal int bank_qty;
		// Max length of memory channel name
		internal int chan_desc_sz;

		// Channel list, zero ended
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.CHANLSTSIZ)]
		internal ChannelList[] chan_list;

		// Receive frequency range list for ITU region 1
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] rx_range_list1;

		// Transmit frequency range list for ITU region 1
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] tx_range_list1;

		// Receive frequency range list for ITU region 2
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] rx_range_list2;

		// Transmit frequency range list for ITU region 2
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		internal FrequencyRange[] tx_range_list2;

		// Tuning step list
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.TSLSTSIZ)]
		internal TuningStep[] tuning_steps;

		// mode/filter table, at -6dB
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FLTLSTSIZ)]
		internal FilterList[] filters_list;

		// S-meter calibration table
		internal CalibrationTable str_cal;

		// Configuration parametres.
		internal IntPtr cfgparams;
		// Private data.
		internal IntPtr priv;

		// function pointers to API functions, if IntPtr.Zero, the function is not available
		internal IntPtr rig_init;
		internal IntPtr rig_cleanup;
		internal IntPtr rig_open;
		internal IntPtr rig_close;

		internal IntPtr set_freq;
		internal IntPtr get_freq;

		internal IntPtr set_mode;
		internal IntPtr get_mode;

		internal IntPtr set_vfo;
		internal IntPtr get_vfo;

		internal IntPtr set_ptt;
		internal IntPtr get_ptt;
		internal IntPtr get_dcd;

		internal IntPtr set_rptr_shift;
		internal IntPtr get_rptr_shift;

		internal IntPtr set_rptr_offs;
		internal IntPtr get_rptr_offs;

		internal IntPtr set_split_freq;
		internal IntPtr get_split_freq;
		internal IntPtr set_split_mode;
		internal IntPtr get_split_mode;
		internal IntPtr set_split_freq_mode;
		internal IntPtr get_split_freq_mode;

		internal IntPtr set_split_vfo;
		internal IntPtr get_split_vfo;

		internal IntPtr set_rit;
		internal IntPtr get_rit;
		internal IntPtr set_xit;
		internal IntPtr get_xit;

		internal IntPtr set_ts;
		internal IntPtr get_ts;

		internal IntPtr set_dcs_code;
		internal IntPtr get_dcs_code;
		internal IntPtr set_tone;
		internal IntPtr get_tone;
		internal IntPtr set_ctcss_tone;
		internal IntPtr get_ctcss_tone;

		internal IntPtr set_dcs_sql;
		internal IntPtr get_dcs_sql;
		internal IntPtr set_tone_sql;
		internal IntPtr get_tone_sql;
		internal IntPtr set_ctcss_sql;
		internal IntPtr get_ctcss_sql;

		internal IntPtr power2mW;
		internal IntPtr mW2power;

		internal IntPtr set_powerstat;
		internal IntPtr get_powerstat;
		internal IntPtr reset;

		internal IntPtr set_ant;
		internal IntPtr get_ant;

		internal IntPtr set_level;
		internal IntPtr get_level;

		internal IntPtr set_func;
		internal IntPtr get_func;

		internal IntPtr set_parm;
		internal IntPtr get_parm;

		internal IntPtr set_ext_level;
		internal IntPtr get_ext_level;

		internal IntPtr set_ext_parm;
		internal IntPtr get_ext_parm;

		internal IntPtr set_conf;
		internal IntPtr get_conf;

		internal IntPtr send_dtmf;
		internal IntPtr recv_dtmf;
		internal IntPtr send_morse;

		internal IntPtr set_bank;
		internal IntPtr set_mem;
		internal IntPtr get_mem;
		internal IntPtr vfo_op;
		internal IntPtr scan;

		internal IntPtr set_trn;
		internal IntPtr get_trn;

		internal IntPtr decode_event;

		internal IntPtr set_channel;
		internal IntPtr get_channel;

		internal IntPtr get_info;

		internal IntPtr set_chan_all_cb;
		internal IntPtr get_chan_all_cb;

		internal IntPtr set_mem_all_cb;
		internal IntPtr get_mem_all_cb;

		[MarshalAs (UnmanagedType.LPStr)]
		internal string clone_combo_set;

		[MarshalAs (UnmanagedType.LPStr)]
		internal string clone_combo_get;
	}
}
