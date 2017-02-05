//
//  RigNative.cs
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
using HamLibSharp.x86;
using HamLibSharp.x64;

namespace HamLibSharp
{
	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate int FreqCallback (IntPtr theRig, int vfo, double freq, IntPtr rig_ptr);

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate int ConfParamsCallback (IntPtr rig, IntPtr confParams, IntPtr dataPtr);

	public partial class Rig : IDisposable
	{
		// From native HamLib rig.h file:

		// typedef double freq_t;
		// typedef int32_t shortfreq_t;
		// typedef shortfreq_t pbwidth_t;
		// typedef int vfo_t;
		// typedef uint32_t setting_t;
		// typedef unsigned int tone_t;
		// typedef int ant_t;
		// typedef union {
		//	signed int i;			/*!< Signed integer */
		//	float f;			/*!< Single precision float */
		//	char *s;			/*!< Pointer to char string */
		//	const char *cs;		/*!< Pointer to constant char string */
		// } value_t;

		internal const int RIGNAMSIZ = 30;
		internal const int RIGVERSIZ = 8;
		internal const int FILPATHLEN = 100;
		internal const int FRQRANGESIZ = 30;
		/* describe channel eg: "WWV 5Mhz" */
		internal const int MAXCHANDESC = 30;
		/* max tuning step list size, zero ended */
		internal const int TSLSTSIZ = 20;
		/* max preamp/att levels supported, zero ended */
		internal const int MAXDBLSTSIZ = 8;
		/* max mem_list size, zero ended */
		internal const int CHANLSTSIZ = 16;
		/* max calibration plots in cal_table_t */
		internal const int MAX_CAL_LENGTH = 32;
		internal const int RIG_SETTING_MAX = 32;
		internal const int RIG_COMBO_MAX = 8;


		[DllImport (HamLib.dllName)]
		internal static extern IntPtr rig_init (int rig_model);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_open (IntPtr rig);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_close (IntPtr rig);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_cleanup (IntPtr rig);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ant (IntPtr rig, int vfo, int ant);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ant (IntPtr rig, int vfo, out int ant);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_get_level (IntPtr rig, uint level);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_set_level (IntPtr rig, uint level);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_get_parm (IntPtr rig, uint parm);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_set_parm (IntPtr rig, uint parm);

		[DllImport (HamLib.dllName, EntryPoint = "rig_set_conf", CharSet = CharSet.Ansi)]
		private static extern RigError rig_set_conf (IntPtr rig, int token, string confParam);

		[DllImport (HamLib.dllName, EntryPoint = "rig_get_conf")]
		private static extern RigError rig_get_conf (IntPtr rig, int token, IntPtr str);

