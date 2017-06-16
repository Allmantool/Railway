'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.ProgressBar = function (opts) {
    //var self = this;
    var defaults = {
        rate: ko.observable(0),
        mode: ko.observable(false),
        donwLoadPeriod: 5
    };

    //динамически копируем свойства
    var self = $.extend(defaults, ko.mapping.fromJS(opts));

    //behavior
    self.textContent = ko.pureComputed(function () {
        return self.rate() + '%';
    }, this)

    self.simulationDownload = function (container, ev) {
        //default event
        location.href =  $(ev.target).attr('href');

        //init
        self.rate(0);
        self.mode(true);

        var timerId = setInterval(function () {
            if (self.rate() >= 100) {
                self.mode(false);

                clearInterval(timerId);
            }

            var val = self.rate() + Math.floor(Math.random() * (15 - 1) + 1) * 1;
            self.rate(val >= 100 ? 100 : val);
        }, 120);
    };

    return self;
}