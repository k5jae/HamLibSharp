//
//  ChannelCapability64.cs
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
	internal struct ChannelCapability64: IChannelCapability
	{
		private short raw0;

		// Bank number
		public bool BankNumber { get { return ((byte)((raw0 >> 0) & 0x01)) != 0; } }
		// VFO
		public bool Vfo{ get { return ((byte)((raw0 >> 1) & 0x01)) != 0; } }
		// Selected antenna
		public bool  Antenna{ get { return ((byte)((raw0 >> 2) & 0x01)) != 0; } }
		// Receive frequency
		public bool  RXFrequency{ get { return ((byte)((raw0 >> 3) & 0x01)) != 0; } }
		// Receive mode
		public bool  RXMode{ get { return ((byte)((raw0 >> 4) & 0x01)) != 0; } }
		// Receive passband width associated with mode
		public bool  RXWidth{ get { return ((byte)((raw0 >> 5) & 0x01)) != 0; } }
		// Transmit frequency
		public bool  TXFrequency{ get { return ((byte)((raw0 >> 6) & 0x01)) != 0; } }
		// Transmit mode
		public bool  TXMode{ get { return ((byte)((raw0 >> 7) & 0x01)) != 0; } }
		// Transmit passband width associated with mode

		public bool  TXWidth{ get { return ((byte)((raw0 >> 8) & 0x01)) != 0; } }
		// Split mode
		public bool  Split{ get { return ((byte)((raw0 >> 9) & 0x01)) != 0; } }
		// Split transmit VFO
		public bool  TXVfo{ get { return ((byte)((raw0 >> 10) & 0x01)) != 0; } }
		// Repeater shift
		public bool  RepeaterShift{ get { return ((byte)((raw0 >> 11) & 0x01)) != 0; } }
		// Repeater offset
		public bool  RepeaterOffset{ get { return ((byte)((raw0 >> 12) & 0x01)) != 0; } }
		// Tuning step
		public bool  TuningStep{ get { return ((byte)((raw0 >> 13) & 0x01)) != 0; } }
		// RIT
		public bool  Rit{ get { return ((byte)((raw0 >> 14) & 0x01)) != 0; } }
		// XIT
		public bool  Xit{ get { return ((byte)((raw0 >> 15) & 0x01)) != 0; } }

		// Function status
		ulong funcs;
		// Level values
		ulong levels;

		public uint Functions { get { return (uint)funcs; } }

		public uint Levels { get { return (uint)levels; } }


		private short raw2;

		// CTCSS tone
		public bool  CtcssTone { get { return ((byte)((raw2 >> 0) & 0x01)) != 0; } }
		// CTCSS squelch tone
		public bool  CtcssSquelch{ get { return ((byte)((raw2 >> 1) & 0x01)) != 0; } }
		// DCS code
		public bool  DcsCode{ get { return ((byte)((raw2 >> 2) & 0x01)) != 0; } }
		// DCS squelch code
		public bool  DcsSquelch{ get { return ((byte)((raw2 >> 3) & 0x01)) != 0; } }
		// Scan group
		public bool  ScanGroup{ get { return ((byte)((raw2 >> 4) & 0x01)) != 0; } }
		// Channel flags
		public bool  ChannelFlags{ get { return ((byte)((raw2 >> 5) & 0x01)) != 0; } }
		// Name
		public bool  ChannelName{ get { return ((byte)((raw2 >> 6) & 0x01)) != 0; } }
		// Extension level value list
		public bool  ExtensionLevels{ get { return ((byte)((raw2 >> 7) & 0x01)) != 0; } }
	}
}

