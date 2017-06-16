'use strict';
var appAdmin = window.appAdmin || {};

//wait init signalR
appAdmin.Hub = (function ($, ko) {
    var _vm = undefined, _principalName = '';
    //var connection = $.hubConnection();
    //var hub = connection.createHubProxy("adminHub")
    var $hub = $.connection.adminHub;
    var $client = $hub.client;
    var $server = $hub.server;

    var currConnId = ko.observable("");
    var countOnline = ko.observable(0);
    var isOnline = ko.observable(false);

    $client.newMessage = function (message) {
        if (isOnline) {
            _vm.addMessage(message);
        }
    };

    $client.onConnected = function (id, userName, users) {
        countOnline(users.length);
        isOnline(true);
        currConnId(id);
        console.log(userName + ' connected (id: ' + id + '). Total count online users: ' + countOnline());
    };

    $client.onNewUserConnected = function (id, userName) {
        countOnline(countOnline() + 1);
        console.log('new user connected (id:' + id + ' name: ' + userName + ')');
    };

    $client.onUserDisconnected = function (id, userName) {
        isOnline(false);
        countOnline(countOnline() - 1);
        console.log('new user disconnected (id:' + id + ' name: ' + userName + ')');
    };

    //set related view model
    function init(vm) {
        _vm = vm;
        _principalName = vm.adminPrincipal().displayName();

        $.connection.hub.logging = true;

        //instantiation connection
        $.connection.hub.start()
            .done(function () { console.log('You have been connected, connection ID=' + $.connection.hub.id); })
            .fail(function () { console.log('Could not Connect!'); });
    }

    return {
        currConnId: currConnId,
        countOnline: countOnline,
        server: $server,
        init: init,
        isOnline: isOnline,
    };
})(jQuery, ko);