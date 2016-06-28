
function SetupMasterPage() {

    var loadCSS = function (href) {

        var cssLink = $("<link>");
        $("head").append(cssLink); //IE hack: append before setting href

        cssLink.attr({
            rel: "stylesheet",
            type: "text/css",
            href: href
        });
    };
    loadCSS(client.main.config.baseURL + "Content/style/Tree-style.css");

    //Logout user - the system cleans all local storage
    $("#user_logout").click(function () {
        var urlGetProfile = "Profile/Logout";
        var data = "";
        clientPostJSON(urlGetProfile, data,
                function (jsonString) {
                    return;
                },
                function (event, status, errorThrown) {
                    //clientShowErrorDialog('<%= Messages.label_error_data %>');
                    //clientAjaxError(event, status);
                    return;
                },
                false);
        sessionStorage.clear("user_code");
        sessionStorage.clear("user_name");
        sessionStorage.clear("user_role");
    });

    var usr_code;
    var usr_role;

    //If user is logged...
    if (sessionStorage.user_code != null){

        usr_code = sessionStorage.user_code;

        $("#user_login").css("display", "none");
        $("#user_logout").css("display", "inline");
        $("#user_dashboard").css("display", "inline");
        $("#user_welcome").css("display", "inline");
        $("#dashboard_menu").css("display", "inline");
        $("#user_welcome").html(sessionStorage.user_name + " " + sessionStorage.user_surname + "<i class='icon-user-3'></i>");
        
        if (sessionStorage.user_role != undefined) {

            var usRole = JSON.parse(sessionStorage.user_role);

            usr_role = usRole.RoleId;

            if (usRole.RoleId == 1) {
                $(".admin_settings").css("display", "inline");
            }
        };
    }
    else {
        if (sessionStorage.view_login != undefined && sessionStorage.view_login=="true")
        { $("#user_login").css("display", "inline"); }
        else
        { $("#user_login").css("display", "none"); }

        $("#user_logout").css("display", "none");
        $("#user_dashboard").css("display", "none");
        $("#user_welcome").css("display", "none");
        $("#dashboard_menu").css("display", "none");
        $("user_welcome").html("");
    }
        //SETTINGS START
        //GET
        var css_main_url = "";

        var data = {
            userCode: usr_code,
            userRole: usr_role,
            setting: {
                main_fontFamily: "Tahoma",
                main_fontSize: "12",
                main_containerWidth: "100%",
                main_css: client.main.config.baseURL + css_main_url,
            }
        };
        clientPostJSON("Settings/GetUserSettings", clientParseObjectToJson(data),
        function (jsonString) {

            var user_settings = clientParseJsonToObject(jsonString);

            if (user_settings=="") return;
            if (user_settings.hasOwnProperty("error")) return;
            

            sessionStorage.setItem("user_settings", clientParseObjectToJson(user_settings));

            var logo_url = user_settings.main_css.replace("Content/style/custom/", "Images/Logo/logo_");
            logo_url = logo_url.replace(".css", ".png");
            logo_url = client.main.config.baseURL + logo_url;

            //SET
            $('body').css('font-family', user_settings.main_fontFamily);
            $('body').css('font-size', user_settings.main_fontSize + 'px');

            //Maincontainer max-width
            if (user_settings.main_css != '') {
                $('head').append($('<link rel="stylesheet" type="text/css" />').attr('href', client.main.config.baseURL + user_settings.main_css));
                $('#img_logo').attr('src', logo_url);
            }
        },
       function () {
       }, false);



    //Function for the font resize
    $('#font-list li').click(function () {
        $('body').css('font-size', $(this).css("font-size"));
        //WIDGET classes Start
        $('.containerT').css('font-size', $(this).css("font-size"));
        $('.container label').css('font-size', $(this).css("font-size"));
        //WIDGET classes End
    });
    //For each "html select" the function will set the customized style ".selectmenu()"

    //SET ALL SELECT
    //$("select").selectmenu();
    //Set the tabindex of the Select
    $("#select-ws-button").attr("tabindex", "4");
    $("#select-locale-button").attr("tabindex", "5");
    //Focus function for force the tabindex of the Select
    $("#select-ws-button").focus(function () {
        $("#select-ws-button").attr("tabindex", "4");
    });
    $("#select-locale-button").focus(function () {
        $("#select-locale-button").attr("tabindex", "5");
    });
    //For each accesskey set underline style
    $('a[accesskey]').each( //selects only those a elements with an accesskey attribute
    function () {
        var aKey = $(this).attr('accesskey'); // finds the accesskey value of the current a
        var text = $(this).text(); // finds the text of the current a
        var newHTML = text.replace(aKey, '<u>' + aKey + '</u>');
        // creates the new html having wrapped the accesskey character with a span
        $(this).html(newHTML); // sets the new html of the current link with the above newHTML variable
    });

    $('#select-locale').selectmenu({
        change: function (event, ui) {
            sessionStorage.setItem("select-locale", $(this).val());
            ChangeLocale($(this).val());
        }
    });

}

function ChangeLocale(twoLetterIso) {
    var data = {
        locale: twoLetterIso
    };
    var dataTreeLocale = null;
    var UrlChangeLocale = "Settings/SetLocale";


    clientPostJSON(
        UrlChangeLocale, clientParseObjectToJson(data),
        function (jsonString) {

            location.reload();
            /*
            var jsonData = clientParseJsonToObject(jsonString);
            var resultTree = null;
            var resultMessage = null;
            $.ajax(
                {
                    url: client.main.config.baseURL+"Main/GetTreeLocale",
                    type: 'post',
                    dataType: 'json',
                    async: false,
                    cache: false,
                    success: function (datareturn) {
                        resultTree = clientParseJsonToObject(datareturn);
                    }
                }
            );

            $.ajax(
                 {
                     url: client.main.config.baseURL+"Settings/GetMessages",
                     type: 'post',
                     dataType: 'json',
                     async: false,
                     cache: false,
                     success: function (datareturn) {
                         resultMessage = datareturn;

                         clientRetrieveMessagesChangeLocale(data.locale, resultMessage);
                         DrawHTML_ButtonControl("#main-treeview");
                         //DrawHTML("#main-table-dataset");
                         $("#main-table-dataset").empty();
                         SetupJsTree("#main-treeview", resultTree, null, "#main-table-dataset", null);

                     }
                 }
             );

             */
            


        },
        function (event, status, errorThrown) {
            errorThrown = 'Select Locale';
            clientAjaxError(event, status, errorThrown);
            return;
        },
        true
    );

 

}
function checkLetter(str) {
    var check = false;
    if ((str.indexOf("<") > -1) || (str.indexOf(">") > -1)) {
        check = true;
    }

    return check;
}

function DrawHTML_Pagging(currentIndex, elementCount, elementMax, callBack) {
    var div_pagging = document.createElement('div');

    var pageCount = parseInt(elementCount / elementMax);
    
    var pageMod = ((elementCount % elementMax) == 0) ? 0 : 1;
    pageCount = pageCount + pageMod;

    if (pageCount > 1) {
        for (var i = 0; i < pageCount ; i++) {
            var label_page = document.createElement('span');
            $(label_page).attr('id', 'btn_pag' + i);
            $(label_page).text((i + 1));
            $(label_page)
                .button({
                    disabled: (currentIndex == i)
                })
                .click(function () {
                    var idx = $(this).attr('id').substring(7);
                    callBack(parseInt(idx));
                });

            $(label_page).appendTo(div_pagging);

        }
    }
    return div_pagging;
}