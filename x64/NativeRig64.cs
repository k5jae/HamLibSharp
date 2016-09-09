//
//  NativeRig64.cs
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

namespace HamLibSharp.x64
{
	[StructLayout (LayoutKind.Sequential)]
	internal class NativeRig64 : INativeRig
	{
		/// <summary>
		/// Pointer to rig capabilities (read only)
		/// </summary>
		public IntPtr caps;
		/// <summary>
		/// Rig state
		/// </summary>
		[MarshalAs (UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
		public RigStateNative64 state;
		/// <summary>
		/// Registered event callbacks
		/// </summary>
		public IntPtr callbacks;

		public IntPtr Caps { get { return caps; } }
		public IRigStateNative State { get { return state; } }
		public IntPtr Callbacks { get { return callbacks; } }
	};
}

