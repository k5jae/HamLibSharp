//
//  IntPtrExtensions.cs
//
//  Author:
//       Jae Stutzman <jaebird@gmail.com>
//
//  Copyright (c) 2016 Jae Stutzman
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Runtime.InteropServices;

namespace HamLibSharp
{
	public static class IntPtrExtensions
	{
		public static IntPtr Increment (this IntPtr ptr, int cbSize)
		{
			return new IntPtr (ptr.ToInt64 () + cbSize);
		}

		public static IntPtr Increment<T> (this IntPtr ptr)
		{
			return ptr.Increment (Marshal.SizeOf (typeof(T)));
		}

		public static T ElementAt<T> (this IntPtr ptr, int index)
		{
			var offset = Marshal.SizeOf (typeof(T)) * index;
			var offsetPtr = ptr.Increment (offset);
			return (T)Marshal.PtrToStructure (offsetPtr, typeof(T));
		}
	}
}

