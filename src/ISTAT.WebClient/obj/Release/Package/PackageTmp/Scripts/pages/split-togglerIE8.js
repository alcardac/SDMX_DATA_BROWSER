$(window).load(function () {

    ResizeLayout();

    $(window).resize(function () {

        ResizeLayout();

        //margin = parseInt($("#split-right").css("margin-left"));
        //html_h = parseInt($("html").css("height"));
        //split_toggler = html_h - (footer_h + header_h + menu_h);

        //$("#split-toggler").css("height", split_toggler);
        //$("#split-left").css('height', split_toggler);
        //$("#split-right").css('height', split_toggler);

        //if ($("#split-left").css("display") == "none") {

        //    var margin_st = parseInt($("#split-toggler").css("margin-left"));
        //    html_w = parseInt($("html").css("width"));
        //    sright_w = html_w - (margin_st + stogg_w + margin);
        //    $("#split-right").css("width", sright_w);

        //} else {

        //    html_w = parseInt($("html").css("width"));
        //    sright_w = html_w - (sleft_w + stogg_w + margin);
        //    $("#split-right").css("width", sright_w);

        //}

    });

    $(window).scroll(function () {

       // html_h = parseInt($("html").css("height"));
        //split_toggler = $("#Maincontent").css("height");
        //$("#split-toggler").css("height", split_toggler) + $("#Maincontent").css("margin-bottom");
        //$("#Maincontent").css("margin-bottom", "35px");

    });

});

function ResizeLayout() {

    var header_h = parseInt($("#header").height());
    var menu_h = parseInt($("#menu").height());
    var footer_h = parseInt($("#footer").height());
    var html_h = parseInt($("html").height());

    var display_h = html_h - (footer_h + header_h + menu_h);

    $("#split-left").height(display_h);
    $("#split-right").height(display_h);
    $("#split-toggler").height(display_h);

    var margin = parseInt($("#split-right").css("margin-left")) + parseInt($("#split-right").css("margin-right"));
    var padding = parseInt($("#split-right").css("padding-left")) + parseInt($("#split-right").css("padding-right"));
    var sleft_w = parseInt($("#split-left").width());
    var stogg_w = parseInt($("#split-toggler").width()) + parseInt($("#split-toggler").css("border-left-width"));
    var html_w = parseInt($("html").width());

    var display_w = html_w - (sleft_w + stogg_w + margin + padding);

    //$("#split-right").width(display_w);
}