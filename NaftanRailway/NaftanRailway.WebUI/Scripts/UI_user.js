/*Datepicker function
http://eonasdan.github.io/bootstrap-datetimepicker/Options/
keepInvalid
Default: false
Will cause the date picker to not revert or overwrite invalid dates.

ignoreReadonly
Default: false
Allow date picker show event to fire even when the associated input element has the readonly="readonly"property
*/
$(function () {
    $("#datetimepicker10").datetimepicker({
        viewMode: 'years',
        showClose: true,
        showClear: false,
        locale: 'ru',
        format: 'MMMM YYYY',
        keepInvalid: true,
        ignoreReadonly: true
    });
    /*Customize http://momentjs.com */
    moment.locale('ru', {
        months: [
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль",
            "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"]
    });
});

/*
AutoComplete shippingNumber some trouble with pass routing! 405 no allow '@Url.Action("SearchNumberShipping","Ceh18")',
function need working state datepicker
*/
$(function() {
    $("#ShippingChoise").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Railway/Ceh18/SearchNumberShipping/",
                type: "POST",
                dataType: "json",
                data: { ShippingChoise: request.term, ReportPeriod: $("#ReportPeriod").val() },
                success: function (data) {
                    response($.map(data.slice(0,12), function (item) {
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
    }).dblclick(function(e) {
        $(this).val('');
    });
});

/*Change color selecting panel */
$(function() {
    $("div .panel").filter("[id^='myPanel']").mouseover(function(e) {
        if (!$(this).hasClass('panel-success')) {
            $(this).removeClass('panel-default');
            $(this).addClass('panel-primary');              
        }
    }).mouseout(function(e) {
        $(this).removeClass('panel-primary');
        $(this).addClass('panel-default');
    });
});

/*choisen*/
$(function() {
    $(".panel .btn").click(function(e) {
        $(this).children('.glyphicon').removeClass('glyphicon-plus').addClass('glyphicon-ok');
        $(this).parents(".panel").removeClass('panel-default').removeClass('panel-primary').addClass('panel-success');
    });
});

