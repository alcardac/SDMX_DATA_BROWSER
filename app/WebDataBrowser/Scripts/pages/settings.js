$(document).ready(function () {
    //Set the different color for the boxes - Checking if user is logged, it loads user settings or loads html settings
    if (sessionStorage.user_code != null && sessionStorage.getItem("user_settings") != null) {
        var user_settings = sessionStorage.getItem("user_settings");
        var main_css = JSON.parse(user_settings).main_css;
        var fontfamily_main = JSON.parse(user_settings).main_fontFamily;
        var fontsize_main = JSON.parse(user_settings).main_fontSize;
        var containerwidth_main = JSON.parse(user_settings).main_containerWidth;
        $('#select-font-family').val(fontfamily_main);
        $('#select-font-size').val(fontsize_main);
        $('#select-screen-size').val(containerwidth_main);
    } else {
        var main_css = "";
        var fontfamily_main = $("body").css("font-family");
        var fontsize_main = $("body").css("font-size");
        var containerwidth_main = $(".Maincontainer").css("max-width");
    }

    //Select click functions (font family, font size, screen size)
    $('#select-font-family').selectmenu({
        change: function (event, ui) {
            var font_family = $('#select-font-family').val();
            $('body').css('font-family', font_family);
        }
    });

    $('#select-font-size').selectmenu({
        change: function (event, ui) {
            var font_size = $('#select-font-size').val();
            $('body').css('font-size', font_size + 'px');
        }
    });

    $('#select-screen-size').selectmenu({
        change: function (event, ui) {
            var screen_size = $('#select-screen-size').val();
            $('.Maincontainer').css('max-width', screen_size);
        }
    });

    $(".color-option").each(function () {
        //Set the css URL
        var css_url = $(this).children(".css_url").val();
        //Detect the main css - if user is logged the system will select a custom css
        if (main_css != "" && main_css != null && main_css!=undefined) {
            var MainCSS = main_css.substring(main_css.lastIndexOf("/") + 1, main_css.length);
            var ThisCSS = css_url.substring(css_url.lastIndexOf("/") + 1, css_url.length)
            if (MainCSS == ThisCSS) {
                $(this).children("input[name='admin_color']").prop("checked", true);
                $(this).addClass(" selected");
            }
        }
            //The system sets the default css as the selected css
        else {
            $("#admin_color_standard").prop("checked", true);
            $(".color-option > #admin_color_standard").addClass(" selected");
        }
        //Preview css function
        $(this).click(function () {
            //Deselect the current css
            $('.color-option').removeClass("selected");
            //Set the selected css
            $(this).children("input[name='admin_color']").prop("checked", true);
            $(this).addClass(" selected");
            //Head last tag
            head_tag = $("head").children(":last");
            //Logo

            var logo_url = "Images/Logo/logo.png";
            if (css_url != '') {
                logo_url = css_url.replace("Content/style/custom/", "Images/Logo/logo_");
                logo_url = logo_url.replace(".css", ".png");
                logo_url = logo_url;
            }
            //CONDIZIONE: QUANDO UN UTENTE HA GIA' UN CSS PERSONALIZZATO
            if (head_tag.attr("type") == "text/css") {
                head_tag.attr('href', client.main.config.baseURL + css_url);
                $('#img_logo').attr('src', client.main.config.baseURL + logo_url);
            }
                //CONDIZIONE: SE L'UTENTE NON HA UN CSS PERSONALIZZATO
            else {
                $('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', client.main.config.baseURL + css_url));
            }
        });
    });
    //Save Settings...
    $("#saveSettings").button();
    $("#saveSettings").click(function (event) {
        event.preventDefault();
        var css_url = $(".color-option.selected").children("input[name='css_url']").val();
        var font_family = $('#select-font-family').val();
        var font_size = $('#select-font-size').val();
        var container_width = $('#select-screen-size').val();

        var usRole = null;
        if (sessionStorage.user_role != undefined) {
            usRole = JSON.parse(sessionStorage.user_role);
        };

        //VALUES SETTINGS
        var data =
        {
            userCode: sessionStorage.user_code,
            userRole: (usRole != null) ? usRole.RoleId : -1,
            setting: {
                main_fontFamily: font_family,
                main_fontSize: font_size,
                main_containerWidth: "100%",
                main_css: css_url,
            }
        };
        sessionStorage.setItem("user_settings", clientParseObjectToJson(data.setting));
        clientPostJSON(
           "Settings/SetUserSettings", clientParseObjectToJson(data),
           function (jsonString) {

               var res = clientParseJsonToObject(jsonString);
               if (res == "") return;
               if (res.hasOwnProperty('Error') && res.Error == true) return;

               $("#dialog-message-settings-ok").dialog({
                   modal: true,
                   buttons: {
                       Ok: function () {
                           $(this).dialog("close");
                       }
                   }
               });

           },
           function () {
           }, true);

    });

    //Transform RGB to HEX
    function rgb2hex(rgb) {
        rgb = rgb.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*(\d+))?\)$/);
        function hex(x) {
            return ("0" + parseInt(x).toString(16)).slice(-2);
        }
        return hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
    }

});