﻿/*
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

namespace Piranha.Manager.Models.Author
{
	/// <summary>
	/// View model for the author list.
	/// </summary>
	public class ListItem
	{
		#region Properties
		/// <summary>
		/// Gets/sets unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Get/sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the email.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Gets/sets the gravatar url.
		/// </summary>
		public string GravatarUrl { get; set; }

		/// <summary>
		/// Gets/sets when the model was created.
		/// </summary>
		public string Created { get; set; }

		/// <summary>
		/// Gets/sets when the model was last updated.
		/// </summary>
		public string Updated { get; set; }

		/// <summary>
		/// Gets/sets if this item was saved during the last operation.
		/// </summary>
		public bool Saved { get; set; }
		#endregion
	}
}