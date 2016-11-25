// use new ES5 syntax (IE8 => ES5-shim)
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.SrcDetailsVM = (function ($, ko, db) {
    var self = this;

    //consructor (represent one model for row (scroll)
 

    //behavior
    function init() {
        db.getScr(function (data) {
            ko.utils.arrayForEach(data || [], function (item) {
                kindPayment.push(new appNomenclature.Scroll(/*item.prop1,item.prop2 etc*/));
            });
        });
    }

    return {
        kindPayment: [],
        init: init
    };
})(jQuery, ko, appNomenclature.DataContext);

//initial knockout
//ko.applyBindings(scrollDetailsVM);
//ko.applyBindings(paggingVM, $('#padding'));