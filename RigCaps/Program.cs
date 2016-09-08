//
//  Program.cs
//
//  Author:
//       Jae Stutzman <jaebird@gmail.com>
//
//  Copyright (c) 2016 Jae Stutzman
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Text;
using HamLibSharp;

namespace RigCaps
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine("HamLib Native Library Version: {0}", HamLib.NativeVersion);
			Console.WriteLine("HamLib Managed Library Version: {0}", HamLib.ManagedVersion);

			var rig = new Rig ("Dummy");
			//var rig = new Rig ("Si570 AVR-USB");
			//var rig = new Rig ("FT-857");
			//rig.Open ();

			var caps = rig.Caps;

			int i;
			int backend_warnings = 0;
			StringBuilder sb;

			Console.WriteLine("Caps dump for model:\t{0}", caps.RigModel);
			Console.WriteLine("Model name:\t\t{0}", caps.ModelName);
			Console.WriteLine("Mfg name:\t\t{0}", caps.MfgName);
			Console.WriteLine("Backend version:\t{0}", caps.Version);
			Console.WriteLine("Backend copyright:\t{0}", caps.Copyright);
			Console.WriteLine("Backend status:\t\t{0}", caps.Status);
			Console.Write("Rig type:\t");
			switch (caps.RigType) {// & RigFlags.Mask) {
			case RigType.Transceiver:
				Console.WriteLine("Transceiver");
				break;
			case RigType.Handheld:
				Console.WriteLine("Handheld");
				break;
			case RigType.Mobile:
				Console.WriteLine("Mobile");
				break;
			case RigType.Receiver:
				Console.WriteLine("Receiver");
				break;
			case RigType.PCReceiver:
				Console.WriteLine("PC Receiver");
				break;
			case RigType.Scanner:
				Console.WriteLine("Scanner");
				break;
			case RigType.TrunkScanner:
				Console.WriteLine("Trunking scanner");
				break;
			case RigType.Computer:
				Console.WriteLine("Computer");
				break;
			case RigType.Tuner:
				Console.WriteLine("Tuner");
				break;
			case RigType.Other:
				Console.WriteLine("Other");
				break;
			default:
				Console.WriteLine ("Unknown");
				backend_warnings++;
				break;
			}

			Console.Write("PTT type:\t");
			switch (caps.PttType) {
			case PttType.Rig:
				Console.WriteLine("Rig capable");
				break;
			case PttType.RigMicData:
				Console.WriteLine("Rig capable (Mic/Data)");
				break;
			case PttType.Parallel:
				Console.WriteLine("Parallel port (DATA0)");
				break;
			case PttType.SerialRts:
				Console.WriteLine("Serial port (CTS/RTS)");
				break;
			case PttType.SerialDtr:
				Console.WriteLine("Serial port (DTR/DSR)");
				break;
			case PttType.None:
				Console.WriteLine("None");
				break;
			default:
				Console.WriteLine ("Unknown");
				backend_warnings++;
				break;
			}

			Console.Write("DCD type:\t");
			switch (caps.DcdType) {
			case RigDcd.Rig:
				Console.WriteLine("Rig capable");
				break;
			case RigDcd.Parallel:
				Console.WriteLine("Parallel port (/STROBE)");
				break;
			case RigDcd.SerialCts:
				Console.WriteLine("Serial port (CTS/RTS)");
				break;
			case RigDcd.SerialDsr:
				Console.WriteLine("Serial port (DTR/DSR)");
				break;
			case RigDcd.SerialCar:
				Console.WriteLine("Serial port (CD)");
				break;
			case RigDcd.None:
				Console.WriteLine("None");
				break;
			default:
				Console.WriteLine ("Unknown");
				backend_warnings++;
				break;
			}
				
			Console.Write("Port type:\t");
			switch (caps.PortType) {
			case RigPort.Serial:
				Console.WriteLine("RS-232");
				Console.WriteLine("Serial speed: {0}..{1} bauds, Databits:{2}, Parity:{3}, Stopbits:{4}, HW Handshake:{5}", caps.SerialRateMin,
					caps.SerialRateMax, caps.SerialDataBits, caps.SerialParity, caps.SerialStopBits,
					caps.SerialHandshake);
				break;
			case RigPort.Parallel:
				Console.WriteLine("Parallel");
				break;
			case RigPort.Device:
				Console.WriteLine("Device driver");
				break;
			case RigPort.USB:
				Console.WriteLine("USB");
				break;
			case RigPort.Network:
				Console.WriteLine("Network link");
				break;
			case RigPort.UdpNetwork:
				Console.WriteLine("UDP Network link");
				break;
			case RigPort.None:
				Console.WriteLine("None");
				break;
			default:
				Console.WriteLine ("Unknown");
				backend_warnings++;
				break;
			}

			Console.WriteLine("Write delay: {0}ms, timeout {1}ms, {2} retry",
				caps.WriteDelay, caps.Timeout, caps.Retry);
			Console.WriteLine("Post Write delay: {0}ms",
				caps.PostWriteDelay);

			Console.WriteLine("Has targetable VFO: {0}",
				caps.TargetableVfo != 0 ? "Y" : "N");

			Console.WriteLine("Has transceive: {0}",
				caps.Transceive != 0 ? "Y" : "N");

			Console.WriteLine("Announce: 0x{0:X}", caps.Announces);
			Console.WriteLine("Max RIT: -{0}.{1}kHz/+{0}.{1}kHz",
				caps.MaxRit / 1000, caps.MaxRit % 1000);

			Console.WriteLine("Max XIT: -{0}.{1}kHz/+{0}.{1}kHz",
				caps.MaxXit / 1000, caps.MaxXit % 1000);

			Console.WriteLine("Max IF-SHIFT: -{0}.{1}kHz/+{0}.{1}kHz",
				caps.MaxIfshift / 1000, caps.MaxIfshift % 1000);
			
			Console.Write("Preamp:");
			for (i = 0; i < caps.Preamp.Length && caps.Preamp [i] != 0; i++) {
				Console.Write (" {0}dB", caps.Preamp [i]);
			}
			if (i == 0) {
				Console.WriteLine (" None");
			} else {
				Console.WriteLine ();
			}

			Console.Write("Attenuator:");
			for (i = 0; i < caps.Attenuator.Length && caps.Attenuator [i] != 0; i++) {
				Console.Write (" {0}dB", caps.Attenuator [i]);
			}
			if (i == 0) {
				Console.WriteLine (" None");
			} else {
				Console.WriteLine ();
			}
				
			Console.Write("CTCSS:");
			for(i = 0; i < caps.CtcssTones.Count; i++) {
				Console.Write(" {0}.{1}", caps.CtcssTones[i] / 10, caps.CtcssTones[i] % 10);
			}
			if (i == 0)
				Console.WriteLine(" None");
			else
				Console.WriteLine(" Hz, {0} tones", i);

			Console.Write("DCS:");
			for(i = 0; i < caps.DcsCodes.Count; i++) {
				Console.Write(" {0}", caps.DcsCodes[i]);
			}
			if (i == 0)
				Console.WriteLine(" None");
			else
				Console.WriteLine(", {0} codes", i);

			Console.WriteLine("Get functions: {0}",  caps.GetFunctions);

			Console.WriteLine("Set functions: {0}", caps.SetFunctions);

			// TODO: integrate the LevelGran into report
			Console.WriteLine("Get Levels: {0}",  caps.GetLevels);

			Console.WriteLine("Set Levels: {0}", caps.SetLevels);

			Console.Write("Extra levels:");
			foreach (var level in caps.ExtLevels) {
				Console.Write (" {0}", level.Name);
			}
			Console.WriteLine();

			// TODO: integrate the LevelGran into report
			Console.WriteLine("Get parameters: {0}",  caps.GetParms);

			Console.WriteLine("Set parameters: {0}", caps.SetParms);

			Console.Write("Extra parameters:");
			foreach (var level in caps.ExtParms) {
				Console.Write (" {0}", level.Name);
			}
			Console.WriteLine();

			if (rig.ModeList != RigMode.None) {
				Console.Write("Mode list: ");
				sb = new StringBuilder ();
				for (i = 0; i < 30; i++) {
					var val = (uint)rig.ModeList & (1U << i);
					if (val > 0) {
						sb.Append (Rig.ModeToString ((RigMode)val) + " ");
					}
				}
				Console.WriteLine ("\t{0}", sb);
			} else {
				Console.WriteLine ("None. This backend might be bogus!");
				backend_warnings++;
			}
				
			if (rig.VfoList != 0) {
				Console.Write("VFO list: ");
				sb = new StringBuilder ();
				for (i = 0; i < 30; i++) {
					var val = (uint)rig.VfoList & (1U << i);
					if (val > 0) {
						sb.Append (Rig.VfoToString (val) + " ");
					}
				}
				Console.WriteLine ("\t{0}", sb);
			} else {
				Console.WriteLine ("None. This backend might be bogus!");
				backend_warnings++;
			}

			sb = new StringBuilder ();
			foreach (RigVfoOperation op in Enum.GetValues(typeof(RigVfoOperation))) {
				sb.Append(Rig.VfoOperationToString(caps.VfoOps & op) + " ");
			}
				
			Console.WriteLine("VFO Ops: {0}", sb);

			sb = new StringBuilder ();
			foreach (RigScanOperation op in Enum.GetValues(typeof(RigScanOperation))) {
				sb.Append(Rig.ScanOperationToString(caps.ScanOps & op) + " ");
			}

			Console.WriteLine("Scan Ops: {0}", sb);

			Console.WriteLine("Number of banks:\t{0}", caps.BankQty);
			Console.WriteLine("Memory name desc size:\t{0}", caps.ChanDescSz);

			Console.WriteLine("Memories:");
			for (i = 0; i < caps.ChanList.Count; i++) {
				Console.WriteLine("\t{0}..{1}:   \t{2}", caps.ChanList[i].Start, caps.ChanList[i].End,
					Rig.MemoryChannelToString(caps.ChanList[i].Type));
				Console.Write("\t  Mem caps: ");
				Console.WriteLine(DumpChannelCaps(caps.ChanList[i].MemCaps));
			}
			if (i == 0)
				Console.WriteLine(" None");
			Console.WriteLine();

			int status = RangeSanityCheck(caps.TxRangeList1, 0);
			Console.WriteLine("TX ranges status, region 1:\t{0} ({1})", status != 0 ? "Bad" : "OK", status);
			if (status != 0) backend_warnings++;
			status = RangeSanityCheck(caps.RxRangeList1, 1);
			Console.WriteLine("RX ranges status, region 1:\t{0} ({1})", status != 0 ? "Bad" : "OK", status);
			if (status != 0) backend_warnings++;

			status = RangeSanityCheck(caps.TxRangeList2, 0);
			Console.WriteLine("TX ranges status, region 2:\t{0} ({1})", status != 0 ? "Bad" : "OK", status);
				if (status != 0) backend_warnings++;
			status = RangeSanityCheck(caps.RxRangeList2, 1);
			Console.WriteLine("RX ranges status, region 2:\t{0} ({1})", status != 0 ? "Bad" : "OK", status);
				if (status != 0) backend_warnings++;

			Console.WriteLine("Tuning steps:");
			for (i = 0; i < caps.TuningSteps.Count; i++) {
				if (caps.TuningSteps[i].Value == ModeValue.Any)
					Console.Write("\tANY");         
				else
					Console.Write("\t{0}", caps.TuningSteps[i].Value);

				sb = new StringBuilder ();
				foreach (RigMode mode in Enum.GetValues(typeof(RigMode))) {
					var rigMode = caps.TuningSteps[i].Modes & mode;
					if (rigMode != RigMode.None) {
						sb.Append(Rig.ModeToString(rigMode) + " ");
					}
				}
				Console.WriteLine("\t{0}", sb);
			}
			if (i == 0) {
				Console.WriteLine(" None! This backend might be bogus!");
				backend_warnings++;
			}
			Console.WriteLine();

			status = TuneStepSanityCheck(caps.TuningSteps);
			Console.WriteLine("Tuning steps status:\t{0} ({1})", status != 0 ? "Bad" : "OK", status);
			if (status != 0) backend_warnings++;


			Console.WriteLine("Filters:");
			for (i = 0; i < caps.Filters.Count; i++) {
				if (caps.Filters[i].Value == ModeValue.Any)
					Console.Write("\tANY");         
				else
					Console.Write("\t{0}", Rig.FrequencyToString(caps.Filters[i].Value));

				sb = new StringBuilder ();
				foreach (RigMode mode in Enum.GetValues(typeof(RigMode))) {
					var rigMode = caps.Filters[i].Modes & mode;
					if (rigMode != RigMode.None) {
						sb.Append(Rig.ModeToString(rigMode) + " ");
					}
				}
				Console.WriteLine("\t{0}", sb);
			}
			if (i == 0) {
				Console.WriteLine(" None. This backend might be bogus!");
				backend_warnings++;
			}
			Console.WriteLine();


			Console.WriteLine("Bandwidths:");
			foreach (RigMode mode in Enum.GetValues(typeof(RigMode))) {
				var passBandFreq = rig.PassbandNormal (mode);
				if (passBandFreq == 0)
					continue;

				Console.Write("\t{0}\tNormal: {1},\t", Rig.ModeToString(mode), Rig.FrequencyToString(passBandFreq));
				Console.Write("Narrow: {0},\t", Rig.FrequencyToString(rig.PassbandNarrow(mode)));
				Console.WriteLine("Wide: {0}", Rig.FrequencyToString(rig.PassbandWide(mode)));
			}
			Console.WriteLine();

			Console.WriteLine("Has priv data:\t{0}", caps.HasPrivateData ? 'Y' : 'N');

			// Status is either 'Y'es, 'E'mulated, 'N'o
			// TODO: keep me up-to-date with API call list!

			Console.WriteLine("Has Init:\t{0}", caps.HasRigInit ? 'Y' : 'N');
			Console.WriteLine("Has Cleanup:\t{0}", caps.HasRigCleanup ? 'Y' : 'N');
			Console.WriteLine("Has Open:\t{0}", caps.HasRigOpen ? 'Y' : 'N');
			Console.WriteLine("Has Close:\t{0}", caps.HasRigClose ? 'Y' : 'N');
			Console.WriteLine("Can set Conf:\t{0}", caps.HasSetConf ? 'Y' : 'N');
			Console.WriteLine("Can get Conf:\t{0}", caps.HasGetConf ? 'Y' : 'N');
			Console.WriteLine("Can set Frequency:\t{0}", caps.HasSetFreq ? 'Y' : 'N');
			Console.WriteLine("Can get Frequency:\t{0}", caps.HasGetFreq ? 'Y' : 'N');
			Console.WriteLine("Can set Mode:\t{0}", caps.HasSetMode ? 'Y' : 'N');
			Console.WriteLine("Can get Mode:\t{0}", caps.HasGetMode ? 'Y' : 'N');
			Console.WriteLine("Can set VFO:\t{0}", caps.HasSetVfo ? 'Y' : 'N');
			Console.WriteLine("Can get VFO:\t{0}", caps.HasGetVfo ? 'Y' : 'N');
			Console.WriteLine("Can set PTT:\t{0}", caps.HasSetPtt ? 'Y' : 'N');
			Console.WriteLine("Can get PTT:\t{0}", caps.HasGetPtt ? 'Y' : 'N');
			Console.WriteLine("Can get DCD:\t{0}", caps.HasGetDcd ? 'Y' : 'N');
			Console.WriteLine("Can set Repeater Duplex:\t{0}", caps.HasSetRptrShift ? 'Y' : 'N');
			Console.WriteLine("Can get Repeater Duplex:\t{0}", caps.HasGetRptrShift ? 'Y' : 'N');
			Console.WriteLine("Can set Repeater Offset:\t{0}", caps.HasSetRptrOffs ? 'Y' : 'N');
			Console.WriteLine("Can get Repeater Offset:\t{0}", caps.HasGetRptrOffs ? 'Y' : 'N');

			var canSplitVfo = caps.HasSetSplitVfo && (caps.HasSetVfo || (rig.HasVfoOP(RigVfoOperation.Toggle) && caps.VfoOps != RigVfoOperation.None));
			Console.WriteLine("Can set Split Freq:\t{0}", caps.HasSetSplitFreq ? 'Y' :
				(canSplitVfo && caps.HasSetFreq ? 'E' : 'N'));
			Console.WriteLine("Can get Split Freq:\t{0}", caps.HasGetSplitFreq ? 'Y' :
				(canSplitVfo && caps.HasGetFreq ? 'E' : 'N'));
			Console.WriteLine("Can set Split Mode:\t{0}", caps.HasSetSplitMode ? 'Y' :
				(canSplitVfo && caps.HasSetMode ? 'E' : 'N'));
			Console.WriteLine("Can get Split Mode:\t{0}", caps.HasGetSplitMode ? 'Y' :
				(canSplitVfo && caps.HasGetMode ? 'E' : 'N'));

			Console.WriteLine("Can set Split VFO:\t{0}", caps.HasSetSplitVfo ? 'Y' : 'N');
			Console.WriteLine("Can get Split VFO:\t{0}", caps.HasGetSplitVfo ? 'Y' : 'N');
			Console.WriteLine("Can set Tuning Step:\t{0}", caps.HasSetTS ? 'Y' : 'N');
			Console.WriteLine("Can get Tuning Step:\t{0}", caps.HasGetTS ? 'Y' : 'N');
			Console.WriteLine("Can set RIT:\t{0}", caps.HasSetRit ? 'Y' : 'N');
			Console.WriteLine("Can get RIT:\t{0}", caps.HasGetRit ? 'Y' : 'N');
			Console.WriteLine("Can set XIT:\t{0}", caps.HasSetXit ? 'Y' : 'N');
			Console.WriteLine("Can get XIT:\t{0}", caps.HasGetXit ? 'Y' : 'N');
			Console.WriteLine("Can set CTCSS:\t{0}", caps.HasSetCtcssTone ? 'Y' : 'N');
			Console.WriteLine("Can get CTCSS:\t{0}", caps.HasGetCtcssTone ? 'Y' : 'N');
			Console.WriteLine("Can set DCS:\t{0}", caps.HasSetDcsCode ? 'Y' : 'N');
			Console.WriteLine("Can get DCS:\t{0}", caps.HasGetDcsCode ? 'Y' : 'N');
			Console.WriteLine("Can set CTCSS Squelch:\t{0}", caps.HasSetCtcssSquelch ? 'Y' : 'N');
			Console.WriteLine("Can get CTCSS Squelch:\t{0}", caps.HasGetCtcssSquelch ? 'Y' : 'N');
			Console.WriteLine("Can set DCS Squelch:\t{0}", caps.HasSetDcsSquelch ? 'Y' : 'N');
			Console.WriteLine("Can get DCS Squelch:\t{0}", caps.HasGetDcsSquelch ? 'Y' : 'N');
			Console.WriteLine("Can set Power Stat:\t{0}", caps.HasSetPowerstat ? 'Y' : 'N');
			Console.WriteLine("Can get Power Stat:\t{0}", caps.HasGetPowerstat? 'Y' : 'N');
			Console.WriteLine("Can Reset:\t{0}", caps.HasReset ? 'Y' : 'N');
			Console.WriteLine("Can get Ant:\t{0}", caps.HasGetAnt ? 'Y' : 'N');
			Console.WriteLine("Can set Ant:\t{0}", caps.HasSetAnt ? 'Y' : 'N');
			Console.WriteLine("Can set Transceive:\t{0}", caps.HasSetTrn ? 'Y' : caps.Transceive == RigTransceive.Rig ? 'E' : 'N');
			Console.WriteLine("Can get Transceive:\t{0}", caps.HasGetTrn ? 'Y' : 'N');
			Console.WriteLine("Can set Func:\t{0}", caps.HasSetFunc ? 'Y' : 'N');
			Console.WriteLine("Can get Func:\t{0}", caps.HasGetFunc ? 'Y' : 'N');
			Console.WriteLine("Can set Level:\t{0}", caps.HasSetLevel ? 'Y' : 'N');
			Console.WriteLine("Can get Level:\t{0}", caps.HasGetLevel ? 'Y' : 'N');
			Console.WriteLine("Can set Param:\t{0}", caps.HasSetParm ? 'Y' : 'N');
			Console.WriteLine("Can get Param:\t{0}", caps.HasGetParm ? 'Y' : 'N');
			Console.WriteLine("Can send DTMF:\t{0}", caps.HasSendDTMF ? 'Y': 'N');
			Console.WriteLine("Can recv DTMF:\t{0}", caps.HasRecvDTMF ? 'Y' : 'N');
			Console.WriteLine("Can send Morse:\t{0}", caps.HasSendMorse ? 'Y' : 'N');
			Console.WriteLine("Can decode Events:\t{0}", caps.HasDecodeEvent ? 'Y' : 'N');
			Console.WriteLine("Can set Bank:\t{0}", caps.HasSetBank ? 'Y' : 'N');
			Console.WriteLine("Can set Mem:\t{0}", caps.HasSetMem ? 'Y' : 'N');
			Console.WriteLine("Can get Mem:\t{0}", caps.HasGetMem ? 'Y' : 'N');

			var can_echannel = caps.HasSetMem && (
				(caps.HasSetVfo && ((rig.VfoList & RigVfo.Memory) == RigVfo.Memory)) ||
				(caps.VfoOps != RigVfoOperation.None && rig.HasVfoOP(RigVfoOperation.MemToVfo | RigVfoOperation.VfoToMem)));

			Console.WriteLine("Can set Channel:\t{0}", caps.HasSetChannel ? 'Y' :
				(can_echannel ? 'E' : 'N'));
			Console.WriteLine("Can get Channel:\t{0}", caps.HasGetChannel ? 'Y' :
				(can_echannel ? 'E' : 'N'));

			Console.WriteLine("Can ctl Mem/VFO:\t{0}", caps.HasVfoOp ? 'Y' : 'N');
			Console.WriteLine("Can Scan:\t{0}", caps.HasScan ? 'Y' : 'N');
			Console.WriteLine("Can get Info:\t{0}", caps.HasGetInfo ? 'Y' : 'N');
			Console.WriteLine("Can get power2mW:\t{0}", caps.HasPower2mW ? 'Y' : 'N');
			Console.WriteLine("Can get mW2power:\t{0}", caps.HasmW2power ? 'Y' : 'N');
			Console.WriteLine("Overall backend warnings: {0}", backend_warnings);
		}

		static string DumpChannelCaps(IChannelCapability chan)
		{
			StringBuilder sb = new StringBuilder ();
			if (chan.BankNumber) sb.Append("BANK ");
			if (chan.Antenna) sb.Append("ANT ");
			if (chan.RXFrequency) sb.Append("FREQ ");
			if (chan.RXMode) sb.Append("MODE ");
			if (chan.RXWidth) sb.Append("WIDTH ");
			if (chan.TXFrequency) sb.Append("TXFREQ ");
			if (chan.TXMode) sb.Append("TXMODE ");
			if (chan.TXWidth) sb.Append("TXWIDTH ");
			if (chan.Split) sb.Append("SPLIT ");
			if (chan.RepeaterShift) sb.Append("RPTRSHIFT ");
			if (chan.RepeaterOffset) sb.Append("RPTROFS ");
			if (chan.TuningStep) sb.Append("TS ");
			if (chan.Rit) sb.Append("RIT ");
			if (chan.Xit) sb.Append("XIT ");
			if (chan.Functions > 0) sb.Append("FUNC "); /* TODO: iterate over the list */
			if (chan.Levels > 0) sb.Append("LEVEL "); /* TODO: iterate over the list */
			if (chan.CtcssTone) sb.Append("TONE ");
			if (chan.CtcssSquelch) sb.Append("CTCSS ");
			if (chan.DcsCode) sb.Append("DCSCODE ");
			if (chan.DcsSquelch) sb.Append("DCSSQL ");
			if (chan.ScanGroup) sb.Append("SCANGRP ");
			if (chan.ChannelFlags) sb.Append("FLAG "); /* TODO: iterate over the RIG_CHFLAG's */
			if (chan.ChannelName) sb.Append("NAME ");
			if (chan.ExtensionLevels) sb.Append("EXTLVL ");

			return sb.ToString ();
		}

		static int RangeSanityCheck(FrequencyRange[] freqRangeList, int rx)
		{
			int i;

			for (i = 0; i < freqRangeList.Length; i++) {
				if (freqRangeList[i].Start == 0 && freqRangeList[i].End == 0)
					break;
				if (freqRangeList[i].Start > freqRangeList[i].End)
					return -1;
				if (freqRangeList[i].Modes == RigMode.None)
					return -2;
				if (rx > 0) {
					if (freqRangeList[i].LowPower > 0 && freqRangeList[i].HighPower > 0)
						return -3;
				} else {
					if (!(freqRangeList[i].LowPower > 0 && freqRangeList[i].HighPower > 0))
						return -3;
					if (freqRangeList[i].LowPower > freqRangeList[i].HighPower)
						return -3;
				}
			}
			return 0;
		}

		static int TuneStepSanityCheck(IList<IModeValue> tuningStepList)
		{
			int i;
			int last_ts = 0;
			RigMode last_modes = RigMode.None;

			for (i=0; i < tuningStepList.Count; i++) {
				if (tuningStepList[i].Value != ModeValue.Any && 
					tuningStepList[i].Value < last_ts &&
					last_modes == tuningStepList[i].Modes)
					return -1;
				if (tuningStepList[i].Modes == RigMode.None)
					return -2;
				last_ts = tuningStepList[i].Value;
				last_modes = tuningStepList[i].Modes;
			}

			return 0;
		}
	}
}
