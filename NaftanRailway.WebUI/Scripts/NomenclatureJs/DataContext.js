/// <reference path="../jquery-1.11.3.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.DataContext = (function () {
    //behavior
    function getScr(callback) {
        if ($.isFunction(callback)) {
            $.getJSON('Data/Catalog.json', function (data) {
                callback(data.Catalog);
            });
        }
    };

    return {
        getScr: getScr
    };
}());