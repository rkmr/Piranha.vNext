﻿/*
 * Copyright (c) 2014 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.vnext
 * 
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Piranha.RavenDb.Tests
{
	/// <summary>
	/// Tests for the alias repository.
	/// </summary>
	[TestClass]
	public class AliasTests : Piranha.Tests.Repositories.AliasTests
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AliasTests() {
			App.Init(c => {
				c.Store = new RavenDb.Store("http://localhost:8080", "piranha.tests", true);
			});
		}

		/// <summary>
		/// Test the alias repository.
		/// </summary>
		[TestMethod]
		[TestCategory("RavenDb")]
		public void Aliases() {
			base.Run();
		}
	}
}
