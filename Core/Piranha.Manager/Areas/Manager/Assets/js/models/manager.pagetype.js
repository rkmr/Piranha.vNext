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
	self.properties = ko.observableArray([]);
	self.propertyTypes = ko.observableArray([]);
	self.regions = ko.observableArray([]);
	self.regionTypes = ko.observableArray([]);

	self.newRegionName = ko.observable('');
	self.newRegionId = ko.observable('');
	self.newRegionCollection = ko.observable(false);
	self.newRegionType = ko.observable('');

	self.newPropertyName = ko.observable('');
	self.newPropertyId = ko.observable('');
	self.newPropertyCollection = ko.observable(false);
	self.newPropertyType = ko.observable('');


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
					self.bind(result.data);
				}
			},
			error: function (result) {
				console.log('error');
			}
		});
	};

	// Saves the page type
	self.save = function () {
		// Create the post data
		var data = {
			Id: self.id(),
			Name: self.name(),
			Slug: self.slug(),
			Description: self.description(),
			Route: self.route(),
			View: self.view(),
			Properties: [],
			Regions: []
		};

		// Add the current property
		for (var n = 0; n < self.properties().length; n++) {
			data.Properties.push({
				Id: self.properties()[n].Id,
				Name: self.properties()[n].Name,
				InternalId: self.properties()[n].InternalId,
				IsCollection: self.properties()[n].IsCollection,
				CLRType: self.properties()[n].CLRType.Value
			});
		}

		// Add the current region
		for (var n = 0; n < self.regions().length; n++) {
			data.Regions.push({
				Id: self.regions()[n].Id,
				Name: self.regions()[n].Name,
				InternalId: self.regions()[n].InternalId,
				IsCollection: self.regions()[n].IsCollection,
				CLRType: self.regions()[n].CLRType.Value
			});
		}

		// Save it
		$.ajax({
			url: baseUrl + 'manager/pagetype/save',
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json',
			data: JSON.stringify(data),
			success: function (result) {
				if (result.success) {
					// Check for new id
					if (result.data.Id != self.id())
						history.pushState({}, 'Edit page type', document.URL + '/' + result.data.Id);

					// Update and notify
					self.bind(result.data);
					manager.notifySave($('body'));
				}
			},
			error: function (result) {
				console.log('error');
			}
		});
	};

	// Moves the given region up in the list
	self.regionUp = function (region) {
		var index = self.regions().indexOf(region);
		if (index > 0) {
			self.regions().splice(index - 1, 0, self.regions().splice(index, 1)[0]);
			self.regions(self.regions());
		}
	};

	// Moves the given region down in the list
	self.regionDown = function (region) {
		var index = self.regions().indexOf(region);
		if (index < (self.regions().length - 1)) {
			self.regions().splice(index + 1, 0, self.regions().splice(index, 1)[0]);
			self.regions(self.regions());
		}
	};

	// Adds a new region to the list
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

	// Deletes the given region from the list
	self.regionDelete = function (region) {
		var index = self.regions().indexOf(region);
		if (index > -1) {
			self.regions().splice(index, 1);
			self.regions(self.regions());
		}
	};

	// Moves the given property up in the list
	self.propertyUp = function (property) {
		var index = self.properties().indexOf(property);
		if (index > 0) {
			self.properties().splice(index - 1, 0, self.properties().splice(index, 1)[0]);
			self.properties(self.properties());
		}
	};

	// Moves the given property down in the list
	self.propertyDown = function (property) {
		var index = self.properties().indexOf(property);
		if (index < (self.properties().length - 1)) {
			self.properties().splice(index + 1, 0, self.properties().splice(index, 1)[0]);
			self.properties(self.properties());
		}
	};

	// Adds a new property to the list
	self.propertyAdd = function () {
		self.properties().push({
			Name: self.newPropertyName(),
			InternalId: self.newPropertyId(),
			IsCollection: self.newPropertyCollection(),
			CLRType: self.newPropertyType()
		});
		self.properties(self.properties());

		self.newPropertyName('');
		self.newPropertyId('');
		self.newPropertyCollection(false);
		self.newPropertyType('');
	};

	// Deletes the given property from the list
	self.propertyDelete = function (property) {
		var index = self.properties().indexOf(property);
		if (index > -1) {
			self.properties().splice(index, 1);
			self.properties(self.properties());
		}
	};

	// Binds the given data to the model.
	self.bind = function (data) {
		self.id(data.Id);
		self.name(data.Name);
		self.slug(data.Slug);
		self.description(data.Description);
		self.route(data.Route);
		self.view(data.View);
		self.properties(data.Properties);
		self.propertyTypes(data.PropertyTypes);
		self.regions(data.Regions);
		self.regionTypes(data.RegionTypes);
	}

	// Load the selected page type
	self.edit(id);
};