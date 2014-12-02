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
using Raven.Client;

namespace Piranha.RavenDb
{
	/// <summary>
	/// Interface for a raven db configuration file. These are used
	/// to add behaviour to the data store from modules or applications.
	/// </summary>
	public interface IRavenConfig
	{
		/// <summary>
		/// Called when the store is initialized.
		/// </summary>
		/// <param name="store">The current document store</param>
		void InitStore(IDocumentStore store);
	}
}
