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

namespace Piranha.Extend.Builder
{
	/// <summary>
	/// Blass for defining page types through code.
	/// </summary>
	public abstract class PageType
	{
		#region Properties
		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the optional route that should handle
		/// posts of this type.
		/// </summary>
		public string Route { get; set; }

		/// <summary>
		/// Gets/sets the optional view that should render
		/// posts of this type.
		/// </summary>
		public string View { get; set; }

		/// <summary>
		/// Gets/sets the available regions.
		/// </summary>
		private IList<ContentPart> Regions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageType() {
			Regions = new List<ContentPart>();
		}

		/// <summary>
		/// Adds a new region to the page type.
		/// </summary>
		/// <typeparam name="T">The region type</typeparam>
		/// <param name="name">The display name</param>
		/// <param name="internalId">The internal id</param>
		/// <param name="isCollection">If this region allows multiple values</param>
		public void AddRegion<T>(string name, string internalId, bool isCollection = false) {
			Regions.Add(new ContentPart() { 
				Name = name,
				InternalId = internalId,
				IsCollection = isCollection,
				CLRType = typeof(T).FullName 
			});
		}

		/// <summary>
		/// Builds the page type in the data store.
		/// </summary>
		/// <param name="api">The current api</param>
		internal void Build(Api api) {
			bool create = false;

			// Ensure slug
			if (String.IsNullOrWhiteSpace(Slug))
				Slug = Utils.GenerateSlug(Name);

			// Create or update page type
			var type = api.PageTypes.GetSingle(t => t.Slug == Slug);
			if (type == null) {
				type = new Models.PageType() {
					Id = Guid.NewGuid(),
					Slug = Slug,
				};
				create = true;
			}

			// Map page type
			type.Name = Name;
			type.Description = Description;
			type.Route = Route;
			type.View = View;

			// Get the removed regions
			var removed = new List<Models.PageTypeRegion>();
			foreach (var region in type.Regions)
				if (Regions.Where(r => r.InternalId == region.InternalId).SingleOrDefault() == null)
					removed.Add(region);

			// Map existing regions
			for (var n = 0; n < Regions.Count; n++) {
				var reg = type.Regions.Where(r => r.InternalId == Regions[n].InternalId).SingleOrDefault();

				if (reg == null) {
					reg = new Models.PageTypeRegion() {
						TypeId = type.Id,
						InternalId = Regions[n].InternalId
					};
					type.Regions.Add(reg);
				}
				reg.Name = Regions[n].Name;
				reg.IsCollection = Regions[n].IsCollection;
				reg.CLRType = Regions[n].CLRType;
				reg.Order = n + 1;
			}

			// Delete removed regions
			foreach (var region in removed)
				type.Regions.Remove(region);

			// Add the page type if its new
			if (create)
				api.PageTypes.Add(type);
		}
	}
}
