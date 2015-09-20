function WidgetDashBoard(options) {

    var settings = {
        widget: {
            manager: options.managerWidgets,
            classCSS: options.classCSS,
            template: options.template,
            endpoint: options.endpoint,
            target: options.target,
            messages: options.messages,
            locale: options.locale,
            event: {
                setup: this.Setup,
                bind: this.Bind,
            }
        },
        $items: {
            container: new clientItem(options.idCSS)
        }
    };

    //$.extend(settings.widget, options);

    this.Setup = function () {
        // get list dashboard
        var render = this.Bind;
        clientPostJSON(
            "Dashboard/GetDashboardsActive", null,
            function (jsonString) {
                try{
                    if (jsonString != undefined
                        && jsonString != null) {
                        var dashboards = clientParseJsonToObject(jsonString);
                        render(dashboards);
                    }
                } catch (ex) {

                }
            },
            function (event, status, errorThrown) {
                errorThrown = 'SetupJsonTree';
                clientAjaxError(event, status, errorThrown);
                return;
            }, false);

    }
    this.Bind = function (data) {
        var dest = settings.$items.container.$getItem();

        //var dashboard_container = document.createElement('div');

        if (data.length > 1){
            //  SetupCombo
            var dashboard_select = document.createElement('select');
            $(dashboard_select).attr('id', 'select-dash');
            
            for (i = 0; i < data.length; i++) {

                var data_wdg = data[i];

                if (data_wdg.text.length) {

                    var opt = document.createElement('option');
                    $(opt).attr('value', i);

                    var title = GetLocalisedText(data_wdg.text, client.main.config.locale);
                    $(opt).text(title);

                    $(opt).appendTo(dashboard_select);
                }
            }
            
            $('#menu-center').append('<p style="float:left;line-height:30px;margin:0px 0px 0px 20px;padding:0px 5px 0px 0px">Dashboard:</p>');
            $(dashboard_select).appendTo('#menu-center');
            $(dashboard_select).selectmenu({
                change: function (event, ui) {
                    $("#main-table-dataset").empty();
                    DrawHTML_DashBoard(dest, data[ui.item.value]);
                }
            });
        }

        if (data.length != 0) {
            // load first
            DrawHTML_DashBoard(dest, data[0]);
        }
    }

};

function DrawHTML_DashBoard(dest, dashboard) {

    $(dest).empty();
    try {

        var title = GetLocalisedText(dashboard.text, client.main.config.locale);
        /*
        var h1 = document.createElement('h1');
        $(h1).text(title);
        $(h1).appendTo(dest);
        */
        var tb = document.createElement('table');
        $(tb).addClass('tableDashboard');
        $(tb).css('width', '98%');
        //$(tb).css('float', 'left');
        $(tb).css('padding', '0');
        $(tb).css('margin', '0');
        for (i = 0; i < dashboard.rows.length; i++) {
            var row = dashboard.rows[i];

            if (row.widgets.length > 0) {

                var tr = document.createElement('tr');
                var td = document.createElement('td');
                $(td).attr('colspan', (row.splitted) ? 1 : 2);

                if (row.splitted) {
                    $(td).css('width', '46%');
                    //$(td).css('float', 'left');
                    $(td).css('padding', '0');
                    $(td).css('margin', '2%');
                    //$(td).css('overflow', 'auto');
                } else {
                    $(td).css('width', '92%');
                    //$(td).css('float', 'left');
                    $(td).css('padding', '0');
                    $(td).css('margin', '2%');
                    //$(td).css('overflow', 'auto');
                }
                $(td).append(CreateWidget(row.widgets[0]));
                $(td).appendTo(tr);

                if (row.splitted) {
                    var td_2 = document.createElement('td');
                    $(td_2).css('width', '46%');
                    //$(td).css('float', 'left');
                    $(td_2).css('padding', '0');
                    $(td_2).css('margin', '2%');
                    //$(td).css('overflow', 'auto');
                    $(td_2).append(CreateWidget(row.widgets[1]));
                    $(td_2).appendTo(tr);
                }

                $(tr).appendTo(tb);
            }
        }
        $(tb).appendTo(dest);

        client.main.manager.widget.SetupWidgets(
            $(tb).find(".dinamic-widget"),
            client.main.config.locale,
            client.main.messages);

    } catch (ex) { }

    $(dest).show();
}

