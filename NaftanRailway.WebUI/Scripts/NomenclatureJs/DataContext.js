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
            //async: true,
            url: location.pathname,
            type: "Get",
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            accept: 'application/json',
            //Exist two method to bind request data from asp.net MVC framework
            //Get(query string) - Request.QueryString -  simple Json object (get have some limitaion in size), 
            //Post(form collection) - (Request.Form)/Json (convert to string) = complex and simple prop)
            data: { 'asService': true },
            beforeSend: function () { },
            complete: function () { },
            success: function (data) { callback(data); },
            error: function (data) {
                console.log(data);
            }
        };

        var $merged = $.extend(true, {}, defaults, opts);

        //json to string
        //$merged.data = JSON.stringify($merged.data);

        if ($.isFunction(callback)) {
            $.ajax($merged);
        }
    };

    /**** public API  ***/
    return {
        getScr: getScr
    };
}(jQuery));