/// <reference path="../jquery-1.11.3.js" />
'use strict';

//namespace
var appRail = window.appRail || {};

appRail.MainVM = (function ($, ko, db) {
    var self = {

    };

    //behavior
    /*Sestem extensation for Jquery unbinding*/
    ko.unapplyBindings = function ($node, remove) {
        // unbind events
        $node.find("*").each(function () {
            $(this).unbind();
        });

        // Remove KO subscriptions and references
        if (remove) {
            ko.removeNode($node[0]);
        } else {
            ko.cleanNode($node[0]);
        }
    };

    function init(params) {

    };

    return {
        init: init
    };
}(jQuery, ko, appRail.DataContext));