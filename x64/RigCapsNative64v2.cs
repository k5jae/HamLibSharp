//
//  RigCapsNative64v2.cs
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

namespace HamLibSharp.x64
{
	/// <summary>
	/// This class holds the caps values and uses the C type "long" as 64-bit.
	/// This is used for 64-bit architectures (except for Windows 64) 
	/// </summary>
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal class RigCapsNative64v2 : IRigCapsNative
	{
		// max mode/filter list size, zero ended
		// NOTE: This was changed from 42 to 60 in version 3.0.1
		internal const int FLTLSTSIZ = 42;

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
		internal ulong has_get_func;
		// List of set functions
		internal ulong has_set_func;
		// List of get level
		internal ulong has_get_level;
		// List of set level
		internal ulong has_set_level;
		// List of get parm
		internal ulong has_get_parm;
		// List of set parm
		internal ulong has_set_parm;

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
		internal long max_rit;
		// max absolute XIT
		internal long max_xit;
		// max absolute IF-SHIFT
		internal long max_ifshift;

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
		internal ChannelList64[] chan_list;

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
		internal ModeValue64[] tuning_steps;

		// mode/filter table, at -6dB
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = FLTLSTSIZ)]
		internal ModeValue64[] filters_list;

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

		// This was added in 3.1
		//internal IntPtr set_split_freq_mode;
		//internal IntPtr get_split_freq_mode;

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

		//[MarshalAs (UnmanagedType.LPStr)]
		internal IntPtr clone_combo_set;

		//[MarshalAs (UnmanagedType.LPStr)]
		internal IntPtr clone_combo_get;

		// Getter Properties to implement the interface
		public int Rig_model { get { return rig_model; } }
		public string Model_name { get { return model_name; } }
		public string Mfg_name { get { return mfg_name; } }
		public string Version { get { return version; } }
		public string Copyright { get { return copyright; } }
		public RigBackendStatus Status { get { return status; } }
		public RigType Rig_type { get { return rig_type; } }
		public PttType Ptt_type { get { return ptt_type; } }
		public RigDcd Dcd_type { get { return dcd_type; } }
		public RigPort Port_type { get { return port_type; } }
		public int Serial_rate_min { get { return serial_rate_min; } }
		public int Serial_rate_max { get { return serial_rate_max; } }
		public int Serial_data_bits { get { return serial_data_bits; } }
		public int Serial_stop_bits { get { return serial_stop_bits; } }
		public RigSerialParity Serial_parity { get { return serial_parity; } }
		public RigSerialHandshake Serial_handshake { get { return serial_handshake; } }
		public int Write_delay { get { return write_delay; } }
		public int Post_write_delay { get { return post_write_delay; } }
		public int Timeout { get { return timeout; } }
		public int Retry { get { return retry; } }
		public uint Has_get_func { get { return (uint)has_get_func; } }
		public uint Has_set_func { get { return (uint)has_set_func; } }
		public uint Has_get_level { get { return (uint)has_get_level; } }
		public uint Has_set_level { get { return (uint)has_set_level; } }
		public uint Has_get_parm { get { return (uint)has_get_parm; } }
		public uint Has_set_parm { get { return (uint)has_set_parm; } }
		public Granularity[] Level_gran { get { return level_gran; } }
		public Granularity[] Parm_gran { get { return parm_gran; } }
		public IntPtr Extparms { get { return extparms; } }
		public IntPtr Extlevels { get { return extlevels; } }
		public IntPtr Ctcss_list { get { return ctcss_list; } }
		public IntPtr Dcs_list { get { return dcs_list; } }
		public int[] Preamp { get { return preamp; } }
		public int[] Attenuator { get { return attenuator; } }
		public int Max_rit { get { return (int)max_rit; } }
		public int Max_xit { get { return (int)max_xit; } }
		public int Max_ifshift { get { return (int)max_ifshift; } }
		public RigAnnounce Announces { get { return announces; } }
		public RigVfoOperation Vfo_ops { get { return vfo_ops; } }
		public RigScanOperation Scan_ops { get { return scan_ops; } }
		public int Targetable_vfo { get { return Targetable_vfo1; } }
		public int Targetable_vfo1 { get { return targetable_vfo; } }
		public RigTransceive Transceive { get { return transceive; } }
		public int Bank_qty { get { return bank_qty; } }
		public int Chan_desc_sz { get { return chan_desc_sz; } }
		public IList<IChannelList> Chan_list { get { return chan_list.CastList<ChannelList64, IChannelList>(); } }
		public FrequencyRange[] Rx_range_list1 { get { return rx_range_list1; } }
		public FrequencyRange[] Tx_range_list1 { get { return tx_range_list1; } }
		public FrequencyRange[] Rx_range_list2 { get { return rx_range_list2; } }
		public FrequencyRange[] Tx_range_list2 { get { return tx_range_list2; } }
		public IList<IModeValue> Tuning_steps { get { return tuning_steps.CastList<ModeValue64, IModeValue>(); } }
		public IList<IModeValue> Filters_list { get { return filters_list.CastList<ModeValue64, IModeValue>(); } }
		public CalibrationTable Str_cal { get { return str_cal; } }
		public IntPtr Cfgparams { get { return cfgparams; } }
		public IntPtr Priv { get { return priv; } }
		public IntPtr Rig_init { get { return rig_init; } }
		public IntPtr Rig_cleanup { get { return rig_cleanup; } }
		public IntPtr Rig_open { get { return rig_open; } }
		public IntPtr Rig_close { get { return rig_close; } }
		public IntPtr Set_freq { get { return set_freq; } }
		public IntPtr Get_freq { get { return get_freq; } }
		public IntPtr Set_mode { get { return set_mode; } }
		public IntPtr Get_mode { get { return get_mode; } }
		public IntPtr Set_vfo { get { return set_vfo; } }
		public IntPtr Get_vfo { get { return get_vfo; } }
		public IntPtr Set_ptt { get { return set_ptt; } }
		public IntPtr Get_ptt { get { return get_ptt; } }
		public IntPtr Get_dcd { get { return get_dcd; } }
		public IntPtr Set_rptr_shift { get { return set_rptr_shift; } }
		public IntPtr Get_rptr_shift { get { return get_rptr_shift; } }
		public IntPtr Set_rptr_offs { get { return set_rptr_offs; } }
		public IntPtr Get_rptr_offs { get { return get_rptr_offs; } }
		public IntPtr Set_split_freq { get { return set_split_freq; } }
		public IntPtr Get_split_freq { get { return get_split_freq; } }
		public IntPtr Set_split_mode { get { return set_split_mode; } }
		public IntPtr Get_split_mode { get { return get_split_mode; } }
		public IntPtr Set_split_freq_mode { get { return IntPtr.Zero; } } //set_split_freq_mode; } }
		public IntPtr Get_split_freq_mode { get { return IntPtr.Zero; } } //get_split_freq_mode; } }
		public IntPtr Set_split_vfo { get { return set_split_vfo; } }
		public IntPtr Get_split_vfo { get { return get_split_vfo; } }
		public IntPtr Set_rit { get { return set_rit; } }
		public IntPtr Get_rit { get { return get_rit; } }
		public IntPtr Set_xit { get { return set_xit; } } 
		public IntPtr Get_xit { get { return get_xit; } } 
		public IntPtr Set_ts { get { return set_ts; } } 
		public IntPtr Get_ts { get { return get_ts; } } 
		public IntPtr Set_dcs_code { get { return set_dcs_code; } } 
		public IntPtr Get_dcs_code { get { return get_dcs_code; } } 
		public IntPtr Set_tone { get { return set_tone; } } 
		public IntPtr Get_tone { get { return get_tone; } }
		public IntPtr Set_ctcss_tone { get { return set_ctcss_tone; } }
		public IntPtr Get_ctcss_tone { get { return get_ctcss_tone; } }
		public IntPtr Set_dcs_sql { get { return set_dcs_sql; } }
		public IntPtr Get_dcs_sql { get { return get_dcs_sql; } }
		public IntPtr Set_tone_sql { get { return set_tone_sql; } }
		public IntPtr Get_tone_sql { get { return get_tone_sql; } }
		public IntPtr Set_ctcss_sql { get { return set_ctcss_sql; } }
		public IntPtr Get_ctcss_sql { get { return get_ctcss_sql; } }
		public IntPtr Power2mW { get { return power2mW; } }
		public IntPtr MW2power { get { return mW2power; } }
		public IntPtr Set_powerstat { get { return set_powerstat; } }
		public IntPtr Get_powerstat { get { return get_powerstat; } }
		public IntPtr Reset { get { return reset; } }
		public IntPtr Set_ant { get { return set_ant; } }
		public IntPtr Get_ant { get { return get_ant; } }
		public IntPtr Set_level { get { return set_level; } }
		public IntPtr Get_level { get { return get_level; } }
		public IntPtr Set_func { get { return set_func; } }
		public IntPtr Get_func { get { return get_func; } }
		public IntPtr Set_parm { get { return set_parm; } }
		public IntPtr Get_parm { get { return get_parm; } }
		public IntPtr Set_ext_level { get { return set_ext_level; } }
		public IntPtr Get_ext_level { get { return get_ext_level; } }
		public IntPtr Set_ext_parm { get { return set_ext_parm; } }
		public IntPtr Get_ext_parm { get { return get_ext_parm; } }
		public IntPtr Set_conf { get { return set_conf; } }
		public IntPtr Get_conf { get { return get_conf; } }
		public IntPtr Send_dtmf { get { return send_dtmf; } }
		public IntPtr Recv_dtmf { get { return recv_dtmf; } }
		public IntPtr Send_morse { get { return send_morse; } }
		public IntPtr Set_bank { get { return set_bank; } }
		public IntPtr Set_mem { get { return set_mem; } }
		public IntPtr Get_mem { get { return get_mem; } }
		public IntPtr Vfo_op { get { return vfo_op; } }
		public IntPtr Scan { get { return scan; } }
		public IntPtr Set_trn { get { return set_trn; } }
		public IntPtr Get_trn { get { return get_trn; } }
		public IntPtr Decode_event { get { return decode_event; } }
		public IntPtr Set_channel { get { return set_channel; } }
		public IntPtr Get_channel { get { return get_channel; } }
		public IntPtr Get_info { get { return get_info; } }
		public IntPtr Set_chan_all_cb { get { return set_chan_all_cb; } }
		public IntPtr Get_chan_all_cb { get { return get_chan_all_cb; } }
		public IntPtr Set_mem_all_cb { get { return set_mem_all_cb; } }
		public IntPtr Get_mem_all_cb { get { return get_mem_all_cb; } }
		public IntPtr Clone_combo_set { get { return clone_combo_set; } }
		public IntPtr Clone_combo_get { get { return clone_combo_get; } }
	}
}
