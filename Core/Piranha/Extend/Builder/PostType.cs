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
	/// Blass for defining post types through code.
	/// </summary>
	public abstract class PostType
	{
		#region Inner classes
		/// <summary>
		/// Gets/sets the default information that will be
		/// seeded the first time the post type is created.
		/// </summary>
		public sealed class DefaultInfo
		{
			/// <summary>
			/// Gets/sets the default archive title.
			/// </summary>
			public string ArchiveTitle { get; set; }

			/// <summary>
			/// Gets/sets the default meta keywords.
			/// </summary>
			public string MetKeywords { get; set; }

			/// <summary>
			/// Gets/sets the default meta description.
			/// </summary>
			public string MetaDescription { get; set; }

			/// <summary>
			/// Gets/sets if posts of this type should be included
			/// in the site RSS or not.
			/// </summary>
			public bool IncludeInRss { get; set; }

			/// <summary>
			/// Gets/sets if archives should be enabled for this post type.
			/// </summary>
			public bool EnableArchive { get; set; }
		}
		#endregion

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
		/// Gets/sets the optional route that should handle
		/// comments for this type.
		/// </summary>
		public string CommentRoute { get; set; }

		/// <summary>
		/// Gets/sets the optional route that should handle
		/// the post archive.
		/// </summary>
		public string ArchiveRoute { get; set; }

		/// <summary>
		/// Gets/sets the optional route that should render
		/// the post archive.
		/// </summary>
		public string ArchiveView { get; set; }

		/// <summary>
		/// Gets/sets the default information that will be
		/// seeded when post type is created.
		/// </summary>
		public DefaultInfo Defaults { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostType() {
			Defaults = new DefaultInfo();
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

			// Create or update post type
			var type = api.PostTypes.GetSingle(t => t.Slug == Slug);
			if (type == null) {
				type = new Models.PostType() {
					Id = Guid.NewGuid(),
					Slug = Slug,
				};
				create = true;
			}

			// Map post type
			type.Name = Name;
			type.Description = Description;
			type.Route = Route;
			type.View = View;
			type.CommentRoute = CommentRoute;
			type.ArchiveRoute = ArchiveRoute;
			type.ArchiveView = ArchiveRoute;

			// If we're creating the post type, seed
			// the default data.
			if (create) {
				type.EnableArchive = Defaults.EnableArchive;
				type.IncludeInRss = Defaults.IncludeInRss;
				type.ArchiveTitle = Defaults.ArchiveTitle;
				type.MetaKeywords = Defaults.MetKeywords;
				type.MetaDescription = Defaults.MetaDescription;
			}

			// Add the post type if its new
			if (create)
				api.PostTypes.Add(type);
		}
	}
}
