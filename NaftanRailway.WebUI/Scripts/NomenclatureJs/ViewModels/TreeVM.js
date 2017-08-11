"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.TreeVM = (function ($, ko) {
    /*** Data  ***/
    var self = {
        activeId: ko.observable(),
        nodes: ko.observableArray(),
        currNode: ko.pureComputed(function () {
            var id = Number(self.activeId());

            if (id >= 0) {
                return searchNode(undefined, Number(id));
            }

            //property selIdNode doesn't set
            return '';
        })//.extend({ notify: 'always' })
    };

    //recursive cast to appropriate type
    function init(data, parent) {
        $.each(data, function (indx, item) {
            item = new appNomenclature.TreeNode(item, parent);

            if (item.children().length > 0) {
                init(item.children())
            }
        });

        return data;
    }

    //search in tree with recursion
    function searchNode(searchArray, key, cancellToken) {
        var data = (searchArray === undefined) ? self.treeStructure() : searchArray;
        var result;

        $.each(data, function (indx, item) {
            if (item.id() === key) {
                result = item;
                return false;//stop loop
            }

            //recursion by children ( it continues the loop until gets nessesary node)
            result = result ? result : searchNode(item.children(), key);
            return true; // continue (new iteration)
        });

        return result;
    }

    //get tree array in json representation
    function getTreeJson() {
        return JSON.parse(ko.mapping.toJSON(self.nodes()));
    }

    /**** public API ***/
    return {
        activeId: self.activeId,
        currNode: self.currNode,

        init: init,
        nodes: ko.observableArray(),
        getTreeJson: getTreeJson
    };
}(jQuery, ko));