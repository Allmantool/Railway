'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Charge = function (data, parent) {
    //private cost
    var _parent = parent;

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //add some bussiness logic (number format before and after dominination)
    self.sm_no_nds = ko.observable(self.sm_no_nds()).extend({ actualKeysbor: self.keysbor() });
    self.sm_nds = ko.observable(self.sm_nds()).extend({ keysbor: self.keysbor() });
    self.sm = ko.observable(self.sm()).extend({ keysbor: self.keysbor() });
    self.summa = ko.observable(self.summa()).extend({ keysbor: self.keysbor() });
    self.nds = ko.observable(self.nds()).extend({ keysbor: self.keysbor() });

    //revert or submit editable property (with same named ko extend)
    self.persist = ko.observable(false);
    self.textm = ko.observable(self.textm()).extend({ editable: self.persist });

    self.date_raskr = ko.computed(function () {
        var result = self.vidsbr().toString().search(new RegExp('30[01]', 'i')) > -1 ?
            moment(self.date_raskr()).format('DD.MM.YYYY HH:MM') :
            moment(self.date_raskr()).format('DD.MM.YYYY');

        return result;
    });
    self.sapodTotalSum = ko.pureComputed(function () {
        return parseFloat((Number(self.summa()) + Number(self.nds())).toFixed(2));
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
        var result = (self.sm() !== self.sapodTotalSum());

        return result;
    });
    self.selected = ko.observable(false);
};