//
//  CastedList.cs
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
using System.Collections;
using System.Collections.Generic;

namespace HamLibSharp
{
	// Note this class comes from: http://stackoverflow.com/a/30662440
	public class CastedList<TTo, TFrom> : IList<TTo>
	{
		public IList<TFrom> BaseList;

		public CastedList(IList<TFrom> baseList)
		{
			BaseList = baseList;
		}

		// IEnumerable
		IEnumerator IEnumerable.GetEnumerator() { return BaseList.GetEnumerator(); }

		// IEnumerable<>
		public IEnumerator<TTo> GetEnumerator() { return new CastedEnumerator<TTo, TFrom>(BaseList.GetEnumerator()); }

		// ICollection
		public int Count { get { return BaseList.Count; } }
		public bool IsReadOnly { get { return BaseList.IsReadOnly; } }
		public void Add(TTo item) { BaseList.Add((TFrom)(object)item); }
		public void Clear() { BaseList.Clear(); }
		public bool Contains(TTo item) { return BaseList.Contains((TFrom)(object)item); }
		public void CopyTo(TTo[] array, int arrayIndex) { BaseList.CopyTo((TFrom[])(object)array, arrayIndex); }
		public bool Remove(TTo item) { return BaseList.Remove((TFrom)(object)item); }

		// IList
		public TTo this[int index]
		{
			get { return (TTo)(object)BaseList[index]; }
			set { BaseList[index] = (TFrom)(object)value; }
		}

		public int IndexOf(TTo item) { return BaseList.IndexOf((TFrom)(object)item); }
		public void Insert(int index, TTo item) { BaseList.Insert(index, (TFrom)(object)item); }
		public void RemoveAt(int index) { BaseList.RemoveAt(index); }
	}

	public class CastedEnumerator<TTo, TFrom> : IEnumerator<TTo>
	{
		public IEnumerator<TFrom> BaseEnumerator;

		public CastedEnumerator(IEnumerator<TFrom> baseEnumerator)
		{
			BaseEnumerator = baseEnumerator;
		}

		// IDisposable
		public void Dispose() { BaseEnumerator.Dispose(); }

		// IEnumerator
		object IEnumerator.Current { get { return BaseEnumerator.Current; } }
		public bool MoveNext() { return BaseEnumerator.MoveNext(); }
		public void Reset() { BaseEnumerator.Reset(); }

		// IEnumerator<>
		public TTo Current { get { return (TTo)(object)BaseEnumerator.Current; } }
	}

	public static class ListExtensions
	{
		public static IList<TTo> CastList<TFrom, TTo>(this IList<TFrom> list)
		{
			return new CastedList<TTo, TFrom>(list);
		}
	}
}

