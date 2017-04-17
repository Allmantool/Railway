/// <reference path="~/Scripts/jquery-1.11.3.js" />
/// <reference path="../jquery-2.2.4.js" />
/// <reference path="~/Scripts/knockout-3.4.2.debug.js" />
/// <reference path="~/Scripts/moment.js" />
'use strict';

//namespace
var appAdmin = window.appAdmin || {};

//bootsrap js
appAdmin.CustBundings = (function ($, ko) {
    var self = this;

    ko.bindingHandlers.popover = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).popover('destroy');
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
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

            var popover = $(element).popover($merged).data('bs.popover');
            popover.options.content = $merged.content;
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

    ko.bindingHandlers.chat = {
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var $chat = $(element);

            var defaults = {};
            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor().data));

            if ($chat.children().length > 0) {

                //last message
                //var horisontalPoint = $chat.children().last().offset().top;

                $chat.animate({
                    scrollTop: $chat[0].scrollHeight
                }, "slow");
            }
        }
    };

})(jQuery, ko);