$(document).ready(function () {
    $("#Mainform").css('overflow','hidden');
    $("#close-left").click(function () {
        ToggleSplitter();
    });
});


function ToggleSplitterOpen() {

    $("#split-left").css("display", 'block');

    $("#close-left").switchClass("close-left", "open-left");
    //$("#close-left > .icon-left-dir").switchClass("icon-right-dir", "icon-left-dir");
    $("#close-left > .icon-right-dir").switchClass("icon-right-dir", "icon-left-dir");

    $("#split-toggler").addClass("closed-toggler");
    $("#split-toggler").css("left", '25%');

    var margin = parseInt($("#split-right").css("margin-left"));
    var margin_st = parseInt($("#split-toggler").css("margin-left"));
    var stogg_w = parseInt($("#split-toggler").width()) + parseInt($("#split-toggler").css("border-left-width"));
    var html_w = parseInt($("html").width());
    var sright_w = html_w - (margin_st + stogg_w + margin);

    //$("#split-right").css("width", sright_w);
    $("#split-right").css("width", "73%");
}
function ToggleSplitterClose() {

    $("#split-left").css("display",'none');

    $("#close-left").switchClass("open-left", "close-left");
    $("#close-left > i").switchClass("icon-left-dir", "icon-right-dir");

    $("#split-toggler").removeClass("closed-toggler");
    $("#split-toggler").css("left", 0);

    var margin = parseInt($("#split-right").css("margin-left"));
    var sleft_w = parseInt($("#split-left").width());
    var stogg_w = parseInt($("#split-toggler").width()) + parseInt($("#split-toggler").css("border-left-width"));
    var html_w = parseInt($("html").width());
    var sright_w = html_w - (sleft_w + stogg_w + margin);

    //$("#split-right").css("width", sright_w);
    $("#split-right").css("width", "98%");
}
function ToggleSplitter() {

    var leftDisplay = $("#split-left").css("display");
    if (leftDisplay == "none") {
        ToggleSplitterOpen();
    } else {
        ToggleSplitterClose();
    }
}