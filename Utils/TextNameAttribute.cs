//
//  FieldDescription.cs
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
using System.Reflection;

namespace HamLibSharp.Utils
{
	/// <summary>
	/// Allows us to add descriptions to interop members
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class TextNameAttribute : Attribute
	{
		/// <summary>
		/// The description
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Field description
		/// </summary>
		public TextNameAttribute(string description)
		{
			Description = description;
		}

		/// <summary>
		/// String representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Description;
		}

		public static string GetTextName(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			TextNameAttribute[] attributes = 
				(TextNameAttribute[])fi.GetCustomAttributes(typeof(TextNameAttribute), false);

			if (attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}
	
	}

}

