﻿'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Charge = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //prop
    self.sm_no_nds = ko.observable(self.sm_no_nds()).extend({ actualKeysbor: self.keysbor() });
    self.sm_nds = ko.observable(self.sm_nds()).extend({ keysbor: self.keysbor() });
    self.sm = ko.observable(self.sm()).extend({ keysbor: self.keysbor() });
    self.date_raskr = ko.pureComputed(function () {
        var result = self.vidsbr().toString().search(new RegExp('30[01]', 'i')) > -1 ?
            moment(self.date_raskr()).format('DD.MM.YYYY HH:MM') :
            moment(self.date_raskr()).format('DD.MM.YYYY');

        return result;
    });

    self.getDocType = ko.pureComputed(function () {
        var result = '';

        switch (self.tdoc()) {
            case 1: result = 'Накладная';
                break;
            case 2: result = 'Ведомость';
                break;
            case 3: result = 'Акт';
                break;
            default: result = 'Накопительная карточка';
        }

        return result;
    });
    self.compareMoney = ko.pureComputed(function () {
        var result = self.sm() !== parseFloat((self.summa() + self.nds()).toFixed(2));

        return result;
    });
    self.selected = ko.observable(false);
};