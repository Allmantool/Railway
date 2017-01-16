"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.PeriodModalVM = (function ($, ko) {
    /*** Data  ***/
    var self = {
        multiMode: ko.observable(false),
        period: ko.observable(),
        active: ko.observable(false)
    };

    /**** public API ***/
    return {
        multiMode: self.multiMode,
        period: self.period,
        active: self.active
    };
}(jQuery, ko));