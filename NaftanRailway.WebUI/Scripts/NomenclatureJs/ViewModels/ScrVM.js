/// <reference path="../../jquery-1.11.3.js" />
/// <reference path="../../knockout-3.4.0.debug.js" />
/// <reference path="../../knockout-3.4.0.js" />
/// <reference path="../../knockout.mapping-latest.js" />
/// <reference path="../../knockout.mapping-latest.debug.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//REVEALING MODULE 
appNomenclature.SrcVM = (function ($, ko, db) {
    var me = {
        currScr: ko.observable(),
        scrolls: ko.observableArray([]),
        pagging: ko.observable(),
        init: init
    };

/**** behavior  ***/
    function init() {
        db.getScr(function (data) {
            me.scrolls = (ko.mapping.fromJS(data.ListKrtNaftan));
            me.pagging = (ko.mapping.fromJS(data.PagingInfo));
        });
    }

    function selActiveScr(scr) {
        me.currScr(scr);
    }

    return me;
}(jQuery, ko, appNomenclature.DataContext));