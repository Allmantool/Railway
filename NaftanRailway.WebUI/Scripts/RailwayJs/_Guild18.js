
/***********************AutoComplete shippingNumber some trouble with pass routing! 405 no allow '@Url.Action("SearchNumberShipping","Ceh18")',***************
function need working state datepicker
url => Specifies the URL to send the request to. Default is the current page
type => Specifies the type of request. (GET or POST)
dataType => The data type expected of the server response.
data => Specifies data to be sent to the server*/
$(function () {
    $("#ShippingChoise").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Ceh18/SearchNumberShipping/",  /*May be it's possible get url from request (from app controller method json request)*/
                type: "Post", dataType: "json",
                data: { ShippingChoise: request.term, ReportPeriod: $("#ReportPeriod").val() },
                success: function (data) {
                    //Translate all items in an array or object to new array of items (jQuery.map( array, callback ))
                    response($.map(data, function (item) {
                        return { label: item };
                    }));
                }
            });
        },
        minLength: 1,
        messages: {
            noResults: "",
            results: function () { }
        }
    }).on('dblclick', function () {
        $(this).val('');
    });
});

/************************Change color selecting panel ceh18***********************************************************/
$(function () {
    $("#Guild18Body").on("mouseover", "[id^='myPanel']", function (e) {
        var wrkElem = $(this);
        if (!wrkElem.hasClass('panel-success')) {
            wrkElem.removeClass('panel-default').addClass('panel-primary');
        }
    }).on('mouseout', "[id^='myPanel']", function (e) {
        var wrkElem = $(this);
        wrkElem.removeClass('panel-primary').addClass('panel-default');
    });
});

/***************************choisen accordion panel*********************************************************************/
$(function () {
    $(".panel .btn").on('click', function () {
        $(this).children('.glyphicon').addClass('glyphicon-ok').parents(".panel").removeClass('panel-default panel-primary').addClass('panel-success');
    });
});

/**********************************http://bootstrap-datepicker.readthedocs.org/en/latest*********************************** */
$('.datepicker').datepicker({
    format: "MM yyyy",
    startView: 1,
    minViewMode: 1,
    language: "ru",
    autoclose: true,
    todayBtn: "linked",
    orientation: "bottom auto",
    forceParse: true
}).on('changeDate', function (e) {
    var target = $('.excelExport');
    var selDate = moment($(this).datepicker('getUTCDate')).format('01-MM-YYYY');

    target.each(function (index, value) {
        var element = $(value);
        var link = element.attr('href');
        element.attr('href', link.replace(link.match("[0-9]{1,2}-[0-9]{1,2}-[0-9]{4}"), selDate));
    });

    $.ajax({
        url: "/All/Page1/Period" + moment(e.date).format('MMYYYY'),
        type: "Get", contentType: "application/x-www-form-urlencoded; charset=UTF-8", dataType: "html",
        //data: JSON.stringify({ "ReportPeriod": moment(e.date).format('01.MM.YYYY')}),
        success: function (result) {
            $("#infoArea").empty().append(result);
            //filterMenu();
        },
        error: function (data) { console.log("datepicker ajax request error:" + data) }
    });
}).on('show', function (e) {
    // var datePicker = moment($(e.date)).format('YYYY.MM.01');
});
/***************************Render modal view (preview for adding process)*********************************************************************/
function findResult(data) {
    $("#previewDeliveryModal").modal('show');
};
/*******************************Update main page ******************************************************************/
/*This type cross-domain request need CORS. In IE9- this don't support => Solusion: 1)extensiton instead XMLHttpRequest native microsoft XDomainRequest 2)don't work with cross domain request, work only server side
More information: https://learn.javascript.ru/xhr-crossdomain*/
function RenderSync() {
    $(".modal").modal('hide');
}
/*******************************Reload information about have choisen invoices on definition period******************************************************************/
//dataType: The type of data that you're expecting back from the server.
//contentType: When sending data to the server, use this content-type.
$("#updateBtn").on('click', function (e) {
    //jQuery.support.cors = true;
    //$(this).css('cursor', 'progress');
    var elem = $("#updateBtn");
    var updIcon = $("#updRefresh");
    var updLoadingPng = $("#updLoading");
    //animation
    elem.toggleClass(" btn-warning");
    updIcon.toggleClass("glyphicon glyphicon-refresh");
    updLoadingPng.css("display", "inline");
    $.ajax({
        url: "/UpdateExists",
        //crossDomain: true,
        type: "Post",
        contenType: "application/x-www-form-urlencoded; charset=UTF-8", dataType: "html",
        data: { reportPeriod: moment($('.datepicker').datepicker('getUTCDate')).format('01.MM.YYYY') },
        success: function (result) {
            //RenderSync();
            $("#infoArea").empty().append(result);
            elem.toggleClass(" btn-warning");
            updIcon.toggleClass("glyphicon glyphicon-refresh");
            updLoadingPng.css("display", "none");
        },
        error: function (data) { console.log("datepicker ajax request error:" + data) }
    });
    //$(this).css('cursor', 'auto');
});