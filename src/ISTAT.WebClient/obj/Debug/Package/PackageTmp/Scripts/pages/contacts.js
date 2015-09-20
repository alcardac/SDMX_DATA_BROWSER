$(document).ready(function () {

    $('.input-field').each(function () {
        $(this).keydown(function (event) {
            if ($(this).is(":invalid")) {
                $(this).css('border-color', '#da0d14');
                $("#err-" + $(this).attr("id")).css('visibility', 'visible');
            }
            else {
                $(this).css('border-color', '#A9A9A9');
                $("#err-" + $(this).attr("id")).css('visibility', 'hidden');
            }
        });
    });
    $("input[type=submit]")
    .button()
    .click(function (event) {
        event.preventDefault();
        //ERRORS CHECKING
        $('.input-field').each(function () {
            if ($(this).is(":invalid")) {
                $(this).css('border-color', '#da0d14');
                $("#err-" + $(this).attr("id")).css('visibility', 'visible');
            }
            else {
                $(this).css('border-color', '#A9A9A9');
                $("#err-" + $(this).attr("id")).css('visibility', 'hidden');
            }
        })
    });

});