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

using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Piranha.Manager.Models.PageType
{
	/// <summary>
	/// View model for the page type edit view.
	/// </summary>
	public class EditModel
	{
		#region Inner classes
		public class PagePart
		{
			/// <summary>
			/// Gets/sets the unique id.
			/// </summary>
			public Guid Id { get; set; }

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
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid? Id { get; set; }

		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		[StringLength(128)]
		public string Slug { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		[Required, StringLength(128)]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		[StringLength(255)]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the optional route that should handle
		/// posts of this type.
		/// </summary>
		[StringLength(255)]
		public string Route { get; set; }

		/// <summary>
		/// Gets/sets the optional view that should render
		/// posts of this type.
		/// </summary>
		[StringLength(255)]
		public string View { get; set; }

		/// <summary>
		/// Gets/sets the available properties.
		/// </summary>
		public IList<PagePart> Properties { get; set; }

		/// <summary>
		/// Gets/sets the available regions.
		/// </summary>
		public IList<PagePart> Regions { get; set; }

		/// <summary>
		/// Gets/sets the available property types.
		/// </summary>
		public SelectList PropertyTypes { get; set; }

		/// <summary>
		/// Gets/sets the available region types.
		/// </summary>
		public SelectList RegionTypes { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EditModel() {
			Properties = new List<PagePart>();
			Regions = new List<PagePart>();
			PropertyTypes = new SelectList(App.Extensions.Properties, "ValueType", "Name");
			RegionTypes = new SelectList(App.Extensions.Regions, "ValueType", "Name");
		}

		/// <summary>
		/// Gets the post type model for the given id.
		/// </summary>
		/// <param name="api">The current api</param>
		/// <param name="id">The unique id</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Api api, Guid id) {
			var type = api.PageTypes.GetSingle(where: t => t.Id == id);

			if (type != null)
				return Mapper.Map<Piranha.Models.PageType, EditModel>(type);
			return new EditModel();
		}

		/// <summary>
		/// Saves the current post type.
		/// </summary>
		/// <param name="api">The current api</param>
		public void Save(Api api) {
			var newModel = false;

			var type = api.PageTypes.GetSingle(where: t => t.Id == Id);
			if (type == null) {
				type = new Piranha.Models.PageType() {
					Id = Guid.NewGuid()
				};
				newModel = true;
				Id = type.Id;
			}
			Mapper.Map<EditModel, Piranha.Models.PageType>(this, type);

			MapParts<Piranha.Models.PageTypeRegion>(type.Regions, Regions);
			MapParts<Piranha.Models.PageTypeProperty>(type.Properties, Properties);

			if (newModel)
				api.PageTypes.Add(type);
			api.SaveChanges();

			this.Id = type.Id;
		}

		#region Private methods
		/// <summary>
		/// Maps the given parts collection.
		/// </summary>
		/// <typeparam name="T">The part type</typeparam>
		/// <param name="oldParts">The old parts</param>
		/// <param name="newParts">The new parts</param>
		private void MapParts<T>(IList<T> oldParts, IList<PagePart> newParts) where T : Piranha.Models.Base.ContentTypePart {
			// Get the removed part
			var removed = new List<T>();
			foreach (var part in oldParts)
				if (newParts.Where(p => p.Id == part.Id).SingleOrDefault() == null)
					removed.Add(part);

			// Map existing part
			for (var n = 0; n < newParts.Count; n++) {
				var part = oldParts.Where(r => r.Id == newParts[n].Id).SingleOrDefault();

				if (part == null) {
					part = Activator.CreateInstance<T>();
		
					part.Id = Guid.NewGuid();
					part.TypeId = Id.Value;
					part.InternalId = newParts[n].InternalId;

					oldParts.Add(part);
				}
				part.Name = newParts[n].Name;
				part.IsCollection = newParts[n].IsCollection;
				part.CLRType = newParts[n].CLRType;
				part.Order = n + 1;
			}

			// Delete removed parts
			foreach (var part in removed) {
				oldParts.Remove(part);
			}
		}
		#endregion
	}
}