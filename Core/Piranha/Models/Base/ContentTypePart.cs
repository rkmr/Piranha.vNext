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

namespace Piranha.Models.Base
{
	/// <summary>
	/// Base class for defining content type parts.
	/// </summary>
	public abstract class ContentTypePart : Model, Data.IModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the id of the content type.
		/// </summary>
		public Guid TypeId { get; set; }

		/// <summary>
		/// Get/sets the internal id.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the CLR type of the part value.
		/// </summary>
		public string CLRType { get; set; }

		/// <summary>
		/// Gets/sets if this part should support multiple values.
		/// </summary>
		public bool IsCollection { get; set; }

		/// <summary>
		/// Gets/sets the current sort order.
		/// </summary>
		public int Order { get; set; }
		#endregion
	}
}
