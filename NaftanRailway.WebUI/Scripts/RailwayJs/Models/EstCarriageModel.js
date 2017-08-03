'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.EstCarriage = function (data, parent) {
    //private cost
    var self = this, _parent = parent;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //v_oper_Asus
    self.isInfoAsusExist = ko.pureComputed(function () {
        var result = ko.isObservable(self.carriage) ? self.carriage() !== null : self.carriage !== null;

        return result;
    });

    //v_02_podhod
    self.isInfoPodhodExist = ko.pureComputed(function () {
        var result = ko.isObservable(self.altCarriage) ? self.altCarriage() !== null : self.altCarriage !== null;

        return result;
    });

    //mark fade row in podhod table if same ixists in asus table
    self.IsAlreadyExists = function (carriageNumber) {
        var asusInvoices = jQuery.map(_parent.estCarriages(), function (item) {
            if (!ko.isObservable(item.carriage)) {
                return item.carriage.in_vgn();
            }
            return;
        });

        var result = jQuery.inArray(carriageNumber, asusInvoices) > -1;

        return result;
    };
};