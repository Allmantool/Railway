﻿/*Fuction for IE8 compebility search space sign*/
if (!Array.prototype.filter) {
    Array.prototype.filter = function(fun/*, thisArg*/) {
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

/*http://bootstrap-datepicker.readthedocs.org/en/latest */
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

/*MultiSelect Bootsrap plugin*/
$(function() {
    $('#nkrt').multiselect({
        includeSelectAllOption: true,
        enableHTML: false,
        disableIfEmpty: true,
        disabledText: 'Нет значений ...',
        nonSelectedText: 'Не выбрано ...',
        buttonWidth: '190px',
        maxHeight: 510,
        allSelectedText: '№ Карточки: (Все)',
        selectAllText: '№ Карточки: (Все)',
        inheritClass: true,
        /*numberDisplayed: 3,
        delimiterText: '; ',*/
        /*checkboxName: 'multiselect[]' (for server side binding)*/
        /*A function which is triggered on the change event of the options. 
        Note that the event is not triggered when selecting or deselecting options using the select and deselect methods provided by the plugin.
        onChange: function(option, checked, select) {
            alert('Changed option ' + $(option).val() + '.');
        }*/
         buttonText: function(options, select) {
                if (options.length === 0) {
                    return 'Не выбрано ...';
                }
                else if (options.length === $(select).children('option').size()) {
                    return '№ Карточки: (Все)' + ' (' + $(select).children('option').length + ')';
                }
                else if (options.length > 3) {
                    return 'Выбрано '+options.length+' карточек';
                }
                 else {
                     var labels = [];
                     options.each(function() {
                         if ($(this).attr('label') !== undefined) {
                             labels.push($(this).attr('label'));
                         }
                         else {
                             labels.push($(this).html());
                         }
                     });
                     return labels.join(', ') + '';
                }
        }
});
    $('#tdoc').multiselect({
        includeSelectAllOption: true,
        selectAllText: 'Тип документа: (Все)',
        allSelectedText: 'Тип документа: (Все)',
        nonSelectedText: 'Не выбрано',
        disableIfEmpty: true,
        buttonText: function(options, select) {
            if (options.length === 0) {
                return 'Не выбрано ...';
            }
            else if (options.length === $(select).children('option').size()) {
                return 'Тип документа: (Все)' + ' (' + $(select).children('option').length + ')';
            }
            else if (options.length > 3) {
                return 'Выбрано ' + options.length + ' типов док.';
            }
            else {
                var labels = [];
                options.each(function() {
                    if ($(this).attr('label') !== undefined) {
                        labels.push($(this).attr('label'));
                    }
                    else {
                        labels.push($(this).html());
                    }
                });
                return labels.join(', ') + '';
            }
        }
    });
    $('#DDMenuVidsbr').multiselect({
        includeSelectAllOption: true,
        selectAllText: '№ Сбора: (Все)',
        allSelectedText:'№ Сбора: (Все)',
        numberDisplayed: 7,
        nonSelectedText: 'Не выбрано',
        disableIfEmpty: true,
        nSelectedText: ' кодов сборов выбрано',
        buttonWidth: '190px',
        buttonText: function(options, select) {
            if (options.length === 0) {
                return 'Не выбрано ...';
            }
            else if (options.length === $(select).children('option').size()) {
                return '№ Сбора: (Все)' + ' (' + $(select).children('option').length + ')';
            }
            else if (options.length > 3) {
                return 'Выбрано ' + options.length + ' сборов';
            }
            else {
                var labels = [];
                options.each(function() {
                    if ($(this).attr('label') !== undefined) {
                        labels.push($(this).attr('label'));
                    }
                    else {
                        labels.push($(this).html());
                    }
                });
                return labels.join(', ') + '';
            }
        }
    });
});

/*AutoComplete shippingNumber some trouble with pass routing! 405 no allow '@Url.Action("SearchNumberShipping","Ceh18")',
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
                url: "/Ceh18/SearchNumberShipping/",  /*May be it's possible get url from request (from app controller method json request)*/
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
/*edit modal (correction)*/
$('#chargeOfList').on('click','tr', function(e) {
//    var selRow = $(e.target).parent('tr');
    var selRow = $(this);
    var _parse = function(name) {
        var element = selRow.find(name),
            text = element.text();
        return parseInt(text.split('').filter(function(i) { return !isNaN(parseInt(i)) }).join(''));
    }
    
    $('#gridSystemModalLabel').html('Первичный документ: ' + selRow.find('.nomot').text() + '&nbsp;&nbsp;&nbsp;' + 'Код сбора № ' + selRow.find('.vidsbr').text());
    var summa = _parse(".summa");
    var sm = _parse(".sm");
    var sm_nds = _parse(".sm_nds"); 
    var nds = _parse(".nds"); 
    var sm_no_nds =  _parse(".sm_no_nds"); 

    $('label[for=sm]').text(sm);
    $('#sm').val(sm);

    $('label[for=sm_nds]').text(sm_nds);
    $('#sm_nds').val(sm_nds);

    $('label[for=sm_no_nds]').text(sm_no_nds);
    $('label[for=summa]').text(summa + nds);
    $('#summa').val(summa);

    $('#nds').val(nds);
    $('#nomot').val(selRow.find('.nomot').text());
    $('#vidsbr').val(selRow.find('.vidsbr').text());

    $("#EditModal").modal('show');
});

$('#summa').on('input', function() {
    $('label[for=summa]').text(parseInt($('#nds').val()) + parseInt(this.value));
});
$('#nds').on('input', function() {
    $('label[for=summa]').text(parseInt($('#summa').val()) + parseInt(this.value));
});
/*IE8*/
$('#summa').on('propertychange', function() {
    $('label[for=summa]').text(parseInt($('#nds').val()) + parseInt(this.value));
});
$('#nds').on('propertychange', function() {
    $('label[for=summa]').text(parseInt($('#summa').val()) + parseInt(this.value));
});

function UpdateFailure(data) { }

/*Update date in confirmed row(s)*/
function UpdateDate(dateRow) {
    var messageInfo = $('#loading').children('td');
    var target = $('table').find('tr input[value*=' + $(dateRow).find('td input[class*=key]').val() + ']').parents('tr');

    $(target.prevAll('tr').andSelf().find('.DTBUHOTCHET')).each(function(index, item) {
        $(item).empty().append(moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
    });
    messageInfo.empty().append("Дата изменена");
    $('.modal').modal('hide');
}

/*Update data in confirmed row + spinner (wait ssrs)*/
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
    }, 'show');
    //error report

    var reportStr = $('#reportShow').attr('href');
//    window.location.assign( $('base').attr('href') + reportStr);
    if ($('#errFrame').length === 0) {
        $('<iframe />', {
            name: 'errFrame',
            id: 'errFrame',
            style: "display: none"
        }).appendTo('body').attr("src", reportStr);
    } else {
        //refresh
        $('#errFrame').attr('src', reportStr);
    }

    //buh report
    /* var arrSplit = reportStr.split('/');
    arrSplit[2] = 'krt_Naftan_BookkeeperReport';
    if ($('#BuhFrame').length === 0) {
        $('<iframe />', {
            name: 'BuhFrame',
            id: 'BuhFrame',
            style: "display: none"
        }).appendTo('body').attr("src", arrSplit.join('/'));
    } else {
        //refresh
        $('#BuhFrame').attr('src', arrSplit.join('/'));
    } */

    var refreshIntervalId = window.setInterval(function() { //monitor for existence of cookie 
        var cookieValue = $.cookie("SSRSfileDownloadToken"); // **uses jquery.cookie plugin
        if (cookieValue === "true") {
            // 100 %
            $('#waitModal .progress-bar').css('width', '100%');
            $('#waitModal').modal('hide');
            $.cookie('SSRSfileDownloadToken', null, { expires: -1 }); //clears cookie
            clearInterval(refreshIntervalId);
        } else {
            $('#waitModal .progress-bar').css('width', '50%');
        }
    }, 500); //interval is time before re-running this function
}

