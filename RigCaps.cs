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
		readonly RigCapsNative rigCapsNative;
		readonly int[] ctcss_list;
		readonly int[] dcs_list;
		readonly ConfigurationParameter[] extparms;
		readonly ConfigurationParameter[] extlevels;
		readonly ChannelList[] channelList;
		readonly TuningStep[] tuningStep;
		readonly FilterList[] filterList;

		internal RigCaps (RigCapsNative rigCapsNative, IntPtr theRig)
		{
			this.rigCapsNative = rigCapsNative;

			if (theRig != IntPtr.Zero) {
				if (rigCapsNative.extparms != IntPtr.Zero) {
					extparms = DecodeExtParms (theRig, rigCapsNative.extparms);
				}

				if (rigCapsNative.extlevels != IntPtr.Zero) {
					extlevels = DecodeExtLevels (theRig, rigCapsNative.extlevels);
				}
			}

			if (rigCapsNative.ctcss_list != IntPtr.Zero)
				ctcss_list = DecodeList (rigCapsNative.ctcss_list);

			if (rigCapsNative.dcs_list != IntPtr.Zero)
				dcs_list = DecodeList (rigCapsNative.dcs_list);

			channelList = new ChannelList[ChannelListCount (rigCapsNative.chan_list)];
			Array.Copy (rigCapsNative.chan_list, channelList, channelList.Length);

			tuningStep = new TuningStep[TuningStepCount (rigCapsNative.tuning_steps)];
			Array.Copy (rigCapsNative.tuning_steps, tuningStep, tuningStep.Length);

			filterList = new FilterList[FilterListCount (rigCapsNative.filters_list)];
			Array.Copy (rigCapsNative.filters_list, filterList, filterList.Length);

		}

		private ConfigurationParameter[] DecodeExtParms (IntPtr theRig, IntPtr ptr)
		{
			List<ConfigurationParameter> cparams = new List<ConfigurationParameter> ();
			Rig.rig_ext_parm_foreach (theRig, (rig, confPtr, rig_ptr) => {
				var conf = Marshal.PtrToStructure<ConfigurationParameter> (confPtr);
				cparams.Add (conf);
				return 1;
			}, IntPtr.Zero);

			return cparams.ToArray ();
		}

		private ConfigurationParameter[] DecodeExtLevels (IntPtr theRig, IntPtr ptr)
		{
			List<ConfigurationParameter> cparams = new List<ConfigurationParameter> ();
			Rig.rig_ext_level_foreach (theRig, (rig, confPtr, rig_ptr) => {
				var conf = Marshal.PtrToStructure<ConfigurationParameter> (confPtr);
				cparams.Add (conf);
				return 1;
			}, IntPtr.Zero);

			return cparams.ToArray ();
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

		private int ChannelListCount (ChannelList[] lists)
		{
			int count = 0;
			foreach (var chan in lists) {
				if (chan.Type == RigMemoryChannel.None) {
					break;
				}
				count++;
			}
			return count;
		}

		private int TuningStepCount (TuningStep[] steps)
		{
			int count = 0;
			foreach (var step in steps) {
				if (step.Modes == RigMode.None && step.Step == 0) {
					break;
				}
				count++;
			}
			return count;
		}

		private int FilterListCount (FilterList[] list)
		{
			int count = 0;
			foreach (var item in list) {
				if (item.Modes == RigMode.None) {
					break;
				}
				count++;
			}
			return count;
		}


		public int RigModel {
			get {
				return rigCapsNative.rig_model;
			}
		}

		public string ModelName {
			get {
				return rigCapsNative.model_name;
			}
		}

		public string MfgName {
			get {
				return rigCapsNative.mfg_name;
			}
		}

		public string Version {
			get {
				return rigCapsNative.version;
			}
		}

		public string Copyright {
			get {
				return rigCapsNative.copyright;
			}
		}

		public RigBackendStatus Status {
			get {
				return rigCapsNative.status;
			}
		}

		public RigType RigType {
			get {
				return rigCapsNative.rig_type;
			}
		}

		public PttType PttType {
			get {
				return rigCapsNative.ptt_type;
			}
		}

		public RigDcd DcdType {
			get {
				return rigCapsNative.dcd_type;
			}
		}

		public RigPort PortType {
			get {
				return rigCapsNative.port_type;
			}
		}

		public int SerialRateMin {
			get {
				return rigCapsNative.serial_rate_min;
			}
		}

		public int SerialRateMax {
			get {
				return rigCapsNative.serial_rate_max;
			}
		}

		public int SerialDataBits {
			get {
				return rigCapsNative.serial_data_bits;
			}
		}

		public int SerialStopBits {
			get {
				return rigCapsNative.serial_stop_bits;
			}
		}

		public RigSerialParity SerialParity {
			get {
				return rigCapsNative.serial_parity;
			}
		}

		public RigSerialHandshake SerialHandshake {
			get {
				return rigCapsNative.serial_handshake;
			}
		}

		public int WriteDelay {
			get {
				return rigCapsNative.write_delay;
			}
		}

		public int PostWriteDelay {
			get {
				return rigCapsNative.post_write_delay;
			}
		}

		public int Timeout {
			get {
				return rigCapsNative.timeout;
			}
		}

		public int Retry {
			get {
				return rigCapsNative.retry;
			}
		}

		public string GetFunctions {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.FuncToString (rigCapsNative.has_get_func & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetFunctions {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.FuncToString (rigCapsNative.has_set_func & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string GetLevels {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.LevelToString (rigCapsNative.has_get_level & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetLevels {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.LevelToString (rigCapsNative.has_set_level & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string GetParms {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.ParmToString (rigCapsNative.has_get_parm & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public string SetParms {
			get {
				StringBuilder sb = new StringBuilder (255);
				for (int i = 0; i < Rig.RIG_SETTING_MAX; i++) {
					sb.Append (Rig.ParmToString (rigCapsNative.has_set_parm & Rig.rig_idx2setting (i)) + " ");
				}

				return sb.ToString ();
			}
		}

		public Granularity[] LevelGran {
			get {
				return rigCapsNative.level_gran;
			}
		}

		public Granularity[] ParmGran {
			get {
				return rigCapsNative.parm_gran;
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

		public IList<ConfigurationParameter> ExtLevels {
			get {
				return Array.AsReadOnly<ConfigurationParameter> (extlevels);
			}
		}

		public IList<ConfigurationParameter> ExtParms {
			get {
				return Array.AsReadOnly<ConfigurationParameter> (extparms);
			}
		}

		public int[] Preamp {
			get {
				return rigCapsNative.preamp;
			}
		}

		public int[] Attenuator {
			get {
				return rigCapsNative.attenuator;
			}
		}

		public int MaxRit {
			get {
				return rigCapsNative.max_rit;
			}
		}

		public int MaxXit {
			get {
				return rigCapsNative.max_xit;
			}
		}

		public int MaxIfshift {
			get {
				return rigCapsNative.max_ifshift;
			}
		}

		public RigAnnounce Announces {
			get {
				return rigCapsNative.announces;
			}
		}

		public RigVfoOperation VfoOps {
			get {
				return rigCapsNative.vfo_ops;
			}
		}

		public RigScanOperation ScanOps {
			get {
				return rigCapsNative.scan_ops;
			}
		}

		public int TargetableVfo {
			get {
				return rigCapsNative.targetable_vfo;
			}
		}

		public RigTransceive Transceive {
			get {
				return rigCapsNative.transceive;
			}
		}

		public int BankQty {
			get {
				return rigCapsNative.bank_qty;
			}
		}

		public int ChanDescSz {
			get {
				return rigCapsNative.chan_desc_sz;
			}
		}

		public IList<ChannelList> ChanList {
			get {
				return Array.AsReadOnly<ChannelList> (channelList);
			}
		}

		public FrequencyRange[] RxRangeList1 {
			get {
				return rigCapsNative.rx_range_list1;
			}
		}

		public FrequencyRange[] TxRangeList1 {
			get {
				return rigCapsNative.tx_range_list1;
			}
		}

		public FrequencyRange[] RxRangeList2 {
			get {
				return rigCapsNative.rx_range_list2;
			}
		}

		public FrequencyRange[] TxRangeList2 {
			get {
				return rigCapsNative.tx_range_list2;
			}
		}

		public IList<TuningStep> TuningSteps {
			get {
				return Array.AsReadOnly<TuningStep> (tuningStep);
			}
		}

		public IList<FilterList> Filters {
			get {
				return Array.AsReadOnly<FilterList> (filterList);
			}
		}

		public bool HasPrivateData { get { return rigCapsNative.priv != IntPtr.Zero; } }

		public bool HasRigInit { get { return rigCapsNative.rig_init != IntPtr.Zero; } }

		public bool HasRigCleanup { get { return rigCapsNative.rig_cleanup != IntPtr.Zero; } }

		public bool HasRigOpen { get { return rigCapsNative.rig_open != IntPtr.Zero; } }

		public bool HasRigClose { get { return rigCapsNative.rig_close != IntPtr.Zero; } }

		public bool HasSetFreq { get { return rigCapsNative.set_freq != IntPtr.Zero; } }

		public bool HasGetFreq { get { return rigCapsNative.get_freq != IntPtr.Zero; } }

		public bool HasSetMode { get { return rigCapsNative.set_mode != IntPtr.Zero; } }

		public bool HasGetMode { get { return rigCapsNative.get_mode != IntPtr.Zero; } }

		public bool HasSetVfo { get { return rigCapsNative.set_vfo != IntPtr.Zero; } }

		public bool HasGetVfo { get { return rigCapsNative.get_vfo != IntPtr.Zero; } }

		public bool HasSetPtt{ get { return rigCapsNative.set_ptt != IntPtr.Zero; } }

		public bool HasGetPtt { get { return rigCapsNative.get_ptt != IntPtr.Zero; } }

		public bool HasGetDcd { get { return rigCapsNative.get_dcd != IntPtr.Zero; } }

		public bool HasSetRptrShift { get { return rigCapsNative.set_rptr_shift != IntPtr.Zero; } }

		public bool HasGetRptrShift { get { return rigCapsNative.get_rptr_shift != IntPtr.Zero; } }

		public bool HasSetRptrOffs { get { return rigCapsNative.set_rptr_offs != IntPtr.Zero; } }

		public bool HasGetRptrOffs { get { return rigCapsNative.get_rptr_offs != IntPtr.Zero; } }

		public bool HasSetSplitFreq { get { return rigCapsNative.set_split_freq != IntPtr.Zero; } }

		public bool HasGetSplitFreq { get { return rigCapsNative.get_split_freq != IntPtr.Zero; } }

		public bool HasSetSplitMode { get { return rigCapsNative.set_split_mode != IntPtr.Zero; } }

		public bool HasGetSplitMode { get { return rigCapsNative.get_split_mode != IntPtr.Zero; } }

		public bool HasSetSplitFreqMode { get { return rigCapsNative.set_split_freq_mode != IntPtr.Zero; } }

		public bool HasGetSplitFreqMode { get { return rigCapsNative.get_split_freq_mode != IntPtr.Zero; } }

		public bool HasSetSplitVfo { get { return rigCapsNative.set_split_vfo != IntPtr.Zero; } }

		public bool HasGetSplitVfo { get { return rigCapsNative.get_split_vfo != IntPtr.Zero; } }

		public bool HasSetRit { get { return rigCapsNative.set_rit != IntPtr.Zero; } }

		public bool HasGetRit { get { return rigCapsNative.get_rit != IntPtr.Zero; } }

		public bool HasSetXit { get { return rigCapsNative.set_xit != IntPtr.Zero; } }

		public bool HasGetXit { get { return rigCapsNative.get_xit != IntPtr.Zero; } }

		public bool HasSetTS { get { return rigCapsNative.set_ts != IntPtr.Zero; } }

		public bool HasGetTS { get { return rigCapsNative.get_ts != IntPtr.Zero; } }

		public bool HasSetDcsCode { get { return rigCapsNative.set_dcs_code != IntPtr.Zero; } }

		public bool HasGetDcsCode { get { return rigCapsNative.get_dcs_code != IntPtr.Zero; } }

		public bool HasSetTone { get { return rigCapsNative.set_tone != IntPtr.Zero; } }

		public bool HasGetTone { get { return rigCapsNative.get_tone != IntPtr.Zero; } }

		public bool HasSetCtcssTone { get { return rigCapsNative.set_ctcss_tone != IntPtr.Zero; } }

		public bool HasGetCtcssTone { get { return rigCapsNative.get_ctcss_tone != IntPtr.Zero; } }

		public bool HasSetDcsSquelch { get { return rigCapsNative.set_dcs_sql != IntPtr.Zero; } }

		public bool HasGetDcsSquelch { get { return rigCapsNative.get_dcs_sql != IntPtr.Zero; } }

		public bool HasSetToneSquelch { get { return rigCapsNative.set_tone_sql != IntPtr.Zero; } }

		public bool HasGetToneSquelch { get { return rigCapsNative.get_tone_sql != IntPtr.Zero; } }

		public bool HasSetCtcssSquelch { get { return rigCapsNative.set_ctcss_sql != IntPtr.Zero; } }

		public bool HasGetCtcssSquelch { get { return rigCapsNative.get_ctcss_sql != IntPtr.Zero; } }

		public bool HasPower2mW { get { return rigCapsNative.power2mW != IntPtr.Zero; } }

		public bool HasmW2power { get { return rigCapsNative.mW2power != IntPtr.Zero; } }

		public bool HasSetPowerstat { get { return rigCapsNative.set_powerstat != IntPtr.Zero; } }

		public bool HasGetPowerstat { get { return rigCapsNative.get_powerstat != IntPtr.Zero; } }

		public bool HasReset { get { return rigCapsNative.reset != IntPtr.Zero; } }

		public bool HasSetAnt { get { return rigCapsNative.set_ant != IntPtr.Zero; } }

		public bool HasGetAnt { get { return rigCapsNative.get_ant != IntPtr.Zero; } }

		public bool HasSetLevel { get { return rigCapsNative.set_level != IntPtr.Zero; } }

		public bool HasGetLevel { get { return rigCapsNative.get_level != IntPtr.Zero; } }

		public bool HasSetFunc { get { return rigCapsNative.set_func != IntPtr.Zero; } }

		public bool HasGetFunc { get { return rigCapsNative.get_func != IntPtr.Zero; } }

		public bool HasSetParm { get { return rigCapsNative.set_parm != IntPtr.Zero; } }

		public bool HasGetParm { get { return rigCapsNative.get_parm != IntPtr.Zero; } }

		public bool HasSetExtLevel { get { return rigCapsNative.set_ext_level != IntPtr.Zero; } }

		public bool HasGetExtLevel { get { return rigCapsNative.get_ext_level != IntPtr.Zero; } }

		public bool HasSetExtParm { get { return rigCapsNative.set_ext_parm != IntPtr.Zero; } }

		public bool HasGetExtParm { get { return rigCapsNative.get_ext_parm != IntPtr.Zero; } }

		public bool HasSetConf { get { return rigCapsNative.set_conf != IntPtr.Zero; } }

		public bool HasGetConf { get { return rigCapsNative.get_conf != IntPtr.Zero; } }

		public bool HasSendDTMF { get { return rigCapsNative.send_dtmf != IntPtr.Zero; } }

		public bool HasRecvDTMF { get { return rigCapsNative.recv_dtmf != IntPtr.Zero; } }

		public bool HasSendMorse { get { return rigCapsNative.send_morse != IntPtr.Zero; } }

		public bool HasSetBank { get { return rigCapsNative.set_bank != IntPtr.Zero; } }

		public bool HasSetMem { get { return rigCapsNative.set_mem != IntPtr.Zero; } }

		public bool HasGetMem { get { return rigCapsNative.get_mem != IntPtr.Zero; } }

		public bool HasVfoOp { get { return rigCapsNative.vfo_op != IntPtr.Zero; } }

		public bool HasScan { get { return rigCapsNative.scan != IntPtr.Zero; } }

		public bool HasSetTrn { get { return rigCapsNative.set_trn != IntPtr.Zero; } }

		public bool HasGetTrn { get { return rigCapsNative.get_trn != IntPtr.Zero; } }

		public bool HasDecodeEvent { get { return rigCapsNative.decode_event != IntPtr.Zero; } }

		public bool HasSetChannel { get { return rigCapsNative.set_channel != IntPtr.Zero; } }

		public bool HasGetChannel { get { return rigCapsNative.get_channel != IntPtr.Zero; } }

		public bool HasGetInfo { get { return rigCapsNative.get_info != IntPtr.Zero; } }

		public string CloneComboSet  { get { return rigCapsNative.clone_combo_set; } }

		public string CloneComboGet  { get { return rigCapsNative.clone_combo_get; } }

		//		public override string ToString ()
		//		{
		//			return string.Format ("{{\"RigModel\": \"{0}\",\"ModelName\": \"{1}\", \"ManufactureName\" \"{2}\" }}",
		//				rig_model, model_name, mfg_name);
		//		}

		public string ToJson ()
		{
			return string.Format ("{{ \"RigModel\": \"{0}\", \"ModelName\": \"{1}\", \"MfgName\": \"{2}\" }}",
				rigCapsNative.rig_model, rigCapsNative.model_name, rigCapsNative.mfg_name);
		}
	}
}
