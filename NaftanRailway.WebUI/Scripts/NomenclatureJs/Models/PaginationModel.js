'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Pagination = function (totalItems, itemsPerPage, currentPage) {
    this.TotalItems = totalItems;
    this.ItemsPerPage = itemsPerPage;
    this.CurrentPage = currentPage;
};