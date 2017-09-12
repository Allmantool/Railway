'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.TreeNode = function (data, parent) {
    //private cost
    var _parent = parent;
    var self = this;

    self = $.extend(true, self, ko.mapping.fromJS(data));

    //get description for rendering purpose
    self.description = ko.pureComputed(function () {
        return self.levelName() + ': ' + self.label()
                + ' (Height: ' + self.treeLevel() + ')'
                + '. Children count: ' + self.children().length
                + '. Total charge count: ' + self.count()
                + '. Base64: ' + self.rootKey()
                + '. Byte Hash: ' + self.strKey();
    }, self);
};