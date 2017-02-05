//
//  Enums.cs
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
using HamLibSharp.Utils;

namespace HamLibSharp
{
	public enum RigDebugLevel
	{
		/// no bug reporting
		None = 0,
		/// serious bug
		Debug,
		/// error case (e.g. protocol, memory allocation)
		Error,
		/// warning
		Warn,
		/// verbose
		Verbose,
		/// tracing
		Trace
	}

	public enum RigBackendStatus
	{
		/// Alpha quality, i.e. development
		Alpha = 0,
		/// Written from available specs, rig unavailable for test, feedback wanted!
		Untested,
		/// Beta quality
		Beta,
		/// Stable
		Stable,
		/// Was stable, but something broke it!
		Buggy,
	};

	public enum PttType
	{
		/// No PTT available
		None = 0,
		/// Legacy PTT
		Rig,
		/// PTT control through serial DTR signal
		SerialDtr,
		/// PTT control through serial RTS signal
		SerialRts,
		/// PTT control through parallel port
		Parallel,
		/// Legacy PTT, supports RIG_PTT_ON_MIC/RIG_PTT_ON_DATA
		RigMicData,
		/// PTT control through CM108 GPIO pin
		CM108
	}

	// Radio power state
	[Flags]
	public enum PowerState
	{
		/// Power off
		Off =	0,
		/// Power on
		On =	(1 << 0),
		/// Standby
		Standby =	(1 << 1)
	}

	//Reset operation
	[Flags]
	public enum RigReset
	{
		/// No reset
		None = 0,
		/// Software reset
		Soft =	(1 << 0),
		/// VFO reset
		Vfo =	(1 << 1),
		/// Memory clear
		MemoryClear =	(1 << 2),
		/// Master reset
		Master =	(1 << 3)
	}

	//DCD type
	public enum RigDcd
	{
		/// No DCD available
		None = 0,
		/// Rig has DCD status support, i.e. rig has get_dcd cap
		Rig,
		/// DCD status from serial DSR signal
		SerialDsr,
		/// DCD status from serial CTS signal
		SerialCts,
		/// DCD status from serial CD signal
		SerialCar,
		/// DCD status from parallel port pin
		Parallel,
		/// DCD status from CM108 vol dn pin
		CM108
	}

	public enum RigPort
	{
		/// No port
		None = 0,
		/// Serial
		Serial,
		/// Network socket type
		Network,
		/// Device driver, like the WiNRADiO
		Device,
		/// AX.25 network type, e.g. SV8CS protocol
		Packet,
		/// DTMF protocol bridge via another rig, eg. Kenwood Sky Cmd System
		DTMF,
		/// IrDA Ultra protocol!
		IrdaUltra,
		/// RPC wrapper
		RPC,
		/// Parallel port
		Parallel,
		/// USB port
		USB,
		/// UDP Network socket type
		UdpNetwork,
		/// CM108 GPIO
		CM108,
		/// GPIO
		Gpio,
		/// GPIO Inverted
		GpioInverted,
	}

	//Serial parity
	public enum RigSerialParity
	{
		/// No parity
		None = 0,
		/// Odd
		Odd,
		/// Even
		Even,
		/// Mark
		Mark,
		/// Space
		Space
	}

	//Serial handshake
	public enum RigSerialHandshake
	{
		/// No handshake
		None = 0,
		/// Software XON/XOFF
		[TextName("XONXOFF")]
		XonXoff,
		/// Hardware CTS/RTS
		Hardware
	}


	//Serial control state
	public enum RigSerialControlState
	{
		/// Unset or tri-state
		Unset = 0,
		/// ON
		On,
		/// OFF
		Off
	}

	public enum RigSerialBaudRate
	{
		BaudNone = 0,
		Baud2400 = 2400,
		Baud4800 = 4800,
		Baud9600 = 9600,
		Baud14400 = 14400,
		Baud19200 = 19200,
		Baud28800 = 28800,
		Baud38400 = 38400,
		Baud56000 = 56000,
		Baud57600 = 57600,
		Baud115200 = 115200,
		Baud128000 = 128000,
		Baud256000 = 256000,
	}

	//Announce
	//Designate optional speech synthesizer.
	[Flags]
	public enum RigAnnounce
	{
		/// None
		None =	0,
		/// disable announces
		Off = None,
		/// Announce frequency
		Frequency =	(1 << 0),
		/// Announce receive mode
		RxMode = (1 << 1),
		/// CW
		CW = (1 << 2),
		/// English
		English = (1 << 3),
		/// Japan
		Japan = (1 << 4)
	}