function CreateWidget(template_widget) {

    var idWidget = template_widget.id;

    var conf;
    var data;

    if (template_widget.type == "text") {
        data = {
            text: template_widget.text
        };
    } else if (template_widget.type == "table") {
        obs_type_sel = [];
        
        data = {
            automation: true,
            text: template_widget.text,
            widgetId: template_widget.id,
            showObsValue: ((template_widget.v) ? 'v' : (template_widget.vt) ? 'vt' : (template_widget.vc) ? 'vc' : 'v')
        };
        conf = {
            dataflow: {
                id: template_widget.dataflow_id,
                agency: template_widget.dataflow_agency_id,
                version: template_widget.dataflow_version
            },
            configuration: {
                DecimalSeparator: template_widget.endPointDecimalSeparator,
                EndPoint: template_widget.endPoint,
                EndPointV20: template_widget.endPointV20,
                EndPointType: template_widget.endPointType,
                EndPointSource: template_widget.endPointSource,
            },
            layout: template_widget.layout,
            criteria: template_widget.criteria
        };
    } else if (template_widget.type == "chart") {

        obs_type_sel = [];
        if (template_widget.v) obs_type_sel.push('v');
        if (template_widget.vt) obs_type_sel.push('vt');
        if (template_widget.vc) obs_type_sel.push('vc');

        data = {
            criteria: clientParseJsonToObject(template_widget.criteria),
            chartType: template_widget.chartype,
            obsValue: obs_type_sel,
            text: template_widget.text,
            widgetId: template_widget.id
        }
        conf = {
            DecimalSeparator: template_widget.endPointDecimalSeparator,
            EndPoint: template_widget.endPoint,
            EndPointV20: template_widget.endPointV20,
            EndPointType: template_widget.endPointType,
            EndPointSource: template_widget.endPointSource,
            dataflow: {
                id: template_widget.dataflow_id,
                agency: template_widget.dataflow_agency_id,
                version: template_widget.dataflow_version
            }
        };

    }

    // Fixed problema con ' quando si trasforma in Json...
    // replace del carattere ' con il codice ASCII &#39; equivalente
    /*
    if (data.text != undefined) {
        for (k = 0; k < data.text.length; k++){
            if (data.text[k].hasOwnProperty('title')) {
                var clear_text = data.text[k].title;
                data.text[k].title = clear_text.replace(/'/g, "&#39;");
                clear_text = data.text[k].title;
                data.text[k].title = clear_text.replace(/"/g, "&#34;");
            }
            if (data.text[k].hasOwnProperty('content')){
                var clear_text = data.text[k].content;
                data.text[k].content = clear_text.replace(/'/g, "&#39;");
                clear_text = data.text[k].content;
                data.text[k].content = clear_text.replace(/"/g, "&#34;");
            }
        }
    }*/

    var html_ret = document.createElement('div');
    $(html_ret).attr('id', idWidget);
    $(html_ret).addClass('dinamic-widget');
    $(html_ret).data('widgetTemplate', template_widget.type);
    $(html_ret).data('widgetStylecss', template_widget.cssClass);
    $(html_ret).data('widgetConfiguration', conf);
    $(html_ret).data('widgetData', data);
    /*
    var html = "<div id='" + idWidget + "' class='dinamic-widget' " +
                " data-widget-template='" + template_widget.type + "' " +
                " data-widget-stylecss='" + template_widget.cssClass + "' "+
                " data-widget-configuration='" + clientParseObjectToJson(conf) + "' " +
                " data-widget-data='" + clientParseObjectToJson(data) + "'></div>";
    */
    return html_ret;
}