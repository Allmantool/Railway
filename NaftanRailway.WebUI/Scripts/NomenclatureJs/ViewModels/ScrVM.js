/// <reference path="../../jquery-1.11.3.js" />
/// <reference path="../../knockout-3.4.0.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//REVEALING MODULE 
appNomenclature.SrcVM = (function ($, ko, db) {
    var self = this;

    //private

    //behavior
    function init() {
        db.getScr(function (data) {
            ko.utils.arrayForEach(data || [], function (item) {
                scroll.push(new appNomenclature.Scroll(/*item.prop1,item.prop2 etc*/));
            });
        });
    }

    //public
    return {
        scrolls: [],
        init: init
    };
}(jQuery, ko, appNomenclature.DataContext));