﻿/// <reference path="../../jquery-1.11.3.js" />
/// <reference path="../../knockout-3.4.0.debug.js" />
/// <reference path="../../knockout-3.4.0.js" />
/// <reference path="../../knockout.mapping-latest.js" />
/// <reference path="../../knockout.mapping-latest.debug.js" />
/// <reference path="~/Scripts/NomenclatureJs/DataContext.js" />
/// <reference path="~/Scripts/NomenclatureJs/Models/ScrollModel.js" />
"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

//REVEALING MODULE 
appNomenclature.SrcVM = (function ($, ko, db, pm, sd) {
    /*** Data  ***/
    var self = {
        containerName: undefined,
        rowsPerPage: ko.observable(15),
        wrkPeriods: ko.observableArray([]),
        wrkSelPeriod: ko.observable(),
        alert: ko.observable(new appNomenclature.AlertMessage({ statusMsg: 'Инициализация' })),
        currScr: ko.observable(undefined),
        pagging: ko.observable(),
        scrolls: ko.observableArray(),
        periodModal: pm,
        scrollDetails: sd,
        loadingState: ko.observable(false),
        progressBar: ko.observable(new appNomenclature.ProgressBar())
    };

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

    /**** behaviors ***/
    //private 
    function _updateSrcByKey(rows) {
        var mappingOptions = {
            key: function (data) {
                return ko.utils.unwrapObservable(data.KEYKRT);
            },
            create: function (optioins) {
                return new appNomenclature.Scroll(optioins.data, true);
            }
        };

        //update/insert
        $.each(rows, function (outIndex, outMember) {
            var isNew = true;

            self.scrolls().forEach(function (inMember, inIndex) {
                if (inMember.KEYKRT() === outMember.KEYKRT) {

                    isNew = false;

                    //update
                    ko.mapping.fromJS(outMember, mappingOptions, self.scrolls()[inIndex]);

                    //break out loop
                    return false;
                }
            });

            //insert
            if (isNew) {
                self.scrolls.push(new appNomenclature.Scroll(outMember, true));
            }
        });

        containerRebind();

        //self.scrolls(ko.mapping.fromJS(incomeArray.ListKrtNaftan, mappingOptions)());
    };

    //public
    function init(opts) {
        //Deferred updates
        //ko.options.deferUpdates = true;

        //get container element
        self.containerName = $("#koContainer")[0];

        //work with options
        var defaults = {
            data: { "initialSizeItem": self.rowsPerPage(), "period": self.wrkSelPeriod() },
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                //firts initialization
                if (opts === undefined) {
                    ko.applyBindings(appNomenclature.SrcVM);
                }

                self.loadingState(false);
            }
        };

        //work with options
        var $merged = $.extend({}, defaults, opts);

        //ajax
        db.getScr(function (data) {
            self.scrolls($.map(data.ListKrtNaftan, function (val, i) {
                return new appNomenclature.Scroll(val, true);
            }));

            //match by key
            var exist = ko.utils.arrayFirst(self.scrolls(), function (item) {
                return self.currScr() ? self.currScr().KEYKRT() === item.KEYKRT() : false;
            });

            //default
            if (!exist) {
                self.currScr(self.scrolls()[0]);
            }

            //modal date
            self.periodModal.period(self.currScr().DTBUHOTCHET());
            //period + list index select
            self.wrkPeriods(data.RangePeriod.reverse());

            //pagging
            self.pagging(new appNomenclature.Pagination(ko.mapping.fromJS(data.PagingInfo, { 'ignore': ["AjaxOptions"] }), ["controller"], self));
        }, $merged);
    };

    function selActiveScr(el, ev) {
        //mark as actived
        self.currScr(el);
        self.periodModal.period(el.DTBUHOTCHET());

        //jquery radio button check
        return true;
    }

    function updatePeriod(formElement) {
        //ajax
        db.getScr(function (data) {
            //update
            _updateSrcByKey(data);
            //hide modal + default
            self.periodModal.active(false).multiMode(false);

            var msg = 'Отчётный период был успешно изменен для перечня(ей): №' + $(data).map(function (indx, src) {
                return src.NKRT;
            }).get().join(', ');

            //show alert
            self.alert().statusMsg(msg).alertType('alert-success').mode(true);
        }, {
            type: "Post",
            url: $(formElement).attr('action'),
            data: ko.mapping.toJSON({
                'model': { 'Period': pm.period(), 'Multimode': pm.multiMode(), 'Item': self.currScr() },
                'asService': true
            }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () { self.loadingState(false); }
        });

        // avoid form submission
        return false;
    };

    function changeCountPerPage(link, ev) {
        init({
            url: self.pagging().getPageUrl() + 1
        }, self);
    };

    function changePeriodMonth() {
        init({
            url: self.pagging().getPageUrl() + 1
        }, self);
    };

    function admitScr(link, ev) {
        db.getScr(function (data) {
            //update
            //_updateSrcByKey(data.ListKrtNaftan);
            init({
                url: self.pagging().getPageUrl() + self.pagging().CurrentPage()
            }, self);

            self.alert().statusMsg('Синхронизация с БД прошла успешно!').alertType('alert-success').mode(true);
        }, {
            url: typeof (link) === 'string' ? link : $(ev.target).attr('href'),
            type: "Post",
            data: ko.mapping.toJSON({ 'asService': true }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                self.loadingState(false);
            },
            error: function () { self.alert().statusMsg('Синхронизация с БД завершилась ошибкой!').alertType('alert-danger').mode(true); }
        });
    };

    function removeSrc(link, src, ev) {
        db.getScr(function (data) {
            //self.scrolls.remove(src);

            init({
                url: self.pagging().getPageUrl() + self.pagging().CurrentPage(),
                data: { "initialSizeItem": self.rowsPerPage(), "period": self.wrkSelPeriod() }
            }, self);

            self.alert().statusMsg('Перечень №' + src.NKRT() + ' успешно удален!').alertType('alert-success').mode(true);
        }, {
            url: typeof (link) === 'string' ? link : $(ev.target).attr('href'),
            type: "Post",
            data: ko.mapping.toJSON({ 'asService': true, 'numberScroll': src.NKRT(), 'reportYear': moment(src.DTBUHOTCHET()).format('YYYY') }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () {
                self.loadingState(false);
            },
            error: function () {
                self.alert().statusMsg('Операция удаление перечня  №' + src.NKRT() + ' завершилось ошибкой!').alertType('alert-danger').mode(true);
            }
        });
    };

    function registrationScr(link, src, ev) {
        //because two menu (base on link and button elements)
        var curWrkSrc = typeof (link) === 'string' ? src : self.currScr();

        curWrkSrc.proccessingState(true);

        db.getScr(function (data) {
            //update
            _updateSrcByKey(data);

            self.alert().statusMsg('Перечень №' + curWrkSrc.NKRT() + ' успешно подтвержден!').alertType('alert-success').mode(true);
        }, {
            //exist two mode of exec (by button click and url click)
            url: typeof (link) === 'string' ? link : $(ev.target).attr('href'),
            type: "Post",
            data: ko.mapping.toJSON({ 'asService': true }),
            beforeSend: function () { self.loadingState(true); },
            complete: function () { self.loadingState(false); curWrkSrc.proccessingState(false); },
            error: function () { curWrkSrc.statusMsg('Прошизошла ошибка при подтверждение перечня! №' + curWrkSrc.NKRT()).alertType('alert-danger').mode(true) }
        });
    };

    function viewScrDetails(link, src, ev) { //link, src, ev (set default src)
        //because two menu (base on link and button elements)
        if (typeof (link) === 'string') {
            selActiveScr(src)
        }

        var reqLink = typeof (link) === 'string' ? link : $(ev.target).attr('href');

        //if confirmed
        if (self.currScr().Confirmed()) {
            sd.init({
                url: reqLink,
                data: ko.mapping.toJSON({ "initialSizeItem": sd.rowsPerPage(), "asService": true, "filters": undefined }),
                beforeSend: function () { self.loadingState(true); },
                complete: function () {
                    //load partial view
                    containerRebind({
                        link: reqLink,
                        callback: function () {
                            self.alert().statusMsg('Информация получена!').alertType('alert-success').mode(true);
                        }
                    });
                }
            }, self);
        } else {
            self.alert().statusMsg('Для просмотра сборов перечня необходимо сначала подтвердить перечень!').alertType('alert-warning').mode(true);
        }
    };

    //work with ajax replace (reaplace dom is leaded to lose binding)
    function containerRebind(opts, ev) {
        self.loadingState(true);

        var defaults = {
            container: self.containerName,
            link: ev ? $(ev.target).attr('href') : '',
            callback: function () { }
        };

        var $merged = $.extend({}, defaults, opts);

        var container = $merged.container;
        var requestLink = $merged.link;

        //Unbind
        if (ko.dataFor(self.containerName)) {
            ko.unapplyBindings($(self.containerName));
        }

        $(self.containerName).load(requestLink, function (partialHtml, statusTxt, xhr) {
            if (statusTxt === "success") {
                //bind
                //ko.options.useOnlyNativeEvents = true;
                ko.applyBindings(appNomenclature.SrcVM, self.containerName);
                self.loadingState(false);

                $merged.callback();
            };

            if (statusTxt === "error")
                alert("Error: containerRebind " + xhr.status + ": " + xhr.statusText);
        });
    };

    /**** public API ***/
    return {
        //prop
        currScr: self.currScr,
        scrolls: self.scrolls,
        pagging: self.pagging,
        loadingState: self.loadingState,
        periodModal: self.periodModal,
        alert: self.alert,
        progressBar: self.progressBar,
        scrollDetails: self.scrollDetails,
        rowsPerPage: self.rowsPerPage,
        wrkPeriods: self.wrkPeriods,
        wrkSelPeriod: self.wrkSelPeriod,
        //behavior
        init: init,
        updatePeriod: updatePeriod,
        admitScr: admitScr,
        registrationScr: registrationScr,
        chooseScr: selActiveScr,
        viewScrDetails: viewScrDetails,
        containerRebind: containerRebind,
        removeSrc: removeSrc,
        changeCountPerPage: changeCountPerPage,
        changePeriodMonth: changePeriodMonth
    };
}(jQuery, ko, appNomenclature.DataContext, appNomenclature.PeriodModalVM, appNomenclature.SrcDetailsVM));