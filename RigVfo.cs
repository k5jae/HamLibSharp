//
//  RigVfo.cs
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
	public static class RigVfo
	{
		public const int None = 0;
		public const int TxFlag = (1 << 30);

		///currVFO -- current "tunable channel"/VFO
		public const int Current = (1 << 29);

		///MEM -- means Memory mode, to be used with set_vfo
		public const int Memory = (1 << 28);

		///VFO -- means (last or any)VFO mode, with set_vfo
		public const int AnyVfo = (1 << 27);

		///TX -- alias for split tx or uplink, of VFO_CURR 
		public static int SplitTxUplink (int v)
		{
			return ((v) | TxFlag);
		}

		public static int Tx { get { return SplitTxUplink (Current); } }

		///RX -- alias for split rx or downlink
		public const int SplitRxDownlink	= Current;

		///Main -- alias for MAIN
		public const int Main = (1 << 26);

		///Sub -- alias for SUB
		public const int Sub = (1 << 25);

		public static int VfoNum (int n)
		{
			return (1 << (n));
		}

		///VFOA -- VFO A
		public static int VfoA { get { return VfoNum (0); } }

		///VFOB -- VFO B
		public static int VfoB { get { return VfoNum (1); } }

		///VFOC -- VFO C
		public static int VfoC { get { return VfoNum (2); } }
	}
}
