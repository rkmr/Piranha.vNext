/*
 * Copyright (c) 2014 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.vnext
 * 
 */

namespace Piranha.EntityFramework.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddPageTypeRegions : DbMigration
	{
		public override void Up() {
			CreateTable(
				"dbo.PiranhaPageTypeRegions",
				c => new {
					Id = c.Guid(nullable: false),
					TypeId = c.Guid(nullable: false),
					InternalId = c.String(nullable: false, maxLength: 32),
					Name = c.String(nullable: false, maxLength: 128),
					CLRType = c.String(nullable: false, maxLength: 512),
					IsCollection = c.Boolean(nullable: false),
					Order = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.PiranhaPageTypes", t => t.TypeId, cascadeDelete: true)
				.Index(t => new { t.TypeId, t.InternalId }, unique: true, name: "IX_InternalId");
		}

		public override void Down() {
			DropForeignKey("dbo.PiranhaPageTypeRegions", "TypeId", "dbo.PiranhaPageTypes");
			DropIndex("dbo.PiranhaPageTypeRegions", "IX_InternalId");
			DropTable("dbo.PiranhaPageTypeRegions");
		}
	}
}
