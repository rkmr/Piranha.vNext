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

namespace Piranha.Cache
{
	/// <summary>
	/// Handles model caching for the repositories. Can also used by 
	/// modules to register their own model caches in.
	/// </summary>
	public class ModelCache
	{
		#region Members
		/// <summary>
		/// The available cache sets.
		/// </summary>
		private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();

		/// <summary>
		/// The current cache provider.
		/// </summary>
		private readonly ICache provider;

		/// <summary>
		/// The private sitemap cache id.
		/// </summary>
		private const string CACHE_SITEMAP = "_piranha_sitemap_";
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="cache">The current cache provider</param>
		public ModelCache(ICache cache) {
			provider = cache;
		}

		/// <summary>
		/// Registers a new cache set for the given type with the
		/// provided functions for resolving the model.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="getId">How to get the model id</param>
		/// <param name="getKey">How to get the model key</param>
		public void RegisterCache<T>(Func<T, Guid> getId, Func<T, string> getKey) {
			cache[typeof(T)] = new ModelCacheSet<T>(provider, getId, getKey);
		}

		/// <summary>
		/// Gets the cached model with the given id.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="id">The unique internal id</param>
		/// <returns>The model</returns>
		public T GetById<T>(Guid id) {
			return ((ModelCacheSet<T>)cache[typeof(T)]).Get(id);
		}

		/// <summary>
		/// Gets the cached model with the given key
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="key">The unique key</param>
		/// <returns>The model</returns>
		public T GetByKey<T>(string key) {
			return ((ModelCacheSet<T>)cache[typeof(T)]).Get(key);
		}

		/// <summary>
		/// Adds a new model to the cache.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="model">The model</param>
		public void Add<T>(T model) {
			((ModelCacheSet<T>)cache[typeof(T)]).Add(model);
		}

		/// <summary>
		/// Removes the model with the given id from the cache.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="id">The unique id</param>
		public void Remove<T>(Guid id) {
			((ModelCacheSet<T>)cache[typeof(T)]).Remove(id);
		}

		/// <summary>
		/// Gets the current sitemap from the cache.
		/// </summary>
		/// <returns>The sitemap</returns>
		public Client.Models.SiteMap GetSiteMap() {
			return provider.Get<Client.Models.SiteMap>(CACHE_SITEMAP);
		}
 
		/// <summary>
		/// Sets the current sitemap.
		/// </summary>
		/// <param name="sitemap">The sitemap</param>
		public void SetSiteMap(Client.Models.SiteMap sitemap) {
			provider.Set(CACHE_SITEMAP, sitemap);
		}
 
		/// <summary>
		/// Removes the current sitemap from the cache.
		/// </summary>
		public void RemoveSiteMap() {
			provider.Remove(CACHE_SITEMAP);
		}
	}
}
