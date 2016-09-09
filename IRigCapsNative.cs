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
using System.Collections.Generic;

namespace HamLibSharp
{
	/// <summary>
	/// Rig caps native interface.
	/// There is an ABI issue with HamLib where shortfreq_t and pbwidth_t are defined as "long"
	/// and setting_t is defined as "unsigned long".
	/// On Linux and Posix:
	/// 	long is defined as 32-bits on 32-bit systems
	/// 	long is defined as 64-bits on 64-bit systems
	/// On Windows:
	/// 	long is always defined as 32-bits for both 32 and 64-bit systems.
	/// 
	/// This interface is used to abstract the native struct from the implementation so
	/// each architecture specific struct can be used to Marshal the data using PtrToStructure
	/// and all variables line up with native memory.
	/// </summary>
	internal interface IRigCapsNative
	{
		int Rig_model { get; }

		string Model_name { get; }

		string Mfg_name { get; }

		string Version { get; }

		string Copyright { get; }

		RigBackendStatus Status { get; }

		RigType Rig_type { get; }

		PttType Ptt_type { get; }

		RigDcd Dcd_type { get; }

		RigPort Port_type { get; }

		int Serial_rate_min { get; }

		int Serial_rate_max { get; }

		int Serial_data_bits { get; }

		int Serial_stop_bits { get; }

		RigSerialParity Serial_parity { get; }

		RigSerialHandshake Serial_handshake { get; }

		int Write_delay { get; }

		int Post_write_delay { get; }

		int Timeout { get; }

		int Retry { get; }

		uint Has_get_func { get; }

		uint Has_set_func { get; }

		uint Has_get_level { get; }

		uint Has_set_level { get; }

		uint Has_get_parm { get; }

		uint Has_set_parm { get; }

		Granularity[] Level_gran { get; }

		Granularity[] Parm_gran { get; }

		IntPtr Extparms { get; }

		IntPtr Extlevels { get; }

		IntPtr Ctcss_list { get; }

		IntPtr Dcs_list { get; }

		int[] Preamp { get; }

		int[] Attenuator { get; }

		int Max_rit { get; }

		int Max_xit { get; }

		int Max_ifshift { get; }

		RigAnnounce Announces { get; }

		RigVfoOperation Vfo_ops { get; }

		RigScanOperation Scan_ops { get; }

		int Targetable_vfo { get; }

		RigTransceive Transceive { get; }

		int Bank_qty { get; }

		int Chan_desc_sz { get; }

		IList<IChannelList> Chan_list { get; }

		FrequencyRange[] Rx_range_list1 { get; }

		FrequencyRange[] Tx_range_list1 { get; }

		FrequencyRange[] Rx_range_list2 { get; }

		FrequencyRange[] Tx_range_list2 { get; }

		IList<IModeValue> Tuning_steps { get; }

		IList<IModeValue> Filters_list { get; }

		CalibrationTable Str_cal { get; }

		IntPtr Cfgparams { get; }

		IntPtr Priv { get; }

		IntPtr Rig_init { get; }

		IntPtr Rig_cleanup { get; }

		IntPtr Rig_open { get; }

		IntPtr Rig_close { get; }

		IntPtr Set_freq { get; }

		IntPtr Get_freq { get; }

		IntPtr Set_mode { get; }

		IntPtr Get_mode { get; }

		IntPtr Set_vfo { get; }

		IntPtr Get_vfo { get; }

		IntPtr Set_ptt { get; }

		IntPtr Get_ptt { get; }

		IntPtr Get_dcd { get; }

		IntPtr Set_rptr_shift { get; }

		IntPtr Get_rptr_shift { get; }

		IntPtr Set_rptr_offs { get; }

		IntPtr Get_rptr_offs { get; }

		IntPtr Set_split_freq { get; }

		IntPtr Get_split_freq { get; }

		IntPtr Set_split_mode { get; }

		IntPtr Get_split_mode { get; }

		IntPtr Set_split_freq_mode { get; }

		IntPtr Get_split_freq_mode { get; }

		IntPtr Set_split_vfo { get; }

		IntPtr Get_split_vfo { get; }

		IntPtr Set_rit { get; }

		IntPtr Get_rit { get; }

		IntPtr Set_xit { get; }

		IntPtr Get_xit { get; }

		IntPtr Set_ts { get; }

		IntPtr Get_ts { get; }

		IntPtr Set_dcs_code { get; }

		IntPtr Get_dcs_code { get; }

		IntPtr Set_tone { get; }

		IntPtr Get_tone { get; }

		IntPtr Set_ctcss_tone { get; }

		IntPtr Get_ctcss_tone { get; }

		IntPtr Set_dcs_sql { get; }

		IntPtr Get_dcs_sql { get; }

		IntPtr Set_tone_sql { get; }

		IntPtr Get_tone_sql { get; }

		IntPtr Set_ctcss_sql { get; }

		IntPtr Get_ctcss_sql { get; }

		IntPtr Power2mW { get; }

		IntPtr MW2power { get; }

		IntPtr Set_powerstat { get; }

		IntPtr Get_powerstat { get; }

		IntPtr Reset { get; }

		IntPtr Set_ant { get; }

		IntPtr Get_ant { get; }

		IntPtr Set_level { get; }

		IntPtr Get_level { get; }

		IntPtr Set_func { get; }

		IntPtr Get_func { get; }

		IntPtr Set_parm { get; }

		IntPtr Get_parm { get; }

		IntPtr Set_ext_level { get; }

		IntPtr Get_ext_level { get; }

		IntPtr Set_ext_parm { get; }

		IntPtr Get_ext_parm { get; }

		IntPtr Set_conf { get; }

		IntPtr Get_conf { get; }

		IntPtr Send_dtmf { get; }

		IntPtr Recv_dtmf { get; }

		IntPtr Send_morse { get; }

		IntPtr Set_bank { get; }

		IntPtr Set_mem { get; }

		IntPtr Get_mem { get; }

		IntPtr Vfo_op { get; }

		IntPtr Scan { get; }

		IntPtr Set_trn { get; }

		IntPtr Get_trn { get; }

		IntPtr Decode_event { get; }

		IntPtr Set_channel { get; }

		IntPtr Get_channel { get; }

		IntPtr Get_info { get; }

		IntPtr Set_chan_all_cb { get; }

		IntPtr Get_chan_all_cb { get; }

		IntPtr Set_mem_all_cb { get; }

		IntPtr Get_mem_all_cb { get; }

		IntPtr Clone_combo_set { get; }

		IntPtr Clone_combo_get { get; }
	}
}
