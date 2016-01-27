/*Datepicker function
http://eonasdan.github.io/bootstrap-datetimepicker/Options/
keepInvalid
Default: false
Will cause the date picker to not revert or overwrite invalid dates.

ignoreReadonly
Default: false
Allow date picker show event to fire even when the associated input element has the readonly="readonly"property

/// <reference path="jquery-2.1.4.js" />
$(function() {
$("#datetimepicker10").datetimepicker({
viewMode: 'years',
showClose: true,
showClear: false,
locale: 'ru',
format: 'MMMM YYYY',
keepInvalid: true,
ignoreReadonly: true
}); */
/*Customize http://momentjs.com 
window.moment.locale('ru', {
months: ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль",
"Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"]
});
});
*/
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

/*choisen*/
$(function() {
    $(".panel .btn").on('click', function() {
        $(this).children('.glyphicon').removeClass('glyphicon-plus').addClass('glyphicon-ok');
        $(this).parents(".panel").removeClass('panel-default').removeClass('panel-primary').addClass('panel-success');
    });
});

/*Work with modal windows in nomenclature project*/
$('#reportShow').on('click', function() { $('.modal').modal('hide'); });
$('#scrolList').on('click', function(e) {
    var td = e.target || e.srcElement;
    var srcRow = td.parentNode.children[0];

    $('#gridSystemModalLabel').empty().append("Подтверждение перечня №" + srcRow.innerText);
    $('#HiddenInputModal').empty().val(srcRow.children[0].value);
    $('#ReportPeriod').empty().val(td.parentNode.children[4].innerText);

    /*Update link (parameters in link) to show correct Report server*/
    var str = "Scroll/ErrorReport?numberKrt=" + $('#HiddenInputModal').val() + "&reportYear=" + td.parentNode.children[4].innerText.replace(/^[^\d]*(\d{4}).*$/, '$1');
    $('#reportShow').attr('href', (str));
});



/*move to top page*/
$("a[href='#top']").on('click', function() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
    return false;
});

/* infinite scrolling */
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
        if ($(window).scrollTop() >= $(document).height() - $(window).height() - 20) {

            loadItems();
        }
    });
});


