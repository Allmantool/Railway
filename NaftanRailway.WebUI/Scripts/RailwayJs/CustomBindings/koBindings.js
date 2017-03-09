/// <reference path="~/Scripts/jquery-1.11.3.js" />
/// <reference path="../jquery-2.2.4.js" />
/// <reference path="~/Scripts/knockout-3.4.2.debug.js" />
/// <reference path="~/Scripts/moment.js" />
'use strict';

//namespace
var appRail = window.appRail || {};

//bootsrap js
appRail.CustBundings = (function ($, ko) {
    var self = this;

    ko.bindingHandlers.tooltip = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            // This will be called when the binding is first applied to an element
            // Set up any initial state, event handlers, etc. here
            var defaults = {
                //title: "",
                container: "body",
                animation: true,
                placement: "top",
                trigger: "hover",
                delay: { show: 500, hide: 100 },
                selState: false
            };

            var $merged = $.extend({}, defaults, allBindings().tooltip);

            // Next, whether or not the supplied model property is observable, get its current value
            var valueUnwrapped = ko.unwrap(valueAccessor());

            $(element).tooltip($merged);

            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).tooltip("destroy");
            });
            //ko.utils.domNodeDisposal.cleanExternalData = function () {
            //    // Do nothing. Now any jQuery data associated with elements will
            //    // not be cleaned up when the elements are removed from the DOM.
            //};
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            // This will be called once when the binding is first applied to an element,
            // and again whenever any observables/computeds that are accessed change
            // Update the DOM element based on the supplied values here.
            var defaults = { selState: false };
            var $merged = $.extend({}, defaults, valueAccessor());

            $merged.selState ? $(element).tooltip('show') : $(element).tooltip('hide');
        }
    };

    ko.bindingHandlers.popover = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                animation: true,
                container: "body",
                delay: { show: 500, hide: 100 },
                placement: "auto",
                title: "",
                content: '',//must be string =>pass duplicate content bug (ex. if you received val parameter add '' (empty string) for explicit converting)
                //selector: false,
                template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title" style="font-weight: bolder;text-align: center;"></h3><div class="small popover-content text-center"></div></div>',
                html: true,
                trigger: "hover",
                viewport: { selector: "body", padding: 0 }
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            $(element).popover($merged);

            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).popover('destroy');
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            // This will be called once when the binding is first applied to an element,
            // and again whenever any observables/computed that are accessed change
            // Update the DOM element based on the supplied values here.
        }
    };

    /**********************************https://uxsolutions.github.io/bootstrap-datepicker/************************************* */
    /**********************************http://bootstrap-datepicker.readthedocs.org/en/latest*********************************** */
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                format: "MM yyyy",
                startView: 1,
                minViewMode: 1,
                language: "ru",
                autoclose: true,
                todayBtn: true,//"linked",
                orientation: "bottom auto",
                forceParse: true,
                container: 'body',//'#koContainer',
                //toggleActive: false,
                //updateViewDate: true,
                //defaultViewDate: new Date( 2016, 4, 1)
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            $(element).datepicker(defaults).on("changeMonth"/*changeDate changeYear changeDecade changeCentury*/, function (ev) {
                var observable = valueAccessor().observable;

                if (ko.isObservable(observable)) {
                    observable(ev.date);
                }

                ev.preventDefault();
            });;

            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).datepicker("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            // This will be called once when the binding is first applied to an element,
            // and again whenever any observables/computeds that are accessed change
            // Update the DOM element based on the supplied values here.
            var format = ko.unwrap(valueAccessor()).format || "MM YYYY";

            $(element).datepicker('setDate', moment(ko.unwrap(valueAccessor().observable())).format(format));
        }
    };

    ko.bindingHandlers.modal = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                show: false,
                keyboard: true,
                backdrop: true,
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            $(element).modal($merged);

            var value = valueAccessor();
            if (ko.isObservable(value)) {
                $(element).on('hide.bs.modal', function () {
                    value(false);
                });
            }

            // Update 13/07/2016
            // based on @Richard's finding,
            // don't need to destroy modal explicitly in latest bootstrap.
            // modal('destroy') doesn't exist in latest bootstrap.
            //ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            //    $(element).modal("destroy");
            //});
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();

            if (ko.utils.unwrapObservable(value)) {
                $(element).modal('show');
            } else {
                $(element).modal('hide');
            }
        }
    };

    ko.bindingHandlers.alert = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                duration: 500,
                statusMsg: "this's alert status message",
                alertType: "alert-info"
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            //Makes an alert listen for click events on descendant elements which have the data-dismiss="alert" attribute. (Not necessary when using the data-api's auto-initialization.)
            $(element).alert().on('close.bs.alert', function (ev) {
                var visibleState = valueAccessor().mode;

                if (ko.isObservable(visibleState)) {
                    visibleState(false);
                    //immediatly
                    $(element).hide();

                    ev.preventDefault();
                }
            }).on('closed.bs.alert', function (ev) {
                ev.preventDefault();
            });

            var style = {
                margin: ".1em",
                textAlign: "center",
                display: "none",
                position: "fixed",
                top: "50%",
                width: "100%",
                zIndex: 9999,
            };
            //structure element
            $(element).css(style)
                .append('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>')
                .append('<span data-bind="text: statusMsg()"></span>');

            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).alert("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                duration: 500
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            // Now manipulate the DOM element
            if ($merged.mode() === true) {
                $(element).fadeIn($merged.duration).delay(2000).fadeOut($merged.duration); // Make the element visible
                $merged.mode(false);
            }
        }
    };

    //http://api.jqueryui.com/autocomplete/
    ko.bindingHandlers.jqAutoComplete = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).jqAutoComplete("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            $(element).autocomplete({
                source: function (request, response) {
                    var defaults = {
                        type: "Post",
                        dataType: "json",
                        //data: { ShippingChoise: '', ReportPeriod: moment().format('01-DD-YYYY') },
                        url: "",  /*May be it's possible get url from request (from app controller method json request)*/
                        success: function (data, status) {
                            //Translate all items in an array or object to new array of items (jQuery.map( array, callback ))
                            response($.map(data, function (item) {
                                return { label: item };
                            }));
                        }
                    };

                    var $mergedAjax = $.extend({}, defaults, ko.unwrap(valueAccessor()));

                    $.ajax($mergedAjax);
                },
                minLength: 1,
                autoFocus: true,
                classes: {
                    "ui-autocomplete": "highlight"
                },
                delay: 300,
                disabled: false,
                //position: { my : "right top", at: "right bottom" },
                messages: {
                    noResults: "",
                    results: function () { }
                },
                select: function (event, ui) {
                    var data = ko.unwrap(valueAccessor()).data;

                    //select single search observeble
                    for (var prop in data) {
                        if (ko.isObservable(data[prop])) {
                            data[prop](ui.item.value)
                        }
                    }
                },
                //search: function( event, ui ) {},
                change: function (event, ui) { },
            }).on('dblclick', function (ev) {
                var $element = $(ev.target);

                $element.autocomplete("close");
            });
        }
    };

    //https://www.tutorialspoint.com/bootstrap/bootstrap_collapse_plugin.htm
    ko.bindingHandlers.collapse = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                toggle: false
                //parent:
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            //$(element).collapse($merged).on("hide.bs.collapse", function () {
            //    $(".btn").html('<span class="glyphicon glyphicon-collapse-down"></span> Open');
            //}).on("show.bs.collapse", function () {
            //    $(".btn").html('<span class="glyphicon glyphicon-collapse-up"></span> Close');
            //});

        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        }
    };

    ko.bindingHandlers.btnRefresh = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();

            //selectors (tips: netxt step => ko.bindgins)
            var $btn = $(element);
            var $trg = $btn.children('span');
            var $img = $btn.find('img');

            $trg.toggleClass("glyphicon glyphicon-refresh");
            $btn.toggleClass(" btn-warning");
            $img.css("display", ko.utils.unwrapObservable(value) ? "inline" : "none");
        }
    };

})(jQuery, ko);