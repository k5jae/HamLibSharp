//
//  RigCaps.cs
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
using System.Text;

namespace HamLibSharp
{
	// NOTE: Only Properties and Methods allowed here. RigCapsNative.cs maintains the struct fields to maintain
	// native struct layout.
	public class RigCaps
	{
		readonly IRigCapsNative rigCapsNative;
		readonly int[] ctcss_list;
		readonly int[] dcs_list;
		readonly IList<IConfigurationParameter> extparms;
		readonly IList<IConfigurationParameter> extlevels;
		readonly IList<IChannelList> channelList;
		readonly IList<IModeValue> tuningStep;
		readonly IList<IModeValue> filterList;

		internal RigCaps (IRigCapsNative rigCapsNative, Rig rig = null)
		{
			this.rigCapsNative = rigCapsNative;

			if (rig != null) {
				if (rigCapsNative.Extparms != IntPtr.Zero) {
					extparms = rig.GetExtParm (rigCapsNative.Extparms); //DecodeExtParms (rig, rigCapsNative.Extparms);
				}

				if (rigCapsNative.Extlevels != IntPtr.Zero) {
					extlevels = rig.GetExtLevels (rigCapsNative.Extlevels); //DecodeExtLevels (rig, rigCapsNative.Extlevels);
				}
			}

			if (rigCapsNative.Ctcss_list != IntPtr.Zero)
				ctcss_list = DecodeList (rigCapsNative.Ctcss_list);

			if (rigCapsNative.Dcs_list != IntPtr.Zero)
				dcs_list = DecodeList (rigCapsNative.Dcs_list);

			channelList = CreateChannelList (rigCapsNative.Chan_list);
			tuningStep = CreateModeValueList (rigCapsNative.Tuning_steps);
			filterList = CreateModeValueList (rigCapsNative.Filters_list);
		}

		private int[] DecodeList (IntPtr listPtr)
		{
			List<int> list = new List<int> ();
			int val = -1;

			while (val != 0) {
				val = Marshal.ReadInt32 (listPtr);
				if (val != 0) {
					list.Add (val);
					listPtr = listPtr.Increment<int> ();
				}
			}

			return list.ToArray ();
		}

		private IList<IChannelList> CreateChannelList (IList<IChannelList> lists)
		{
			var newList = new List<IChannelList> ();
			foreach (var chan in lists) {
				if (chan.Type != RigMemoryChannel.None) {
					newList.Add (chan);
				} else {
					break;
				}
			}
			return newList.AsReadOnly ();
		}

		private IList<IModeValue> CreateModeValueList (IList<IModeValue> values)
		{
			var newList = new List<IModeValue> ();

			foreach (var val in values) {
				if (val.Modes != RigMode.None || val.Value != 0) {
					newList.Add (val);
				} else {
					break;
				}
			}
			return newList.AsReadOnly ();
		}

		public int RigModel {
			get {
				return rigCapsNative.Rig_model;
			}
		}

		public string ModelName {
			get {
				return rigCapsNative.Model_name;
			}
		}

		public string MfgName {
			get {
				return rigCapsNative.Mfg_name;
			}
		}

		public string Version {
			get {
				return rigCapsNative.Version;
			}
		}

		public string Copyright {
			get {
				return rigCapsNative.Copyright;
			}
		}

		public RigBackendStatus Status {
			get {
				return rigCapsNative.Status;
			}
		}

		public RigType RigType {
			get {
				return rigCapsNative.Rig_type;
			}
		}

		public PttType PttType {
			get {
				return rigCapsNative.Ptt_type;
			}
		}

		public RigDcd DcdType {
			get {
				return rigCapsNative.Dcd_type;
			}
		}

		public RigPort PortType {
			get {
				return rigCapsNative.Port_type;
			}
		}

		public int SerialRateMin {
			get {
				return rigCapsNative.Serial_rate_min;
			}
		}

		public int SerialRateMax {
			get {
				return rigCapsNative.Serial_rate_max;
			}
		}

		public int SerialDataBits {
			get {
				return rigCapsNative.Serial_data_bits;
			}
		}

		public int SerialStopBits {
			get {
				return rigCapsNative.Serial_stop_bits;
			}
		}

		public RigSerialParity SerialParity {
			get {
				return rigCapsNative.Serial_parity;
			}
		}

		public RigSerialHandshake SerialHandshake {
			get {
				return rigCapsNative.Serial_handshake;
			}
		}

		public int WriteDelay {
			get {
				return rigCapsNative.Write_delay;
			}
		}

		public int PostWriteDelay {
			get {
				return rigCapsNative.Post_write_delay;
			}
		}

