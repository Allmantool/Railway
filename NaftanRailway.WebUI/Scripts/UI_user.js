﻿/*http://bootstrap-datepicker.readthedocs.org/en/latest */
$('#sandbox-container .input-group').datepicker({
    format: "MM yyyy",
    startView: 1,
    minViewMode: 1,
    language: "ru",
    autoclose: true,
    todayBtn: "linked",
    orientation: "bottom auto",
    forceParse: true
}).on('changeDate', function(e) {
    var datePicker = moment($(e.date)).format('YYYY.MM.01');
    //    var strReportDate = chkRow.children("td[class*=DTBUHOTCHET]").text().split(/\s+/);
    //    moment(new Date(strReportDate[1],
    //            $.inArray(strReportDate[0], ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"]) + 1))
    //        .format('YYYY.MM.01');
}).on('show', function(e) {
    var datePicker = moment($(e.date)).format('YYYY.MM.01');
});

/*
AutoComplete shippingNumber some trouble with pass routing! 405 no allow '@Url.Action("SearchNumberShipping","Ceh18")',
function need working state datepicker
url => Specifies the URL to send the request to. Default is the current page
type => Specifies the type of request. (GET or POST)
dataType => The data type expected of the server response.
data => Specifies data to be sent to the server
*/
$(function() {
    $("#ShippingChoise").autocomplete({
        source: function(request, response) {
            $.ajax({
                url: "/Ceh18/SearchNumberShipping/",
                type: "POST",
                dataType: "json",
                data: { ShippingChoise: request.term, ReportPeriod: $("#ReportPeriod").val() },
                success: function(data) {
                    response($.map(data.slice(0, 12), function(item) {
                        return { label: item };
                    }));
                }
            });
        },
        minLength: 1,
        messages: {
            noResults: "",
            results: function() { }
        }
    }).on('dblclick', function() {
        $(this).val('');
    });
});

/*Change color selecting panel ceh18*/
$(function() {
    $("div .panel").filter("[id^='myPanel']").mouseover(function() {
        if (!$(this).hasClass('panel-success')) {
            $(this).removeClass('panel-default');
            $(this).addClass('panel-primary');
        }
    }).on('mouseout', function() {
        $(this).removeClass('panel-primary');
        $(this).addClass('panel-default');
    });
});

/*choisen accordion panel*/
$(function() {
    $(".panel .btn").on('click', function() {
        $(this).children('.glyphicon').removeClass('glyphicon-plus').addClass('glyphicon-ok');
        $(this).parents(".panel").removeClass('panel-default panel-primary').addClass('panel-success');
    });
});

