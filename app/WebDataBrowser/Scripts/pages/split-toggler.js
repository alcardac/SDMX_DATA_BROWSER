$(window).ready(function () {

    ResizeLayout();

    $(window).resize(function () {

        ResizeLayout();

    });

});


function ResizeLayout() {

    var header_h = parseInt($("header").height());
    var menu_h = parseInt($("menu").height());
    var footer_h = parseInt($("footer").height());
    var html_h = parseInt($("html").height());

    var display_h = html_h - (footer_h + header_h + menu_h);

    $("#split-left").height(display_h);
    $("#split-right").height(display_h);
    $("#split-toggler").height(display_h);

    $("#panel_info_extra").height(display_h);

    var margin = parseInt($("#split-right").css("margin-left")) + parseInt($("#split-right").css("margin-right"));
    var padding = parseInt($("#split-right").css("padding-left")) + parseInt($("#split-right").css("padding-right"));
    var sleft_w = parseInt($("#split-left").width());
    var stogg_w = parseInt($("#split-toggler").width()) + parseInt($("#split-toggler").css("border-left-width"));
    var html_w = parseInt($("html").width());

    var display_w = html_w - (sleft_w + stogg_w + margin + padding);

    //$("#split-right").width(display_w);
}

