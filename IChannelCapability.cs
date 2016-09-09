//
//  IChannelCapability.cs
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

namespace HamLibSharp
{
	// Channel capability definition
	//
	// Definition of the attributes that can be stored/retrieved in/from memory
	public interface IChannelCapability
	{
		// Bank number
		bool BankNumber { get; }
		// VFO
		bool Vfo { get; }
		// Selected antenna
		bool  Antenna { get; }
		// Receive frequency
		bool  RXFrequency { get; }
		// Receive mode
		bool  RXMode { get; }
		// Receive passband width associated with mode
		bool  RXWidth { get; }
		// Transmit frequency
		bool  TXFrequency { get; }
		// Transmit mode
		bool  TXMode { get; }
		// Transmit passband width associated with mode

		bool  TXWidth { get; }
		// Split mode
		bool  Split { get; }
		// Split transmit VFO
		bool  TXVfo { get; }
		// Repeater shift
		bool  RepeaterShift { get; }
		// Repeater offset
		bool  RepeaterOffset { get; }
		// Tuning step
		bool  TuningStep { get; }
		// RIT
		bool  Rit { get; }
		// XIT
		bool  Xit { get; }

		uint Functions  { get; }

		uint Levels  { get; }

		// CTCSS tone
		bool  CtcssTone { get; }
		// CTCSS squelch tone
		bool  CtcssSquelch { get; }
		// DCS code
		bool  DcsCode { get; }
		// DCS squelch code
		bool  DcsSquelch { get; }
		// Scan group
		bool  ScanGroup { get; }
		// Channel flags
		bool  ChannelFlags { get; }
		// Name
		bool  ChannelName { get; }
		// Extension level value list
		bool  ExtensionLevels { get; }
	}

}

