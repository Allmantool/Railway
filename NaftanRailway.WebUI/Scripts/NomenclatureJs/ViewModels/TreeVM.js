"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.TreeVM = (function ($, ko) {
    var _parent, _deep = 0;
    /*** Data  ***/
    var self = {
        activeId: ko.observable(),
        nodes: ko.observableArray([]),
        currNode: ko.pureComputed(function () {
            var id = Number(self.activeId());

            if (id >= 0) {
                return searchNode(undefined, Number(id));
            }

            //property selIdNode doesn't set
            return '';
        }),
        //get tree array in json representation
        getTreeJson: ko.pureComputed(function () {
            return JSON.parse(ko.mapping.toJSON(self.nodes()));
        }).extend({ notify: 'always' })
    };

    //recursive cast to appropriate type
    function init(data, parent) {
        //link to global VM
        _parent = parent;
        //deep of this tree
        var _deep = 0;

        //var mappingOptions = {
        //    key: function (data) {
        //        return ko.utils.unwrapObservable(data.id);
        //    },
        //    create: function (optioins) {
        //        return new appNomenclature.TreeNode(optioins.data, parent);
        //    }
        //};

        //ko.mapping.fromJS(data, mappingOptions, self.nodes);

        _castToNode(data);

        return self.nodes();
    }

    // go throught the loop and covert each node to appropriate type(class)
    function _castToNode(data, destData) {
        //Is it another iteration or  a first loop?
        destData = (destData === undefined) ? self.nodes() : destData;

        $.each(data, function (indx, item) {
            var node = new appNomenclature.TreeNode(item, _parent);
            destData[indx] = node;//destData.push(node);

            if (node.children().length > 0) {
                _deep++;
                //recursive
                _castToNode(node.children(), destData[indx].children());
            }
            return true; //continue
        });

        return console.log('The tree deep is ' + _deep);
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
    //function getTreeJson() {
    //    return JSON.parse(ko.mapping.toJSON(self.nodes()));
    //}

    /**** public API ***/
    return {
        activeId: self.activeId,
        currNode: self.currNode,

        init: init,
        nodes: self.nodes,
        getTreeJson: self.getTreeJson
    };
}(jQuery, ko));