//
//  HamLib.cs
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
using HamLibSharp.Utils;
using HamLibSharp.x86;
using HamLibSharp.x64;

namespace HamLibSharp
{
	internal enum HamLibVersion
	{
		Unknown = 0,
		V2 = 2,
		V301 = 3,
		Current,
	}

	public static class HamLib
	{
		internal readonly static bool bitsize64;
		internal readonly static bool isWindows;
		internal readonly static HamLibVersion hamLibVersion;

		public static bool Initialized { get; private set; }

		public readonly static SortedDictionary<string, RigCaps> Rigs = new SortedDictionary<string, RigCaps> ();

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int RigListCallback (IntPtr rig_caps, IntPtr rig_ptr);

		internal const string dllName = "libhamlib-2.dll";

		static HamLib ()
		{
			// determine platform and bit size...
			if (System.Environment.Is64BitProcess) {
				bitsize64 = true;
			}

			if (System.Environment.OSVersion.Platform != PlatformID.MacOSX && System.Environment.OSVersion.Platform != PlatformID.Unix) {
				isWindows = true;
			}

			// take care of 32/64 bit native windows dll
			Library.LoadLibrary (dllName);

			hamLibVersion = PreInitLibrary ();
		}

		static HamLibVersion PreInitLibrary ()
		{
			INativeRig initRig = null;
			IRigCapsNative initCaps = null;
			HamLibVersion version;

			SetDebugLevel (RigDebugLevel.None);

			// test what version of hamlib is on our system use the Dummy Rig
			var theRig = Rig.rig_init (1);

			if (!isWindows && bitsize64) {
				initRig = Marshal.PtrToStructure<NativeRig64> (theRig);
				initCaps = Marshal.PtrToStructure<RigCapsNative64> (initRig.Caps);
				if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
					version = HamLibVersion.Current;
				} else {
					initCaps = Marshal.PtrToStructure<RigCapsNative64v301> (initRig.Caps);
					if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
						version = HamLibVersion.V301;
					} else {
						initCaps = Marshal.PtrToStructure<RigCapsNative64v2> (initRig.Caps);
						if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
							version = HamLibVersion.V2;
						} else {
							version = HamLibVersion.Unknown;
						}
					}
				}
			} else {
				initRig = Marshal.PtrToStructure<NativeRig32> (theRig);

				initCaps = Marshal.PtrToStructure<RigCapsNative32> (initRig.Caps);
				if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
					version = HamLibVersion.Current;
				} else {
					initCaps = Marshal.PtrToStructure<RigCapsNative32v301> (initRig.Caps);
					if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
						version = HamLibVersion.V301;
					} else {
						initCaps = Marshal.PtrToStructure<RigCapsNative32v2> (initRig.Caps);
						if (initCaps.Priv == IntPtr.Zero && initCaps.Decode_event == IntPtr.Zero) {
							version = HamLibVersion.V2;
						} else {
							version = HamLibVersion.Unknown;
						}
					}
				}
			}

			return version;
		}

		public static void Initialize ()
		{
			//SetDebugLevel (RigDebugLevel.None);

			rig_load_all_backends ();

			rig_list_foreach ((rig_caps, rig_ptr) => {
				if (rig_caps != IntPtr.Zero) {
					var caps = MarshalRigCaps (rig_caps);
					AddRig (new RigCaps (caps));
				}
				return 1;
			}, IntPtr.Zero);

			//SetDebugLevel (RigDebugLevel.Verbose);

			Initialized = true;
		}

		internal static IRigCapsNative MarshalRigCaps (IntPtr rig_caps)
		{
			IRigCapsNative caps = null;

			switch (hamLibVersion) {
			case HamLibVersion.Current:
				// if the platform is 64-bit, but not windows
				if (!isWindows && bitsize64) {
					caps = Marshal.PtrToStructure<RigCapsNative64> (rig_caps);
				} else {
					caps = Marshal.PtrToStructure<RigCapsNative32> (rig_caps);
				}
				break;
			case HamLibVersion.V301:
				if (!isWindows && bitsize64) {
					caps = Marshal.PtrToStructure<RigCapsNative64v301> (rig_caps);
				} else {
					caps = Marshal.PtrToStructure<RigCapsNative32v301> (rig_caps);
				}
				break;
			case HamLibVersion.V2:
				if (!isWindows && bitsize64) {
					caps = Marshal.PtrToStructure<RigCapsNative64v2> (rig_caps);
				} else {
					caps = Marshal.PtrToStructure<RigCapsNative32v2> (rig_caps);
				}
				break;
			default:
				throw new RigException ("Unknown or Incompatible HamLib library found");
			}

			return caps;
		}

		private static void AddRig (RigCaps caps)
		{
			int index = 2;
			string modelName = string.Format ("{0}", caps.ModelName);

			while (Rigs.ContainsKey (modelName)) {
				modelName = string.Format ("{0}_{1}", caps.ModelName, index);
				index++;
			} 

			Rigs.Add (modelName, caps);
		}

		[DllImport (HamLib.dllName, EntryPoint = "rig_set_debug")]
		public static extern void SetDebugLevel (RigDebugLevel debugLevel);

		[DllImport (HamLib.dllName, EntryPoint = "rig_load_all_backends")]
		private static extern int rig_load_all_backends ();

		[DllImport (HamLib.dllName, EntryPoint = "rig_list_foreach")]
		private static extern int rig_list_foreach (RigListCallback call, IntPtr ptr);

		//Note: Function introduced in version 3.1, earlier versions do not have it
		[DllImport (HamLib.dllName)]
		private static extern IntPtr rig_copyright ();

		public static string NativeCopyright {
			get {
				try {

					return Marshal.PtrToStringAnsi (rig_copyright ());
				} catch (EntryPointNotFoundException) {
					return "Unknown";
				}
			}
		}

		//Note: Function introduced in version 3.1, earlier versions do not have it
		[DllImport (HamLib.dllName)]
		private static extern IntPtr rig_license ();

		public static string NativeLicense {
			get {
				try {

					return Marshal.PtrToStringAnsi (rig_license ());
				} catch (EntryPointNotFoundException) {
					return "Unknown";
				}
			}
		}

		//Note: Function introduced in version 3.1, earlier versions do not have it
		[DllImport (HamLib.dllName)]
		private static extern IntPtr rig_version ();

		public static string NativeVersion {
			get {
				try {
					switch (hamLibVersion) {
					case HamLibVersion.Current:
						var ver = Marshal.PtrToStringAnsi (rig_version ());
						return ver.Replace ("Hamlib", string.Empty).Trim ();
					case HamLibVersion.V301:
						return "3.0.1";
					case HamLibVersion.V2:
						return "1.2 or earlier";
					default:
						return "Unknown";
					}
				} catch (EntryPointNotFoundException) {
					//Console.WriteLine (e);
					// Entry point not found, so it has to be 3.0.1 or earlier
					return "3.0.1 or earlier";
					//				} catch (DllNotFoundException e) {
					//					return "Native HamLib Not Found: " + e.Message;
				}
			}
		}

		public static string ManagedCopyright {
			get {
				return Library.Copyright;
			}
		}

		public static string ManagedLicense {
			get {
				return Library.License;
			}
		}

		public static Version ManagedVersion {
			get {
				return Library.Version;
			}
		}
	}
}
