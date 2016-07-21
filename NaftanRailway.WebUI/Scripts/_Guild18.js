﻿
/*AutoComplete shippingNumber some trouble with pass routing! 405 no allow '@Url.Action("SearchNumberShipping","Ceh18")',
function need working state datepicker
url => Specifies the URL to send the request to. Default is the current page
type => Specifies the type of request. (GET or POST)
dataType => The data type expected of the server response.
data => Specifies data to be sent to the server
*/
$(function () {
    $("#ShippingChoise").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Ceh18/SearchNumberShipping/",  /*May be it's possible get url from request (from app controller method json request)*/
                type: "Post",
                dataType: "json",
                data: { ShippingChoise: request.term, ReportPeriod: $("#ReportPeriod").val() },
                success: function (data) {
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
        $(this).children('.glyphicon').removeClass('glyphicon-plus').addClass('glyphicon-ok').parents(".panel").removeClass('panel-default panel-primary').addClass('panel-success');
    });
});

/**********************************http://bootstrap-datepicker.readthedocs.org/en/latest*********************************** */
$('#sandbox-container .input-group').datepicker({
    format: "MM yyyy",
    startView: 1,
    minViewMode: 1,
    language: "ru",
    autoclose: true,
    todayBtn: "linked",
    orientation: "bottom auto",
    forceParse: true
}).on('changeDate', function (e) {
    //$(location).attr('href', '/All/Page1/Period' + moment(e.date).format('MMYYYY'));
    $.ajax({
        url: "/All/Page1/Period" + moment(e.date).format('MMYYYY'),
        type: "Get", contentType: "application/x-www-form-urlencoded; charset=UTF-8",
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
function RenderSync() {
    $(".modal").modal('hide');
    $.ajax({
        url: "/",
        type: "Get", contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (result) {
            $("#infoArea").empty().append(result);
        },
        error: function (data) { console.log("datepicker ajax request error:" + data) }
    });
}
/*******************************Reload information about have choisen invoices on definition period******************************************************************/
$("#updateBtn").on('click', function () {
    $.ajax({
        url: "/UpdateExists",
        type: "Post", contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        data: { reportPeriod: moment($('#ReportPeriod').val(), "mmmm YYYY").format('01.MM.YYYY') },
        success: function () {
            RenderSync();
        },
        error: function (data) { console.log("datepicker ajax request error:" + data) }
    });
});