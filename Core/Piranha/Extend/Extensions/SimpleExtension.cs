/*
 * Piranha CMS
 * Copyright (c) 2014, Håkan Edling, All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;

namespace Piranha.Extend.Extensions
{
	/// <summary>
	/// Base class for creating a simple extension.
	/// </summary>
	/// <typeparam name="T">The value type</typeparam>
	public abstract class SimpleExtension<T> : IExtension
	{
		#region Members
		/// <summary>
		/// The private value formatter.
		/// </summary>
		private readonly Func<T, object> FormatValue;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the value.
		/// </summary>
		public T Value { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="formatter">The value formatter</param>
		public SimpleExtension(Func<T, object> formatter) {
			FormatValue = formatter;
		}

		/// <summary>
		/// Transforms the extensions value for the client models.
		/// </summary>
		/// <returns>The transformed value</returns>
		public object GetValue() {
			return FormatValue(Value);
		}
	}
}
