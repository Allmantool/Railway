/// <reference path="../jquery-1.11.3.js" />
"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.TreeVM = (function ($, ko, db) {
    var _parent;
    /*** Data  ***/
    var self = {
        fakeText: ko.observable(),
        activeId: ko.observable(),
        nodes: ko.observableArray([]),
        currNode: ko.pureComputed(function () {
            var id = Number(self.activeId());

            if (id >= 0) {
                var node = searchNode(self.nodes(), Number(id));
                //expand
                expendNodes.call(this, node);

                return node;
            }
            //property selIdNode doesn't set
            return '';
        }).extend({ rateLimit: 10 }),
        //get tree array in json representation
        getTreeJson: ko.pureComputed(function () {
            return JSON.parse(ko.mapping.toJSON(self.nodes));
        }).extend({ notify: 'always' })
    };

    // go through the loop and covert each node to appropriate type(class)
    //Is it another iteration or  a first loop? always obserableArray
    function castToNode(data, destData) {
        var deep;
        $.each(data, function (indx, item) {
            //deep of this tree
            deep = 0;
            //remove if item exists, convert to appropriate class and insert at the beginning of the array
            //with this modus we'll need to rebind nodes array to Dom, a.e containerRebind in our case.
            //Also footnote that we must use array without ().I mean we need use self.nodes (.children) instead self.nodes() and (.children())
            var exist = ko.utils.arrayFirst(destData(), function (x) {
                return (x.id() === item.id);
            });

            //if it's observable then convert to js
            var node = exist ? ko.mapping.toJS(destData.remove(function (x) { return x.id() === item.id; })[0]) : item;

            destData.push(new appNomenclature.TreeNode(node, _parent));

            if (node.children.length > 0) {
                //Because push methods adds a new item to the end of array.
                var listIndex = destData().length - 1;
                deep++;
                //recursive
                castToNode(node.children, destData()[listIndex].children);
            }
        });

        return deep;
    }

    //recursive cast to appropriate type
    function init(data, parent) {
        //link to global VM
        _parent = parent;

        //var mappingOptions = {
        //    key: function (data) {
        //        return ko.utils.unwrapObservable(data.id);
        //    },
        //    create: function (options) {
        //        return new appNomenclature.TreeNode(optioins.data, parent);
        //    }
        //};
        //ko.mapping.fromJS(data, mappingOptions, self.nodes);

        console.log('Max height of tree is: ' + castToNode(data, self.nodes));

        return self.nodes();
    }

    //add additional nodes to selected node
    function expendNodes(data, destNode) {
        console.log(data.description());

        db.getScr(function (data) {
            console.log("Program've received " + data.length + ' nodes.');

        }, {
            url: "/api/DocTree/" + data.treeLevel() + '/' + data.rootKey(),
            type: "Get",
            //data: ko.mapping.toJSON({ 'typeDoc': data.treeLevel(), 'rootKey': data.rootKey() }),
            beforeSend: function () { _parent.loadingState(true); },
            complete: function () {
                _parent.loadingState(false);
            },
            error: function () { _parent.alert().statusMsg('К сожалению не удалось получить данные от сервиса!').alertType('alert-danger').mode(true); }
        });
    }

    //search in tree with recursion
    //data - source for search
    //key - some indification indication
    function searchNode(data, key) {
        //var data = (searchArray === undefined) ? self.nodes() : searchArray;
        var result;

        $.each(data, function (indx, item) {
            if (item.id() === key) {
                result = item;
                return false;//stop loop
            }

            //recursion by children ( it continues the loop until gets necessary node)
            result = result ? result : searchNode(item.children(), key);
            return true; // continue (new iteration)
        });

        return result;
    }

    /**** public API ***/
    return {
        activeId: self.activeId,
        currNode: self.currNode,

        init: init,
        nodes: self.nodes,
        getTreeJson: self.getTreeJson,
        fakeText: self.fakeText
    };
}(jQuery, ko, appNomenclature.DataContext));