/// <reference path="../jquery-1.11.3.js" />
/// <reference path="../jquery-2.2.4.js" />
/// <reference path="../jquery-3.4.2.debug.js" />
'use strict';
var appAdmin = window.appAdmin || {};

appAdmin.Hub = (function ($) {
    var _vm = undefined;
    var self = $.connection.adminHub;

    $.connection.hub.logging = true;
    $.connection.hub.start()
        .done(function () { console.log('Now connected, connection ID=' + $.connection.hub.id); })
        .fail(function () { console.log('Could not Connect!'); });

    self.client.newMessage = function (message) {
        _vm.addMessage(message);
    };

    //set realted view model
    function init(vm) {
        _vm = vm;
    };

    return {
        server: self.server,
        init: init
    };
})(jQuery);