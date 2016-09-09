//
//  ConfigurationParameter64.cs
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
	/// Configuration parameter structure.
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal class ConfigurationParameter64 : IConfigurationParameter
	{
		public int Token { get { return (int)token; } }

		public string Name { get { return name; } }

		public string Label { get { return label; } }

		public string Tooltip { get { return tooltip; } }

		public string Default { get { return dflt; } }

		public float Min { get { return min; } }

		public float Max { get { return max; } }

		public float Step { get { return step; } }

		/*!< Conf param token ID */
		long token;
		/*!< Param name, no spaces allowed */
		[MarshalAs (UnmanagedType.LPStr)]
		string name;
		/*!< Human readable label */
		[MarshalAs (UnmanagedType.LPStr)]
		string label;
		/*!< Hint on the parameter */
		[MarshalAs (UnmanagedType.LPStr)]
		string tooltip;
		/*!< Default value */
		[MarshalAs (UnmanagedType.LPStr)]
		string dflt;
		/*!< Type of the parameter */
		RigConf type;
		/*!< */
		//[MarshalAs (UnmanagedType.Struct)]
		//paramU u;
		/*!< Minimum value */
		float min;
		/*!< Maximum value */
		float max;
		/*!< Step */
		float step;
		/*!< Numeric type */
	}
}