	//VFO operation
	//
	// A VFO operation is an action on a VFO (or tunable memory).
	// The difference with a function is that an action has no on/off
	// status, it is performed at once.
	//
	// Note: the vfo argument for some vfo operation may be irrelevant,
	// and thus will be ignored.
	//
	// The VFO/MEM "mode" is set by rig_set_vfo.
	// STRING used in rigctl
	[Flags]
	public enum RigVfoOperation : uint
	{
		/// '' No VFO_OP
		None = 0,
		/// CPY -- VFO A = VFO B
		Copy = (1 << 0),
		/// XCHG -- Exchange VFO A/B
		Exchange = (1 << 1),
		/// FROM_VFO -- VFO->MEM
		VfoToMem =	(1 << 2),
		/// TO_VFO -- MEM->VFO
		MemToVfo = (1 << 3),
		/// MCL -- Memory clear
		MemoryClear = (1 << 4),
		/// UP -- UP increment VFO freq by tuning step*/
		StepUp = (1 << 5),
		/// DOWN -- DOWN decrement VFO freq by tuning step*/
		StepDown = (1 << 6),
		/// BAND_UP -- Band UP
		BandUp =	(1 << 7),
		/// BAND_DOWN -- Band DOWN
		BandDown =	(1 << 8),
		/// LEFT -- LEFT
		Left = (1 << 9),
		/// RIGHT -- RIGHT
		Right = (1 << 10),
		/// TUNE -- Start tune
		Tune = (1 << 11),
		/// TOGGLE -- Toggle VFOA and VFOB
		Toggle = (1 << 12)
	}

	//Rig Scan operation
	//
	// Various scan operations supported by a rig.
	// STRING used in rigctl
	//
	[Flags]
	public enum RigScanOperation
	{
		/// '' No Scan
		None = 0,
		//// MEM -- Scan all memory channels
		Memory = (1 << 0),
		/// SLCT -- Scan all selected memory channels
		Selected = (1 << 1),
		/// PRIO -- Priority watch (mem or call channel)
		Priority = (1 << 2),
		/// PROG -- Programmed(edge) scan
		Programmed = (1 << 3),
		/// DELTA -- delta-f scan
		Delta =	(1 << 4),
		/// VFO -- most basic scan
		Vfo = (1 << 5),
		/// PLT -- Scan using pipelined tuning
		PipelinedTuning = (1 << 6),
		/// STOP -- Stop scanning
		Stop = (1 << 7),
	}


	//Memory channel type definition
	//
	// Definition of memory types. Depending on the type, the content
	// of the memory channel has to be interpreted accordingly.
	// For instance, a RIG_MTYPE_EDGE channel_t will hold only a start
	// or stop frequency.
	//
	public enum RigMemoryChannel
	{
		/// None
		None = 0,
		/// Regular
		Memory,
		/// Scan edge
		Edge,
		/// Call channel
		Call,
		/// Memory pad
		MemoryPad,
		/// Satellite
		Satellite,
		/// VFO/Band channel
		VfoBand,
		/// Priority channel
		Priority
	}

	//Radio mode
	//
	// Various modes supported by a rig.
	// STRING used in rigctl
	[Flags]
	public enum RigMode
	{
		/// '' -- None
		None = 0,
		/// AM -- Amplitude Modulation
		AM = (1 << 0),
		/// CW -- CW "normal" sideband
		CW = (1 << 1),
		/// USB -- Upper Side Band
		USB = (1 << 2),
		/// LSB -- Lower Side Band
		LSB = (1 << 3),
		/// RTTY -- Radio Teletype
		RTTY = (1 << 4),
		/// FM -- "narrow" band FM
		FM = (1 << 5),
		/// WFM -- broadcast wide FM
		WFM = (1 << 6),
		/// CWR -- CW "reverse" sideband
		CWR = (1 << 7),
		/// RTTYR -- RTTY "reverse" sideband
		RTTYR =	(1 << 8),
		/// AMS -- Amplitude Modulation Synchronous
		AMS = (1 << 9),
		/// PKTLSB -- Packet/Digital LSB mode (dedicated port)
		[TextName("Packet LSB")]
		PacketLSB = (1 << 10),
		/// PKTUSB -- Packet/Digital USB mode (dedicated port)
		[TextName("Packet USB")]
		PacketUSB = (1 << 11),
		/// PKTFM -- Packet/Digital FM mode (dedicated port)
		[TextName("Packet FM")]
		PacketFM = (1 << 12),
		/// ECSSUSB -- Exalted Carrier Single Sideband USB
		EcssUSB = (1 << 13),
		/// ECSSLSB -- Exalted Carrier Single Sideband LSB
		EcssLSB = (1 << 14),
		/// FAX -- Facsimile Mode
		FAX = (1 << 15),
		/// SAM -- Synchronous AM double sideband
		SAM = (1 << 16),
		/// SAL -- Synchronous AM lower sideband
		SAL = (1 << 17),
		/// SAH -- Synchronous AM upper (higher) sideband
		SAH = (1 << 18),
		/// DSB -- Double sideband suppressed carrier
		DSB = (1 << 19),
		/// MUST ALWAYS BE LAST, Max Count for dumpcaps.c
		//TestMax
	}
	//rmode_t;

