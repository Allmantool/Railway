'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Scroll = function (parentObj) {
    var self = this;

    //if (typeof (parentObj) === 'object') {
    //    //прототипное наследование
    //    self.prototype = Object.create(parentObj);
    //    //назначаем конструктор
    //    self.prototype.constructor = appNomenclature.Scroll;

    //    /**** behaviors ***/
    //    self.active = ko.observable("0");
    //}


    //динамически копируем свойства
    for (var prop in parentObj) {
        if (parentObj.hasOwnProperty(prop)) {
            self[prop] = parentObj[prop];
        }
    }

    /**** behaviors ***/
    self.active = ko.observable("0");

};