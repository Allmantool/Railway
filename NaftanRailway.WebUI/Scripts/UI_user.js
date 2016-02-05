/*Datepicker function
http://eonasdan.github.io/bootstrap-datetimepicker/Options/
keepInvalid
Default: false
Will cause the date picker to not revert or overwrite invalid dates.

ignoreReadonly
Default: false
Allow date picker show event to fire even when the associated input element has the readonly="readonly"property

/*http://bootstrap-datepicker.readthedocs.org/en/latest */
$('#sandbox-container .input-group').datepicker({
    format: "MM yyyy",
    startView: 2,
    minViewMode: 1,
    language: "ru",
    autoclose: true,
    todayBtn: "linked",
    orientation: "bottom auto",
    forceParse: true
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

/*Change color selecting panel */
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
        $(this).parents(".panel").removeClass('panel-default').removeClass('panel-primary').addClass('panel-success');
    });
});

/*Work with modal windows in nomenclature project*/
$('#reportShow').on('click', function() { $('.modal').modal('hide'); });


function UpdateFailure(data) {
    
}
/*Update data in confirmed row*/
function UpdateData(dataRow) {
    var target = $("#updateRow");
    target.empty().append($(dataRow).children('td'));
    var messageInfo = $('#MessageInfo');

    if (messageInfo.length === 0 ) {
        $('#wrkTable').before('<div id="MessageInfo" class="alert alert-info">Успешно добавлен перечень №' +dataRow.nper+'</div>');
    } else {
        $('#MessageInfo').val = "Успешно добавлен перечень №" +dataRow.nper;
    }
//    local.href();
}

/*Event click on table row + mark as work row for ajax request*/
$('#scrolList').on('click', function(e) {
    var td = e.target || e.srcElement;
    var srcKey = $(td).parents('tr').find('td input[class*=key]').last();
    var chkRadio = $(td).parents('tr').find('td input').first();
    var strDate = $(td).parents('tr').children("td[class*=DTBUHOTCHET]").text();

    $(td).parents('tr').addClass('info');
    chkRadio.prop("checked", true);

    /*color row (selected)*/
    $('#scrolList').children('tr').not($(td).parents('tr')).each(function(index, value) {
        if ($(value).find('.confirmed').val() === "False") {
            $(value).removeClass('info').addClass('success');
        } else {
            $(value).removeClass('info').removeClass('success');
        }
        $(value).removeAttr("id");
        $('.load').remove();
    });

    $('#gridSystemModalLabel').empty().append("Подтверждение перечня №" + srcKey.parents('td').text());
    $('#HiddenInputModal').empty().val($(srcKey[0]).val());
    $('#ReportPeriod').empty().val(strDate);

    /*Update link (parameters in link) to show correct Report server*/
    var str = "Scroll/ErrorReport?numberKrt=" + $('#HiddenInputModal').val() + "&reportYear=" + strDate.replace(/^[^\d]*(\d{4}).*$/, '$1');
    $('#reportShow').attr('href', (str));

    var chkRow = $('input[name=optionsRadios]:checked').parents('tr');
    chkRow.attr('id', 'updateRow');
    var strReportDate = chkRow.children("td[class*=DTBUHOTCHET]").text().split(/\s+/);
    chkRow.before("<tr id='loading' class='load' style='display: none' ><td colspan = '15' class='text-center'>Loading Data...</td> </tr>");
    var longKey = chkRow.find('td input[class*=key]').val();
    var strAdd = "/Scroll/Confirmed?scrollKey=" + longKey + "&period=" +
        moment(new Date(strReportDate[1],
            $.inArray(strReportDate[0], ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"]) + 1))
        .format('YYYY.MM.01');
    $('#Reglink').attr('href', (strAdd));
});


/*move to top page*/
$("a[href='#top']").on('click', function() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
    return false;
});

/* infinite scrolling (get information thoughout ajax request)*/
$(function() {
    $.support.cors = true;
    $('div#loading').hide();

    var page = 0;
    var inCallback = false;

    function loadItems() {
        if (page > -1 && !inCallback) {
            inCallback = true;
            page++;

            $('div#loading').show();

            $.ajax({
                cache: false,
                type: 'GET',
                url: 'Scroll/Index/' + page,
                success: function(data) {
                    if (data !== '') {
                        $("#scrolList").append(data);
                    } else {
                        page = -1;
                    }
                    inCallback = false;
                    $("div#loading").hide();
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
        if ($(window).scrollTop() >= $(document).height() - $(window).height() - 27) {
            loadItems();
        }
    });
});