	//Hamlib error codes
	// Error code definition that can be returned by the Hamlib functions.
	// Unless stated otherwise, Hamlib functions return the negative value
	// of rig_errcode_e definitions in case of error, or 0 when successful.
	public enum RigError
	{
		/// No error, operation completed successfully
		OK = 0,
		/// invalid parameter
		InvalidParameter,
		/// invalid configuration (serial,..)
		InvalidConfiguration,
		/// memory shortage
		MemoryShortage,
		/// function not implemented, but will be
		NotImplemented,
		/// communication timed out
		Timeout,
		/// IO error, including open failed
		IO,
		/// Internal Hamlib error, huh!
		Internal,
		/// Protocol error
		Protocol,
		/// Command rejected by the rig
		CommandRejected,
		/// Command performed, but arg truncated
		ArgTruncated,
		/// function not available
		FunctionNotAvailable,
		/// VFO not targetable
		VFONotTargetable,
		/// Error talking on the bus
		BusError,
		/// Collision on the bus
		BusBusy,
		/// NULL RIG handle or any invalid pointer parameter in get arg
		InvalidRigHandle,
		/// Invalid VFO
		InvalidVFO,
		/// Argument out of domain of func
		ArgumentDomain

	}

	//PTT status
	public enum PttMode
	{
		/// PTT desactivated
		Off = 0,
		/// PTT activated
		On,
		/// PTT Mic only, fallbacks on RIG_PTT_ON if unavailable
		OnMic,
		/// PTT Data (Mic-muted), fallbacks on RIG_PTT_ON if unavailable
		OnData
	}



	//DCD status
	public enum DcdState
	{
		/// Squelch closed
		Off = 0,
		/// Squelch open
		On
	}

	//Repeater shift type
	public enum RepeaterShift
	{
		/// No repeater shift
		None = 0,
		/// "-" shift
		Minus,
		/// "+" shift
		Plus
	}

	//Split mode
	public enum RigSplit
	{
		/// Split mode disabled
		Off = 0,
		/// Split mode enabled
		On
	}

	[Flags]
	public enum RigLevel
	{
		/// '' -- No Level
		None =	0,
		/// PREAMP -- Preamp, arg int (dB)
		Preamp =	(1 << 0),
		/// ATT -- Attenuator, arg int (dB)
		Attenuator = (1 << 1),
		/// VOX -- VOX delay, arg int (tenth of seconds)
		Vox = (1 << 2),
		/// AF -- Volume, arg float [0.0 ... 1.0]
		Volume = (1 << 3),
		/// RF -- RF gain (not TX power), arg float [0.0 ... 1.0]
		RF = (1 << 4),
		/// SQL -- Squelch, arg float [0.0 ... 1.0]
		Squelch = (1 << 5),
		/// IF -- IF, arg int (Hz)
		IF = (1 << 6),
		/// APF -- Audio Peak Filter, arg float [0.0 ... 1.0]
		AudioPeakFilter = (1 << 7),
		/// NR -- Noise Reduction, arg float [0.0 ... 1.0]
		NoiseReduction = (1 << 8),
		/// PBT_IN -- Twin PBT (inside), arg float [0.0 ... 1.0]
		TwinPbtIn =	(1 << 9),
		/// PBT_OUT -- Twin PBT (outside), arg float [0.0 ... 1.0]
		TwinPbtOut =	(1 << 10),
		/// CWPITCH -- CW pitch, arg int (Hz)
		CWPitch =	(1 << 11),
		/// RFPOWER -- RF Power, arg float [0.0 ... 1.0]
		RFPower =	(1 << 12),
		/// MICGAIN -- MIC Gain, arg float [0.0 ... 1.0]
		MicGain =	(1 << 13),
		/// KEYSPD -- Key Speed, arg int (WPM)
		KeySpeed =	(1 << 14),
		/// NOTCHF -- Notch Freq., arg int (Hz)
		NotchFrequency =	(1 << 15),
		/// COMP -- Compressor, arg float [0.0 ... 1.0]
		Compressor =	(1 << 16),
		/// AGC -- AGC, arg int (see enum agc_level_e)
		Agc = (1 << 17),
		/// BKINDL -- BKin Delay, arg int (tenth of dots)
		BKInDelay =	(1 << 18),
		/// BAL -- Balance (Dual Watch), arg float [0.0 ... 1.0]
		Balance =	(1 << 19),
		/// METER -- Display meter, arg int (see enum meter_level_e)
		DisplayMeter =	(1 << 20),
		/// VOXGAIN -- VOX gain level, arg float [0.0 ... 1.0]
		VoxGain =	(1 << 21),
		/// Synonym of RIG_LEVEL_VOX
		VoxDelay = Vox,
		/// ANTIVOX -- anti-VOX level, arg float [0.0 ... 1.0]
		AntiVox =	(1 << 22),
		/// SLOPE_LOW -- Slope tune, low frequency cut,
		SlopeLow =	(1 << 23),
		/// SLOPE_HIGH -- Slope tune, high frequency cut,
		SlopeHigh =	(1 << 24),
		/// BKIN_DLYMS -- BKin Delay, arg int Milliseconds
		BKInDelayMS =	(1 << 25),

