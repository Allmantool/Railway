/// <reference path="../jquery-1.11.3.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.DataContext = (function ($) {
    var self = this;

    /*** behavior ***/

    //return specific scrolls .requestUrl, httpMethod, sendJSON
    function getScr(callback, opts) {
        //work with options
        var defaults = {
            url: location.pathname,
            type: "Get",
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            accept: 'application/json',
            data: { "asService": true },
            beforeSend: function(){},
            complete: function () { },
            success: function (data) { callback(data); },
            error: function (data) { 
                console.log(data); }
        };

        var $merged = $.extend(true, defaults, opts);

        if ($.isFunction(callback)) {
            $.ajax($merged);
        }
    };

    /**** public API  ***/
    return {
        getScr: getScr
    };
}(jQuery));