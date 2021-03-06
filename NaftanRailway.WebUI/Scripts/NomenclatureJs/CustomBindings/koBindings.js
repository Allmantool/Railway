﻿/// <reference path="~/Scripts/jquery-1.11.3.js" />
/// <reference path="~/Scripts/knockout-3.4.2.debug.js" />
/// <reference path="~/Scripts/moment.js" />
'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//bootsrap js
appNomenclature.CustBundings = (function ($, ko) {
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
                trigger: "hover focus",
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
                template: '<div style="white-space: pre-wrap;" class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title" style="font-weight: bolder;text-align: center;"></h3><div class="small popover-content text-center"></div></div>',
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
            // and again whenever any observables/computeds that are accessed change
            // Update the DOM element based on the supplied values here.
        }
    };

    //https://uxsolutions.github.io/bootstrap-datepicker/
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                format: "MM yyyy",
                startView: 1,
                minViewMode: 1,
                language: "ru",
                autoclose: true,
                todayBtn: true, //"linked",
                orientation: "bottom auto",
                forceParse: true,
                container: 'body',//'#koContainer',
                toggleActive: true
                //defaultViewDate: new Date( 2016, 4, 1)
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            $(element).datepicker(defaults).on("changeMonth"/*changeDate changeYear changeDecade changeCentury*/, function (ev) {
                var observable = valueAccessor().observable;

                if (ko.isObservable(observable)) {
                    observable(ev.date);
                }

                ev.preventDefault();
            });

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
                backdrop: true
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
                zIndex: 9999
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

    //http://davidstutz.github.io/bootstrap-multiselect/
    ko.bindingHandlers.multiselect = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                includeSelectAllOption: true,
                selectAllJustVisible: true,
                enableFiltering: true,
                filterPlaceholder: 'Поиск...',
                enableCaseInsensitiveFiltering: true,
                enableHTML: false,
                //disableIfEmpty: true,
                disabledText: 'Нет значений ...',
                nonSelectedText: 'Не выбрано ...',
                buttonWidth: '200px',
                maxHeight: 350,
                allSelectedText: "all",
                selectAllText: "Выбрать все",
                inheritClass: false,
                buttonText: function (options, select) {
                    return 'buttonText';
                }
                //onInitialized: function (select, container) {
                //    alert('Initialized.');
                //}
                //buttonTitle: function (options, select) {
                //    return '';
                //},
                //templates: {
                //    button: '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown"></button>',
                //    ul: '<ul class="multiselect-container dropdown-menu"></ul>',
                //    filter: '<li class="multiselect-item filter"><div class="input-group"><span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span><input class="form-control multiselect-search" type="text"></div></li>',
                //    filterClearBtn: '<span class="input-group-btn"><button class="btn btn-default multiselect-clear-filter" type="button"><i class="glyphicon glyphicon-remove-circle"></i></button></span>',
                //    li: '<li><a href="javascript:void(0);"><label></label></a></li>',
                //    divider: '<li class="multiselect-item divider"></li>',
                //    liGroup: '<li class="multiselect-item group"><label class="multiselect-group"></label></li>'
                //}
            };

            var $merged = $.extend(true, defaults, ko.unwrap(valueAccessor()));

            $(element).multiselect($merged);

            //Custom disposal logic
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                //$(element).css('visibility', 'hidden');

                $(element).remove();
                $(element).multiselect('destroy');
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var $merged = $.extend({}, ko.unwrap(valueAccessor()));
        }
    };

    ko.bindingHandlers.disableClick = {
        init: function (element, valueAccessor) {

            $(element).click(function (evt) {
                if (!ko.unwrap(valueAccessor()))
                    evt.preventDefault();
            });
        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            //ko.bindingHandlers.css.update(element, function () { return { disabled_anchor: value }; });
        }
    };

    //http://stackoverflow.com/questions/14321012/prevent-event-bubbling-when-using-the-checked-binding-in-knockoutjs
    ko.bindingHandlers.stopBubble = {
        init: function (element) {
            ko.utils.registerEventHandler(element, "click", function (event) {
                event.cancelBubble = true;
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            });
        }
    };

    ko.bindingHandlers.progressBar = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                minRate: 0,
                maxRate: 100,
                textContent: ''
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));
            //html template
            $(element).append("<div data-bind='text: $root.progressBar().textContent()," +
                                        "attr: {ariaValuenow: $root.progressBar().rate()}," +
                                        "style: {width: $root.progressBar().textContent()}'" +
                        'class="progress-bar progress-bar-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100">');

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).progressBar("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
        }
    };

    ko.bindingHandlers.scroll = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var self = this;

            var defaults = {
                selector: "arrowTop",
                icon: "",
                container: "body"
            };

            var $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            $(window).on('scroll.ko.scrollHandler', function (ev) {
                var style = {
                    position: "fixed",
                    top: "85%",
                    left: "90%",
                    zIndex: '9999',
                    width: '5em',
                    height: '5em',
                    zoom: '1',
                    display: 'none',
                    opacity: '0.7'
                };

                var $template = $('<img id=' + $merged.selector + ' src=' + $merged.icon + ' alt="вверх">');
                var $sel = $('#' + $merged.selector);

                if ($sel.length === 0) {
                    $template.css(style).on('click', function (container, ev) {
                        $($merged.container).animate({ scrollTop: 0 }, "slow");
                    });

                    $(element).append($template);
                } else {
                    $sel.stop(true, false).fadeIn(200).delay(2000).fadeOut(700);
                }
            });

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(window).off("scroll.ko.scroll");
                $(window).off("resize.ko.scroll");
                $(window).off("scroll.ko.scrollHandler");
            });
        }
    };

    //http://api.jqueryui.com/dialog/#option-show
    ko.bindingHandlers.jqDialog = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var self = this;
            var valueUnwrapped = ko.unwrap(valueAccessor().state);

            var defaults = {
                appendTo: "#koContainer",
                draggable: false,
                modal: true,
                autoOpen: false,
                closeOnEscape: true,
                position: { my: "center", at: "center", of: window },
                resizable: false,
                minWidth: 50, maxWidth: 600,
                minHeight: 50, maxHeight: 500,
                title: "Выберите необходимую операцию над сбором:",
                show: {
                    effect: "blind",
                    duration: 100
                },
                hide: {
                    effect: "explode",
                    duration: 300
                },
                close: function (event, ui) {
                    valueAccessor().state(false);
                }
            };

            var $el = $(element), $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            //listen to close method
            $el.dialog($merged).on("close", function (ev) {
                var observable = valueAccessor().state;

                if (ko.isObservable(observable)) {
                    observable(false);
                }
            });

            // This will be called when the element is removed by Knockout or
            // if some other part of your code calls ko.removeNode(element)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                var $el = $(element);

                //errase unexpected rendering on destroy processing
                //$el.remove();
                $el.dialog('close');
                $el.dialog("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor().state;

            var defauls = {
            };

            if (ko.utils.unwrapObservable(value)) {
                $(element).dialog('open');
            } else {
                $(element).dialog('close');
            }
        }
    };

    ko.bindingHandlers.numeric = {
        init: function (element, valueAccessor) {
            $(element).on("keydown", function (event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
                    // Allow: Ctrl+A
                    (event.keyCode === 65 && event.ctrlKey === true) ||
                    // Allow: . ,
                    (event.keyCode === 188 || event.keyCode === 190 || event.keyCode === 110) ||
                    // Allow: home, end, left, right
                    (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            });
        }
    };

    //http://gijgo.com/tree/demos/bootstrap-treeview-checkbox
    ko.bindingHandlers.treeView = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var self = this;
            //var valueUnwrapped = ko.unwrap(valueAccessor().state);

            var defaults = {
                //autoLoad: false,
                //border: false,
                //Width of the tree.
                width: '30%',
                cascadeCheck: false,
                //Add checkbox for each node, if set to true.
                checkboxes: false,
                //Enables drag and drop functionality for each node.
                dragAndDrop: false,
                //The type of the node selection.
                //If the type is set to multiple the user will be able to select more then one node in the tree.
                //selectionType: 'single',//'multiple',
                //Collapse icon definition.
                icons: {
                    expand: '<i class="glyphicon glyphicon-plus"></i>', //Expand icon definition.
                    collapse: '<i class="glyphicon glyphicon-minus"></i>' //Collapse icon definition.
                },
                //Primary key field name.
                primaryKey: 'id',
                //Name of the source field, that indicates if the checkbox is checked.
                //checkedField: 'checkedFieldName',
                //Text field name.
                textField: 'newTextName',
                //Children field name.
                //childrenField: 'myChildrenNode',
                //The name of the UI library that is going to be in use.
                //The css file for bootstrap should be manually included if you use bootstrap.
                uiLibrary: 'bootstrap',
                //Image url field name.
                //imageUrlField: 'icon', //children: [ { text: 'USA', icon: 'http://gijgo.com/content/icons/usa-oval-icon.png' } ]
                imageHtmlField: 'icon',
                //Image css class field name.
                imageCssClassField: 'faCssClass',
                //The name of the icons library that is going to be in use. Currently we support Material Icons, Font Awesome and Glyphicons.
                //If you use Bootstrap 3 as uiLibrary, then the iconsLibrary is set to Glyphicons by default.
                //If you use Material Design as uiLibrary, then the iconsLibrary is set to Material Icons by default.
                //The css files for Material Icons, Font Awesome or Glyphicons should be manually included to the page where the grid is in use.
                iconsLibrary: 'fontawesome',
                //The data source of tree.
                //If set to string, then the tree is going to use this string as a url for ajax requests to the server.
                //If set to object, then the tree is going to use this object as settings for the jquery ajax function.
                //If set to array, then the tree is going to use the array as data for tree nodes.
                dataSource: '/wroingSourceForTree' // it may be json
                //click: function (event, ui) {
                //    //valueAccessor().state(false);
                //}
            };

            var $el = $(element), $merged = $.extend({}, defaults, ko.unwrap(valueAccessor()));

            //listen to close method
            $el.tree($merged).on("select expand", function (ev, node, id) {
                //var key = $(ev.target).closest("li").attr("data-id");
                var idNode = valueAccessor().selIdNode;

                if (ko.isObservable(idNode) && id >= 0) {
                    //set id
                    idNode(id);
                }
            }).on("collapse", function (ev, data, id) { })
              .on("dataBound", function (ev, data) { });

            // This will be called when the element is removed by Knockout or
            // if some other part of your code calls ko.removeNode(element)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                var $tree = $(element).tree;

                //unbind event handlers
                $tree.off('collapse dataBound select expand');

                $tree('destroy');
            });
        },
        //Initially when the binding is first evaluated (after the init function)
        //Any time that an observable changes that was accessed as part of the the previous execution of the update function for this binding. 
        //Bindings are implemented internally using computed observables, so any observables that have their value accessed create dependencies for the binding.
        //Any time that another binding in the same data-bind attribute is triggered. This helps ensure things like the value is appropriate when the options are changed.
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var defaults = {
                //autoLoad: false,
                //border: false,
                //Width of the tree.
                width: '30%',
                cascadeCheck: false,
                //Add checkbox for each node, if set to true.
                checkboxes: false,
                //Enables drag and drop functionality for each node.
                dragAndDrop: false,
                //The type of the node selection.
                //If the type is set to multiple the user will be able to select more then one node in the tree.
                //selectionType: 'single',//'multiple',
                //Collapse icon definition.
                icons: {
                    expand: '<i class="glyphicon glyphicon-plus"></i>', //Expand icon definition.
                    collapse: '<i class="glyphicon glyphicon-minus"></i>' //Collapse icon definition.
                },
                //Primary key field name.
                primaryKey: 'id',
                //Name of the source field, that indicates if the checkbox is checked.
                //checkedField: 'checkedFieldName',
                //Text field name.
                textField: 'newTextName',
                //Children field name.
                //childrenField: 'myChildrenNode',
                //The name of the UI library that is going to be in use.
                //The css file for bootstrap should be manually included if you use bootstrap.
                uiLibrary: 'bootstrap',
                //Image url field name.
                //imageUrlField: 'icon', //children: [ { text: 'USA', icon: 'http://gijgo.com/content/icons/usa-oval-icon.png' } ]
                imageHtmlField: 'icon',
                //Image css class field name.
                imageCssClassField: 'faCssClass',
                //The name of the icons library that is going to be in use. Currently we support Material Icons, Font Awesome and Glyphicons.
                //If you use Bootstrap 3 as uiLibrary, then the iconsLibrary is set to Glyphicons by default.
                //If you use Material Design as uiLibrary, then the iconsLibrary is set to Material Icons by default.
                //The css files for Material Icons, Font Awesome or Glyphicons should be manually included to the page where the grid is in use.
                iconsLibrary: 'fontawesome',
                //The data source of tree.
                //If set to string, then the tree is going to use this string as a url for ajax requests to the server.
                //If set to object, then the tree is going to use this object as settings for the jquery ajax function.
                //If set to array, then the tree is going to use the array as data for tree nodes.
                dataSource: '/wroingSourceForTree' // it may be json
                //click: function (event, ui) {
                //    //valueAccessor().state(false);
                //}
            };

            //The update function of a binding handler will run whenever the observables it accesses are updated
            var value = ko.unwrap(valueAccessor()) || {};
            var $el = $(element), $merged = $.extend({}, defaults, value);

            var $tree = $el.tree($merged);

            //Render data in the tree
            $tree.render(value.dataSource);
            //$tree.reload();

            //ata	        {object}	The node data.
            //parentNode	{object}	Parent node as jquery object.
            //position	    {Number}	Position where the new node need to be added.
            $tree.addNode({ text: 'New Node' });
        }
    };
}(jQuery, ko));