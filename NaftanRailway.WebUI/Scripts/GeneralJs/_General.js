/********************************** Global object *********************************************************************************/
//window.cmsRail = {
//    Widgets: {},
//    Utilities: {}
//}


/*************************************** Fuction for IE8 compebility search space sign ***************************************/
if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun/*, thisArg*/) {
        'use strict';

        if (this === void 0 || this === null) {
            throw new TypeError();
        }

        var t = Object(this);
        var len = t.length >>> 0;
        if (typeof fun !== 'function') {
            throw new TypeError();
        }

        var res = [];
        var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i];

                // ПРИМЕЧАНИЕ: Технически, здесь должен быть Object.defineProperty на
                //             следующий индекс, поскольку push может зависеть от
                //             свойств на Object.prototype и Array.prototype.
                //             Но этот метод новый и коллизии должны быть редкими,
                //             так что используем более совместимую альтернативу.
                if (fun.call(thisArg, val, i, t)) {
                    res.push(val);
                }
            }
        }
        return res;
    };
}

///******************************* Resolve conflict between JQuery UI and Bootsrap UI ***********************************/
/* Change JQueryUI plugin names to fix name collision with Bootstrap.
* These commands need to be executed after the JQueryUI and before the Bootstrap js file references. 
* In the following example we can see a full implementation example in the <head> block of a common webpage:
*/

//$.widget.bridge('uitooltip', $.ui.tooltip);
//$.widget.bridge('uibutton', $.ui.button);

/*Sometimes it is necessary to use Bootstrap plugins with other UI frameworks. 
In these circumstances, namespace collisions can occasionally occur. 
If this happens, you may call .noConflict on the plugin you wish to revert the value of.
*/
var btn = $.fn.button.noConflict(); // reverts $.fn.button to jqueryui btn
$.fn.btn = btn;// assigns bootstrap button functionality to $.fn.btn


/* jquery auto-ganarate code (throught $().button = > after .noConflict bootsrap UI will not see .button method
<button type="button" class="ui-button ui-corner-all ui-widget ui-button-icon-only ui-dialog-titlebar-close" title="Close">
    <span class="ui-button-icon ui-icon ui-icon-closethick"></span>
    <span class="ui-button-icon-space"> </span>
    Close
</button>*/