/*Modal*/
//hide modal when show report
$('#reportShow').on('click', function() {
    $('.modal').modal('hide');
});
$('#dateModal').on('show.bs.modal', function(e) {
    $('#ReportPeriod').attr('value', moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
});

function UpdateFailure(data) { }
/*Update date in confirmed row(s)*/
function UpdateDate(dateRow) {
    var messageInfo = $('#loading').children('td');
    var target = $('table').find('tr input[value*=' + $(dataRow).find('td input[class*=key]').val() + ']').parents('tr'); ;

    $(target.prevAll('tr').andSelf().find('.DTBUHOTCHET')).each(function(index, item) {
        $(item).empty().append(moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
    });
    messageInfo.empty().append("Дата изменена");
    $('.modal').modal('hide');
}

/*Update data in confirmed row*/
function UpdateData(dataRow) {
    var messageInfo = $('#loading').children('td');

    var target = $('table').find('tr input[value*=' + $(dataRow).find('td input[class*=key]').val() + ']').parents('tr');

    var nper = $.trim($(dataRow).find('td input[class*=key]').parent().text());

    target.empty().append($(dataRow).children('td'));
    $(target).find('td input[class*=radio]').attr("checked", true);
    messageInfo.empty().append('Успешно добавлен перечень №' + nper);

    /*  request to ReportServer */

    $('#waitModal').modal({
        keyboard: false,
        backdrop: 'static'
    },'show');
    //error report
    window.location.href = $('#reportShow').attr('href');
    //buh report
    if ($('#myFrame').length===0) {
        $('<iframe />', {
            name: 'myFrame',
            id: 'myFrame',
            style: "display: none",  
//            load: function() {
//                alert('iframe loaded !');
//            }
        }).appendTo('body').attr("src", $('#reportShow').attr('href'));
    }

    $('#myFrame').on('onload', function() {
        console.log("load");
    }).on('onmessage',function() {
        console.log("message");
    });

    $('#myFrame').load(function() {
        console.log("load");
    });
}

/*Event click on table row + mark as work row for ajax request*/
$('#scrollList').on('click', function(e) {
    /*The target property can be the element that registered for the event or a descendant of it. 
    It is often useful to compare event.target to this in order to determine if the event is being handled due to event bubbling. 
    This property is very useful in event delegation, when events bubble.*/
    var td = $(e.target) || $(this).val();
    var chkRow = $(td).parents('tr');
    var srcKey = chkRow.find('td input[class*=key]');
    var chkRadio = chkRow.find('td input[class*=radio]');
    moment.locale('ru');
    var dpDate = moment(chkRow.children("td[class*=DTBUHOTCHET]").text(), 'MMMM YYYY').format('MMMM YYYY');

    //check(select row)
    chkRadio.prop("checked", true);
    chkRow.addClass('info');
    //    chkRow.attr('id', 'updateRow');

    /*color row (selected)*/
    $('#scrollList').children('tr').not(chkRow).each(function(index, value) {
        if ($(value).find('.confirmed').val() === "False") {
            $(value).removeClass('info').addClass('success');
        } else {
            $(value).removeClass('info success');
        }
        //Delete id Update row and loading part
        $(value).removeAttr("id");
        $('.load').remove();
    });

    //loading
    $(chkRow).before("<tr id='loading' class='load' style='display: none' >" +
        "<td colspan = '15' class='text-center'>Загрузка сборов перечня...</td> " +
      "</tr>");

    //modal window(key and report period)
    $('#gridSystemModalLabel').empty().append("Изменение отчётной даты перечня №" + $.trim(srcKey.parent().text()));
    $('#HiddenInputModal').empty().val(srcKey.val());

    $('#ReportPeriod').attr('value', dpDate);

    /*Update link (parameters in link) to show correct Report server (modal & menu links)*/
    var str = "/Scroll/ErrorReport?numberKrt=" + $('#HiddenInputModal').val() + "&reportYear=" + moment(dpDate, 'MMMM YYYY').year();
    var strDetails = "Scroll/ScrollDetails?scrollKey=" + $('#HiddenInputModal').val();

    $('#reportShow').attr('href', (str));
    $('#MenuLinkErrReport').attr('href', (str));
    $('#scrollDetails').attr("href", strDetails);
    //Confirmed
    var strAdd = "/Scroll/Confirmed?scrollKey=" + srcKey.val() + "&period=" + moment(dpDate, 'MMMM YYYY').format('DD.MM.YYYY');
    $('#Reglink').attr('href', (strAdd));

    /*change date*/
    if (td.hasClass('DTBUHOTCHET')) {
        $("#dateModal").modal('show');
    }
});

/*move to top page*/
$("a[href='#top']").on('click', function() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
    return false;
});

/* infinite scrolling (get information thoughout ajax request)*/
$(function() {
    $.support.cors = true;
    $('div#loadingInfiniteScroll').hide();

    var page = 0;
    var inCallback = false;

    function loadItems(e) {
        if (page > -1 && !inCallback) {
            inCallback = true;
            page++;

            $('div#loadingInfiniteScroll').show();

            $.ajax({
                cache: false,
                type: 'GET',
                url: 'Scroll/ScrollDetails/',
                data: { scrollKey: $('#key').text(), page: page },
                success: function(data) {
                    if (data !== '') {
                        $("#chargeOfList").append(data);
                    } else {
                        page = -1;
                    }
                    inCallback = false;
                    $("div#loadingInfiniteScroll").hide();
                }
            });
        }
    }

    /* обработка события скроллинга
    ScrollTop = полж. ползунка
    doc.Height = высота всего документа
    win.Height = высота вид. окна
    */
    $(window).on('scroll', function() {
        if (($(window).scrollTop() >= $(document).height() - $(window).height() - 21
            && window.location.pathname.indexOf("ScrollDetails")) > 0
            && navigator.appName !== "Microsoft Internet Explorer") {
            loadItems();
        }
    });
});