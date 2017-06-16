//var xdr = new XDomainRequest();
//xdr.open("get", url);
//xdr.onprogress = function () { };
//xdr.ontimeout = function () { };
//xdr.onerror = function () { };
//xdr.onload = function () {
//    success(xdr.responseText);
//}
//setTimeout(function () { xdr.send(); }, 0);
///*******************************************************************/
//if ($.browser.msie && window.XDomainRequest) {
//    if (window.XDomainRequest) {
//        var xdr = new XDomainRequest();
//        var query = 'MyUrl';
//        if (xdr) {
//            xdr.onload = function () {
//                alert(xdr.responseText);
//            }
//            xdr.onerror = function () { /* error handling here */ }
//            xdr.open('GET', query);
//            xdr.send();
//        }
//    }
//}

//polyfill console functionality
var alertFallback = true;
if (typeof console === "undefined" || typeof console.log === "undefined") {
    console = {};
    if (alertFallback) {
        console.log = function (msg) {
           // alert(msg);
        };
    } else {
        console.log = function () { };
    }
}

//polyfill for array indexOf functionality
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;

        var from = Number(arguments[1]) || 0;
        from = (from < 0)
             ? Math.ceil(from)
             : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (from in this &&
                this[from] === elt)
                return from;
        }
        return -1;
    };
}