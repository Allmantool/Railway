'use strict';
var appAdmin = window.appAdmin || {};

//wait init signalR
//$(document).ready(function () {
    appAdmin.Hub = (function ($) {
        var _vm = undefined, _principalName ='';
        //var connnection = $.hubConnection();
        //var hub = connection.createHubProxy("adminHub")
        var $self = $.connection.adminHub;
        var $client = $self.client;
        var $server = $self.server;

        $.connection.hub.logging = true;
        //instantion connection
        $.connection.hub.start()
            .done(function () {
                //$server.connect("I'm");
                console.log('Now connected, connection ID=' + $.connection.hub.id);
            })
            .fail(function () { console.log('Could not Connect!'); });

        //self.client.on("newMessage", function (msg) {
        //});

        $client.newMessage = function (message) {
            _vm.addMessage(message);
        };

        $client.onConnected = function (id, userName, Users) {
            console.log('user connected (id:' + id + ' name: ' + _principalName + ')');
        };

        $client.onNewUserConnected = function (id, userName) {
            console.log('new user connected (id:' + id + ' name: ' + _principalName + ')');
        };

        $client.onUserDisconnected = function (id, name) {
            console.log('user disconnected (id:' + id + ' name: ' + _principalName + ')');
        };

        //set realted view model
        function init(vm) {
            _vm = vm;
            _principalName = vm.adminPrincipal().displayName();
        };

        return {
            server: $server,
            init: init
        };
    })(jQuery);
//});