		public int Timeout {
			get {
				return rigCapsNative.Timeout;
			}
		}

		public int Retry {
			get {
				return rigCapsNative.Retry;
			}
		}

		public string GetFunctions {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.FuncToString (rigCapsNative.Has_get_func & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetFunctions {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.FuncToString (rigCapsNative.Has_set_func & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string GetLevels {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.LevelToString (rigCapsNative.Has_get_level & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetLevels {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.LevelToString (rigCapsNative.Has_set_level & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string GetParms {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.ParmToString (rigCapsNative.Has_get_parm & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetParms {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.ParmToString (rigCapsNative.Has_set_parm & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public Granularity[] LevelGran {
			get {
				return rigCapsNative.Level_gran;
			}
		}

		public Granularity[] ParmGran {
			get {
				return rigCapsNative.Parm_gran;
			}
		}

		public IList<int> CtcssTones {
			get {
				return Array.AsReadOnly<int> (ctcss_list);
			}
		}

		public IList<int> DcsCodes {
			get {
				return Array.AsReadOnly<int> (dcs_list);
			}
		}

		public IList<IConfigurationParameter> ExtLevels {
			get {
				return extlevels;
			}
		}

		public IList<IConfigurationParameter> ExtParms {
			get {
				return extparms;
			}
		}

		public int[] Preamp {
			get {
				return rigCapsNative.Preamp;
			}
		}

		public int[] Attenuator {
			get {
				return rigCapsNative.Attenuator;
			}
		}

		public int MaxRit {
			get {
				return rigCapsNative.Max_rit;
			}
		}

		public int MaxXit {
			get {
				return rigCapsNative.Max_xit;
			}
		}

		public int MaxIfshift {
			get {
				return rigCapsNative.Max_ifshift;
			}
		}

		public RigAnnounce Announces {
			get {
				return rigCapsNative.Announces;
			}
		}

		public RigVfoOperation VfoOps {
			get {
				return rigCapsNative.Vfo_ops;
			}
		}

		public RigScanOperation ScanOps {
			get {
				return rigCapsNative.Scan_ops;
			}
		}

		public int TargetableVfo {
			get {
				return rigCapsNative.Targetable_vfo;
			}
		}

		public RigTransceive Transceive {
			get {
				return rigCapsNative.Transceive;
			}
		}

		public int BankQty {
			get {
				return rigCapsNative.Bank_qty;
			}
		}

		public int ChanDescSz {
			get {
				return rigCapsNative.Chan_desc_sz;
			}
		}

		public IList<IChannelList> ChanList {
			get {
				return channelList;
			}
		}

		public FrequencyRange[] RxRangeList1 {
			get {
				return rigCapsNative.Rx_range_list1;
			}
		}

		public FrequencyRange[] TxRangeList1 {
			get {
				return rigCapsNative.Tx_range_list1;
			}
		}

		public FrequencyRange[] RxRangeList2 {
			get {
				return rigCapsNative.Rx_range_list2;
			}
		}

		public FrequencyRange[] TxRangeList2 {
			get {
				return rigCapsNative.Tx_range_list2;
			}
		}

		/// <summary>
		/// Gets the tuning steps available for each mode.
		///
		/// If a ts field in the list has RIG_TS_ANY value,
		/// this means the rig allows its tuning step to be
		/// set to any value ranging from the lowest to the
		/// highest (if any) value in the list for that mode.
		/// The tuning step must be sorted in the ascending
		/// order, and the RIG_TS_ANY value, if present, must
		/// be the last one in the list.
		///
		/// Note also that the minimum frequency resolution
		/// of the rig is determined by the lowest value
		/// in the Tuning step list.
		/// </summary>
		/// <value>The tuning steps.</value>
		public IList<IModeValue> TuningSteps {
			get {
				return tuningStep;
			}
		}

		/// <summary>
		/// Gets the filters available for each mode.
		///
		/// If more than one filter is available for a given mode,
		/// the first entry in the array will be the default
		/// filter to use for the normal passband of this mode.
		/// The first entry in the array below the default normal passband
		/// is the default narrow passband and the first entry in the array
		/// above the default normal passband is the default wide passband.
		/// Note: if there's no lower width or upper width, then narrow or
		/// respectively wide passband is equal to the default normal passband.
		///
		/// If a width field in the list has RIG_FLT_ANY value,
		/// this means the rig allows its passband width to be
		/// set to any value ranging from the lowest to the
		/// highest value (if any) in the list for that mode.
		/// The RIG_FLT_ANY value, if present, must
		/// be the last one in the list.
		///
		/// The width field is the narrowest passband in a transmit/receive chain
		/// with regard to different IF.
		/// </summary>
		/// <value>The filters.</value>
		public IList<IModeValue> Filters {
			get {
				return filterList;
			}
		}

		public bool HasPrivateData { get { return rigCapsNative.Priv != IntPtr.Zero; } }

		public bool HasRigInit { get { return rigCapsNative.Rig_init != IntPtr.Zero; } }

		public bool HasRigCleanup { get { return rigCapsNative.Rig_cleanup != IntPtr.Zero; } }

		public bool HasRigOpen { get { return rigCapsNative.Rig_open != IntPtr.Zero; } }

		public bool HasRigClose { get { return rigCapsNative.Rig_close != IntPtr.Zero; } }

		public bool HasSetFreq { get { return rigCapsNative.Set_freq != IntPtr.Zero; } }

		public bool HasGetFreq { get { return rigCapsNative.Get_freq != IntPtr.Zero; } }

		public bool HasSetMode { get { return rigCapsNative.Set_mode != IntPtr.Zero; } }

		public bool HasGetMode { get { return rigCapsNative.Get_mode != IntPtr.Zero; } }

		public bool HasSetVfo { get { return rigCapsNative.Set_vfo != IntPtr.Zero; } }

		public bool HasGetVfo { get { return rigCapsNative.Get_vfo != IntPtr.Zero; } }

		public bool HasSetPtt{ get { return rigCapsNative.Set_ptt != IntPtr.Zero; } }

		public bool HasGetPtt { get { return rigCapsNative.Get_ptt != IntPtr.Zero; } }

		public bool HasGetDcd { get { return rigCapsNative.Get_dcd != IntPtr.Zero; } }

		public bool HasSetRptrShift { get { return rigCapsNative.Set_rptr_shift != IntPtr.Zero; } }

		public bool HasGetRptrShift { get { return rigCapsNative.Get_rptr_shift != IntPtr.Zero; } }

		public bool HasSetRptrOffs { get { return rigCapsNative.Set_rptr_offs != IntPtr.Zero; } }

		public bool HasGetRptrOffs { get { return rigCapsNative.Get_rptr_offs != IntPtr.Zero; } }

		public bool HasSetSplitFreq { get { return rigCapsNative.Set_split_freq != IntPtr.Zero; } }

		public bool HasGetSplitFreq { get { return rigCapsNative.Get_split_freq != IntPtr.Zero; } }

		public bool HasSetSplitMode { get { return rigCapsNative.Set_split_mode != IntPtr.Zero; } }

		public bool HasGetSplitMode { get { return rigCapsNative.Get_split_mode != IntPtr.Zero; } }

		public bool HasSetSplitFreqMode { get { return rigCapsNative.Set_split_freq_mode != IntPtr.Zero; } }

		public bool HasGetSplitFreqMode { get { return rigCapsNative.Get_split_freq_mode != IntPtr.Zero; } }

		public bool HasSetSplitVfo { get { return rigCapsNative.Set_split_vfo != IntPtr.Zero; } }

		public bool HasGetSplitVfo { get { return rigCapsNative.Get_split_vfo != IntPtr.Zero; } }

		public bool HasSetRit { get { return rigCapsNative.Set_rit != IntPtr.Zero; } }

		public bool HasGetRit { get { return rigCapsNative.Get_rit != IntPtr.Zero; } }

		public bool HasSetXit { get { return rigCapsNative.Set_xit != IntPtr.Zero; } }

		public bool HasGetXit { get { return rigCapsNative.Get_xit != IntPtr.Zero; } }

		public bool HasSetTS { get { return rigCapsNative.Set_ts != IntPtr.Zero; } }

		public bool HasGetTS { get { return rigCapsNative.Get_ts != IntPtr.Zero; } }

		public bool HasSetDcsCode { get { return rigCapsNative.Set_dcs_code != IntPtr.Zero; } }

		public bool HasGetDcsCode { get { return rigCapsNative.Get_dcs_code != IntPtr.Zero; } }

		public bool HasSetTone { get { return rigCapsNative.Set_tone != IntPtr.Zero; } }

		public bool HasGetTone { get { return rigCapsNative.Get_tone != IntPtr.Zero; } }

		public bool HasSetCtcssTone { get { return rigCapsNative.Set_ctcss_tone != IntPtr.Zero; } }

		public bool HasGetCtcssTone { get { return rigCapsNative.Get_ctcss_tone != IntPtr.Zero; } }

		public bool HasSetDcsSquelch { get { return rigCapsNative.Set_dcs_sql != IntPtr.Zero; } }

		public bool HasGetDcsSquelch { get { return rigCapsNative.Get_dcs_sql != IntPtr.Zero; } }

		public bool HasSetToneSquelch { get { return rigCapsNative.Set_tone_sql != IntPtr.Zero; } }

		public bool HasGetToneSquelch { get { return rigCapsNative.Get_tone_sql != IntPtr.Zero; } }

		public bool HasSetCtcssSquelch { get { return rigCapsNative.Set_ctcss_sql != IntPtr.Zero; } }

		public bool HasGetCtcssSquelch { get { return rigCapsNative.Get_ctcss_sql != IntPtr.Zero; } }

		public bool HasPower2mW { get { return rigCapsNative.Power2mW != IntPtr.Zero; } }

		public bool HasmW2power { get { return rigCapsNative.MW2power != IntPtr.Zero; } }

		public bool HasSetPowerstat { get { return rigCapsNative.Set_powerstat != IntPtr.Zero; } }

		public bool HasGetPowerstat { get { return rigCapsNative.Get_powerstat != IntPtr.Zero; } }

		public bool HasReset { get { return rigCapsNative.Reset != IntPtr.Zero; } }

		public bool HasSetAnt { get { return rigCapsNative.Set_ant != IntPtr.Zero; } }

		public bool HasGetAnt { get { return rigCapsNative.Get_ant != IntPtr.Zero; } }

		public bool HasSetLevel { get { return rigCapsNative.Set_level != IntPtr.Zero; } }

		public bool HasGetLevel { get { return rigCapsNative.Get_level != IntPtr.Zero; } }

		public bool HasSetFunc { get { return rigCapsNative.Set_func != IntPtr.Zero; } }

		public bool HasGetFunc { get { return rigCapsNative.Get_func != IntPtr.Zero; } }

		public bool HasSetParm { get { return rigCapsNative.Set_parm != IntPtr.Zero; } }

		public bool HasGetParm { get { return rigCapsNative.Get_parm != IntPtr.Zero; } }

		public bool HasSetExtLevel { get { return rigCapsNative.Set_ext_level != IntPtr.Zero; } }

		public bool HasGetExtLevel { get { return rigCapsNative.Get_ext_level != IntPtr.Zero; } }

		public bool HasSetExtParm { get { return rigCapsNative.Set_ext_parm != IntPtr.Zero; } }

		public bool HasGetExtParm { get { return rigCapsNative.Get_ext_parm != IntPtr.Zero; } }

		public bool HasSetConf { get { return rigCapsNative.Set_conf != IntPtr.Zero; } }

		public bool HasGetConf { get { return rigCapsNative.Get_conf != IntPtr.Zero; } }

		public bool HasSendDTMF { get { return rigCapsNative.Send_dtmf != IntPtr.Zero; } }

		public bool HasRecvDTMF { get { return rigCapsNative.Recv_dtmf != IntPtr.Zero; } }

		public bool HasSendMorse { get { return rigCapsNative.Send_morse != IntPtr.Zero; } }

		public bool HasSetBank { get { return rigCapsNative.Set_bank != IntPtr.Zero; } }

		public bool HasSetMem { get { return rigCapsNative.Set_mem != IntPtr.Zero; } }

		public bool HasGetMem { get { return rigCapsNative.Get_mem != IntPtr.Zero; } }

		public bool HasVfoOp { get { return rigCapsNative.Vfo_op != IntPtr.Zero; } }

		public bool HasScan { get { return rigCapsNative.Scan != IntPtr.Zero; } }

		public bool HasSetTrn { get { return rigCapsNative.Set_trn != IntPtr.Zero; } }

		public bool HasGetTrn { get { return rigCapsNative.Get_trn != IntPtr.Zero; } }

		public bool HasDecodeEvent { get { return rigCapsNative.Decode_event != IntPtr.Zero; } }

		public bool HasSetChannel { get { return rigCapsNative.Set_channel != IntPtr.Zero; } }

		public bool HasGetChannel { get { return rigCapsNative.Get_channel != IntPtr.Zero; } }

		public bool HasGetInfo { get { return rigCapsNative.Get_info != IntPtr.Zero; } }

		public string CloneComboSet { get { return string.Empty; } }
// rigCapsNative.Clone_combo_set; } }

		public string CloneComboGet  { get { return string.Empty; } }
//rigCapsNative.Clone_combo_get; } }

		//		public override string ToString ()
		//		{
		//			return string.Format ("{{\"RigModel\": \"{0}\",\"ModelName\": \"{1}\", \"ManufactureName\" \"{2}\" }}",
		//				rig_model, model_name, mfg_name);
		//		}

		public string ToJson ()
		{
			return string.Format ("{{ \"RigModel\": \"{0}\", \"ModelName\": \"{1}\", \"MfgName\": \"{2}\" }}",
				RigModel, ModelName, MfgName);
		}
	}
}
