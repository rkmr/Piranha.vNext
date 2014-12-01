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
using System.Collections.Generic;
using System.Reflection;
using Piranha.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Piranha.RavenDb
{
	/// <summary>
	/// Store implementation for RavenDb.
	/// </summary>
	public class Store : IStore
	{
		#region Members
		/// <summary>
		/// The document store.
		/// </summary>
		private readonly IDocumentStore store;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Store(string url, string defaultDatabase, bool waitForStaleResults = false) {
			// Create the store
			store = new DocumentStore() { Url = url, DefaultDatabase = defaultDatabase }.Initialize();

			// Add resolver
			store.Conventions.JsonContractResolver = new PiranhaResolver(true);

			// Add listeners
			((DocumentStore)store).RegisterListener(new ConversionListener());
			((DocumentStore)store).RegisterListener(new StoreListener());

			// Should we wait for stale results
			if (waitForStaleResults)
				((DocumentStore)store).RegisterListener(new NoStaleQueriesListener());

			// Initialize
			store.Initialize();
		}

		/// <summary>
		/// Opens a new session on the current store.
		/// </summary>
		/// <returns>The new session</returns>
		public ISession OpenSession() {
			return new Session(store.OpenSession());
		}

		#region Resolvers
		/// <summary>
		/// Removes attributes that shouldn't be stored by RavenDb.
		/// </summary>
		private class PiranhaResolver : DefaultRavenContractResolver
		{
			/// <summary>
			/// Default constructor.
			/// </summary>
			public PiranhaResolver(bool shareCache) : base(shareCache) { }

			/// <summary>
			/// Returns the members that should be serialized for a document of the given type.
			/// </summary>
			/// <param name="type">The document type</param>
			/// <returns>The serializable members</returns>
			protected override List<MemberInfo> GetSerializableMembers(Type type) {
				var members = base.GetSerializableMembers(type);

				if (typeof(Models.Comment).IsAssignableFrom(type)) {
					members.RemoveAll(m => m.Name == "Post");
				} else if (typeof(Models.Page).IsAssignableFrom(type)) {
					members.RemoveAll(m => m.Name == "Author" || m.Name == "Type");
				} else if (typeof(Models.PageTypeRegion).IsAssignableFrom(type)) {
					members.RemoveAll(m => m.Name == "TypeId");
				} else if (typeof(Models.Post).IsAssignableFrom(type)) {
					members.RemoveAll(m => m.Name == "Author" || m.Name == "Type" || m.Name == "Comments");
				}
				return members;
			}
		}
		#endregion

		#region Listeners
		/// <summary>
		/// Listener for stale queries.
		/// </summary>
		private class NoStaleQueriesListener : IDocumentQueryListener
		{
			/// <summary>
			/// Wait for store to index.
			/// </summary>
			/// <param name="queryCustomization">The query customizer</param>
			public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization) {
				queryCustomization.WaitForNonStaleResults();
			}
		}

		/// <summary>
		/// Listener for document conversions.
		/// </summary>
		private class ConversionListener : IDocumentConversionListener
		{
			/// <summary>
			/// Called after converting an entity to a document and metadata
			/// </summary>
			public void AfterConversionToDocument(string key, object entity, RavenJObject document, RavenJObject metadata) { }

			/// <summary>
			/// Called after converting a document and metadata to an entity
			/// </summary>
			public void AfterConversionToEntity(string key, RavenJObject document, RavenJObject metadata, object entity) { 
				// Call events
				if (entity is Models.Model) {
					((Models.Model)entity).OnLoad();
				}
			}

			/// <summary>
			/// Called before converting an entity to a document and metadata
			/// </summary>
			public void BeforeConversionToDocument(string key, object entity, RavenJObject metadata) { }

			/// <summary>
			/// Called when converting a document and metadata to an entity
			/// </summary>
			public void BeforeConversionToEntity(string key, RavenJObject document, RavenJObject metadata) { }
		}

		/// <summary>
		/// Listener for storing documents.
		/// </summary>
		private class StoreListener : IDocumentStoreListener
		{
			/// <summary>
			/// Invoked before the store request is sent to the server.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <param name="entityInstance">The entity instance.</param>
			/// <param name="metadata">The metadata.</param>
			/// <param name="original">The original document that was loaded from the server</param>
			/// <returns>
			/// Whatever the entity instance was modified and requires us re-serialize it.
			/// Returning true would force re-serialization of the entity, returning false would
			/// mean that any changes to the entityInstance would be ignored in the current SaveChanges call.
			/// </returns>
			public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original) {
				// Ensure id
				if (entityInstance is IModel) {
					var model = (IModel)entityInstance;

					if (model.Id == Guid.Empty)
						model.Id = Guid.NewGuid();
				}

				// Track changes
				if (entityInstance is Data.IChanges) {
					var model = (Data.IChanges)entityInstance;
					var now = DateTime.Now;

					// Set updated date
					model.Updated = now;

					// Set created date for new models
					if (model.Created == DateTime.MinValue)
						model.Created = now;
				}

				// Call events
				if (entityInstance is Models.Model) {
					((Models.Model)entityInstance).OnSave();
				}
				return true;
			}

			/// <summary>
			/// Invoked after the store request is sent to the server.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <param name="entityInstance">The entity instance.</param>
			/// <param name="metadata">The metadata.</param>
			public void AfterStore(string key, object entityInstance, RavenJObject metadata) { }
		}
		#endregion
	}
}
