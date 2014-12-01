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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Piranha.Tests
{
	/// <summary>
	/// Tests for extensions
	/// </summary>
	[TestClass]
	public class ExtensionTests
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ExtensionTests() {
			App.Init();
		}

		/// <summary>
		/// Checks that all of the properties have been imported properly
		/// </summary>
		[TestMethod]
		[TestCategory("Extensions")]
		public void Properties() {
			Assert.AreEqual(5, App.Extensions.Properties.Count);
		}

		/// <summary>
		/// Checks that all of the regions have been imported properly
		/// </summary>
		[TestMethod]
		[TestCategory("Extensions")]
		public void Regions() {
			Assert.AreEqual(3, App.Extensions.Regions.Count);
		}
	}
}
