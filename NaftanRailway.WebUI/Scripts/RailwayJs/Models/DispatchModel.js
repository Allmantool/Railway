'use strict';

//namespace
var appRail = window.appRail || {};

//constructor
appRail.Dispach = function (data) {
    //private cost

    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //css purpose (mouseover, mouseout)
    self.selected = ko.observable(false);

    //check if we have information about shipping
    self.shInfoExists = ko.pureComputed(function () {
        var shInfo = (ko.isObservable(self.vOtpr) ? ko.unwrap(self.vOtpr) : self.vOtpr);

        var result = (shInfo === null ? false : true);

        return result;
    });

    //dynamic change bootstrap glyphicon css
    self.directionGlyphicon = ko.pureComputed(function () {
        var shInfo = ko.unwrap(self.vOtpr);

        var result = 'glyphicon ' + (shInfo === null ? 'glyphicon-usd' : shInfo.oper() === 1 ? 'glyphicon-road' : 'glyphicon-home');

        return result;
    });

    self.fullNameSender = ko.pureComputed(function () {
        var result = self.vOtpr.adr_otpr() + ' ' + self.vOtpr.nam_otpr();

        return result;
    });

    self.fullNameReceiver = ko.pureComputed(function () {
        var result = self.vOtpr.adr_pol() + ' ' + self.vOtpr.nam_pol();

        return result;
    });

    self.carriageList = ko.pureComputed(function () {
        var result = $.map(self.vovs(), function (val, i) {
            return val.n_vag();
        });

        return result.join(', ');
    });

    self.statementList = ko.pureComputed(function () {
        var result = $.map(self.vPams(), function (val, i) {
            return val.nved();
        });

        return result.join(', ');
    });

    self.actList = ko.pureComputed(function () {
        var result = $.map(self.vAkts(), function (val, i) {
            return val.nakt();
        });

        return result.join(', ');
    });

    self.cardList = ko.pureComputed(function () {
        var result = $.map(self.vKarts(), function (val, i) {
            return val.num_kart();
        });

        return result.join(', ');
    });

    self.scrollList = ko.pureComputed(function () {
        var result = $.map(self.kNaftan(), function (val, i) {
            return val.nkrt();
        });

        return result.join(', ');
    });

    self.scrollListDate = ko.pureComputed(function () {
        var result = $.map(self.kNaftan(), function (val, i) {
            return moment(val.endDate_PER()).format('DD.MM.YYYY');
        });

        return result.join(', ');
    });
};