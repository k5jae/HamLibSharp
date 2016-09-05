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

namespace HamLibSharp
{
	public static class HamLib
	{
		public static bool Initialized { get; private set; }

		public readonly static SortedDictionary<string, RigCaps> Rigs = new SortedDictionary<string, RigCaps> ();

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int RigListCallback (IntPtr rig_caps, IntPtr rig_ptr);

		internal const string dllName = "libhamlib-2.dll";

		static HamLib ()
		{
			// take care of 32/64 bit native windows dll
			Library.LoadLibrary (dllName);
		}

		public static void Initialize ()
		{
			SetDebugLevel (RigDebugLevel.None);
			var result = rig_load_all_backends ();

			rig_list_foreach ((rig_caps, rig_ptr) => {
				var caps = Marshal.PtrToStructure<RigCapsNative> (rig_caps);
				AddRig (new RigCaps (caps, IntPtr.Zero));
				return 1;
			}, IntPtr.Zero);

			//SetDebugLevel (RigDebugLevel.Verbose);

			Initialized = true;
		}

		private static void AddRig (RigCaps caps)
		{
			int index = 2;
			//string modelName = string.Format("{0} {1} ({2})", caps.ModelName, caps.MfgName, caps.Status);
			string modelName = string.Format ("{0}", caps.ModelName);

			while (Rigs.ContainsKey (modelName)) {
				modelName = string.Format ("{0}_{1}", caps.ModelName, index);
				//modelName = string.Format("{0}_{1} {2} ({3})", caps.ModelName, index, caps.MfgName, caps.Status);
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
				} catch (EntryPointNotFoundException e) {
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
				} catch (EntryPointNotFoundException e) {
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

					var ver = Marshal.PtrToStringAnsi (rig_version ());
					return ver.Replace ("Hamlib", string.Empty).Trim ();
				} catch (EntryPointNotFoundException e) {
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
