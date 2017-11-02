//ready
/// <reference path="../jquery-1.11.3.js" />
/// <reference path="../moment.js" />
'use strict';
$(document).ready(function () {

    //localisation
    moment.locale('ru');

    //upper case in Month's names
    moment.updateLocale('ru', {
        months: [
            "Январь", "Февраль", "Март",
            "Апрель", "Май", "Июнь", "Июль",
            "Август", "Сентябрь", "Октябрь",
            "Ноябрь", "Декабрь"
        ]
    });

    //wait ajax populate module prop
    appNomenclature.SrcVM.init();
});