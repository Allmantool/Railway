/************************************MultiSelect Bootsrap plugin (_AjaxTableKrtNaftan_ORC_SAPOD.cshtml) ****************************************************/
http://davidstutz.github.io/bootstrap-multiselect/*/
    function filterMenu(recieveData, status) {
        var selRequest = "#filterForm div>select";
        //if (recieveData.length > 0 && recieveData.length < 50) { selRequest = recieveData; } else { selRequest = "#filterForm div>select"; }
        $(selRequest).each(function () {
            var $this = $(this);
            $this.multiselect({
                includeSelectAllOption: true,
                enableHTML: false,
                disableIfEmpty: true,
                disabledText: 'Нет значений ...',
                nonSelectedText: 'Не выбрано ...',
                buttonWidth: '230px',
                maxHeight: '750px',
                allSelectedText: "allSelectedText",
                selectAllText: "Выбрать все",
                inheritClass: true,
                /*numberDisplayed: 3,*/
                /*A function which is triggered on the change event of the options. 
            Note that the event is not triggered when selecting or deselecting options using the select and deselect methods provided by the plugin.*/
                //onChange: function (option) {
                //    var $selMulti = $(option).parent('select');

                //    if ($selMulti.next().find('li.active').size() === 0) { return; }
                //    $.ajax({
                //        url: "UserInterface/FilterMenu/",
                //        type: "Post",
                //        traditional: true,
                //        contentType: 'application/json; charset=utf-8',
                //        /*Json pass throuhg out HttpPost, $.param (+change for httpGet)*/
                //        data: JSON.stringify({
                //            "typeFilter": 1,
                //            "numberScroll": $("#numberScroll").val(),
                //            "reportYear": $("#reportYear").val(),
                //            "filters": (function () {
                //                var filters = [];

                //                filters.push.apply(filters, $('#filterForm>div').map(function (i, val) {
                //                    var $filter = $(val).attr('id');
                //                    var $filterName = $(val).find("input[name$='FieldName']").val();

                //                    var item = {
                //                        FieldName: $filterName,
                //                        NameDescription: $(val).find("input[name$='NameDescription']").val(),
                //                        ActiveFilter: ($filterName === $selMulti.attr('id')) ? true : false,
                //                        CheckedValues: $('#' + $filter + ' option:selected').map(function (i, item) { return item.value; }).toArray(),
                //                        AllAvailableValues: $('#' + $filter + ' option').map(function (i, item) { return item.value; }).toArray()
                //                    }

                //                    return item;
                //                }));

                //                return filters;
                //            }).call()
                //        }),
                //        success: function (result) {
                //            var sel = $(result).find('input[value=True]').parent('div').attr('id');
                //            //update filter element
                //            $('#filterForm div>select').parent('div:not(#' + sel + ')').each(function (indx, element) {
                //                $(element).replaceWith($(result).filter('#' + $(element).attr('id') + ':not(#' + sel + ')'));
                //            });

                //            filterMenu('#filterForm div>select:not(#' + sel + ')');
                //        },
                //        error: function (data) { console.log("multiselect custom error:" + data) }
                //    });
                //},
                buttonText: function (options, select) {
                    var nameFilter = $('#' + $(select).attr('id') + "block > input[name*='NameDescription']").val();
                    var $selSize = options.length;

                    if ($selSize === 0) {
                        return 'Не выбрано ...';
                    } else if ($selSize === $(select).children('option').size()) {
                        return nameFilter + '(Все)' + ' (' + $(select).children('option').length + ')';
                    } else if ($selSize > 3) {
                        return 'Выбрано ' + $selSize + ' ' + nameFilter;
                    } else {
                        var labels = [];
                        options.each(function () {
                            if ($(this).attr('label') !== undefined) {
                                labels.push($(this).attr('label'));
                            } else {
                                labels.push($(this).html());
                            }
                        });
                        return labels.join(', ') + '';
                    }
                }
            });
            $this.next(".btn-group").css("margin-left", "0.3em");
        });
        PaggingSuccess();
    };