		private static RigError rig_get_conf (IntPtr rig, int token, out string conf)
		{
			IntPtr ptr = Marshal.AllocHGlobal (255);
			var ret = rig_get_conf (rig, token, ptr);
			conf = Marshal.PtrToStringAnsi (ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_powerstat (IntPtr rig, PowerState status);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_powerstat (IntPtr rig, out PowerState status);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_reset (IntPtr rig, RigReset reset);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ext_level (IntPtr rig, int vfo, int token, Value val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ext_level (IntPtr rig, int vfo, int token, out Value val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ext_parm (IntPtr rig, int vfo, int token, Value val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ext_parm (IntPtr rig, int vfo, int token, out Value val);

		[DllImport (HamLib.dllName)]
		internal static extern RigError rig_ext_level_foreach (IntPtr rig, ConfParamsCallback cb, IntPtr data);

		[DllImport (HamLib.dllName)]
		internal static extern RigError rig_ext_parm_foreach (IntPtr rig, ConfParamsCallback cb, IntPtr data);

		//		extern HAMLIB_EXPORT(const struct confparams*) rig_ext_lookup HAMLIB_PARAMS((RIG *rig, const char *name));
		//		extern HAMLIB_EXPORT(const struct confparams *)  rig_ext_lookup_tok HAMLIB_PARAMS((RIG *rig, token_t token));
		//		extern HAMLIB_EXPORT(token_t) rig_ext_token_lookup HAMLIB_PARAMS((RIG *rig, const char *name));
		//
		//
		//		extern HAMLIB_EXPORT(int) rig_token_foreach HAMLIB_PARAMS((RIG *rig, int (*cfunc)(const struct confparams *, rig_ptr_t), rig_ptr_t data));

		[DllImport (HamLib.dllName)]
		private static extern IntPtr rig_confparam_lookup (IntPtr rig, string name);

		private static IConfigurationParameter confparam_marshal(IntPtr configParamPtr)
		{
			IConfigurationParameter confParam = null;

			// if the platform is 64-bit, but not windows
			if (!HamLib.isWindows && HamLib.bitsize64) {
				confParam = Marshal.PtrToStructure<ConfigurationParameter64> (configParamPtr);
			} else {
				confParam = Marshal.PtrToStructure<ConfigurationParameter32> (configParamPtr);
			}

			return confParam;
		}

		[DllImport (HamLib.dllName, EntryPoint = "rig_token_lookup", CharSet = CharSet.Ansi)]
		private static extern int rig_token_lookup (IntPtr rig, string name);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ptt (IntPtr rig, int vfo, PttMode ptt);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ptt (IntPtr rig, int vfo, out PttMode ptt);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_dcd (IntPtr rig, int vfo, out DcdState dcd);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_freq (IntPtr rig, int vfo, double freq);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_freq (IntPtr rig, int vfo, out double freq);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_mode (IntPtr rig, int vfo, RigMode mode, long width);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_mode (IntPtr rig, int vfo, out RigMode mode, out long width);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_vfo (IntPtr rig, int vfo);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_vfo (IntPtr rig, out int vfo);

		[DllImport (HamLib.dllName, EntryPoint = "rigerror")]
		private static extern IntPtr rigerror (int errnum);

		private static string ErrorString (int error)
		{
			return Marshal.PtrToStringAnsi (rigerror (error));
		}

		private static string ErrorString (RigError error)
		{
			return ErrorString ((int)error);
		}

		[DllImport (HamLib.dllName)]
		private static extern IntPtr rig_get_info (IntPtr rig);

		private static string RigGetInfo (IntPtr rig)
		{
			return Marshal.PtrToStringAnsi (rig_get_info (rig));
		}

		[DllImport (HamLib.dllName)]
		private static extern int rig_passband_normal (IntPtr rig, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern int rig_passband_narrow (IntPtr rig, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern int rig_passband_wide (IntPtr rig, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_rptr_shift (IntPtr rig, int vfo, RepeaterShift rptr_shift);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_rptr_shift (IntPtr rig, int vfo, out RepeaterShift rptr_shift);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_rptr_offs (IntPtr rig, int vfo, int rptr_offs);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_rptr_offs (IntPtr rig, int vfo, out int rptr_offs);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ctcss_tone (IntPtr rig, int vfo, uint tone);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ctcss_tone (IntPtr rig, int vfo, out uint tone);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_dcs_code (IntPtr rig, int vfo, uint code);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_dcs_code (IntPtr rig, int vfo, out uint code);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ctcss_sql (IntPtr rig, int vfo, uint tone);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ctcss_sql (IntPtr rig, int vfo, out uint tone);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_dcs_sql (IntPtr rig, int vfo, uint code);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_dcs_sql (IntPtr rig, int vfo, out uint code);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_split_freq (IntPtr rig, int vfo, double tx_freq);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_split_freq (IntPtr rig, int vfo, out double tx_freq);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_split_mode (IntPtr rig, int vfo, RigMode tx_mode, int tx_width);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_split_mode (IntPtr rig, int vfo, out RigMode tx_mode, out int tx_width);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_split_vfo (IntPtr rig, int rx_vfo, RigSplit split, int tx_vfo);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_split_vfo (IntPtr rig, int rx_vfo, out RigSplit split, out int tx_vfo);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_rit (IntPtr rig, int vfo, int rit);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_rit (IntPtr rig, int vfo, out int rit);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_xit (IntPtr rig, int vfo, int xit);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_xit (IntPtr rig, int vfo, out int xit);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_ts (IntPtr rig, int vfo, int ts);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_ts (IntPtr rig, int vfo, out int ts);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_power2mW (IntPtr rig, out uint mwpower, float power, double freq, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_mW2power (IntPtr rig, out float power, uint mwpower, double freq, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern int rig_get_resolution (IntPtr rig, RigMode mode);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_level (IntPtr rig, int vfo, uint level, Value val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_level (IntPtr rig, int vfo, uint level, out Value val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_level (IntPtr rig, int vfo, uint level, int val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_level (IntPtr rig, int vfo, uint level, out int val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_level (IntPtr rig, int vfo, uint level, float val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_level (IntPtr rig, int vfo, uint level, out float val);

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern RigError rig_set_level (IntPtr rig, int vfo, uint level, string val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_level (IntPtr rig, int vfo, uint level, IntPtr val);

		private static RigError rig_get_level (IntPtr rig, int vfo, uint level, out string val)
		{
			IntPtr ptr = Marshal.AllocHGlobal (255);
			var ret = rig_get_level (rig, vfo, level, ptr);
			val = Marshal.PtrToStringAnsi (ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_parm (IntPtr rig, uint parm, int val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_parm (IntPtr rig, uint parm, out int val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_parm (IntPtr rig, uint parm, float val);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_parm (IntPtr rig, uint parm, out float val);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_get_func (IntPtr rig, uint func);

		[DllImport (HamLib.dllName)]
		private static extern uint rig_has_set_func (IntPtr rig, uint func);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_func (IntPtr rig, int vfo, uint func, int status);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_func (IntPtr rig, int vfo, uint func, out int status);

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern RigError rig_send_dtmf (IntPtr rig, int vfo, string digits);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_recv_dtmf (IntPtr rig, int vfo, IntPtr digits, ref int length);

		private static RigError rig_recv_dtmf (IntPtr rig, int vfo, out string val)
		{
			int length = 255;
			IntPtr ptr = Marshal.AllocHGlobal (255);
			var ret = rig_recv_dtmf (rig, vfo, ptr, ref length);
			val = Marshal.PtrToStringAnsi (ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern RigError rig_send_morse (IntPtr rig, int vfo, string msg);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_bank (IntPtr rig, int vfo, int bank);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_mem (IntPtr rig, int vfo, int ch);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_mem (IntPtr rig, int vfo, out int ch);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_vfo_op (IntPtr rig, int vfo, RigVfoOperation op);

		[DllImport (HamLib.dllName)]
		private static extern RigVfoOperation rig_has_vfo_op (IntPtr rig, RigVfoOperation op);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_scan (IntPtr rig, int vfo, RigScanOperation scan, int ch);

		[DllImport (HamLib.dllName)]
		private static extern RigScanOperation rig_has_scan (IntPtr rig, RigScanOperation scan);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_trn (IntPtr rig, int trn);

		[DllImport (HamLib.dllName)]
		private static extern RigError rig_get_trn (IntPtr rig, out int trn);


		[DllImport (HamLib.dllName)]
		internal static extern int rig_setting2idx (uint s);

		internal static uint rig_idx2setting (int i)
		{
			return 1U << (i);
		}

		// Misc calls
		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern IntPtr rig_strrmode (RigMode mode);

		public static string ModeToString (RigMode mode)
		{
			return Marshal.PtrToStringAnsi (rig_strrmode (mode));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern IntPtr rig_strvfo (uint vfo);

		public static string VfoToString (uint vfo)
		{
			return Marshal.PtrToStringAnsi (rig_strvfo (vfo));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern IntPtr rig_strfunc (uint func);

		public static string FuncToString (uint func)
		{
			return Marshal.PtrToStringAnsi (rig_strfunc (func));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		private static extern IntPtr rig_strlevel (uint level);

		public static string LevelToString (uint level)
		{
			return Marshal.PtrToStringAnsi (rig_strlevel (level));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strparm (uint parmt);

		public static string ParmToString (uint parmt)
		{
			return Marshal.PtrToStringAnsi (rig_strparm (parmt));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strptrshift (RepeaterShift shift);

		public static string RepeaterShiftToString (RepeaterShift shift)
		{
			return Marshal.PtrToStringAnsi (rig_strptrshift (shift));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strvfop (RigVfoOperation op);

		public static string VfoOperationToString (RigVfoOperation op)
		{
			return Marshal.PtrToStringAnsi (rig_strvfop (op));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strscan (RigScanOperation scan);

		public static string ScanOperationToString (RigScanOperation scan)
		{
			return Marshal.PtrToStringAnsi (rig_strscan (scan));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strstatus (RigBackendStatus status);

		public static string BackendStatusToString (RigBackendStatus status)
		{
			return Marshal.PtrToStringAnsi (rig_strstatus (status));
		}

		[DllImport (HamLib.dllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr rig_strmtype (RigMemoryChannel mtype);

		public static string MemoryChannelToString (RigMemoryChannel mtype)
		{
			return Marshal.PtrToStringAnsi (rig_strmtype (mtype));
		}

		// callbacks
		[DllImport (HamLib.dllName)]
		private static extern RigError rig_set_freq_callback (IntPtr rig, FreqCallback cb, IntPtr ptr);

	}
}
