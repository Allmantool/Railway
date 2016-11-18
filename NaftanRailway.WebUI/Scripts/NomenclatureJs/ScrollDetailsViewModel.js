// use new ES5 syntax (IE8 => ES5-shim)
"use strict";
//custom namespace 
var app = windows.app || (function ($) {
	"use strict";
    //consructor (represent one model for row (scroll)
    function DetailRow(orcKey, sapodKey) {
        return {
            orckey: ko.observable(orcKey),
            sapodKey: ko.observable(sapodKey),
            getDiscription: function () { return "full Name" }
        };
    };
    //constuctor (reprent one model per filter)
    function DetailFilter() {
        return {
            avaibleValues: [""],
            selectedValues: [""],
            fieldname: "",
            nameDescription: "",
            state: true
        };
    };

    return {
        rows: ko.observableArray([new DetailRow(1, 100), new DetailRow(2, 100)]),
        showQuckMenu: function () { },
        applyfilter: function () { },
        pagging: { totalcount: 100, countPerPage: 20, current: 3 },
        filters: []
    };
})(jQuery)

//initial knockout
ko.applyBindings(scrollDetailViewModel);