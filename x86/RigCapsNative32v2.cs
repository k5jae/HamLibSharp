//
//  RigCapsNative32v2.cs
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
	/// <summary>
	/// This class holds the caps values and uses the C type "long" as 32-bit.
	/// This is used for 32-bit architectures and all Windows architectures (32 and 64 bit)
	/// </summary>
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal class RigCapsNative32v2 : IRigCapsNative
	{
		// max mode/filter list size, zero ended
		// NOTE: This was changed from 42 to 60 in version 3.0.1
		internal const int FLTLSTSIZ = 42;

		// Rig model
		private int rig_model;
		//rig_model_t

		// Model name.
		[MarshalAs (UnmanagedType.LPStr)]
		private string model_name;

		// Manufacturer.
		[MarshalAs (UnmanagedType.LPStr)]
		private string mfg_name;

		// Driver version.
		[MarshalAs (UnmanagedType.LPStr)]
		private string version;

		// Copyright info.
		[MarshalAs (UnmanagedType.LPStr)]
		private string copyright;

		// Driver status.
		private RigBackendStatus status;

		// Rig type.
		private RigType rig_type;
		// Type of the PTT port.
		private PttType ptt_type;
		// Type of the DCD port.
		private RigDcd dcd_type;
		// Type of communication port.
		private RigPort port_type;

		// Minimum serial speed.
		private int serial_rate_min;
		// Maximum serial speed.
		private int serial_rate_max;
		// Number of data bits.
		private int serial_data_bits;
		// Number of stop bits.
		private int serial_stop_bits;

		// Parity.
		private RigSerialParity serial_parity;
		// Handshake.
		private RigSerialHandshake serial_handshake;

		// Delay between each byte sent out, in mS
		private int write_delay;
		// Delay between each commands send out, in mS
		private int post_write_delay;
		// Timeout, in mS
		private int timeout;

		// Maximum number of retries if command fails, 0 to disable
		private int retry;

		// List of get functions
		private uint has_get_func;
		// List of set functions
		private uint has_set_func;
		// List of get level
		private uint has_get_level;
		// List of set level
		private uint has_set_level;
		// List of get parm
		private uint has_get_parm;
		// List of set parm
		private uint has_set_parm;

		// level granularity (i.e. steps)
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.RIG_SETTING_MAX)]
		private Granularity[] level_gran;

		// parm granularity (i.e. steps)
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.RIG_SETTING_MAX)]
		private Granularity[] parm_gran;

		// Extension parm list, \sa ext.c

		//[MarshalAs (UnmanagedType.Struct)]
		private IntPtr extparms;
		// Extension level list, \sa ext.c
		//[MarshalAs (UnmanagedType.Struct)]
		private IntPtr extlevels;

		// CTCSS tones list, zero ended
		private IntPtr ctcss_list;
		// DCS code list, zero ended
		private IntPtr dcs_list;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		private int[] preamp;

		// Preamp list in dB, 0 terminated
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = Rig.MAXDBLSTSIZ)]
		private int[] attenuator;

		// max absolute RIT
		private int max_rit;
		// max absolute XIT
		private int max_xit;
		// max absolute IF-SHIFT
		private int max_ifshift;

		// Announces bit field list
		private RigAnnounce announces;

		// VFO op bit field list
		private RigVfoOperation vfo_ops;
		// Scan bit field list
		private RigScanOperation scan_ops;
		// Bit field list of direct VFO access commands
		private int targetable_vfo;
		// Supported transceive mode
		private RigTransceive transceive;

		// Number of banks
		private int bank_qty;
		// Max length of memory channel name
		private int chan_desc_sz;

		// Channel list, zero ended
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.CHANLSTSIZ)]
		private ChannelList32[] chan_list;

		// Receive frequency range list for ITU region 1
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		private FrequencyRange[] rx_range_list1;

		// Transmit frequency range list for ITU region 1
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		private FrequencyRange[] tx_range_list1;

		// Receive frequency range list for ITU region 2
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		private FrequencyRange[] rx_range_list2;

		// Transmit frequency range list for ITU region 2
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.FRQRANGESIZ)]
		private FrequencyRange[] tx_range_list2;

		// Tuning step list
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Rig.TSLSTSIZ)]
		private ModeValue32[] tuning_steps;

		// mode/filter table, at -6dB
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = FLTLSTSIZ)]
		private ModeValue32[] filters_list;

		// S-meter calibration table
		private CalibrationTable str_cal;

		// Configuration parametres.
		private IntPtr cfgparams;
		// Private data.
		private IntPtr priv;

		// function pointers to API functions, if IntPtr.Zero, the function is not available
		private IntPtr rig_init;
		private IntPtr rig_cleanup;
		private IntPtr rig_open;
		private IntPtr rig_close;

		private IntPtr set_freq;
		private IntPtr get_freq;

		private IntPtr set_mode;
		private IntPtr get_mode;

		private IntPtr set_vfo;
		private IntPtr get_vfo;

		private IntPtr set_ptt;
		private IntPtr get_ptt;
		private IntPtr get_dcd;

		private IntPtr set_rptr_shift;
		private IntPtr get_rptr_shift;

		private IntPtr set_rptr_offs;
		private IntPtr get_rptr_offs;

		private IntPtr set_split_freq;
		private IntPtr get_split_freq;
		private IntPtr set_split_mode;
		private IntPtr get_split_mode;

		// This was added in 3.1
		//		private IntPtr set_split_freq_mode;
		//		private IntPtr get_split_freq_mode;

		private IntPtr set_split_vfo;
		private IntPtr get_split_vfo;

		private IntPtr set_rit;
		private IntPtr get_rit;
		private IntPtr set_xit;
		private IntPtr get_xit;

		private IntPtr set_ts;
		private IntPtr get_ts;

		private IntPtr set_dcs_code;
		private IntPtr get_dcs_code;
		private IntPtr set_tone;
		private IntPtr get_tone;
		private IntPtr set_ctcss_tone;
		private IntPtr get_ctcss_tone;

		private IntPtr set_dcs_sql;
		private IntPtr get_dcs_sql;
		private IntPtr set_tone_sql;
		private IntPtr get_tone_sql;
		private IntPtr set_ctcss_sql;
		private IntPtr get_ctcss_sql;

		private IntPtr power2mW;
		private IntPtr mW2power;

		private IntPtr set_powerstat;
		private IntPtr get_powerstat;
		private IntPtr reset;

		private IntPtr set_ant;
		private IntPtr get_ant;

		private IntPtr set_level;
		private IntPtr get_level;

		private IntPtr set_func;
		private IntPtr get_func;

		private IntPtr set_parm;
		private IntPtr get_parm;

		private IntPtr set_ext_level;
		private IntPtr get_ext_level;

		private IntPtr set_ext_parm;
		private IntPtr get_ext_parm;

		private IntPtr set_conf;
		private IntPtr get_conf;

		private IntPtr send_dtmf;
		private IntPtr recv_dtmf;
		private IntPtr send_morse;

		private IntPtr set_bank;
		private IntPtr set_mem;
		private IntPtr get_mem;
		private IntPtr vfo_op;
		private IntPtr scan;

		private IntPtr set_trn;
		private IntPtr get_trn;

		private IntPtr decode_event;

		private IntPtr set_channel;
		private IntPtr get_channel;

		private IntPtr get_info;

		private IntPtr set_chan_all_cb;
		private IntPtr get_chan_all_cb;

		private IntPtr set_mem_all_cb;
		private IntPtr get_mem_all_cb;

		//[MarshalAs (UnmanagedType.LPStr)]
		private IntPtr clone_combo_set;

		//[MarshalAs (UnmanagedType.LPStr)]
		private IntPtr clone_combo_get;

		private int test1;
		private int test2;
		private int test3;
		private int test4;
		private int test5;
		private int test6;
		private int test7;
		private int test8;
		private int test9;
		private int test10;

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

		public uint Has_get_func { get { return has_get_func; } }

		public uint Has_set_func { get { return has_set_func; } }

		public uint Has_get_level { get { return has_get_level; } }

		public uint Has_set_level { get { return has_set_level; } }

		public uint Has_get_parm { get { return has_get_parm; } }

		public uint Has_set_parm { get { return has_set_parm; } }

		public Granularity[] Level_gran { get { return level_gran; } }

		public Granularity[] Parm_gran { get { return parm_gran; } }

		public IntPtr Extparms { get { return extparms; } }

		public IntPtr Extlevels { get { return extlevels; } }

		public IntPtr Ctcss_list { get { return ctcss_list; } }

		public IntPtr Dcs_list { get { return dcs_list; } }

		public int[] Preamp { get { return preamp; } }

		public int[] Attenuator { get { return attenuator; } }

		public int Max_rit { get { return max_rit; } }

		public int Max_xit { get { return max_xit; } }

		public int Max_ifshift { get { return max_ifshift; } }

		public RigAnnounce Announces { get { return announces; } }

		public RigVfoOperation Vfo_ops { get { return vfo_ops; } }

		public RigScanOperation Scan_ops { get { return scan_ops; } }

		public int Targetable_vfo { get { return Targetable_vfo1; } }

		public int Targetable_vfo1 { get { return targetable_vfo; } }

		public RigTransceive Transceive { get { return transceive; } }

		public int Bank_qty { get { return bank_qty; } }

		public int Chan_desc_sz { get { return chan_desc_sz; } }

		public IList<IChannelList> Chan_list { get { return chan_list.CastList<ChannelList32, IChannelList> (); } }

		public FrequencyRange[] Rx_range_list1 { get { return rx_range_list1; } }

		public FrequencyRange[] Tx_range_list1 { get { return tx_range_list1; } }

		public FrequencyRange[] Rx_range_list2 { get { return rx_range_list2; } }

		public FrequencyRange[] Tx_range_list2 { get { return tx_range_list2; } }

		public IList<IModeValue> Tuning_steps { get { return tuning_steps.CastList<ModeValue32, IModeValue> (); } }

		public IList<IModeValue> Filters_list { get { return filters_list.CastList<ModeValue32, IModeValue> (); } }

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

		public IntPtr Set_split_freq_mode { get { return IntPtr.Zero; } }
		//set_split_freq_mode; } }
		public IntPtr Get_split_freq_mode { get { return IntPtr.Zero; } }
		//get_split_freq_mode; } }
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
