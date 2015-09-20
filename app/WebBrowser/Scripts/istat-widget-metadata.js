// widget base
function WidgetMetadata(options) {

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
            GetMetaData: "Main/GetMetadata",
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


    function DrawHTML_DettailArtefact(dest, typeArtefact, _id, _agency, _version, configuration, url) {

        clientShowWaitDialog();

        var _data = {
            Artefact: {
                id: _id,
                agency: _agency,
                version: _version
            },
            Configuration: configuration,
            ArtefactType: typeArtefact
        };
        clientPostJSON(
                url,
                clientParseObjectToJson(_data),
                function (jsonString) {

                    $(dest).empty();
                    clientCloseWaitDialog();

                    var result = clientParseJsonToObject(jsonString);

                    if (result != null && !result.hasOwnProperty('error')) {
                        if (typeArtefact == "DSD") {


                            // DATASTRUCTURE DEFINITION
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);
                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('DATASTRUCTURE DEFINITION');
                            $(td).appendTo(tr);
                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('Name & Description');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Dsd.Name);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Dsd.Id);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Dsd.Version);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Dsd.Agency);
                            $(td).appendTo(tr);

                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Dsd.Desc);
                            $(td).appendTo(tr);

                            // DIMENSIONS
                            table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);

                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('colspan', '10');
                            $(td).append('DIMENSIONS');
                            $(td).appendTo(tr);

                            // Row 2
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('colspan', '5');
                            $(td).append('CONCEPT');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('RAPPRESENTATION');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '1');
                            $(td).attr('rowspan', '3');
                            $(td).append('Dimension Type');
                            $(td).appendTo(tr);

                            // Row 3
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('Name');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CONCEPT_SCHEME');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CODELIST');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('TEXT FORMAT');
                            $(td).appendTo(tr);
                            // Row 4
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row Items
                            $.each(result.Dimension, function (key, dimension) {

                                var _tr = document.createElement('tr');
                                $(_tr).appendTo(table_dett);

                                var _td = document.createElement('td');
                                $(_td).append(dimension.Concept_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.Concept_Name);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(dimension.ConceptScheme_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.ConceptScheme_Agency);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.ConceptScheme_Version);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(dimension.Codelist_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.Codelist_Agency);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.Codelist_Version);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(dimension.TextFormat);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(dimension.DimensionType);
                                $(_td).appendTo(_tr);

                            });

                            // Measure
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);

                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '12');
                            $(td).append('MEASURES');
                            $(td).appendTo(tr);

                            // Row 2
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('TYPE');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '5');
                            $(td).append(' CONCEPT');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('RAPPRESENTATION');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('MEASURE DIMENSION');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('CODE');
                            $(td).appendTo(tr);

                            // Row 3
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('Name');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CONCEPT_SCHEME');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CODELIST');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('TEXT FORMAT');
                            $(td).appendTo(tr);
                            // Row 4
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row Items
                            $.each(result.Measure, function (key, measure) {

                                var _tr = document.createElement('tr');
                                $(_tr).appendTo(table_dett);

                                var _td = document.createElement('td');
                                $(_td).append(measure.Type);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(measure.Concept_Id);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(measure.Concept_Name);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(measure.ConceptScheme_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.ConceptScheme_Agency);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.ConceptScheme_Version);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(measure.Codelist_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.Codelist_Agency);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.Codelist_Version);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(measure.TextFormat);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.MeasureDimension);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(measure.Code);
                                $(_td).appendTo(_tr);
                            });

                            // Attribute
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);

                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '12');
                            $(td).append('ATTRIBUTE');
                            $(td).appendTo(tr);

                            // Row 2
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('ATTACHMENT LEVEL');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '5');
                            $(td).append('CONCEPT');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('RAPPRESENTATION');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('ATTRIBUTE TYPE');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '3');
                            $(td).append('ASSIGNMENT STATUS');
                            $(td).appendTo(tr);

                            // Row 3
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('Name');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CONCEPT_SCHEME');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CODELIST');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).attr('rowspan', '2');
                            $(td).append('TEXT FORMAT');
                            $(td).appendTo(tr);
                            // Row 4
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row Items
                            $.each(result.Attribute, function (key, attribute) {

                                var _tr = document.createElement('tr');
                                $(_tr).appendTo(table_dett);

                                var _td = document.createElement('td');
                                $(_td).append(attribute.AttachmentLevel);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(attribute.Concept_Id);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(attribute.Concept_Name);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(attribute.ConceptScheme_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.ConceptScheme_Version);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.ConceptScheme_Agency);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(attribute.Codelist_Id);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.Codelist_Version);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.Codelist_Agency);
                                $(_td).appendTo(_tr);

                                _td = document.createElement('td');
                                $(_td).append(attribute.TextFormat);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.AttributeType);
                                $(_td).appendTo(_tr);
                                _td = document.createElement('td');
                                $(_td).append(attribute.AssignmentStatus);
                                $(_td).appendTo(_tr);

                            });

                        } else if (typeArtefact == "CONCEPTSCHEME") {

                            // CONCEPTSCHEME
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);
                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('CONCEPT SCHEME');
                            $(td).appendTo(tr);
                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('Name & Description');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Name);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Id);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Version);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Agency);
                            $(td).appendTo(tr);

                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Desc);
                            $(td).appendTo(tr);

                            // ITEMS
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);
                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '2');
                            $(td).append('CONCEPTS');
                            $(td).appendTo(tr);

                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('Name');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);

                            $.each(result.Items, function (key, el) {

                                var _tr = document.createElement('tr');
                                $(_tr).appendTo(table_dett);
                                var _td_items = document.createElement('td');
                                $(_td_items).append(el.Name);
                                $(_td_items).appendTo(_tr);
                                _td_items = document.createElement('td');
                                $(_td_items).append(el.Code);
                                $(_td_items).appendTo(_tr);

                            });

                        } else if (typeArtefact == "CODELIST") {

                            // CODELIST
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);
                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '4');
                            $(td).append('CODELIST');
                            $(td).appendTo(tr);
                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('Name & Description');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('ID');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('VER');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('AGENCY');
                            $(td).appendTo(tr);

                            // Row 1
                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Name);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Id);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Version);
                            $(td).appendTo(tr);
                            td = document.createElement('td');
                            $(td).attr('rowspan', '2');
                            $(td).append(result.Agency);
                            $(td).appendTo(tr);

                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('td');
                            $(td).append(result.Desc);
                            $(td).appendTo(tr);

                            // ITEMS
                            var table_dett = document.createElement('table');
                            $(table_dett).addClass('table_metadata');
                            $(table_dett).appendTo(dest);
                            // Row 1
                            var tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            var td = document.createElement('th');
                            $(td).attr('colspan', '3');
                            $(td).append('CODES');
                            $(td).appendTo(tr);

                            tr = document.createElement('tr');
                            $(tr).appendTo(table_dett);
                            td = document.createElement('th');
                            $(td).append('Name');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('CODE');
                            $(td).appendTo(tr);
                            td = document.createElement('th');
                            $(td).append('PARENT');
                            $(td).appendTo(tr);

                            $.each(result.Items, function (key, el) {

                                var _tr = document.createElement('tr');
                                $(_tr).appendTo(table_dett);
                                var _td_items = document.createElement('td');
                                $(_td_items).append(el.Name);
                                $(_td_items).appendTo(_tr);
                                _td_items = document.createElement('td');
                                $(_td_items).append(el.Code);
                                $(_td_items).appendTo(_tr);
                                _td_items = document.createElement('td');
                                $(_td_items).append(el.Parent);
                                $(_td_items).appendTo(_tr);

                            });

                        }
                    }
                },
                function (event, status, errorThrown) {
                    clientCloseWaitDialog();
                    clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                    return;
                },
                false);
    }


};