/******************************************************* DatePicker UI **************************************************************************************/
$('.datepicker').datepicker({
    format: "MM yyyy",
    startView: 1,
    minViewMode: 1,
    language: "ru",
    autoclose: true,
    todayBtn: "linked",
    orientation: "bottom auto",
    forceParse: true
}).on('changeDate', function () {
}).on('show', function () {
    // var datePicker = moment($(e.date)).format('YYYY.MM.01');
});

/******************************************************* Modal **************************************************************************************/
//hide modal when show report
$('#reportShow').on('click', function () {
    $('.modal').modal('hide');
});
$('#dateModal').on('show.bs.modal', function () {
    $('#ReportPeriod').attr('value', moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
});
/********************************************************* Dialog UI (quick Menu) *********************************************************************/
$('body').on('click', '#chargeOfList>tr>td', function () {
    var $selCell = $(this);
    var $selRow = $selCell.parent('tr');
    var $preveosSelRow = $('#chargeOfList>tr.info');
    var $idPayment = $selRow.find('.idPayment');
    var $menuIdsPayment = $('body .feeKey');

    //refresh hightlight area
    $preveosSelRow.removeClass('info');
    $selRow.addClass('info');

    //set value in inputs elemnt for binding on the server side
    $menuIdsPayment.val($idPayment.text());

    $("#dialog").dialog({
        autoOpen: false,
        closeOnEscape: true,
        position: { my: "left top", at: "left bottom", of: $selCell },
        resizable: false,
        width: 80,
        title: "Find / Edit / Delete",
        show: {
            effect: "blind",
            duration: 100
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        close: function () {
            $selRow.removeClass('info');
        }
    });

    $("#dialog").dialog("open");
});

/**************************************** General alert msgBox **************************************************************************************/
function reDrawingDetail(recieveRequest, status) {
    var $result = $(recieveRequest.responseText);

    switch (status) {
        case "success":

            $('#detailsTable').find('#chargeOfList').html($result.find('#chargeOfList'));
            $('#detailsTable').filter('#loadingInfiniteScroll, #dialog').html($result.filter('#loadingInfiniteScroll, #dialog'));

            filterMenu();
            break;
        case "error":
            var $snippet = $(htmlSnippet).draggable();
            var $alertDom = $('#alertMsg');

            //add or update exist
            $alertDom == null ? $('body').append($snippet) : $alertDom.replaceWith($snippet);
            break;
        default:
            var reportStatus = {
                file: "_Scroll.js",
                message: "json status",
                Mode: status
            }
            return JSON.stringify(reportStatus);
    }
}

/********************************************************** Update date in confirmed row(s) **************************************************************************/
function UpdateDate(dateRow) {
    var $messageInfo = $('#loading').children('td');
    var $currentNkrt = $(dateRow).find('.numberScroll').first().text();
    var $target = $('table').find(".numberScroll:contains('" + $currentNkrt + "')").parents('tr');

    if ($('#valMultiDate').val() === "True") {
        $($target.prevAll('tr').andSelf().find('.DTBUHOTCHET')).each(function (index, item) {
            $(item).empty().append(moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
        });
    } else {
        $target.find('.DTBUHOTCHET').empty().append(moment($('#ReportPeriod').val(), 'MMMM YYYY').format('MMMM YYYY'));
    }

    $messageInfo.empty().append("Дата изменена");
    $('.modal').modal('hide');
}

/**********************************Update data in confirmed row + spinner (wait ssrs)****************************************************************************/
function UpdateData(dataRow) {
    var messageInfo = $('#loading').children('td');
    var currentNkrt = $(dataRow).find('.numberScroll').text();
    var target = $('table').find(".numberScroll:contains('" + currentNkrt + "')").parents('tr');

    target.empty().append($(dataRow).children('td'));
    $(target).find('td input[class*=radio]').attr("checked", true);
    messageInfo.empty().append('Успешно добавлен перечень №' + currentNkrt);

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

    var refreshIntervalId = window.setInterval(function () { //monitor for existence of cookie 
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

/************************************** Event click on table row + mark as work row for ajax request (event delegation) *******************************************/
$('body').on('click', '#scrollList tr', function (e) {
    /*The target property can be the element that registered for the event or a descendant of it. 
    It is often useful to compare event.target to this in order to determine if the event is being handled due to event bubbling. 
    This property is very useful in event delegation, when events bubble.*/
    var td = $(e.target) || $(this).val();
    var chkRow = $(this);
    var chkRadio = chkRow.find('td input[class*=radio]');
    moment.locale('ru');
    var nkrt = chkRow.find('.numberScroll').text(); //    var srcKey = chkRow.find('td input[class*=key]');
    var dpDate = moment(chkRow.children("td[class*=DTBUHOTCHET]").text(), 'MMMM YYYY').format('MMMM YYYY');

    //check(select row)
    chkRadio.prop("checked", true);
    chkRow.addClass('info');
    //    chkRow.attr('id', 'updateRow');

    /*color row (selected)*/
    $('#scrollList').children('tr').not(chkRow).each(function (index, value) {
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
    $('#gridSystemModalLabel').empty().append("Изменение отчётной даты перечня №" + $.trim(nkrt));
    $('#Nkrt').empty().val(nkrt);

    $('#ReportPeriod').attr('value', dpDate);

    /*Update link (parameters in link) to show correct Report server (modal & menu links)*/
    var arg = $.trim($('.info .numberScroll').text()) + "/" + moment(dpDate, 'MMMM YYYY').year();
    var correctionState = (($('.info .singCorrection span').attr('class').length > 0) ? "krt_Naftan_Scroll_compare_Correction" : "krt_Naftan_Scroll_Compare_Normal");
    var errorReportStr = "Scroll/Reports/" + correctionState + "/" + arg;
    var bookKeeperStr = "Scroll/Reports/krt_Naftan_BookkeeperReport/" + arg;
    var strDetails = "Scroll/ScrollDetails/" + arg;
    var actReportStr = 'Scroll/Reports/krt_Naftan_act_of_Reconciliation/' + arg;;

    $('#reportShow').attr('href', errorReportStr);
    $('#MenuLinkErrReport').attr('href', errorReportStr);
    $('#MenuLinkBookKeeperReport').attr('href', bookKeeperStr);
    $('#scrollDetails').attr("href", strDetails);
    $('#MenuLinkReconciliationActReport').attr('href', actReportStr);

    //Confirmed
    var strAdd = "Scroll/Confirmed/" + arg;
    $('#Reglink').attr('href', (strAdd));

    /*change date*/
    if (td.hasClass('DTBUHOTCHET')) {
        $("#dateModal").modal('show');
    }
});

/*****************************************change mode date update (multiDate in intext.csthml (scroll controller))************************************************/
$('#multiDateRadio input').on('change', function () {
    $('#valMultiDate').attr('value', $(this).val());
});

/*move to top page*/
$("a[href='#top']").on('click', function () {
    $("html, body").animate({ scrollTop: 0 }, "slow");
    return false;
});

/********************************ajax pagging (index.cshtml & etc)**************************************************************************/
function PaggingSuccess(e) {
    /*Dont support in Html4 browsers (IE8)
    Solustion: https://github.com/browserstate/history.js*/
    History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
        var state = History.getState(); // Note: We are using History.getState() instead of event.state
        console.log(state);
    });

    History.pushState(null, null, $(e).find('.active a').attr('href'));

    //    window.location = $(e).find('.active a').attr('href');
    //    location.assign($(e).find('.active a').attr('href'));
    $("html, body").animate({ scrollTop: 0 }, 0);
}