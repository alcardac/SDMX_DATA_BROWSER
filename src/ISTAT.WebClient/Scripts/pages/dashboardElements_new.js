
var selectedLang="en";
var systemLang = {};
var objLang = {};
var objTitle = { title: objLang, };
var objContent = { content: objLang, };

var maxNumObservation;

var _layoutDashboard = [];

var current_dashBoardId = -1;
var curIdxPaging = 0;
var elemnPaging = 1;

var _endPoint;

var dashBoard;

var _url={
    CreateDashboard: "DashBoard/CreateDashboard",
    GetGetDashboard: "DashBoard/GetDashboards",
    ActiveDashboard: "DashBoard/GetDashboardsActive",
    PreviewDashboard: "DashBoard/PreviewDashboard",
    DeleteDashboard: "DashBoard/DeleteDashboard",
    AddRow: "DashBoard/AddRow",
    UpdateRow: "DashBoard/UpdateRow",
    DeleteRow: "DashBoard/DeleteRow",
    AddWidget: "DashBoard/AddWidget",
    UpdateWidget: "DashBoard/UpdateWidget",
    DeleteWidget: "DashBoard/DeleteWidget",
};

$(document).ready(function () {
    $("#div_Dashboard").tooltip({ items: "a[title], div[title]" });

    var dashboardsContainer = document.createElement('div');
    $(dashboardsContainer).addClass('dashboardsContainer');
    $(dashboardsContainer).appendTo("#div_Dashboard");

    var div_d_dashboard = document.createElement('div');
    $(div_d_dashboard).appendTo(dashboardsContainer);

    var d_dashboard = document.createElement('input');
    $(d_dashboard).attr('type', 'button');
    $(d_dashboard).attr('id', 'd_dashboard');
    $(d_dashboard).attr('onclick', 'AddDashboard(this);');
    $(d_dashboard).val((client.main.messages.label_addDashboard)?client.main.messages.label_addDashboard:"New");
    $(d_dashboard).appendTo(div_d_dashboard);

    var dashboardsList = document.createElement('div');
    $(dashboardsList).attr('id', 'dashboardsList');
    $(dashboardsList).appendTo(dashboardsContainer);

    ReloadListDashboards(dashboardsList);
});

