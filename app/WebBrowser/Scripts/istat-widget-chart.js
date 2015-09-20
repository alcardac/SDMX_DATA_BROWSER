// widget base
function WidgetChart(options) {

    var settings = {
        widget: {
            manager: options.managerWidgets,
            classCSS: options.classCSS,
            template: options.template,
            endpoint: options.endpoint,
            target: options.target,
            configuration: options.configuration,
            data: options.data,
            messages: options.messages,
            locale: options.locale,
            event: {
                setup: this.Setup,
                bind: this.Bind,
            }
        },
        baseURL: "/",
        url: {
            GetChartData: "Main/GetChartData",
        },
        $items: {
            container: new clientItem(options.idCSS),
        },
        cssClass: {
            table_chart: "table_chart",
            td_ul: "td_ul",
            td_chart: "td_chart",
            div_chart: "div_chart",
        }
    };

    this.Setup = function () {

        try {
            settings.$items.container.$getItem().empty();
            $("<h3><i class='waitInLinee icon-spin6 animate-spin'></i>" + client.main.messages.text_wait + "</h3>").appendTo(settings.$items.container.$getItem());

            if (settings.widget.configuration == undefined) return;

            var data = {
                dataflow: {
                    id: settings.widget.configuration.dataflow.id,
                    agency: settings.widget.configuration.dataflow.agency,
                    version: settings.widget.configuration.dataflow.version
                },
                configuration: {
                    DecimalSeparator: settings.widget.configuration.DecimalSeparator,
                    EndPoint: settings.widget.configuration.EndPoint,
                    EndPointV20: settings.widget.configuration.EndPointV20,
                    EndPointType: settings.widget.configuration.EndPointType,
                    EndPointSource: settings.widget.configuration.EndPointSource,
                },
                //layout: settings.widget.data.query.layout,
                criteria: settings.widget.data.criteria,
                chartType: settings.widget.data.chartType,
                obsValue: settings.widget.data.obsValue,
                dimensionAxe: settings.widget.data.dimensionAxe,
                customKey: settings.widget.data.customKey,
                customChartType: settings.widget.data.customChartType,
                widgetId: settings.widget.data.widgetId
            }

            if (data.chartType == "pie") {
                var times = data.criteria["TIME_PERIOD"];
                data.criteria["TIME_PERIOD"] = [times[0]];
            }

            var render = this.Bind;

            clientPostJSON(settings.url.GetChartData, clientParseObjectToJson(data),
            function (jsonString) {
                if (jsonString != null) {
                    var result = clientParseJsonToObject(jsonString);
                    render((result != null) ? result : jsonString);
                    clientCloseWaitDialog();
                }
            },
            function (event, status, errorThrown) {
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                //clientAjaxError(event, status);
                return;
            },
            false);

        } catch (e) {
            clientCloseWaitDialog();
            return null;
        }

    };

    this.Bind = function (data){

        try {
            var title;
            if (settings.widget.data.text != undefined) {
                title = settings.widget.data.text[0].title;
                for (i = 0; i < settings.widget.data.text.length; i++)
                    if (settings.widget.data.text[i].locale == settings.widget.locale)
                        if (settings.widget.data.text[i].title != "")
                            title = settings.widget.data.text[i].title;

                var h3_name = document.createElement('h3');
                $(h3_name).append(title);
            }

            if (!isValidDataInput_Chart(data)) {

                settings.$items.container.$getItem().empty();
                $(h3_name).appendTo(settings.$items.container.$getItem());
                $(settings.$items.container.$getItem()).append('<p>' + data + '</p>');
                return;
            }

            /* CHART*/
            CanvasJS.addCultureInfo("en",
            {
                decimalSeparator: ".",// Observe ToolTip Number Format
                digitGroupSeparator: ""
            });

            var chart = document.createElement("div");
            $(chart).addClass(settings.cssClass.div_chart);
            $(chart).css('width', '100%');

            var options = get_chart_option(
                data.series_title,
                data.primary_name,
                data.primary_max,
                data.primary_min,
                data.secondary_name,
                data.secondary_max,
                data.secondary_min,
                data.x_name);

            options.data = data.series;

            if (data.series.length) {
                $(chart).CanvasJSChart(options);
                $(settings.$items.container.$getItem()).resize(function () { $(chart).CanvasJSChart().render(); });
            } else {
                clientCloseWaitDialog();
                $(chart).append(client.main.messages.no_result_found);
            }

            var full_screen_btn = document.createElement('span');
            $(full_screen_btn).css('float', 'right');
            $(full_screen_btn).css('margin', '-5px -5px 0px 0px');
            $(full_screen_btn).append("<i class='icon-monitor'></i>");
            $(full_screen_btn).button().click(function () {

                $(full_screen_btn).hide();

                $(settings.$items.container.$getItem()).dialog({
                    title: title,
                    height: $(window).height() - 20,
                    width: $(window).width() - 20,
                    modal: true,
                    resizable: true,
                    closeOnEscape: false,
                    draggable: true,
                    position: { my: "center", at: "center", of: window },
                    autoOpen: false,
                    buttons: [
                        {
                            text: client.main.messages.label_cancel,
                            click: function () {
                                $(full_screen_btn).show();
                                $(this).dialog("destroy");
                                $(chart).CanvasJSChart().render();
                            }
                        }
                    ]
                });
                $(settings.$items.container.$getItem()).dialog("open");
                $(chart).CanvasJSChart().render();
            });

            settings.$items.container.$getItem().empty();
            $(full_screen_btn).appendTo(h3_name);
            $(h3_name).appendTo(settings.$items.container.$getItem());
            $(chart).appendTo(settings.$items.container.$getItem());

            $(chart).CanvasJSChart().render();

            var widget_target = settings.widget.manager.GetWidget(settings.widget.target);
            if (widget_target != undefined)
                widget_target.Bind(data);

        }catch (e) {
            clientCloseWaitDialog();
            return null;
        }
    }

    function get_chart_option(title, primary_label, primary_max, primary_min, secondary_label, secondary_max, secondary_min, x_label) {

        var title_fontsize = 10;
        var label_fontsize = 10;

        var option = {
            culture: "en",
            toolTip: {
                shared: false,
                content: "<div style='max-width:150px;overflow:hidden;font-size:" + label_fontsize + "px'>" +
                    "<b>" + client.main.messages.label_varSerie + "</b>: {name}<br/>" +
                    "<b>" + x_label + "</b>: {label}<br/>" +
                    "<b>" + client.main.messages.label_varValue + "</b>: {y}</div>"
            },
            exportFileName: title,  //Give any name accordingly
            exportEnabled: true,
            zoomEnabled: true,
            panEnabled: true,
            animationEnabled: true,
            axisX: {
                labelAngle: -50,
                //valueFormatString: "####.0",
                title: x_label,
                titleFontColor: "#000000",
                titleFontSize: title_fontsize,
                titleFontFamily: "tahoma",
                titleFontWeight: "bolder",
                labelFontSize: label_fontsize,
                labelMaxWidth: 100,
                gridThickness: 0.5,
                tickLength: 8,
                tickColor: "#AAAAAA",
                tickThickness: 0.5,
                lineColor: "#EEEEEE",
                gridColor: "#DDDDDD",
            },
            axisY: {
                title: (primary_label != undefined) ? primary_label : "Primary",
                titleFontColor: "#000000",
                titleFontSize: title_fontsize,
                titleFontFamily: "tahoma",
                titleFontWeight: "bolder",
                labelFontSize: label_fontsize,
                labelMaxWidth: 100,
                gridThickness: 0.5,
                tickLength: 5,
                tickColor: "#AAAAAA",
                tickThickness: 0.5,
                lineColor: "#EEEEEE",
                gridColor: "#DDDDDD",
            },
            axisY2: {
                //valueFormatString: "####.0",
                title: (secondary_label != undefined) ? secondary_label : "Secondary",
                titleFontColor: "#000000",
                titleFontSize: title_fontsize,
                titleFontFamily: "tahoma",
                titleFontWeight: "bolder",
                labelFontSize: label_fontsize,
                labelMaxWidth: 100,
                gridThickness: 0.5,
                tickLength: 5,
                tickColor: "#AAAAAA",
                tickThickness: 0.5,
                lineColor: "#EEEEEE",
                gridColor: "#DDDDDD",
            },
            legend: {
                cursor: "pointer",
                itemclick: function (e) {
                    if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                        e.dataSeries.visible = false;
                    } else {
                        e.dataSeries.visible = true;
                    }
                    e.chart.render();
                }
            },
            data: []
        };

        if (primary_max != null && primary_min != null) {
            option.axisY.minimum = primary_min;
            option.axisY.maximum = primary_max;
        }
        if (secondary_max != null && secondary_min != null) {
            option.axisY2.minimum = secondary_min;
            option.axisY2.maximum = secondary_max;
        }

        return option;

    }

    function isValidDataInput_Chart(data) {

        if (data == null || data == undefined) return false;
        if (!data.hasOwnProperty('series_title')) return false;
        if (!data.hasOwnProperty('series')) return false;
        if (data.series==null) return false;

        return true;
    }

};