//
// Piranha CMS
// Copyright (c) 2014, Håkan Edling, All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.
//

if (!manager.models)
	manager.models = {};

//
// Page tye view model
//
manager.models.pagetype = function (id) {
	'use strict';

	var self = window.pagetype = this;

	// Members
	self.id = ko.observable('');
	self.name = ko.observable('');
	self.slug = ko.observable('');
	self.description = ko.observable('');
	self.route = ko.observable('');
	self.view = ko.observable('');
	self.regions = ko.observableArray([]);
	self.regionTypes = ko.observableArray([]);

	self.newRegionName = ko.observable('');
	self.newRegionId = ko.observable('');
	self.newRegionCollection = ko.observable(false);
	self.newRegionType = ko.observable('');

	// Computed members
	self.descriptionLength = ko.computed(function () {
		return self.description() ? self.description().length : 0;
	});

	// Edits the specified page type
	self.edit = function (id) {
		$.ajax({
			url: baseUrl + 'manager/pagetype/get/' + id,
			type: 'GET',
			dataType: 'json',
			success: function (result) {
				if (result.success) {
					self.id(result.data.Id);
					self.name(result.data.Name);
					self.slug(result.data.Slug);
					self.description(result.data.Description);
					self.route(result.data.Route);
					self.view(result.data.View);
					self.regions(result.data.Regions);
					self.regionTypes(result.data.RegionTypes);
				}
			},
			error: function (result) {
				console.log('error');
			}
		});
	};

	self.save = function () {
		var data = {
			Id: self.id(),
			Name: self.name(),
			Slug: self.slug(),
			Description: self.description(),
			Route: self.route(),
			View: self.view(),
			Regions: []
		};

		for (var n = 0; n < self.regions().length; n++) {
			data.Regions.push({
				Name: self.regions()[n].Name,
				InternalId: self.regions()[n].InternalId,
				IsCollection: self.regions()[n].IsCollection,
				CLRType: self.regions()[n].CLRType.Value
			});
		}

		$.ajax({
			url: baseUrl + 'manager/pagetype/save',
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json',
			data: JSON.stringify(data),
			success: function (result) {
				if (result.success) {
					manager.notifySave($('body'));
				}
			},
			error: function (result) {
				console.log('error');
			}
		});
	};

	self.regionUp = function (region) {
		var index = self.regions().indexOf(region);
		if (index > 0) {
			self.regions().splice(index - 1, 0, self.regions().splice(index, 1)[0]);
			self.regions(self.regions());
		}
	};

	self.regionDown = function (region) {
		var index = self.regions().indexOf(region);
		if (index < (self.regions().length - 1)) {
			self.regions().splice(index + 1, 0, self.regions().splice(index, 1)[0]);
			self.regions(self.regions());
		}
	};

	self.regionAdd = function () {
		self.regions().push({
			Name: self.newRegionName(),
			InternalId: self.newRegionId(),
			IsCollection: self.newRegionCollection(),
			CLRType: self.newRegionType()
		});
		self.regions(self.regions());

		self.newRegionName('');
		self.newRegionId('');
		self.newRegionCollection(false);
		self.newRegionType('');
	};

	self.regionDelete = function (region) {
		var index = self.regions().indexOf(region);
		if (index > -1) {
			self.regions().splice(index, 1);
			self.regions(self.regions());
		}
	};

	// Load the selected page type
	self.edit(id);
};