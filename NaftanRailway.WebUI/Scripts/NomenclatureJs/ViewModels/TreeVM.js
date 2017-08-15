"use strict";

//namespace
var appNomenclature = window.appNomenclature || {};

appNomenclature.TreeVM = (function ($, ko) {
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

    // go throught the loop and covert each node to appropriate type(class)
    //Is it another iteration or  a first loop? always obserableArray
    function _castToNode(data, destData) {
        var _deep;
        $.each(data, function (indx, item) {
            _deep = 0;
            //remove if item exists, convert to appropriate class and insert at the beginning of the array
            //with this modus we'll need to rebind nodes array to Dom, a.e containerRebind in our case. 
            //Alose footnote that we must use array without ().I mean we need use self.nodes (.children) instead self.nodes() and (.children())
            var exist = ko.utils.arrayFirst(destData(), function (x) {
                return (x.id() === item.id);
            });

            //if it's observable then convert to js
            var node = exist ? ko.mapping.toJS(destData.remove(function (x) { return x.id() === item.id; })[0]) : item;

            destData.push(new appNomenclature.TreeNode(node, _parent));

            if (node.children.length > 0) {
                //Because push methods adds a new item to the end of array.
                var listIndex = destData().length - 1;
                _deep++;
                //recursive
                _castToNode(node.children, destData()[listIndex].children);
            }
        });

        return _deep;
    }

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

        console.log('Max height of tree is: ' + _castToNode(data, self.nodes));

        return self.nodes();
    }

    //add additional nodes to selected node
    function expendNodes(data, destNode) {
        console.log(data.description());
    }

    //search in tree with recursion
    function searchNode(data, key, cancellToken) {
        //var data = (searchArray === undefined) ? self.nodes() : searchArray;
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

    /**** public API ***/
    return {
        activeId: self.activeId,
        currNode: self.currNode,

        init: init,
        nodes: self.nodes,
        getTreeJson: self.getTreeJson,
        fakeText: self.fakeText
    };
}(jQuery, ko));