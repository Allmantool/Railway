//download virtual recources for SingnalR
(function ($) {
    $.ajax({
        url: "/signalr/hubs",
        dataType: "script",
        async: false
    });
}(jQuery));