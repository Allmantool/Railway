/// <reference path="~/Scripts/jquery-1.11.3.js" />
/// <reference path="~/Scripts/knockout-3.4.0.debug.js" />
/// <reference path="~/Scripts/moment.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.CustExtend = (function ($, ko) {
    //number format before and after dominination
    ko.extenders.keysbor = function (target, key) {
        //create a writable computed observable to intercept writes to our observable
        var result = ko.pureComputed({
            read: target,  //always return the original observables value
            write: function (newValue) {
                var _donominationConst = 15072000209229;
                var current = (target() === null ? '' : target().toLocaleString('Ru-ru'));

                //only write if it changed
                if (key >= _donominationConst) {
                    //ToString("#,0.00")
                    target(current);
                } else {
                    //ToString("#,#")
                    //if the rounded value is the same, but a different value was written, force a notification for the current field
                }
            }
        }).extend({ notify: 'always' });

        result(target());

        //return the new computed observable
        return result;
    };

    //change observeable only if submit
    ko.extenders.undoRedo = function (target, submit) {
        var _startVal = target;
        var result;

        //target.subscribe(function (oldValue) {
        //    //Only legecy value (start value)
        //    if (oldValue === target()) {
        //        result = target();
        //    }
        //}, this, "beforeChange");

        target.subscribe(function (newValue) {
            if (submit) { result = newValue; }
        }, this);

        //return the new computed observable
        return _startVal;
    };

}(jQuery, ko));