﻿var xdr = new XDomainRequest();
xdr.open("get", url);
xdr.onprogress = function () { };
xdr.ontimeout = function () { };
xdr.onerror = function () { };
xdr.onload = function () {
    success(xdr.responseText);
}
setTimeout(function () { xdr.send(); }, 0);
/*******************************************************************/
if ($.browser.msie && window.XDomainRequest) {
    if (window.XDomainRequest) {
        var xdr = new XDomainRequest();
        var query = 'MyUrl';
        if (xdr) {
            xdr.onload = function () {
                alert(xdr.responseText);
            }
            xdr.onerror = function () { /* error handling here */ }
            xdr.open('GET', query);
            xdr.send();
        }
    }
}