		//These are not settable

		/// RAWSTR -- Raw (A/D) value for signal strength, specific to each rig, arg int
		RIG_LEVEL_RAWSTR =	(1 << 26),
		/// SQLSTAT -- SQL status, arg int (open=1/closed=0). Deprecated, use get_dcd instead
		RIG_LEVEL_SQLSTAT =	(1 << 27),
		/// SWR -- SWR, arg float [0.0 ... infinite]
		Swr = (1 << 28),
		/// ALC -- ALC, arg float
		Alc = (1 << 29),
		/// STRENGTH -- Effective (calibrated) signal strength relative to S9, arg int (dB)
		RIG_LEVEL_STRENGTH =	(1 << 30)
		/// Bandwidth Control, arg int (Hz)
		// RIG_LEVEL_BWC =	(1<<31)
	}

	//Rig Parameters
	//
	// Parameters are settings that are not VFO specific.
	// STRING used in rigctl
	[Flags]
	public enum RigParm
	{
		/// '' -- No Parm
		None = 0,
		/// ANN -- "Announce" level, see ann_t
		Announce = (1 << 0),
		/// APO -- Auto power off, int in minute
		AutoPowerOff = (1 << 1),
		/// BACKLIGHT -- LCD light, float [0.0 ... 1.0]
		Backlight = (1 << 2),
		/// BEEP -- Beep on keypressed, int (0,1)
		Beep = (1 << 4),
		/// TIME -- hh:mm:ss, int in seconds from 00:00:00
		Time = (1 << 5),
		/// BAT -- battery level, float [0.0 ... 1.0]
		BatteryLevel = (1 << 6),
		/// KEYLIGHT -- Button backlight, on/off
		ButtonBacklight =	(1 << 7)
	}

	// Rig type flags
	[Flags]
	public enum RigFlags
	{
		Other = 0,
		/// Receiver
		Receiver =	(1 << 1),
		/// Transmitter
		Transmitter =	(1 << 2),
		Transceiver = (Receiver | Transmitter),
		/// Scanner
		Scanner =	(1 << 3),
		/// mobile sized
		Mobile =	(1 << 4),
		/// handheld sized
		Handheld =	(1 << 5),
		/// "Computer" rig
		Computer =	(1 << 6),
		/// has trunking
		Trunking =	(1 << 7),
		/// has APRS
		Aprs = (1 << 8),
		/// has TNC
		Tnc = (1 << 9),
		/// has DXCluster
		DXCluster =	(1 << 10),
		/// dumb tuner
		Tuner =	(1 << 11),
		Mask = (Transceiver | Scanner | Mobile | Handheld |
		Computer | Trunking | Tuner),
	}

	[Flags]
	public enum RigType
	{
		Other	=	0,
		Transceiver	= RigFlags.Transceiver,
		Handheld	= (RigFlags.Transceiver | RigFlags.Handheld),
		Mobile = (RigFlags.Transceiver | RigFlags.Mobile),
		Receiver	= RigFlags.Receiver,
		PCReceiver	= (RigFlags.Computer | RigFlags.Receiver),
		Scanner	= (RigFlags.Scanner | RigFlags.Receiver),
		TrunkScanner =	(RigFlags.Scanner | RigFlags.Trunking),
		Computer	= (RigFlags.Transceiver | RigFlags.Computer),
		Tuner = RigFlags.Tuner,
	}

	public enum RigConf
	{
		/// String type
		String,
		/// Combo type
		Combo,
		/// Numeric type integer or real
		Numeric,
		/// on/off type
		CheckButton,
		/// Button type
		Button

	}

	public enum RigTransceive
	{
		Off = 0,
		Rig = 1,
		Poll = 2,
	}

	public enum RigAgcLevel
	{
		Off = 0,
		Superfast,
		Fast,
		Slow,
		User,
		Medium,
		Auto
	};

	public static class ModeValue
	{
		public const int Any = 0;
	}
}
