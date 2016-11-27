'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Pagination = function (totalItems, itemsPerPage, currentPage) {
    var self = this;

    self.TotalItems = totalItems;
    self.ItemsPerPage = itemsPerPage;
    self.CurrentPage = currentPage;
};