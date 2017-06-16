//ready
$(document).ready(function () {
    'use strict';
    //localisation
    moment.locale('ru');

    //upper case in Month's names
    moment.updateLocale('ru', {
        months: [
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль",
            "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        ]
    });

    //wait ajax populate module prop
    appNomenclature.SrcVM.init();
});