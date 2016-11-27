/// <reference path="../../jquery-1.11.3.js" />
/// <reference path="../../knockout-3.4.0.debug.js" />
/// <reference path="../../knockout-3.4.0.js" />
/// <reference path="../../knockout.mapping-latest.js" />
/// <reference path="../../knockout.mapping-latest.debug.js" />
/// <reference path="~/Scripts/NomenclatureJs/DataContext.js" />
/// <reference path="~/Scripts/NomenclatureJs/Models/ScrollModel.js" />
"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

//REVEALING MODULE 
appNomenclature.SrcVM = (function ($, ko, db) {
    /*** Data  ***/
    var self = {
        currScr: ko.observable(),
        pagging: ko.observable(),
        scrolls: ko.observableArray([])
    };

    /**** behaviors ***/
    function init() {
        db.getScr(function (data) {
            self.scrolls($.map(data.ListKrtNaftan, function (val, i) {
                return new appNomenclature.Scroll(val);
            }));
            //self.scrolls(ko.mapping.fromJS(data.ListKrtNaftan));
            self.pagging(ko.mapping.fromJS(data.PagingInfo, { 'ignore': ["AjaxOptions", "RoutingDictionary"] }));
        });
    }

    function selActiveScr(selectScr) {
        //mark as actived
        self.currScr(selectScr.active(true));
    }

    /**** public API ***/
    return {
        currScr: self.currScr,
        scrolls: self.scrolls,
        pagging: self.pagging,
        init: init,
        chooseScr: selActiveScr
    };
}(jQuery, ko, appNomenclature.DataContext));