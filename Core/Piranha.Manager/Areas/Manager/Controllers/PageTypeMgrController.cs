﻿/*
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
using System.Web.Mvc;
using Piranha.Manager.Models.PageType;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Controller for managing page types.
	/// </summary>
	[Authorize]
	[RouteArea("Manager", AreaPrefix="manager")]
	public class PageTypeMgrController : ManagerController
	{
		/// <summary>
		/// Gets the list of the currently available page types.
		/// </summary>
		/// <returns>The view result</returns>
		[Route("pagetypes")]
		public ActionResult List() {
			return View(ListModel.Get());
		}

		/// <summary>
		/// Gets an edit model for an existing page type or
		/// creates a new one.
		/// </summary>
		/// <param name="id">The optional id</param>
		/// <returns>The edit model</returns>
		[Route("pagetype/get/{id:Guid?}")]
		public ActionResult GetSingle(Guid? id) {
			if (id.HasValue)
				return JsonData(true, EditModel.GetById(api, id.Value));
			return JsonData(true, new EditModel());
		}

		/// <summary>
		/// Gets the edit view for a new or existing page type.
		/// </summary>
		/// <param name="id">The optional id</param>
		/// <returns>The view result</returns>
		[Route("pagetype/edit/{id:Guid?}")]
		public ActionResult Edit(Guid? id = null) {
			if (id.HasValue) {
				ViewBag.Title = Piranha.Manager.Resources.PageType.EditTitle;
				return View(id);
			} else {
				ViewBag.Title = Piranha.Manager.Resources.PageType.AddTitle;
				return View(id);
			}
		}

		/// <summary>
		/// Saves the given page type.
		/// </summary>
		/// <param name="model">The page type</param>
		/// <returns>The view result</returns>
		[HttpPost]
		[Route("pagetype/save")]
		public ActionResult Save(EditModel model) {
			if (ModelState.IsValid) {
				model.Save(api);

				return JsonData(true, model);
			}
			return JsonData(false);
		}

		/// <summary>
		/// Deletes the page type with the given id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The redirect result</returns>
		[Route("pagetype/delete/{id:Guid}")]
		public ActionResult Delete(Guid id) {
			var type = api.PageTypes.GetSingle(where: t => t.Id == id);
			if (type != null) {
				api.PageTypes.Remove(type);
				api.SaveChanges();
			}
			return RedirectToAction("List");
		}
	}
}