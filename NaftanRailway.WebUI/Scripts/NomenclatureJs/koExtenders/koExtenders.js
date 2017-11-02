/// <reference path="../jquery-1.11.3.js" />
/// <reference path="../knockout-3.4.2.debug.js" />
/// <reference path="../moment.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.CustExtend = (function ($, ko) {
    //number format before and after denominational
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
        return target;//result;
    };

    //change observable only if submit (simple edit behavior)
    //By default edit modal not persist change if they were done without pushing on submit button
    //target => current observable
    //persist => save mode
    ko.extenders.editable = function (target, params) {
        var originalState = ko.toJS(target), lastValue = ko.observable();
        var defaults = {
            editMode: ko.observable(true),
            persiste: ko.observable(false),
            onlyNumber: ko.observable(true)
        };

        var $merged = $.extend(true, defaults, params);

        //click save button
        $merged.persiste.subscribe(function (mode) {
            if ($merged.persiste()) {
                return result(lastValue());
            }
        }/*, this, "beforeChange"*/);

        //close edit form
        $merged.editMode.subscribe(function (mode) {
            if (!$merged.editMode() && !$merged.persiste()) {
                return result(originalState);
            }
        }/*, this, "beforeChange"*/);

        //dynamic control
        var result = ko.computed({
            read: target,
            write: function (newValue) {
                //it's a bypass for issue with save without changes
                var formatNumber = $merged.onlyNumber ? Number(newValue) : newValue;
                var value = newValue === undefined ? target() : formatNumber;

                lastValue(value);
                target(value);
                //$merged.persiste() ? target(newValue) : target();

                //to default
                //return $merged.persiste(false);
            }
        }).extend({ notify: 'always' });

        //return the new computed observable (initialize)
        return result;
    };

}(jQuery, ko));