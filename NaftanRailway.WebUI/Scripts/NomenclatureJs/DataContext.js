/// <reference path="../jquery-1.11.3.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.DataContext = (function () {
    /*** behavior ***/

    //return specific scrolls
    function getScr(callback) {
        if ($.isFunction(callback)) {
            $.ajax({
                url: "Scroll/Index/",
                type: "Get",
                traditional: true,
                contentType: 'application/json; charset=utf-8',
                data: { "asService": true },
                success: function (data) {
                    callback(data);
                },
                error: function (data) {
                    console.log("getScr error:" + data)
                }
            });
        }
    };

    //return detalisation per srcoll
    function getScrDetails(callback) {
        if ($.isFunction(callback)) {
            $.getJSON('Data/Catalog.json', function (data) {
                callback(data.Catalog);
            });
        }
    };

    return {
        getScr: getScr,
        getScrDetails: getScrDetails
    };
}());