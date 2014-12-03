/*
 * Copyright (c) 2014 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.vnext
 * 
 */

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Piranha.EntityFramework
{
	/// <summary>
	/// Internal utility class for Db context.
	/// </summary>
	internal static class DbUtils
	{
		/// <summary>
		/// Processes the loaded entities before returning them.
		/// </summary>
		public static void OnLoad(DbContext context, ObjectMaterializedEventArgs e) {
			if (e.Entity is Models.Model) {
				((Models.Model)e.Entity).OnLoad();
			}
		}

		/// <summary>
		/// Processes all entity changes before saving them to the database.
		/// </summary>
		public static void OnSave(DbContext context) {
			foreach (var entry in context.ChangeTracker.Entries()) {
				// Ensure id
				if (entry.Entity is Data.IModel) {
					var model = (Data.IModel)entry.Entity;

					if (entry.State == EntityState.Added) {
						if (model.Id == Guid.Empty)
							model.Id = Guid.NewGuid();
					}
				}

				// Track changes
				if (entry.Entity is Data.IChanges) {
					var model = (Data.IChanges)entry.Entity;
					var now = DateTime.Now;

					// Set updated date
					if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
						model.Updated = now;

					// Set created date for new models
					if (entry.State == EntityState.Added)
						model.Created = now;
				}

				// Call events
				if (entry.Entity is Models.Model) {
					if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
						((Models.Model)entry.Entity).OnSave();
					else if (entry.State == EntityState.Deleted)
						((Models.Model)entry.Entity).OnDelete();
				}

				// Handle state lists
				if (entry.Entity is Models.PageType) {
					var type = (Models.PageType)entry.Entity;

					if (type.Properties.GetRemoved().Count > 0)
						context.Set<Models.PageTypeProperty>().RemoveRange(type.Properties.GetRemoved());
					if (type.Regions.GetRemoved().Count > 0)
						context.Set<Models.PageTypeRegion>().RemoveRange(type.Regions.GetRemoved());
				}
			}
		}
	}
}