/*Scroll Details fix table update*/
function FixUpdate(dataRow) {
    $('#chargeOfList').empty().append($(dataRow));
    $('#EditModal').modal('hide');
}

/*Event click on table row + mark as work row for ajax request (event delegation)*/
$('#scrollList').on('click', 'tr', function(e) {
    /*The target property can be the element that registered for the event or a descendant of it. 
    It is often useful to compare event.target to this in order to determine if the event is being handled due to event bubbling. 
    This property is very useful in event delegation, when events bubble.*/
    var td = $(e.target) || $(this).val();
    var chkRow = $(this);
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
    var arg = $.trim($('.info .numberScroll').text()) + "/" + moment(dpDate, 'MMMM YYYY').year();
    var correctionState = (($('.info .singCorrection span').attr('class').length > 0) ? "krt_Naftan_Scroll_compare_Correction" : "krt_Naftan_Scroll_Compare_Normal");
    var errorReportStr = "Scroll/Reports/" + correctionState + "/" + arg;
    var bookKeeperStr = "Scroll/Reports/krt_Naftan_BookkeeperReport/" + arg;
    var strDetails = "Scroll/ScrollDetails/" + arg;
    var strCorrection = "Scroll/ScrollCorrection/" + arg;

    $('#reportShow').attr('href', errorReportStr);
    $('#MenuLinkErrReport').attr('href', errorReportStr);
    $('#MenuLinkBookKeeperReport').attr('href', bookKeeperStr);
    $('#scrollDetails').attr("href", strDetails);
    $('#scrollFix').attr("href", strCorrection);

    //Confirmed
    var strAdd = "Scroll/Confirmed/" + arg;
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
                url: 'Scroll/ScrollDetails/' + $('#key').text() + '/' + $('#scrollDate').text(),
               data: { page: page },
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
        if (($(window).scrollTop() >= $(document).height() - $(window).height() - 31
            && window.location.pathname.indexOf("ScrollDetails")) > 0
            && navigator.appName !== "Microsoft Internet Explorer") {
            loadItems();
        }
    });
});