/*************************************/
function ReloadListDashboards(dest) {

    $(dest).empty();
    $(dest).append("<i class='icon-spin6 animate-spin'></i>" + client.main.messages.text_wait);

    var dash_list_container = document.createElement("div");

    clientPostJSON(
        _url.GetGetDashboard, null,
        function (jsonString) {

            var dash_list = document.createElement("ul");

            try {

                var dashboards = clientParseJsonToObject(jsonString);

                var div_pag = DrawHTML_Pagging(
                        curIdxPaging,
                        dashboards.length,
                        elemnPaging,
                        function (idx) {
                            curIdxPaging = idx;
                            ReloadListDashboards("#dashboardsList");
                        }
                    );

                $(div_pag).appendTo(dash_list_container);
                $(div_pag).buttonset();

                var pagFrom = (curIdxPaging * elemnPaging);
                var pagTo = pagFrom + elemnPaging;
                var idxPag = -1;

                $(dash_list).appendTo(dash_list_container);
                $.each(dashboards, function (idx, dashboard) {

                    idxPag++;

                    if (idxPag < pagFrom || idxPag > (pagTo - 1))
                        return;


                    var li = document.createElement('li');
                    $(li).addClass("dashName");

                    var div_active = document.createElement('li');
                    $(div_active).addClass("d-active");
                    $(div_active).html("<input type='checkbox' " + ((dashboard.active) ? "checked='checked'" : "''") + "  onclick='ActiveDashboard(" + dashboard.id + "," + ((dashboard.active) ? false : true) + ");' title='" + client.main.messages.label_buttonActive + "' class='addDashboard' />");
                    $(div_active).appendTo(li);

                    var title = dashboard.text[0].title;
                    for (i = 0; i < dashboard.text.length; i++)
                        if (dashboard.text[i].locale == selectedLang)
                            if (dashboard.text[i].title != "")
                                title = dashboard.text[i].title;

                    var div_title = document.createElement('li');
                    $(div_title).addClass("d-title");
                    $(div_title).html(/*"<i class='icon-gauge'> </i>" +*/ title);
                    $(div_title).appendTo(li);

                    var div_default = document.createElement('li');
                    $(div_default).addClass("d-edit");
                    $(div_default).html("<a title='"+client.main.messages.label_buttonDefault+"' class='addDashboard' onclick='DefaultDashboard(" + dashboard.id + ");'><i class='icon-star-empty-1'></i></a>");
                    $(div_default).appendTo(li);

                    var div_preview = document.createElement('li');
                    $(div_preview).addClass("d-edit");
                    $(div_preview).html("<a title='"+client.main.messages.label_buttonEdit+"' class='addDashboard' onclick='EditDashboard(" + dashboard.id + ");'><i class='icon-edit'></i></a>");
                    $(div_preview).appendTo(li);

                    var div_delete = document.createElement('li');
                    $(div_delete).addClass("d-delete");
                    $(div_delete).html("<a title='" + client.main.messages.label_buttonDelete + "' class='addDashboard' onclick='DeleteDashboard(" + dashboard.id + ");'><i class='icon-cancel'></i></a>");
                    $(div_delete).appendTo(li);

                    var div_clear = document.createElement('li');
                    $(div_clear).addClass("clear-box");
                    $(div_clear).appendTo(li);

                    $(li).appendTo(dash_list);
                });

            } catch (ex) {

            }

            $(dest).empty();
            $(dash_list_container).appendTo(dest);

        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);
}
/*************************************/
function EditDashboard(dashboardId) {

    data = { id: dashboardId }

    clientPostJSON(
        _url.PreviewDashboard,
        clientParseObjectToJson(data),
        function (jsonString){

            EmptyDashboardWorkingArea();

            if (jsonString != undefined) {

                var dashBoard = clientParseJsonToObject(jsonString);

                $("#div_Dashboard").append("<div id='div_table' class='container_div'></div>");

                DrawTable(dashBoard[0].numrow, dashBoard[0], dashboardId);

            } 

        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);

}
/*************************************/
function DeleteDashboard(dashboardId) {

    if (current_dashBoardId != dashboardId) {

        ShowDialogDashboard(
            client.main.messages.form_Message,
            client.main.messages.confRemDashBoard,
            true,function () {
                
                data = { id: dashboardId }

                clientPostJSON(
                    _url.DeleteDashboard,
                    clientParseObjectToJson(data),
                    function (jsonString) {
                        ReloadListDashboards("#dashboardsList");
                    },
                    function (event, status, errorThrown) {
                        clientAjaxError(event, status, errorThrown);
                        return;
                    }, false);
            }, true,null);

    } else {
        
        ShowDialogDashboard(
            client.main.messages.form_Message,
            client.main.messages.label_delDashBoard, true, null, false, null);
            
    }
}
/*************************************/
function DefaultDashboard(dashboardId) {

}
/*************************************/
function ActiveDashboard(dashboardId, enabled) {
    data = {
        id: dashboardId,
        active: enabled
    }
    clientPostJSON(
        _url.ActiveDashboard,
        clientParseObjectToJson(data),
        function (jsonString) {
            ReloadListDashboards("#dashboardsList");
        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);

}
/*************************************/
function PreviewDashboard(dashboardId) {

    data = {
        id: dashboardId
    }

    clientPostJSON(
        _url.PreviewDashboard,
        clientParseObjectToJson(data),
        function (jsonString) {

            EmptyDashboardWorkingArea();

            if (jsonString != undefined) {

                LoadHTMLPreview(jsonString);

            }

        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);

}
/*************************************/
function RemoveDashboard(sender) {

    ShowDialogDashboard(
        client.main.messages.label_delDashBoard,
        client.main.messages.label_sureDelDashBoard,
        true, function () {
            EmptyDashboardWorkingArea();
        }, true);
}
/*************************************/
function SaveDashboard(sender, dashboardId) {

    //RECOVERY DATA
    var dashboardText;
    var dashboardRows = $('.divRow').length;
    var _dashboard_text = [];
    var _dashboard_rows = [];
    if (dashboardId == -1) {
        dashboardText = JSON.parse($(".dashboardAdd").find("input[name='title']").val());
        //dashboardRows = $('#id_numRig').val();
        $.each(dashboardText.title, function (key, value) {
            var obj = {
                title: value,
                locale: key,
            }
            _dashboard_text.push(obj);
        });
    } else {
        _dashboard_text = JSON.parse($("#dashTitleHidden").val());
        //dashboardRows = $('#dashRowsHidden').val();
    }
    //ROWS
    var enableSave = true;
    var rowsContainer = $("#div_table").find('.divTable');
    var rowsOrder = 0;
    $.each($(rowsContainer).find('.divRow'), function () {
        rowsOrder++;

        var row_splitted = true;
        var setRow = $(this).find('.setRow');
        var divSplit = $(this).find('.divSplit');
        var selectSplit = $(setRow).find('select');
        if ($(selectSplit).val() == "100%") { row_splitted = false; }

        //DATA VALIDATION
        var totWdgFull = 0;
        var totCngFull = 0;
        var totSelFull = 0;
        var totWdgHalf = 0;
        var totCngHalf = 0;
        var totSelHalf = 0;
        var divTdFull = $(divSplit).find('.divTd.full');
        var divTdHalf = $(divSplit).find('.divTd.half');
        totCngFull = $(divTdFull).find('.confWdg.configured').length;
        totSelFull = $(divTdFull).find('.selWidget').length;
        totCngHalf = $(divTdHalf).find('.confWdg.configured').length;
        totSelHalf = $(divTdHalf).find('.selWidget').length;

        if (totCngFull < totSelFull || totCngHalf < totSelHalf) { enableSave = false; }

        //WIDGETS
        var _dashboard_widgets = [];
        var wdg_order = 0;
        //CYCLE WIDGETS
        $.each($(divSplit).find('.confWdg.configured'), function () {

            var wdg_class = $(this).parent('.divTd ').attr('class');
            var wdg_type = $(this).find('.valConf').val();
            var wdg_text = [];
            var wdg_chartype = "";
            var wdg_v = true;
            var wdg_vt = false;
            var wdg_vc = false;
            var wdg_endPoint = "";
            var wdg_endPointType = "";
            var wdg_endPointV20 = "";
            var wdg_dataflow_id = "";
            var wdg_dataflow_agency_id = "";
            var wdg_dataflow_version = "";
            var wdg_criteria = "";
            var wdg_layout = "";
            var wdg_labels = "";
            var wdg_title = JSON.parse($(this).find("input[name='title']").val());

            if (wdg_type == "text") {

                var wdg_content = JSON.parse($(this).find("input[name='content']").val());
                $.each(wdg_title.title, function (key, value) {
                    var cvalue = wdg_content.content[key]; //GET THE CONTENT WITH THE SAME LOCATION KEY
                    var obj = {
                        title: value,
                        content: cvalue,
                        locale: key,
                    }
                    wdg_text.push(obj);
                });

            } else if (wdg_type == "chart") {

                $.each(wdg_title.title, function (key, value) {
                    var obj = {
                        title: value,
                        content: '',
                        locale: key,
                    }
                    wdg_text.push(obj);
                });
                //PARAMETERS
                var wdg_chartype = $(this).find("input[name='chartType']").val();

                var b_wdg_v = ($(this).find("input[name='v']").val() == "true") ? true : false;
                var b_wdg_vt = ($(this).find("input[name='vt']").val() == "true") ? true : false;
                var b_wdg_vc = ($(this).find("input[name='vc']").val() == "true") ? true : false;

                var wdg_v = b_wdg_v;
                var wdg_vt = b_wdg_vt;
                var wdg_vc = b_wdg_vc;
                var wdg_endPoint = $(this).find("input[name='endPoint']").val();
                var wdg_endPointType = $(this).find("input[name='endPointType']").val();
                var wdg_endPointV20 = $(this).find("input[name='endPointV20']").val();
                var wdg_dataflow_id = $(this).find("input[name='id']").val();
                var wdg_dataflow_agency_id = $(this).find("input[name='agency']").val();
                var wdg_dataflow_version = $(this).find("input[name='version']").val();
                var wdg_criteria = $(this).find("input[name='criteria']").val();
                var wdg_labels = $(this).find("input[name='label_series']").val();

            } else if (wdg_type == "table") {

                $.each(wdg_title.title, function (key, value) {
                    var obj = {
                        title: value,
                        content: '',
                        locale: key,
                    }
                    wdg_text.push(obj);
                });
                //PARAMETERS
                var b_wdg_v = ($(this).find("input[name='v']").val() == "true") ? true : false;
                var b_wdg_vt = ($(this).find("input[name='vt']").val() == "true") ? true : false;
                var b_wdg_vc = ($(this).find("input[name='vc']").val() == "true") ? true : false;

                var wdg_v = b_wdg_v;
                var wdg_vt = b_wdg_vt;
                var wdg_vc = b_wdg_vc;
                var wdg_endPoint = $(this).find("input[name='endPoint']").val();
                var wdg_endPointType = $(this).find("input[name='endPointType']").val();
                var wdg_endPointV20 = $(this).find("input[name='endPointV20']").val();
                var wdg_dataflow_id = $(this).find("input[name='id']").val();
                var wdg_dataflow_agency_id = $(this).find("input[name='agency']").val();
                var wdg_dataflow_version = $(this).find("input[name='version']").val();
                var wdg_criteria = $(this).find("input[name='criteria']").val();
                var wdg_layout = $(this).find("input[name='layout']").val();
            }

            wdg_order++;

            var _widget = {
                cssClass: wdg_class,
                order: wdg_order,
                text: wdg_text,
                type: wdg_type,
                chartype: wdg_chartype,
                v: wdg_v,
                vt: wdg_vt,
                vc: wdg_vc,
                endPoint: wdg_endPoint,
                endPointType: wdg_endPointType,
                endPointV20: wdg_endPointV20,
                dataflow_id: wdg_dataflow_id,
                dataflow_agency_id: wdg_dataflow_agency_id,
                dataflow_version: wdg_dataflow_version,
                criteria: wdg_criteria,
                layout: wdg_layout,
                labels: wdg_labels
            };
            _dashboard_widgets.push(_widget);
        });
        //
        var obj = {
            splitted: row_splitted,
            order: rowsOrder,
            widgets: _dashboard_widgets
        }
        _dashboard_rows.push(obj);

    });

    var time = new Date();
    //final object dashboard
    var _dashboard = {
        id: dashboardId,
        usercode: sessionStorage.user_code,
        numrow: dashboardRows, //
        active: true,
        date: time.getDay() + " - " + time.getMonth() + " - " + time.getFullYear(),
        text: _dashboard_text, //
        rows: _dashboard_rows,
    }
    if (enableSave) {
        //alert(JSON.stringify(_dashboard));
        clientPostJSON(
            _url.SaveDashboard,
            clientParseObjectToJson(_dashboard),
            function (jsonString) {

                var result = clientParseJsonToObject(jsonString);
                if (result.hasOwnProperty('success')) {

                    EmptyDashboardWorkingArea();

                    ReloadListDashboards("#dashboardsList");

                } else {
                    var div_fail = document.createElement("div");
                    $(div_fail).attr("title", client.main.messages.label_msgGenError);
                    $(div_fail).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + client.main.messages.label_error_dash_save + "</p>");
                    $(div_fail).dialog({
                        resizable: false,
                        height: 140,
                        position: { my: "center", at: "center", of: window },
                        modal: true,
                        buttons: {
                            'Ok': function () {
                                $(this).dialog("close");
                            }
                        }
                    });
                }

            }
        );
    } else {
        var div_fail = document.createElement("div");
        $(div_fail).attr("title", client.main.messages.label_msgGenError);
        $(div_fail).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + client.main.messages.label_invalidWidgets + "</p>");
        $(div_fail).dialog({
            resizable: false,
            height: 140,
            position: { my: "center", at: "center", of: window },
            modal: true,
            buttons: {
                'Ok': function () {
                    $(this).dialog("close");
                }
            }
        });
    }

    current_dashBoardId = -1;
}
/*************************************/
function splitDiv(divSplit, selVal) {

    var rowNum = $(divSplit);

    var selValue = $(selVal).val();
    if (selValue == "50%") {
        $('#' + divSplit + ">.full").switchClass('full', 'half');
        $('#' + divSplit + ">.hide").switchClass('hide', 'half');
        _layoutDashboard[rowNum] = 1;
    } else {
        $('#' + divSplit + ">.half:nth-child(2)").switchClass('half', 'hide');
        $('#' + divSplit + ">.half:first-child").switchClass('half', 'full');
        _layoutDashboard[rowNum] = 2;
    }
}
/*************************************/
function selectWidget(button) {

    var hiddenFields = "";
    var aClass = "";

    ShowDialogDashboard(
    client.main.messages.label_widgetSelection,
    "<select id='selWdg'><option value='text'>" + client.main.messages.label_wdgText + "</option><option value='chart'>" + client.main.messages.label_wdgChart + "</option><option value='table'>" + client.main.messages.label_wdgTable + "</option></select>",
    true, function () {
        if ($("#selWdg").val() == "text") {
            hiddenFields = GetHTML_TextFields();
            aClass = "pencil";
        } else if ($("#selWdg").val() == "chart") {
            hiddenFields = GetHTML_ChartFields();
            aClass = "chart-bar";
        } else if ($("#selWdg").val() == "table") {
            hiddenFields = GetHTML_TableFields();
            aClass = "table";
        }
        $(button).parent(".divTd").append("<div class='confWdg' ><input type='hidden' value='" + $("#selWdg").val() + "' class='valConf'/>" +
                                            "<div class='wdgName'><i class='icon-" + aClass + "'></i>" + $("#selWdg").val() + " widget</div>" +
                                            "<div class='wdgSettings' onclick='ConfigWidget(this);' title='"+client.main.messages.label_configureWidget+"' ><i class='icon-menu'></i></div>" +
                                            "<div class='wdgDelete' onclick='RemoveWidget(this);' title='"+client.main.messages.label_removeWidget+"'><i class='icon-cancel'></i></div>" +
                                            hiddenFields +
                                            "<div class='clear-box'></div>" +
                                        "</div>");
        $(button).addClass("hide");
        $(this).dialog('destroy').remove();
    }, true);

}
/*************************************/
function Draw_Button_Layout(link) {

    var aDiv = $(link).parent(".widget-button");
    var divContainer = $(aDiv).parent(".widgetSettings");
    var config = $(divContainer).find("input[name='configuration']").val();

    if ($(divContainer).find("input[name='endPoint']").val() == "" ||
        $(divContainer).find("input[name='endPointType']").val() == "" ||
        $(divContainer).find("input[name='endPointV20']").val() == "" ||
        $(divContainer).find("input[name='id']").val() == "" ||
        $(divContainer).find("input[name='agency']").val() == "" ||
        $(divContainer).find("input[name='version']").val() == ""
        ) {

        OnDashboardError(client.main.messages.label_allFieldsInserted);
    }
    else {

        var _data = {
            configuration: {
                EndPoint: $(divContainer).find("input[name='endPoint']").val(),
                EndPointType: $(divContainer).find("input[name='endPointType']").val(),
                EndPointV20: $(divContainer).find("input[name='endPointV20']").val(),
            },
            dataflow: {
                id: $(divContainer).find("input[name='id']").val(),
                agency: $(divContainer).find("input[name='agency']").val(),
                version: $(divContainer).find("input[name='version']").val(),
            }
        }
        clientShowWaitDialog();
        clientPostJSON(
            "Main/GetLayout",
            clientParseObjectToJson(_data),
            function (jsonString) {

                clientCloseWaitDialog();

                if (jsonString.indexOf("CONFIG ERROR") > -1) {
                    OnDashboardError(client.main.messages.label_invalidFields);
                } else {
                    var o = JSON.parse(jsonString);
                    if (o.hasOwnProperty('error')) {
                        OnDashboardError(client.main.messages.label_invalidFields);
                    } else {
                        var result = clientParseJsonToObject(jsonString);
                        DrawHTML_Layout(result.DefaultLayout, $(divContainer));
                    }
                }

            },
            function (event, status, errorThrown) {
                clientAjaxError(event, status, errorThrown);
                return;
            }, false);
    }
}
/*************************************/
function DrawHTML_Layout(layout, sender) {

    var prev_selc = $(sender).find("input[name='layout']").val();
    var defaultLayout;
    if (prev_selc == ""
        || prev_selc == null
        || prev_selc == '{"axis_x":[],"axis_y":[],"axis_z":[]}') {
        defaultLayout = layout;
    }
    else {
        defaultLayout = clientParseJsonToObject(prev_selc);
    }

    OpenPopUpLayout(defaultLayout,
        function (args) {
            $(sender).find("input[name='layout']").val(clientParseObjectToJson(args));
        },
    {
        table_layout: "tb_layout",  // layout table
        list_layout: "ul_layout",
        list_layout_connected: "ul_layout_cnt",
        list_item_layout: "li_layout"
    },
    client.main.messages,
    {
        sdmxKey: "sdmxKey",
        sdmxValue: "sdmxValue",
        sdmxSerieV: "sdmxSerieV",
        sdmxSerieH: "sdmxSerieH",
    });
}
/*************************************/
function testWBS(sender, input) {
    var wdgContainer = $(sender).parent().parent(".widgetSettings");
    var endpoint = $(wdgContainer).find('input[name=' + input + ']').val();

    OnDashboardError(client.main.messages.messageWorkProg);
}
/*************************************/
function ConfigWidget(div) {

    var configWidget = document.createElement("div");
    var displayConf = "";
    var divWidget = $(div).parent(".confWdg");

    $(configWidget).attr("title", client.main.messages.label_wdgConfiguration);

    if ($(divWidget).children(".valConf").val() == "text") {
        displayConf = GetHTML_SettingsText();
    } else if ($(divWidget).children(".valConf").val() == "chart") {
        displayConf = GetHTML_SettingsChart();
    } else if ($(divWidget).children(".valConf").val() == "table") {
        displayConf = GetHTML_SettingsTable();
    }

    $("#selWebService").tabs();

    $(configWidget).html(displayConf);
    //CHANGE LANGUAGE
    var selectLocale = $(configWidget).find("select[name='select-locale']");
    $(selectLocale).change(function () { checkLangField($(selectLocale), $(divWidget).children(".valConf").val(), divWidget) });
    //ENTER TITLE
    var thisTitle = $(configWidget).find("input[name='title']");
    $(thisTitle).keyup(function () { insertLangField($(thisTitle), $(divWidget).children(".valConf").val(), divWidget) });
    var dialog_height = 430;

    // Is widget text
    if ($(divWidget).children(".valConf").val() == "text") {
        //
        var inputTitle = $(divWidget).children("input[name='title']").val();
        var inputContent = $(divWidget).children(".text").val();
        var oTitle = JSON.parse(inputTitle);
        var oContent = JSON.parse(inputContent);

        //FILL THE TEXTFIELD WITH THE HIDDEN VALUE BY SELECTED LANGUAGE
        $(configWidget).find("input[name='title']").val(oTitle.title[$(selectLocale).val()]);
        $(configWidget).find("textarea").val(oContent.content[$(selectLocale).val()]);
        //ENTER CONTENT
        var thisContent = $(configWidget).find("textarea");
        $(thisContent).keyup(function () { insertLangField($(thisContent), $(divWidget).children(".valConf").val(), divWidget) });

    }
        // Is no widget text
    else if ($(divWidget).children(".valConf").val() != "text") {

        var inputTitle = $(divWidget).children("input[name='title']").val();
        var oTitle = JSON.parse(inputTitle);

        dialog_height = 705;

        var webServiceSelector = $(configWidget).find(".webServiceSelector");

        // Retrive Endpoints
        clientPostJSON(
            "Settings/GetSettings", null,
            function (jsonString) {

                var _sett_obj = clientParseJsonToObject(jsonString);
                var conf = null;
                $.each(_sett_obj.endpoints, function (index, endpoint) {
                    if (endpoint.Active) {
                        if (conf == null) { conf = endpoint; }
                        var option = document.createElement('option');
                        $(option).text(endpoint.Title + " - " + endpoint.EndPoint);
                        $(option).val(clientParseObjectToJson(endpoint));
                        if (endpoint.EndPoint == $(divWidget).children("input[name='endPoint']").val()) {
                            $(option).attr("selected", "selected");
                        }
                        $(webServiceSelector).append(option);
                    }
                });

                if ($(divWidget).find("input[name='endPoint']").val() == "") { $(divWidget).find("input[name='endPoint']").val(conf.EndPoint); }
                if ($(divWidget).find("input[name='endPointType']").val() == "") { $(divWidget).find("input[name='endPointType']").val(conf.EndPointType); }
                if ($(divWidget).find("input[name='endPointV20']").val() == "") { $(divWidget).find("input[name='endPointV20']").val(conf.EndPointV20); }

                if ($(divWidget).find("input[name='id']").val() == "") { $(divWidget).find("input[name='id']").val(conf.id); }
                if ($(divWidget).find("input[name='agency']").val() == "") { $(divWidget).find("input[name='agency']").val(conf.agency); }
                if ($(divWidget).find("input[name='version']").val() == "") { $(divWidget).find("input[name='version']").val(conf.version); }

                $(webServiceSelector).change(function () {

                    var conf = clientParseJsonToObject($(this).val());

                    $(configWidget).find("input[name='endPoint']").val(conf.EndPoint);
                    $(configWidget).find("input[name='endPointType']").val(conf.EndPointType);
                    $(configWidget).find("input[name='endPointV20']").val(conf.EndPointV20);
                    $(configWidget).find("input[name='id']").val(conf.id);
                    $(configWidget).find("input[name='agency']").val(conf.agency);
                    $(configWidget).find("input[name='version']").val(conf.version);

                });

                /*************************/

                var DIVcallTreeView = $(configWidget).find(".callTreeView");

                var callTree = document.createElement('a');
                $(callTree).addClass('selWidget');
                $(callTree).addClass('openTree');
                $(callTree).html("<i class='icon-flow-cascade'></i>" + client.main.messages.button_openTree);

                $(callTree).appendTo(DIVcallTreeView);

                $(callTree).click(function (event) {

                    var _configuration = { configuration: clientParseJsonToObject($(webServiceSelector).val()) };

                    var dest = document.createElement("div");
                    $(dest).attr("name", "openTree");

                    LoadJsTree($(dest),
                        "Main/GetTree",
                        _configuration,
                        {
                            dataflow: {
                                id: $(configWidget).find("input[name='id']").val(),
                                agency: $(configWidget).find("input[name='agency']").val(),
                                version: $(configWidget).find("input[name='version']").val(),
                            }
                        },
                        null,
                        function (data) {

                            // in data contienen dataflow e and point
                            $(configWidget).find("input[name='configuration']").val(clientParseObjectToJson(data));
                            //CRITERIA AND LAYOUT
                            $(configWidget).find("input[name='criteria']").val(clientParseObjectToJson(data.criteria));
                            if ($(divWidget).children(".valConf").val() == "table") {
                                $(configWidget).find("input[name='layout']").val(clientParseObjectToJson(data.layout));
                            }
                            // URL URL_V20
                            $(configWidget).find("input[name='endPoint']").val(clientParseObjectToJson(data.configuration.EndPoint).replace(/"/g, ''));
                            $(configWidget).find("input[name='endPointType']").val(clientParseObjectToJson(data.configuration.EndPointType).replace(/"/g, ''));
                            $(configWidget).find("input[name='endPointV20']").val(clientParseObjectToJson(data.configuration.EndPointV20).replace(/"/g, ''));

                            //ID - AGENCY - VERSION
                            $(configWidget).find("input[name='id']").val(clientParseObjectToJson(data.dataflow.id).replace(/"/g, ''));
                            $(configWidget).find("input[name='agency']").val(clientParseObjectToJson(data.dataflow.agency).replace(/"/g, ''));
                            $(configWidget).find("input[name='version']").val(clientParseObjectToJson(data.dataflow.version).replace(/"/g, ''));

                        });

                    $(dest).dialog({
                        resizable: true,
                        height: 600,
                        width: 800,
                        position: { my: "center", at: "center", of: window },
                        modal: true,
                        buttons: {
                            'Ok': function () { $(this).dialog("close"); },
                            'Cancel': function () { $(this).dialog("close"); }
                        }
                    });
                });

                var val_v = false;
                var val_vt = false;
                var val_vc = false;

                if ($(divWidget).children("input[name='v']").val() == "true") { val_v = true; }
                if ($(divWidget).children("input[name='vt']").val() == "true") { val_vt = true; }
                if ($(divWidget).children("input[name='vc']").val() == "true") { val_vc = true; }

                $(configWidget).find("input[name='title']").val(oTitle.title[$(selectLocale).val()]);
                if ($(divWidget).children(".valConf").val() == "chart") {
                    $(configWidget).find("select[name='chartType']").val($(divWidget).children("input[name='chartType']").val());
                }
                $(configWidget).find("input[name='v']").prop("checked", val_v);
                $(configWidget).find("input[name='vt']").prop("checked", val_vt);
                $(configWidget).find("input[name='vc']").prop("checked", val_vc);

                $(configWidget).find("input[name='endPoint']").val($(divWidget).children("input[name='endPoint']").val());
                $(configWidget).find("input[name='endPointType']").val($(divWidget).children("input[name='endPointType']").val());
                $(configWidget).find("input[name='endPointV20']").val($(divWidget).children("input[name='endPointV20']").val());

                $(configWidget).find("input[name='id']").val($(divWidget).children("input[name='id']").val());
                $(configWidget).find("input[name='agency']").val($(divWidget).children("input[name='agency']").val());
                $(configWidget).find("input[name='version']").val($(divWidget).children("input[name='version']").val());
                $(configWidget).find("input[name='configuration']").val($(divWidget).children("input[name='configuration']").val());
                $(configWidget).find("input[name='criteria']").val($(divWidget).children("input[name='criteria']").val());
                $(configWidget).find("input[name='label_series']").val($(divWidget).children("input[name='label_series']").val());

                if ($(divWidget).children(".valConf").val() == "table") {
                    $(configWidget).find("input[name='layout']").val($(divWidget).children("input[name='layout']").val());
                }

                /************************/
            },
            function (event, status, errorThrown) {
                clientAjaxError(event, status, errorThrown);
                return;
            }, false);
    }

    $(configWidget).dialog({
        resizable: true,
        height: dialog_height,
        width: 765,
        position: { my: "center", at: "center", of: window },
        modal: true,
        buttons: {
            'Ok': function () {
                var ok = false;

                if (($(divWidget).children(".valConf").val() == "text")
                    && ($(this).find("textarea").val().trim() != "")
                    && ($(this).find("input[name='title']").val().trim() != "")) {
                    //SAVING TEXT HIDDEN
                    ok = true;
                } else if (($(divWidget).children(".valConf").val() == "chart")
                    && ($(this).find("input[name='title']").val().trim() != "")
                    && (($(this).find("input[name='v']").is(":checked")
                        || ($(this).find("input[name='vt']").is(":checked"))
                        || ($(this).find("input[name='vc']").is(":checked"))))
                    && ($(this).find("input[name='id']").val().trim() != "")
                    && ($(this).find("input[name='agency']").val().trim() != "")
                    && ($(this).find("input[name='version']").val().trim() != "")) {
                    //SAVING CHART HIDDEN
                    //$(divWidget).children("input[name='title']").val($(this).find("input[name='title']").val());
                    $(divWidget).children("input[name='chartType']").val($(this).find("select[name='chartType']").val());
                    $(divWidget).children("input[name='v']").val($(this).find("input[name='v']").is(":checked"));
                    $(divWidget).children("input[name='vt']").val($(this).find("input[name='vt']").is(":checked"));
                    $(divWidget).children("input[name='vc']").val($(this).find("input[name='vc']").is(":checked"));
                    $(divWidget).children("input[name='webService']").val($(this).find("select[name='webServiceSelector']").val());
                    $(divWidget).children("input[name='endPoint']").val($(this).find("input[name='endPoint']").val());
                    $(divWidget).children("input[name='endPointType']").val($(this).find("input[name='endPointType']").val());
                    $(divWidget).children("input[name='endPointV20']").val($(this).find("input[name='endPointV20']").val());
                    $(divWidget).children("input[name='id']").val($(this).find("input[name='id']").val());
                    $(divWidget).children("input[name='agency']").val($(this).find("input[name='agency']").val());
                    $(divWidget).children("input[name='version']").val($(this).find("input[name='version']").val());
                    $(divWidget).children("input[name='configuration']").val($(this).find("input[name='configuration']").val());
                    $(divWidget).children("input[name='criteria']").val($(this).find("input[name='criteria']").val());
                    $(divWidget).children("input[name='label_series']").val($(this).find("input[name='label_series']").val());
                    ok = true;
                } else if (($(divWidget).children(".valConf").val() == "table")
                    && ($(this).find("input[name='title']").val().trim() != "")
                    && (($(this).find("input[name='v']").is(":checked")
                        || ($(this).find("input[name='vt']").is(":checked"))
                        || ($(this).find("input[name='vc']").is(":checked"))))
                    && ($(this).find("input[name='id']").val().trim() != "")
                    && ($(this).find("input[name='agency']").val().trim() != "")
                    && ($(this).find("input[name='version']").val().trim() != "")) {
                    //SAVING TABLE HIDDEN
                    //$(divWidget).children("input[name='title']").val($(this).find("input[name='title']").val());
                    $(divWidget).children("input[name='v']").val($(this).find("input[name='v']").is(":checked"));
                    $(divWidget).children("input[name='vt']").val($(this).find("input[name='vt']").is(":checked"));
                    $(divWidget).children("input[name='vc']").val($(this).find("input[name='vc']").is(":checked"));
                    $(divWidget).children("input[name='webService']").val($(this).find("select[name='webServiceSelector']").val());
                    $(divWidget).children("input[name='endPoint']").val($(this).find("input[name='endPoint']").val());
                    $(divWidget).children("input[name='endPointType']").val($(this).find("input[name='endPointType']").val());
                    $(divWidget).children("input[name='endPointV20']").val($(this).find("input[name='endPointV20']").val());
                    $(divWidget).children("input[name='id']").val($(this).find("input[name='id']").val());
                    $(divWidget).children("input[name='agency']").val($(this).find("input[name='agency']").val());
                    $(divWidget).children("input[name='version']").val($(this).find("input[name='version']").val());
                    $(divWidget).children("input[name='configuration']").val($(this).find("input[name='configuration']").val());
                    $(divWidget).children("input[name='criteria']").val($(this).find("input[name='criteria']").val());
                    $(divWidget).children("input[name='layout']").val($(this).find("input[name='layout']").val());

                    ok = true;

                    if ($(this).find("input[name='layout']").val() == '{"axis_x":[],"axis_y":[],"axis_z":[]}'
                        || $(this).find("input[name='layout']").val() == '{}'
                        || $(this).find("input[name='layout']").val() == '') { ok = false; }

                }


                if (!ok) OnDashboardError(client.main.messages.label_allFieldsInserted);
                else {
                    $(this).dialog("close");
                    $(divWidget).addClass("configured");
                }
            },
            'Cancel': function () {
                $(this).dialog("close");
            }
        }
    });

    if ($(divWidget).children(".valConf").val() != "text") {
        $(function () { $(".selWebService").tabs({ active: 0 }); });
    }
}
/*************************************/
function AddDashboard(link) {

    current_dashBoardId = -1;

    if ($(".dashboardAdd").length == 0 && $(".divTable").length == 0) {
        var str_html = "<div class='container_div' id='div_table'>";
        str_html += "<div class='dashboardAdd'>";
        str_html += "<div><h4>" + client.main.messages.label_language + "</h4></div>";
        str_html += "<div><select name='select-locale' onchange='checkLangField(this,\"dashboard\");'>";

        $.each(systemLang, function (value, display) {
            str_html += '<option value="' + value + '" ' + ((value == selectedLang) ? ' selected="selected" ' : "") + ' >' + display + '</option>';
        });

        str_html += "</select></div>";
        str_html += "<div><h4>" + client.main.messages.label_dashboardTitle + "</h4></div><div><input id='id_dashTitle' onkeyup='insertLangField(this,\"dashboard\");' type='text'/></div>";
        str_html += "<div><h4>" + client.main.messages.label_hide_row + "</h4><input id='id_hide_rows' type='checkbox'/></div>";
        str_html += "<div>";
        str_html += "<input type='hidden' name='title' value='" + JSON.stringify(objTitle) + "'/>";
        str_html += "<input id='d_continue' type='button' value='" + client.main.messages.label_continue + "' />";
        str_html += "<input id='d_delete' type='button' value='" + client.main.messages.label_cancel + "' onclick='RemoveDashboard(this)' />";
        str_html += "</div></div>";
        str_html += "</div><div id='clean-box' class='clear-box'></div>";
        $("#div_Dashboard").append(str_html);
        // btm create
        $("#d_continue").click(function () {
            var til = $.trim($("#id_dashTitle").val()); //SCREEN TITLE
            var multiTil = $(this).parent().parent(".dashboardAdd").find("input[name='title']").val(); //MULTILANGUAGE TITLE

            if (til == "") {

                // init first array of colspan
                _layoutDashboard[0] = 2;

                // Dashboard row num and title
                var div_fail = document.createElement("div");
                $(div_fail).attr("title", client.main.messages.label_msgGenError);
                $(div_fail).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + client.main.messages.label_allFieldsInserted + "</p>");
                $(div_fail).dialog({
                    resizable: false,
                    height: 140,
                    position: { my: "center", at: "center", of: window },
                    modal: true,
                    buttons: {
                        'Ok': function () {
                            $(this).dialog("close");
                        }
                    }
                });

            } else {

                // get text title
                var _dashboard_text = [];
                $.each(clientParseJsonToObject(multiTil).title, function (key, value) {
                    var obj = {
                        title: value,
                        locale: key,
                    }
                    _dashboard_text.push(obj);
                });

                var data = {
                    id: -1,
                    usercode: sessionStorage.user_code,
                    numrow: 1,
                    active: false,
                    date: '',
                    text: _dashboard_text,
                    rows: null,
                    toggleRows: $("#id_hide_rows").prop('checked'),
                };

                clientPostJSON(
                "DashBoard/CreateDashboard", clientParseObjectToJson(data),
                    function (jsonString) {

                        dashBoard = clientParseJsonToObject(jsonString);

                        // Create dashboard skeletron
                        DrawTable(1);
                        $("#d_continue").attr("disabled", "disabled");
                        $("#id_numRig").attr("disabled", "disabled");
                        $("#id_dashTitle").attr("disabled", "disabled");
                        $(".dashboardAdd").css("display", "none");

                    },
                    function (event, status, errorThrown) {
                        errorThrown = 'SetupJsonTree';
                        clientAjaxError(event, status, errorThrown);
                        return;
                    }, false);
            }
        });

    } else {
        // Dashboard already exist
        OnDashboardError("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + client.main.messages.label_anotherDashboard + "</p>");
    }
}
/*************************************/
function RemoveRow(link) {

    var divRow = $(link).parents(".divRow");
    var id_row = $(link).attr('id');
    if (id_row != undefined) id_row = id_row.substring(4);

    ShowDialogDashboard(
        client.main.messages.label_rowRemove, 
        client.main.messages.label_rowConfirmRemove,
        true, function () {

             // if not if only row
             if ($('.divRow').length > 1) {

                 var data = { id: id_row };

                 clientPostJSON(
                 _url.DeleteRow, clientParseObjectToJson(data),
                     function (jsonString) {
                         $(divRow).remove();
                     },
                     function (event, status, errorThrown) {
                         errorThrown = 'SetupJsonTree';
                         clientAjaxError(event, status, errorThrown);
                         return;
                     }, false);

             }
             else { OnDashboardError(client.main.messages.label_errorDeleteRow); }
         }, true);

}
/*************************************/
function AddRow(link) {
    var divTable = $(link).parents(".divTable");
    var totDivRow = $(divTable).find(".divRow").length + 1;
    var lastDivRow = $(divTable).find(".divRow:last");
    var inputDiv = "divSplit-" + totDivRow;

    clientPostJSON(
    _url.AddRow, null,
        function (jsonString) {
            var row_dash = clientParseJsonToObject(jsonString);
            $(lastDivRow).after('<div class="divRow"><div class="setRow">' +
                            '<span class="removeRow"><a id="row_' + row_dash.id+ '" title="' + client.main.messages.label_removeRow + '" href="#" onclick="RemoveRow(this);"><i class="icon-cancel-squared"></i></a></span>' +
                            '<select class="changeSlice" onchange="splitDiv(\'' + inputDiv + '\',this);">' +
                            '<option value="100%">100%</option><option value="50%">50%</option>' +
                            '</select></div><div class="divSplit" id="divSplit-' + i + '">' +
                            '<div class="divTd full">' +
                            '<a class="selWidget" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a>' +
                    '</div><div class="divTd hide">' +
                    '<a class="selWidget" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a>' +
                    '</div><div class="clear-box"></div></div></div>');

        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);
}
/*************************************/
function RemoveWidget(div) {

    var divWidget = $(div).parent(".confWdg");
    var divContainer = $(divWidget).parent(".divTd");

    var id_widget = $(divWidget).attr('id');
    if (id_widget != undefined) id_widget = id_widget.substring(4);

    var data = { id: id_widget };

    ShowDialogDashboard(
        client.main.messages.label_wdgRemove,
        client.main.messages.label_wdgConfirmRemove,
        true, function(){
            clientPostJSON(
            _url.DeleteWidget, data,
            function (jsonString) {
                $(divWidget).remove();
                $(divContainer).children(".selWidget").removeClass("hide");
            },
            function (event, status, errorThrown) {
                clientAjaxError(event, status, errorThrown);
                return;
            }, false);
        }, true);
}
/*************************************/

function Draw_Button_Query(link) {

    var aDiv = $(link).parent(".widget-button");
    var divContainer = $(aDiv).parent(".widgetSettings");
    OnDashboardError(messageWorkProg);
}
/*************************************/
function Draw_Button_LabelSeries(link) {

    var aDiv = $(link).parent(".widget-button");
    var divContainer = $(aDiv).parent(".widgetSettings");

    if (
        $(divContainer).find("input[name='endPoint']").val() == "" ||
        $(divContainer).find("input[name='endPointType']").val() == "" ||
        $(divContainer).find("input[name='endPointV20']").val() == "" ||
        $(divContainer).find("input[name='id']").val() == "" ||
        $(divContainer).find("input[name='agency']").val() == "" ||
        $(divContainer).find("input[name='version']").val() == ""
        ) {
        // Errore
    } else {
        clientShowWaitDialog();

        var inputDataFlow = {
            id: $(divContainer).find("input[name='id']").val(),
            agency: $(divContainer).find("input[name='agency']").val(),
            version: $(divContainer).find("input[name='version']").val(),
        };
        var inputConfiguration = {
            EndPoint: $(divContainer).find("input[name='endPoint']").val(),
            EndPointType: $(divContainer).find("input[name='endPointType']").val(),
            EndPointV20: $(divContainer).find("input[name='endPointV20']").val(),
        };

        var _data = {
            dataflow: inputDataFlow,
            configuration: inputConfiguration,
        }

        clientPostJSON("Main/GetCodemap", clientParseObjectToJson(_data),
        function (jsonString) {

            clientCloseWaitDialog();

            if (jsonString.indexOf("CONFIG ERROR") > -1) {
                OnDashboardError(client.main.messages.label_invalidFields);
            } else {
                var o = JSON.parse(jsonString);
                if (o.error == true) {
                    OnDashboardError(client.main.messages.label_invalidFields);
                } else {
                    var result = clientParseJsonToObject(jsonString);
                    DrawHTML_Label_Serie(result.codemap, $(divContainer));
                }
            }
        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);
    }
}
/*************************************/
function DrawHTML_Label_Serie(filters, sender) {

    var prev_label_series = $(sender).find("input[name='label_series']").val();
    var label_series = clientParseJsonToObject(prev_label_series);

    var prev_selc = $(sender).find("input[name='criteria']").val();
    var criteria = clientParseJsonToObject((prev_selc == "" || prev_selc == null) ? "{}" : prev_selc)

    var key_concept = "";
    var str_concept_fixed = "";
    var key_code = "";

    $.each(filters, function (concept, codes) {
        if (concept != "TIME_PERIOD") {
            var codes_arr = null;

            if (criteria.hasOwnProperty(concept)) codes_arr = criteria[concept];
            else codes_arr = filters[concept].codes;

            if (codes_arr.length > 1) key_concept = concept;
            else str_concept_fixed += ', ' + filters[concept].codes[codes_arr[0]].name;

            key_code += "-" + codes_arr[0];
        }
    });

    var codes_arr = null;
    if (criteria.hasOwnProperty(key_concept)) codes_arr = criteria[key_concept];
    else codes_arr = filters[key_concept].codes;

    var div_labels = document.createElement('div');
    $(div_labels).css('height', '300px');
    $(div_labels).css('overflow', 'auto');

    var select_loc = document.createElement('select');
    $.each(systemLang, function (value, display) {
        $(select_loc).append('<option value="' + value + '" ' + (value == selectedLang) ? 'selected="selected"' : "" + '>' + display + '</option>');
    });
    $(select_loc).change(function (event) {
        var loc = $(this).val();
        $(div_labels).find('.input_loc_label').each(function () {
            var loc_text = $(this).data('loc_' + loc);
            $(this).val(loc_text);
        });
    });
    $(select_loc).appendTo(div_labels);

    for (idx = 0; idx < codes_arr.length; idx++) {

        var code = codes_arr[idx];
        var str_label = filters[key_concept].codes[code].name + str_concept_fixed;

        var div_serie_container = document.createElement('div');
        $(div_serie_container).attr('id', key_code);
        $(div_serie_container).addClass('serie_container');

        var p_labels = document.createElement('p');
        $(p_labels).css('width', '100%');
        $(p_labels).text(str_label);
        $(p_labels).appendTo(div_serie_container);

        var select_type_chart = document.createElement('select');
        $(select_type_chart).css('width', '10%');
        $(select_type_chart).attr('id', "chartType_" + key_code);
        $(select_type_chart).append("<option value='spline' " +
            ((label_series != null) ?
                (label_series[key_code].chartType == 'spline') ? "selected=selected" : "" : "") + ">Spline</option>");
        $(select_type_chart).append("<option value='column' " +
            ((label_series != null) ?
                (label_series[key_code].chartType == 'column') ? "selected=selected" : "" : "") + ">Bars</option>");
        $(select_type_chart).append("<option value='area' " +
            ((label_series != null) ?
                (label_series[key_code].chartType == 'area') ? "selected=selected" : "" : "") + ">Area</option>");
        $(select_type_chart).appendTo(div_serie_container);

        var input_labels = document.createElement('input');
        $(input_labels).attr('id', "input_" + key_code);
        $(input_labels).addClass('input_loc_label');
        $(input_labels).attr('type', 'text');
        $(input_labels).css('width', '90%');
        $(input_labels).val((label_series != null) ?
            label_series[key_code].label[$(select_loc).val()] : "");

        $(input_labels).change(function (event) {
            var loc = $(select_loc).val();
            var text = $(this).val();

            $(this).data('loc_' + loc, text);
        });
        $(input_labels).appendTo(div_serie_container);

        $(div_serie_container).appendTo(div_labels);
    };

    var div_dialog = document.createElement('div');
    $(div_labels).appendTo(div_dialog);
    $(div_dialog).dialog({
        resizable: false,
        position: { my: "center", at: "center", of: window },
        modal: true,
        width: 1000,
        heigth: 600,
        buttons: {
            'Ok': function () {

                var label_series = [];
                $(div_dialog).find('.serie_container').each(function () {
                    var idSerie = $(this).attr('id');
                    var loc_labels = [];

                    $(select_loc).find('option').each(function () {
                        var loc = $(this).val();
                        var text = $('#input_' + idSerie).data('loc_' + loc);
                        loc_labels[loc] = {
                            title: (text != undefined) ? text : "",
                            content: "",
                        }
                    });
                    label_series[idSerie] = {
                        label: loc_labels,
                        chartType: $('#chartType_' + idSerie).val()
                    }
                });

                $(sender).find("input[name='label_series']").val(clientParseObjectToJson(label_series));

                $(this).dialog("destroy").remove();
            }
        }
    });

}
/*************************************/
function Draw_Button_DimensionAxe(link) {
    var aDiv = $(link).parent(".widget-button");
    var divContainer = $(aDiv).parent(".widgetSettings");
    OnDashboardError(messageWorkProg);
}
/*************************************/
function Draw_Button_Filter(link) {

    var aDiv = $(link).parent(".widget-button");
    var divContainer = $(aDiv).parent(".widgetSettings");

    if ($(divContainer).find("input[name='endPoint']").val() == "" ||
        $(divContainer).find("input[name='endPointType']").val() == "" ||
        $(divContainer).find("input[name='endPointV20']").val() == "" ||
        $(divContainer).find("input[name='id']").val() == "" ||
        $(divContainer).find("input[name='agency']").val() == "" ||
        $(divContainer).find("input[name='version']").val() == "") {

        var aDiv = $(link).parent(".widget-button");
        var divContainer = $(aDiv).parent(".widgetSettings");

        OnDashboardError(client.main.messages.label_allFieldsInserted);

    }
    else {
        clientShowWaitDialog();

        var inputDataFlow = {
            id: $(divContainer).find("input[name='id']").val(),
            agency: $(divContainer).find("input[name='agency']").val(),
            version: $(divContainer).find("input[name='version']").val(),
        };
        var inputConfiguration = {
            EndPoint: $(divContainer).find("input[name='endPoint']").val(),
            EndPointType: $(divContainer).find("input[name='endPointType']").val(),
            EndPointV20: $(divContainer).find("input[name='endPointV20']").val(),
        };

        var _data = {
            dataflow: inputDataFlow,
            configuration: inputConfiguration,
        }

        clientPostJSON("Main/GetCodemap",clientParseObjectToJson(_data),
        function (jsonString) {

            clientCloseWaitDialog();

            if (jsonString.indexOf("CONFIG ERROR") > -1) {
                OnDashboardError(client.main.messages.label_invalidFields);
            } else {
                var o = JSON.parse(jsonString);
                if (o.error == true) {
                    OnDashboardError(client.main.messages.label_invalidFields);
                } else {
                    var result = clientParseJsonToObject(jsonString);
                    DrawHTML_Filter(result.codemap, $(divContainer));
                }
            }

        },
        function (event, status, errorThrown) {
            clientAjaxError(event, status, errorThrown);
            return;
        }, false);
    }
}
/*************************************/
function DrawHTML_Filter(filters, sender) {

    var prev_selc = $(sender).find("input[name='criteria']").val();

    OpenPopUpFilters(
        filters,
        clientParseJsonToObject((prev_selc == "" || prev_selc == null) ? "{}" : prev_selc),
        function (args) {

            if ($(sender).find("input[name='widget_type']").val() == 'chart') {

                var chartFilters = args;
                var dimSelection = 0;
                $.each(filters, function (idx, filter) {

                    if (idx != undefined && idx != 'TIME_PERIOD') {

                        if (!chartFilters.hasOwnProperty(idx)) {
                            if (filters.hasOwnProperty(idx))
                                chartFilters[idx] = $.map(filters[idx].codes, function (Code, idxCode) { return idxCode; });
                            else
                                chartFilters[idx] = $.map(filter.codes, function (Code, idxCode) { return idxCode; });
                        }
                        if (chartFilters[idx].length != 1) dimSelection++;
                    }
                });

                if (dimSelection > 1) {

                    var div_fail = document.createElement("div");
                    $(div_fail).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + client.main.messages.label_dimoutlimit_desc + "</p>");
                    $(div_fail).dialog({
                        resizable: false,
                        height: 140,
                        position: { my: "center", at: "center", of: window },
                        modal: true,
                        buttons: {
                            'Ok': function () {
                                $(this).dialog("close");
                            }
                        }
                    });

                    return;
                }


                $(sender).find("input[name='criteria']").val(clientParseObjectToJson(chartFilters));
            } else {

                $(sender).find("input[name='criteria']").val(clientParseObjectToJson(args));
            }

        },
        {
            table_layout: "tb_layout",  // layout table
            list_layout: "ul_layout",
            list_layout_connected: "ul_layout_cnt",
            list_item_layout: "li_layout",

            tab_header: "a_tab_header_criteria", // criteria tabs
            coded_list: "ul_coded_criteria",
            tabs_div: "tabs_div_criteria",
            tab_div: "tab_div_criteria",
            coded_title: "h3_title_criteria",
            coded_btn_tree: "btn_tree_criteria",

        },
        client.main.messages,
        "TIME_PERIOD", maxNumObservation);

}

/*************************************/
function GetHTML_SettingsChart() {
    
    var settingsChart = '<div class="widgetSettings chart">';
    settingsChart+='<input type="hidden" name="widget_type" value="chart"/>' ;
    settingsChart+='<div class="title-half">Select Language</div>';
    settingsChart+='<div class="area-half">';
    settingsChart+='<select name="select-locale">';
    $.each(systemLang, function (value, display) {
        settingsChart += '<option value="' + value + '" ' + ((value == selectedLang) ? ' selected="selected" ' : "") + ' >' + display + '</option>';
    });
    settingsChart+='</select>';
    settingsChart+='</div>' ;
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_title+'*</div>';
    settingsChart+='<div class="area-half">';
    settingsChart+='<input type="text" name="title"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_type+'*</div>';
    settingsChart+='<div class="area-half">';
    settingsChart+='<select class="type" name="chartType">';
    settingsChart+='<option value="spline">Spline</option>';
    settingsChart+='<option value="column">Bars</option>';
    settingsChart+='<option value="area">Area</option>';
    settingsChart+='</select>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_variation+'*</div>';
    settingsChart+='<div class="area-half">';
    settingsChart+='<div class="var"><input type="checkbox" name="v"/>'+client.main.messages.label_varValue+'</div>';
    settingsChart+='<div class="var"><input type="checkbox" name="vt"/>'+client.main.messages.label_varTrend+'</div>';
    settingsChart+='<div class="var"><input type="checkbox" name="vc"/>'+client.main.messages.label_varCyclical+'</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half bothMargin">'+client.main.messages.label_webService+'*</div>';
    settingsChart+='<div class="area-half-1 bothMargin">';
    settingsChart+='<select class="webServiceSelector" name="webServiceSelector"></select>';
    settingsChart+='</div>';
    settingsChart+='<div class="area-half-2 bothMargin callTreeView">';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_endPoint+'*</div>';
    settingsChart+='<div class="area-half-1">';
    settingsChart+='<input type="text" name="endPoint"/>' ;
    settingsChart+='</div>' ;
    settingsChart+='<div class="area-half-2">';
    settingsChart+='<input type="button" value="Test" onclick="testWBS(this,\'endPoint\');"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_endpointType+'*</div>';
    settingsChart+='<div class="area-half-1">';
    settingsChart+='<input type="text" name="endPointType"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half downMargin">'+client.main.messages.label_endPointV20+'*</div>';
    settingsChart+= '<div class="area-half-1 downMargin">';
    settingsChart+='<input type="text" name="endPointV20"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="area-half-2">';
    settingsChart+='<input type="button" value="Test" value="Test" onclick="testWBS(this,\'endPointV20\');"/>' ;
    settingsChart+='</div>' ;
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_id+'*</div>' ;
    settingsChart+='<div class="area-half">';
    settingsChart+='<input type="text" name="id"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_agency+'*</div>';
    settingsChart+='<div class="area-half">';
    settingsChart+='<input type="text" name="agency"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<div class="title-half">'+client.main.messages.label_version+'*</div>' ;
    settingsChart+='<div class="area-half">';
    settingsChart+='<input type="text" name="version"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>';
    settingsChart+='<hr/>' ;
    settingsChart+='<div class="widget-button" id="wdgCriteria">';
    settingsChart+='<a class="data-option" onclick="Draw_Button_Filter(this);"><i class="icon-filter"></i>'+client.main.messages.label_open_criteria+'</a>';
    settingsChart+='<input type="hidden" name="criteria"/>';
    settingsChart+='</div>' ;
    /*
    settingsChart+='<div class="widget-button" id="wdgQuery">';
    settingsChart+='<a class="data-option" onclick="Draw_Button_LabelSeries(this);"><i class="icon-menu"></i>'+client.main.messages.label_serie_label+'</a>' ;
    settingsChart+='<input type="hidden" name="label_series"/>';
    settingsChart+='</div>';
    settingsChart+='<div class="clear-box"></div>' ;
    settingsChart+='<div class="widget-button" id="wdgQuery">';
    settingsChart+='<a class="data-option" onclick="Draw_Button_DimensionAxe(this);"><i class="icon-menu"></i>'+client.main.messages.label_dimension_axe+'</a>';
    settingsChart+='<input type="hidden" name="dimension_axe"/>';
    settingsChart+='</div>';
    settingsChart += '<div class="clear-box"></div>';
    */
    settingsChart+='<div class="widget-button" id="wdgQuery">';
    settingsChart+='<a class="data-option" onclick="Draw_Button_Query(this);"><i class="icon-check"></i>'+client.main.messages.label_test_query+'</a>' ;
    settingsChart+='<input type="hidden" name="configuration"/>';
    settingsChart+='</div>' ;
    settingsChart+='<div class="clear-box"></div>' ;
    settingsChart+='</div>';
    return settingsChart;
}
/*************************************/
function GetHTML_SettingsText() {
    var settingsText = '<div class="widgetSettings text">' ;
    settingsText += '<input type="hidden" name="widget_type" value="text"/>';
    settingsText +='<div class="title-half">'+client.main.messages.label_language+'</div>';
    settingsText +='<div class="area-half">';
    settingsText += '<select name="select-locale">';
    $.each(systemLang, function (value, display) {
        settingsText += '<option value="' + value + '" ' + ((value == selectedLang) ? ' selected="selected" ' : "") + ' >' + display + '</option>';
    });
    settingsText += '</select>';
    settingsText += '</div>';
    settingsText += '<div class="clear-box"></div>';
    settingsText += '<div class="title-full">' + client.main.messages.label_title + '*</div>';
    settingsText += '<div class="area-full">';
    settingsText += '<input type="text" name="title"/>';
    settingsText += '</div>';
    settingsText += '<div class="title-full">' + client.main.messages.label_wdgContent + '*</div>';
    settingsText += '<div class="area-full">';
    settingsText += '<textarea></textarea>';
    settingsText += '</div>';
    settingsText += '</div>';
    return settingsText;
}
/*************************************/
function GetHTML_SettingsTable() {
    var settingsTable = '<div class="widgetSettings table">';
    settingsTable+='<input type="hidden" name="widget_type" value="table"/>';
    settingsTable+='<div class="title-half">Select Language</div>';
    settingsTable+='<div class="area-half">';
    settingsTable += '<select name="select-locale">';
    $.each(systemLang, function (value, display) {
        settingsTable += '<option value="' + value + '" ' + ((value == selectedLang) ? ' selected="selected" ' : "") + ' >' + display + '</option>';
    });
    settingsTable += '</select>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_title + '*</div>';
    settingsTable += '<div class="area-half">';
    settingsTable += '<input type="text" name="title"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_variation + '*</div>';
    settingsTable += '<div class="area-half">';
    settingsTable += '<div class="var"><input class="sing_checkbox" type="checkbox" onclick="clearOther(this);" name="v"/>' + client.main.messages.label_varValue + '</div>';
    settingsTable += '<div class="var"><input class="sing_checkbox" type="checkbox" onclick="clearOther(this);" name="vt"/>' + client.main.messages.label_varTrend + '</div>';
    settingsTable += '<div class="var"><input class="sing_checkbox" type="checkbox" onclick="clearOther(this);" name="vc"/>' + client.main.messages.label_varCyclical + '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half bothMargin">' + client.main.messages.label_webService + '*</div>';
    settingsTable += '<div class="area-half-1 bothMargin">';
    settingsTable += '<select class="webServiceSelector" name="webServiceSelector"></select>';
    settingsTable += '</div>';
    settingsTable += '<div class="area-half-2 bothMargin callTreeView">';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_endPoint + '*</div>';
    settingsTable += '<div class="area-half-1">';
    settingsTable += '<input type="text" name="endPoint"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="area-half-2">';
    settingsTable += '<input type="button" value="Test" value="Test" onclick="testWBS(this,\'endPoint\');"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_endpointType + '*</div>';
    settingsTable += '<div class="area-half-1">';
    settingsTable += '<input type="text" name="endPointType"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half downMargin">' + client.main.messages.label_endPointV20 + '*</div>';
    settingsTable += '<div class="area-half-1 downMargin">';
    settingsTable += '<input type="text" name="endPointV20"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="area-half-2">';
    settingsTable += '<input type="button" value="Test" value="Test" onclick="testWBS(this,\'endPointV20\');"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_id + '*</div>';
    settingsTable += '<div class="area-half">';
    settingsTable += '<input type="text" name="id"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_agency + '*</div>';
    settingsTable += '<div class="area-half">';
    settingsTable += '<input type="text" name="agency"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<div class="title-half">' + client.main.messages.label_version + '*</div>';
    settingsTable += '<div class="area-half">';
    settingsTable += '<input type="text" name="version"/>';
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '<hr/>';
    settingsTable += '<div class="widget-button" id="wdgCriteria">';
    settingsTable += '<a class="data-option" onclick="Draw_Button_Filter(this);"><i class="icon-filter"></i>' + client.main.messages.label_open_criteria + '</a>';
    settingsTable += "<input type='hidden' name='criteria'/>";
    settingsTable += '</div>';
    settingsTable += '<div class="widget-button" id="wdgLayout">';
    settingsTable += '<a class="data-option" onclick="Draw_Button_Layout(this);"><i class="icon-filter"></i>' + client.main.messages.label_open_layout + '</a>';
    settingsTable += "<input type='hidden' name='layout'/>";
    settingsTable += '</div>';
    settingsTable += '<div class="widget-button" id="wdgQuery">';
    settingsTable += '<a class="data-option" onclick="Draw_Button_Query(this);"><i class="icon-check"></i>' + client.main.messages.label_test_query + '</a>';
    settingsTable += "<input type='hidden' name='configuration'/>";
    settingsTable += '</div>';
    settingsTable += '<div class="clear-box"></div>';
    settingsTable += '</div>';
    return settingsTable;
}
/*************************************/
function GetHTML_TextFields() {
    var textFields = "<input type='hidden' name='widget_type' value='text'/>" +
                        "<input type='hidden' name='title' value='" + JSON.stringify(objTitle) + "'/>" +
                        "<input type='hidden' class='text'  name='content' value='" + JSON.stringify(objContent) + "'/>";
    return textFields;
}
/*************************************/
function GetHTML_ChartFields() {
    var chartFields = "<input type='hidden' name='widget_type' value='chart'/>" +
                        "<input type='hidden' name='title' value='" + JSON.stringify(objTitle) + "' />" +
                        "<input type='hidden' name='chartType'/>" +
                        "<input type='hidden' name='v'/>" +
                        "<input type='hidden' name='vt'/>" +
                        "<input type='hidden' name='vc'/>" +
                        "<input type='hidden' name='webService'/>" +
                        "<input type='hidden' name='endPoint'/>" +
                        "<input type='hidden' name='endPointType'/>" +
                        "<input type='hidden' name='endPointV20'/>" +
                        "<input type='hidden' name='id'/>" +
                        "<input type='hidden' name='agency'/>" +
                        "<input type='hidden' name='version'/>" +
                        "<input type='hidden' name='configuration'/>" +
                        "<input type='hidden' name='criteria'/>" +
                        "<input type='hidden' name='label_series'/>" +
                        "<input type='hidden' name='dimension_axe'/>";
    return chartFields;
}
/*************************************/
function GetHTML_TableFields() {
    var tableFields = "<input type='hidden' name='widget_type' value='table'/>" +
                        "<input type='hidden' name='title' value='" + JSON.stringify(objTitle) + "' />" +
                        "<input type='hidden' name='v'/>" +
                        "<input type='hidden' name='vt'/>" +
                        "<input type='hidden' name='vc'/>" +
                        "<input type='hidden' name='webService'/>" +
                        "<input type='hidden' name='endPoint'/>" +
                        "<input type='hidden' name='endPointType'/>" +
                        "<input type='hidden' name='endPointV20'/>" +
                        "<input type='hidden' name='id'/>" +
                        "<input type='hidden' name='agency'/>" +
                        "<input type='hidden' name='version'/>" +
                        "<input type='hidden' name='configuration'/>" +
                        "<input type='hidden' name='criteria'/>" +
                        "<input type='hidden' name='layout'/>";
    return tableFields;
}

/*************************************/
function DrawTable(num, objDashBoard, dashBoardId) {

    current_dashBoardId = dashBoardId;

    var tab = $(document.createElement('div'));
    var dashBoard_ID = -1;
    //EDITING AN EXISTING DASHBOARD
    if (objDashBoard != undefined) {
        var dbTitle;
        if (dashBoardId != undefined) { dashBoard_ID = dashBoardId; }
        $.each(objDashBoard.text, function (k, v) {
            if (v.locale == selectedLang) { dbTitle = v.title }
        });
        $(tab).addClass('divTable');
        $(tab).append('<h2><i class="icon-gauge"> </i>Dashboard: ' + dbTitle + '</h2><input type="hidden" id="dashTitleHidden" name="dashTitleHidden"/><input type="hidden" id="dashRowsHidden" name="dashRowsHidden"/>');
        $(tab).append('<div class="addRowTable"><a class="selWidget" onclick="AddRow(this);"><i class="icon-plus"></i>' + client.main.messages.label_addRow + '</a></div>');
        $(tab).find('#dashTitleHidden').val(JSON.stringify(objDashBoard.text));
        $(tab).find('#dashRowsHidden').val(num);
        for (i = 1; i <= num; i++) {
            var wdgTitleLang = {};
            var wdgContentLang = {};
            //CONTENT {"title":{"it":"","en":""}}
            $.each(systemLang, function (i, val) {
                wdgTitleLang[val] = "";
            });
            //TITLE {"title":{"it":"","en":""}}
            var objTitle = {
                wdgContentLang: objLang,
            };
            var objRow = objDashBoard.rows[i - 1];
            var splittedRow = false;
            if (objRow.splitted) { splittedRow = 'selected="selected"'; }
            var inputDiv = "divSplit-" + i;
            var widgets = "";
            var countWidgets = 0;
            $.each(objRow.widgets, function (k, v) {
                countWidgets++;
                $.each(v.text, function (k, v) {
                    wdgTitleLang[v.locale] = v.title;
                    wdgContentLang[v.locale] = v.content;
                });
                var wdgHiddenTitle = { title: wdgTitleLang };
                var wdgHiddenContent = { content: wdgContentLang };
                //POPULATE CONFIGURATION
                if (v.type == "text") {
                    var hiddenParams = $(document.createElement('div'));
                    $(hiddenParams).html(GetHTML_TextFields());
                    $(hiddenParams).find("input[name='title']").val(JSON.stringify(wdgHiddenTitle));
                    $(hiddenParams).find("input[name='content']").val(JSON.stringify(wdgHiddenContent));
                    hiddenFields = $(hiddenParams).html();
                    aClass = "pencil";
                } else if (v.type == "chart") {
                    var hiddenParams = $(document.createElement('div'));
                    $(hiddenParams).html(GetHTML_ChartFields());
                    $(hiddenParams).find("input[name='title']").val(JSON.stringify(wdgHiddenTitle));
                    $(hiddenParams).find("input[name='chartType']").val(v.chartype);
                    $(hiddenParams).find("input[name='endPoint']").val(v.endPoint);
                    $(hiddenParams).find("input[name='endPointType']").val(v.endPointType);
                    $(hiddenParams).find("input[name='endPointV20']").val(v.endPointV20);
                    $(hiddenParams).find("input[name='v']").val(v.v);
                    $(hiddenParams).find("input[name='vt']").val(v.vt);
                    $(hiddenParams).find("input[name='vc']").val(v.vc);
                    $(hiddenParams).find("input[name='id']").val(v.dataflow_id);
                    $(hiddenParams).find("input[name='agency']").val(v.dataflow_agency_id);
                    $(hiddenParams).find("input[name='version']").val(v.dataflow_version);
                    $(hiddenParams).find("input[name='criteria']").val(v.criteria);
                    hiddenFields = $(hiddenParams).html();
                    aClass = "chart-bar";
                } else if (v.type == "table") {
                    var hiddenParams = $(document.createElement('div'));
                    $(hiddenParams).html(GetHTML_TableFields());
                    $(hiddenParams).find("input[name='title']").val(JSON.stringify(wdgHiddenTitle));
                    $(hiddenParams).find("input[name='endPoint']").val(v.endPoint);
                    $(hiddenParams).find("input[name='endPointType']").val(v.endPointType);
                    $(hiddenParams).find("input[name='endPointV20']").val(v.endPointV20);
                    $(hiddenParams).find("input[name='v']").val(v.v);
                    $(hiddenParams).find("input[name='vt']").val(v.vt);
                    $(hiddenParams).find("input[name='vc']").val(v.vc);
                    $(hiddenParams).find("input[name='id']").val(v.dataflow_id);
                    $(hiddenParams).find("input[name='agency']").val(v.dataflow_agency_id);
                    $(hiddenParams).find("input[name='version']").val(v.dataflow_version);
                    $(hiddenParams).find("input[name='criteria']").val(v.criteria);
                    $(hiddenParams).find("input[name='layout']").val(v.layout);
                    hiddenFields = $(hiddenParams).html();
                    aClass = "table";
                }
                widgets = widgets + '<div class="' + v.cssClass + '">' +
                 "<div class='confWdg configured' ><input type='hidden' value='" + v.type + "' class='valConf'/>" +
                                                                             "<div class='wdgName'><i class='icon-" + aClass + "'></i>" + v.type + " widget</div>" +
                                                                             "<div class='wdgSettings' onclick='ConfigWidget(this);'  title='" + client.main.messages.label_configureWidget + "' ><i class='icon-menu'></i></div>" +
                                                                             "<div class='wdgDelete' onclick='RemoveWidget(this);'  title='" + client.main.messages.label_removeWidget + "' ><i class='icon-cancel'></i></div>" +
                                                                             hiddenFields +
                                                                             "<div class='clear-box'></div>" +
                                                                         "</div>" +
                 '<a class="selWidget hide" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a></div>';
            });
            //IF THERE IS ONLY ONE WIDGET AND THE ROW IS NOT SPLITTED, ADD THE "ADD WIDGET" BUTTON (HIDDEN) 
            if (countWidgets == 1 && !objRow.splitted) {
                widgets = widgets + '<div class="divTd hide">' +
                    '<a class="selWidget" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a>' +
                    '</div>';
            }

            $(tab).append('<div class="divRow"><div class="setRow">' +
                            '<span class="removeRow"><a id="#row_' + i + '" title="' + client.main.messages.label_removeRow + '" href="#" onclick="RemoveRow(this);"><i class="icon-cancel-squared"></i></a></span>' +
                            '<select class="changeSlice" onchange="splitDiv(\'' + inputDiv + '\',this);">' +
                            '<option value="100%">100%</option><option ' + splittedRow + ' value="50%">50%</option>' +
                            '</select></div><div class="divSplit" id="divSplit-' + i + '">' + widgets + '<div class="clear-box"></div></div></div>');
        }

        //ADDING A NEW DASBOARD
    } else {

        $(tab).addClass('divTable');
        $(tab).append('<h2><i class="icon-gauge"> </i>Dashboard: ' + $("#id_dashTitle").val() + '</h2>');
        $(tab).append('<div class="addRowTable"><a class="selWidget" onclick="AddRow(this);"><i class="icon-plus"></i>' + client.main.messages.label_addRow + '</a></div>');
        for (i = 1; i <= num; i++) {
            var inputDiv = "divSplit-" + i;
            $(tab).append('<div class="divRow"><div class="setRow">' +
                            '<span class="removeRow"><a title="' + client.main.messages.label_removeRow + '" href="#" onclick="RemoveRow(this);"><i class="icon-cancel-squared"></i></a></span>' +
                            '<select class="changeSlice" onchange="splitDiv(\'' + inputDiv + '\',this);">' +
                            '<option value="100%">100%</option><option value="50%">50%</option>' +
                            '</select></div><div class="divSplit" id="divSplit-' + i + '">' +
                            '<div class="divTd full">' +
                            '<a class="selWidget" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a>' +
                    '</div><div class="divTd hide">' +
                    '<a class="selWidget" onclick="selectWidget(this);"><i class="icon-plus"></i>' + client.main.messages.label_addWidget + '</a>' +
                    '</div><div class="clear-box"></div></div></div>');
        }

    }

    $("#div_table").append(tab);
    $("#div_table").append('<div class="dashboardButtons">' +
                    "<div class='dash-save'>" +
                        " <input type='button' id='d-saveDash' value='" + client.main.messages.label_buttonSave + "' onclick='SaveDashboard(this," + dashBoard_ID + ");'/>" +
                    "</div>" +
                    "<div class='dash-delete'>" +
                        " <input type='button' id='d-removeDash' value='" + client.main.messages.label_cancel + "' onclick='RemoveDashboard(this);'/>" +
                    "</div>" +
                    "<div class='clear-box'></div>" +
                '</div>');

}

/*************************************/
function ShowDialogDashboard(title, text, useOk,callBackOk,useCancel, callBackCancel) {

    var options={
        resizable: false,
        position: { my: "center", at: "center", of: window },
        modal: true,
        buttons: {}
    };

    var div_fail = document.createElement("div");
    $(div_fail).attr("title", (title!=undefined)?title:"");
    $(div_fail).html((text != undefined) ? "<p><span class='ui-icon ui-icon-circle-check' style='float:left; margin:0 7px 20px 0;'></span>" + text + "</p>" : "");

    if (useOk)
        options.buttons[client.main.messages.label_ok] = function () {
            $(this).dialog("close");
            if (callBackOk != undefined) callBackOk();
        };

    if (useCancel)
        options.buttons[client.main.messages.label_cancel] = function () {
            $(this).dialog("close");
            if (callBackCancel != undefined) callBackCancel();
        };

    $(div_fail).dialog(options);

}
/*************************************/
function OnDashboardError(text) {
    var div_fail = document.createElement("div");
    $(div_fail).attr("title", client.main.messages.label_msgGenError);
    $(div_fail).html(text);
    $(div_fail).dialog({
        resizable: false,
        height: 140,
        position: { my: "center", at: "center", of: window },
        modal: true,
        buttons: {
            'Ok': function () {
                $(this).dialog("close");
            }
        }
    });
}
/*************************************/
function EmptyDashboardWorkingArea() {

    current_dashBoardId = -1;

    $(".container_div").remove();
    $("#clean-box").remove();

}
/*************************************/
function insertLangField(inputText, typeObj, contObj) {
    //SETTING TITLE VALUE BY SELECTED LANGUAGE
    var selectedLang;
    var inputTitle;
    var oTitle;
    var oContent;
    if (typeObj == "dashboard") {
        selectedLang = $(inputText).parent().parent(".dashboardAdd").find("select[name='select-locale']").val(); //selected language
        inputTitle = $(inputText).parent().parent(".dashboardAdd").find("input[name='title']"); //dashboard hidden title with all languages

        oTitle = JSON.parse($(inputTitle).val()); //object made by input hidden value (array)
        oTitle.title[selectedLang] = $(inputText).val();
        $(inputTitle).val(JSON.stringify(oTitle));
    }
    else {
        selectedLang = $(inputText).parent().parent(".widgetSettings").find("select[name='select-locale']").val(); //selected language
        if ($(inputText).attr("name") == "title") {
            inputTitle = $(contObj).find("input[name='title']"); //dashboard hidden title with all languages
            oTitle = JSON.parse($(inputTitle).val()); //object made by input hidden value (array)
            oTitle.title[selectedLang] = $(inputText).val();
            $(inputTitle).val(JSON.stringify(oTitle));
        } else {
            inputTitle = $(contObj).find("input[name='content']"); //dashboard hidden title with all languages
            oContent = JSON.parse($(inputTitle).val()); //object made by input hidden value (array)
            oContent.content[selectedLang] = $(inputText).val();
            $(inputTitle).val(JSON.stringify(oContent));
        }
    }
}
/*************************************/
function checkLangField(langObj, typeObj, contObj) {
    //READING TITLE VALUE BY SELECTED LANGUAGE
    var selectedLang = $(langObj).val(); //selected language
    var idashTitle;
    var inputTitle;
    var idashContent;
    var inputContent;
    if (typeObj == "dashboard") {
        idashTitle = $(langObj).parent().parent(".dashboardAdd").find("#id_dashTitle"); //dashboard title on screen
        inputTitle = $(langObj).parent().parent(".dashboardAdd").find("input[name='title']"); //dashboard hidden title with all languages
    } else if (typeObj == "text") {
        idashTitle = $(langObj).parent().parent(".widgetSettings").find("input[name='title']"); //dashboard title on screen
        inputTitle = $(contObj).find("input[name='title']"); //dashboard hidden title with all languages
        idashContent = $(langObj).parent().parent(".widgetSettings").find("textarea"); //dashboard content on screen
        inputContent = $(contObj).find("input[name='content']"); //dashboard hidden content with all languages
        var oContent = JSON.parse($(inputContent).val()); //object made by input hidden value (array)
        $(idashContent).val(oContent.content[selectedLang]);
    } else {
        idashTitle = $(langObj).parent().parent(".widgetSettings").find("input[name='title']"); //dashboard title on screen
        inputTitle = $(contObj).find("input[name='title']"); //dashboard hidden title with all languages
    }
    var oTitle = JSON.parse($(inputTitle).val()); //object made by input hidden value (array)
    $(idashTitle).val(oTitle.title[selectedLang]);
}
/*************************************/
function clearOther(sender) {
    $(".sign_chekbox").prop('checked', false);
    $(sender).prop('checked', true);
}