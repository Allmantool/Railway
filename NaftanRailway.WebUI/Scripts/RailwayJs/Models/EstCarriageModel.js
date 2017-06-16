'use strict';

//namespace
var appRail = window.appRail || {};

//constuctor
appRail.EstCarriage = function (data, parent) {
    //private cost
    var self = this, _parent = parent;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //v_oper_Asus
    self.IsInfoAsusExist = ko.pureComputed(function () {
        var result = ko.isObservable(self.Carriage) ? self.Carriage() !== null : self.Carriage !== null;

        return result;
    });

    //v_02_podhod
    self.IsInfoPodhodExist = ko.pureComputed(function () {
        var result = ko.isObservable(self.AltCarriage) ? self.AltCarriage() !== null : self.AltCarriage !== null;

        return result;
    });

    //mark fade row in podhod table if same ixists in asus table
    self.IsAlreadyExists = function (carriageNumber) {
        var asusInvoices = jQuery.map(_parent.estCarriages(), function (item) {
            if (!ko.isObservable(item.Carriage)) {
                return item.Carriage.in_vgn();
            }
            return;
        });

        var result = jQuery.inArray(carriageNumber, asusInvoices) > -1;

        return result;
    };
};