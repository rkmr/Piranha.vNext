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
using System.Reflection;

namespace Piranha.Extend
{
	/// <summary>
	/// The extension manager is responsible for managing the
	/// imported extensions.
	/// </summary>
	public sealed class ExtensionManager
	{
		#region Inner classes
		/// <summary>
		/// An imported extension.
		/// </summary>
		public sealed class Import
		{
			/// <summary>
			/// Gets the display name.
			/// </summary>
			public string Name { get; internal set; }

			/// <summary>
			/// Gets the CLR value type.
			/// </summary>
			public Type ValueType { get; internal set; }

			/// <summary>
			/// Gets the extension type.
			/// </summary>
			public ExtensionType Type { get; internal set; }

			/// <summary>
			/// Internal constructor.
			/// </summary>
			internal Import() { }
		}		
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the available extensions.
		/// </summary>
		private IList<Import> Extensions { get; set; }

		/// <summary>
		/// Gets the currently imported modules.
		/// </summary>
		public IList<IModule> Modules { get; private set; }

		/// <summary>
		/// Gets the currently imported page types.
		/// </summary>
		public IList<Builder.PageType> PageTypes { get; private set; }

		/// <summary>
		/// Gets the currently imported post types.
		/// </summary>
		public IList<Builder.PostType> PostTypes { get; private set; }

		/// <summary>
		/// Gets the available properties.
		/// </summary>
		public IList<Import> Properties {
			get { return Extensions.Where(i => i.Type.HasFlag(ExtensionType.Property)).OrderBy(i => i.Name).ToList(); }
		}

		/// <summary>
		/// Gets the available regions.
		/// </summary>
		public IList<Import> Regions {
			get { return Extensions.Where(i => i.Type.HasFlag(ExtensionType.Region)).OrderBy(i => i.Name).ToList(); }
		}
		#endregion

		/// <summary>
		/// Default internal constructor.
		/// </summary>
		internal ExtensionManager() {
			Extensions = new List<Import>();
			Modules = new List<IModule>();
			PageTypes = new List<Builder.PageType>();
			PostTypes = new List<Builder.PostType>();

			// Scan all assemblies
			App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: Scanning assemblies");
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach (var info in assembly.DefinedTypes) {
					if (info.IsClass && !info.IsAbstract) {
						if (typeof(IExtension).GetTypeInfo().IsAssignableFrom(info)) {
							//
							// Extensions
							//
							var attr = info.GetCustomAttribute<ExtensionAttribute>();
							if (attr != null) {
								Extensions.Add(new Import() {
									Name = attr.Name,
									Type = attr.Type,
									ValueType = info
								});
							}
						} else if (typeof(IModule).GetTypeInfo().IsAssignableFrom(info)) {
							//
							// Modules
							//
							Modules.Add((IModule)Activator.CreateInstance(info));
						} else if (typeof(Builder.PageType).GetTypeInfo().IsAssignableFrom(info)) {
							//
							// Page types
							//
							var attr = info.GetCustomAttribute<BuilderAttribute>();
							if (attr != null)
								PageTypes.Add((Builder.PageType)Activator.CreateInstance(info));
						} else if (typeof(Builder.PostType).GetTypeInfo().IsAssignableFrom(info)) {
							//
							// Post types
							//
							var attr = info.GetCustomAttribute<BuilderAttribute>();
							if (attr != null)
								PostTypes.Add((Builder.PostType)Activator.CreateInstance(info));
						}
					} 
				}
			}

			if (PageTypes.Count > 0) {
				// Build page types
				App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: Building page types");

				using (var api = new Api()) {
					foreach (var type in PageTypes) {
						App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: > " + type.Name);

						try {
							type.Build(api);
						} catch (Exception ex) {
							App.Logger.Log(Log.LogLevel.ERROR, "ExtensionManager: Error building page type" + type.Name, ex);
						}
					}
					api.SaveChanges();
				}
			}

			if (PostTypes.Count > 0) {
				// Build post types
				App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: Building post types");

				using (var api = new Api()) {
					foreach (var type in PostTypes) {
						App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: > " + type.Name);

						try {
							type.Build(api);
						} catch (Exception ex) {
							App.Logger.Log(Log.LogLevel.ERROR, "ExtensionManager: Error building post type" + type.Name, ex);
						}
					}
					api.SaveChanges();
				}
			}

			// Initialize all modules
			App.Logger.Log(Log.LogLevel.INFO, "ExtensionManager: Initializing modules");
			foreach (var module in Modules)
				module.Init();
		}
	}
}
