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
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Models
{
	/// <summary>
	/// Page types are used to define different kinds of pages.
	/// </summary>
	public sealed class PageType : Base.ContentType, Data.IModel, Data.IChanges
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available properties.
		/// </summary>
		public StateList<PageTypeProperty> Properties { get; set; }

		/// <summary>
		/// Gets/sets the available regions.
		/// </summary>
		public StateList<PageTypeRegion> Regions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageType() {
			Properties = new StateList<PageTypeProperty>();
			Regions = new StateList<PageTypeRegion>();
		}

		#region Events
		/// <summary>
		/// Called before the model is saved by the DbContext.
		/// </summary>
		public override void OnSave() {
			// Order regions
			Regions = Regions.OrderBy(r => r.Order).ToStateList();

			// Ensure region id
			foreach (var reg in Regions)
				reg.Id = reg.Id == Guid.Empty ? Guid.NewGuid() : reg.Id;
		}
		#endregion
	}
}