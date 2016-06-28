var criteriaMode = "standard";

var criteriCostraint = false;
var criteriLimit = false;
var count_glob = 0 ;
function WidgetDataset(options) {
    

    var dataTableID = null;

    var time_dimension_key = 'TIME_PERIOD';
    var time_dimension_name = 'Time period';

    var needCriteria = true;

    var viewMode = 0;

    var stub = false;

    var settings = {
        widget: {
            id:options.idCSS,
            manager: options.managerWidgets,
            classCSS: options.classCSS,
            template: options.template,
            target: options.target,
            configuration: options.configuration,
            data: options.data,
            messages: options.messages,
            locale: options.locale,
            event: {
                setup: this.Setup,
                bind: this.Bind,
            },
            store: {}
        },
        $items: {
            container: new clientItem(options.idCSS),
        },
        baseURL: "/",
        url: {
            GetResults: "Main/GetData",
            GetResultsLayout: "Main/GetDataLayout",
            GetResultTable: "Main/GetDataTable",
            GetFilters: "Main/GetCodemap",
            GetFiltersCostraint: "Main/GetSpecificCodemap",
            GetCodemapCostraintNoLimit:"Main/GetCodemapCostraintNoLimit",
            GetLayout: "Main/GetDefaultLayout",
            SaveQuery: "Query/Add",
            SaveTemplate: "Template/Add",
            DeleteTemplate: "Template/Del",
            GetChartData: "Main/GetChartData",
            GetQueryData: "Main/GetQueryData",
            CheckCacheDataset: "Main/IsCachingDataSet",
            GetMetadata: "Main/GetMetadata",
            Get_DOTSTAT_StructureObject: "Main/Get_DOTSTAT_StructureObject",
        },
        cssClass: {

            header_bar: "header_bar",
            button_bar: "button_bar",
            viewmode_bar: "viewmode_bar",
            slice_container: "slice_container",
            pagination_container: "pagination_container",
            data_container: "data_container",
            chart_container: "chart_container",
            meta_container: "meta_container",
            tool_bar: "tool_bar",
            btn_tool_bar: "data-option",

            table: "tb_data",// Table data
            tr: "tr_data",          
            td: "td_data",          
            th_fixed: "th_fixed",
            th_code: "th_code",

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
            
            td_slice_code: "td_slice_code", // criteria tabs
            td_slice_concept: "td_slice_concept", // criteria tabs
            tr_slice: "tr_slice", // criteria tabs
            table_slice: "table_slice", // criteria tabs

            pagination_link: "data-option",
            pagination_link_selected: "data-option-sel",

            
        },
        dataKey: {
            sdmxKey: "sdmxKey",
            sdmxValue: "sdmxValue",
            sdmxSerieV: "sdmxSerieV",
            sdmxSerieH: "sdmxSerieH",
        }
    };

    this.Setup = function () {
        try {

            settings.$items.container.$getItem().empty();
            $("<h3><i class='waitInLinee icon-spin6 animate-spin'></i>" + client.main.messages.text_wait + "</h3>").appendTo(settings.$items.container.$getItem());

            if (settings.widget.data != undefined
                && settings.widget.data.query != undefined){

                // if manual query is set 
                var data = {
                    dataflow: {
                        id: settings.widget.data.query.dataflow.id,
                        agency: settings.widget.data.query.dataflow.agency,
                        version: settings.widget.data.query.dataflow.version
                    },
                    configuration: {
                        DecimalSeparator: settings.widget.data.query.dataflow.configuration.DecimalSeparator,
                        EndPoint: settings.widget.data.query.dataflow.configuration.EndPoint,
                        EndPointType: settings.widget.data.query.dataflow.configuration.EndPointType,
                        EndPointV20: settings.widget.data.query.dataflow.configuration.EndPointV20,
                        EndPointSource: settings.widget.data.query.dataflow.configuration.EndPointSource,
                    },
                    layout: settings.widget.data.query.layout,
                    criteria: settings.widget.data.query.criteria
                }

                //criteriCostraint = false;
                needCriteria = false;

                $('#main-dashboard').hide();

                this.Bind(data);

            } else {

                if (settings.widget.configuration != undefined) {

                        // Try to get a start parameter

                        settings.widget.configuration.criteria = clientParseJsonToObject(settings.widget.configuration.criteria);
                        settings.widget.configuration.layout = clientParseJsonToObject(settings.widget.configuration.layout);

                        stub = (settings.widget.data != undefined) ?
                            (settings.widget.data.hasOwnProperty('automation')) ?
                            settings.widget.data.automation : false : false;

                        needCriteria = false;

                        this.Bind(settings.widget.configuration);
                }
            }
        } catch (e) {
            return null;
        }

    };
    this.Bind = function (data) {

        if (criteriaMode == "standard") {
            criteriCostraint = false;
            criteriLimit = false;
        } else if (criteriaMode == "costraint") {
            criteriCostraint = true;
            criteriLimit = true;
        } else if (criteriaMode == "costraint_no_limit") {
            criteriCostraint = true;
            criteriLimit = false;
            //criteriLimit = true;
        }
        /*
        if (settings.widget.data != undefined
            && settings.widget.data.query != undefined)
            criteriCostraint = false;
        */
        if(!stub)clientShowWaitDialog();

        if(!isValidDataInput_Configuration(data)) {

            clientCloseWaitDialog();

            // Log error if admin loged in show complex message
            if (sessionStorage.user_role != undefined) {
                var usRole = JSON.parse(sessionStorage.user_role);
                if (usRole.RoleId == 1) {
                    $(settings.$items.data_container).html(data);
                } else {
                    $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
                }
            } else {
                $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
            }
        }

        var _data = {
            dataflow: data.dataflow,
            configuration: data.configuration,
        }

        settings.widget.store = data;

        if (settings.widget.data != undefined
            && settings.widget.data.widgetId != undefined) {

            //settings.widget.store.configuration.locale = client.main.config.locale;

            var _dataCheck = {
                dataflow: settings.widget.store.dataflow,
                configuration: settings.widget.store.configuration,
                WidgetId: settings.widget.data.widgetId
            };

            clientPostJSON(
                settings.url.CheckCacheDataset,
                clientParseObjectToJson(_dataCheck),
                function (jsonString) {
                    
                    var result = clientParseJsonToObject(jsonString);
                    if (result.hasOwnProperty('success')) {


                        DrawHTML(settings.$items.container.$getItem());
                        //Process_RequestData();

                    } else {

                        switch (data.dataflow.source) {
                            case "DOTSTAT": Process_Type_DOTSTAT(_data); break;
                            case "ECB": Process_Type_ECB(_data); break;
                            case "ESTAT": Process_Type_ESTAT(_data); break;
                            case "RI": Process_Type_RI(_data); break;
                            default: Process_Type_RI(_data); break;
                        }
                    }
                });

        } else {
            switch (data.configuration.EndPointSource) {
                case "DOTSTAT": Process_Type_DOTSTAT(_data); break;
                case "ECB": Process_Type_ECB(_data); break;
                case "ESTAT": Process_Type_ESTAT(_data); break;
                case "RI": Process_Type_RI(_data); break;
                default: Process_Type_RI(_data); break;
            }
        }
    }

    function Process_Type_RI_CNL(_data) {
        clientPostJSON(
            settings.url.GetCodemapCostraintNoLimit,
            clientParseObjectToJson(_data),
            function (jsonString) {
                //alert(jsonString);
                var result = clientParseJsonToObject(jsonString);


                settings.widget.store.dimension = result.codemap;

                /*nuovo fabio 07/01/2016*/
                settings.widget.store.SliceKeyValidValues = {};
                if (result.hasOwnProperty('SliceKeyValidValues') && result.SliceKeyValidValues != null)
                    settings.widget.store.SliceKeyValidValues = result.SliceKeyValidValues;
                else settings.widget.store.SliceKeyValidValues = undefined;

                settings.widget.store.SliceKeyValues = {};
                if (result.hasOwnProperty('SliceKeyValues') && result.SliceKeyValues != null)
                    settings.widget.store.SliceKeyValues = result.SliceKeyValues;
                else settings.widget.store.SliceKeyValues = undefined;

                settings.widget.store.SliceKeys = {};
                if (result.hasOwnProperty('SliceKeys') && result.SliceKeys != null)
                    settings.widget.store.SliceKeys = result.SliceKeys;
                else settings.widget.store.SliceKeys = undefined;
            },
            function (event, status, errorThrown) {
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                //clientAjaxError(event, status);
                return;
            },
            false);

    }

    function Process_Type_RI(data){

        /*****************************************************************************
        Process request for filter
        *****************************************************************************/
        
        if (!stub)
            clientShowWaitDialog(settings.widget.messages.dataset_request_filters);

        clientPostJSON(
            (!criteriCostraint) ?
            settings.url.GetFilters :
            settings.url.GetFiltersCostraint,
            clientParseObjectToJson(data),
            function (jsonString) {
                //alert(jsonString);
                var result = clientParseJsonToObject(jsonString);
                
                if (!isValidDataOutput_Filter(result)) {

                    clientCloseWaitDialog();

                    // Log error if admin loged in show complex message
                    if (sessionStorage.user_role != undefined) {
                        var usRole = JSON.parse(sessionStorage.user_role);
                        if (usRole.RoleId == 1) {
                            clientShowErrorDialog(jsonString);
                        } else {
                            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                        }
                    } else {
                        clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                    }
                    return;
                }

                if (settings.widget.store.dataflow.name == undefined)
                    //fabio 4/11/2015
                    settings.widget.store.dataflow.name = result.dataflow.name.replace("'", "&#39;");
                    //fine fabio 4/11/2015
                if (settings.widget.store.dataflow.desc == undefined)
                    settings.widget.store.dataflow.desc = result.dataflow.desc;

                settings.widget.store.dimension = result.codemap;

                if (settings.widget.data != undefined
                    && settings.widget.data.automation != undefined) {

                    DrawHTML(settings.$items.container.$getItem());

                } else if (settings.widget.data != undefined
                    && settings.widget.data.query != undefined) {

                    DrawHTML(settings.$items.container.$getItem());

                } else if (settings.widget.data != undefined
                    && settings.widget.data.previus_query != undefined) {

                    DrawHTML(settings.$items.container.$getItem());

                } else {

                    settings.widget.store.enabledCri =true;
                    if (result.hasOwnProperty('enabledCri') && result.enabledCri != null)
                        settings.widget.store.enabledCri = result.enabledCri;
                    settings.widget.store.enabledVar = true;
                    if (result.hasOwnProperty('enabledVar') && result.enabledVar != null)
                        settings.widget.store.enabledVar = result.enabledVar;

                    settings.widget.store.hideDimension = {};
                    if (result.hasOwnProperty('hideDimension') && result.hideDimension != null)
                        settings.widget.store.hideDimension = result.hideDimension;

                    settings.widget.store.criteria = {};
                    if (result.hasOwnProperty('costraint') && result.costraint != null)
                        settings.widget.store.criteria = result.costraint;

                    settings.widget.store.codelist_target = {};
                    if (result.hasOwnProperty('codelist_target') && result.codelist_target != null)
                        settings.widget.store.codelist_target = result.codelist_target;
                    else settings.widget.store.codelist_target = undefined;
                    /*nuovo fabio*/
                    settings.widget.store.conceptnew = {};
                    if (result.hasOwnProperty('concept') && result.concept != null)
                        settings.widget.store.conceptnew = result.concept;
                    else settings.widget.store.conceptnew = undefined;

                    /*nuovo fabio 07/01/2016*/
                    settings.widget.store.SliceKeyValidValues = {};
                    if (result.hasOwnProperty('SliceKeyValidValues') && result.SliceKeyValidValues != null)
                        settings.widget.store.SliceKeyValidValues = result.SliceKeyValidValues;
                    else settings.widget.store.SliceKeyValidValues = undefined;

                    settings.widget.store.SliceKeyValues = {};
                    if (result.hasOwnProperty('SliceKeyValues') && result.SliceKeyValues != null)
                        settings.widget.store.SliceKeyValues = result.SliceKeyValues;
                    else settings.widget.store.SliceKeyValues = undefined;

                    settings.widget.store.SliceKeys = {};
                    if (result.hasOwnProperty('SliceKeys') && result.SliceKeys != null)
                        settings.widget.store.SliceKeys = result.SliceKeys;
                    else settings.widget.store.SliceKeys = undefined;

                    /*****************************************************************************/
                    /* Process request for layout
                    *****************************************************************************/

                    if (!stub) clientShowWaitDialog(settings.widget.messages.dataset_request_layout);
                    clientPostJSON(settings.url.GetLayout, clientParseObjectToJson(data),
                        function (jsonString) {

                            var result = clientParseJsonToObject(jsonString);

                            if (!isValidDataOutput_Layout(result)) {
                                // Log error if admin loged in show complex message
                                if (sessionStorage.user_role != undefined) {
                                    var usRole = JSON.parse(sessionStorage.user_role);
                                    if (usRole.RoleId == 1) {
                                        clientShowErrorDialog(jsonString);
                                    } else {
                                        clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                                    }
                                } else {
                                    clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                                }
                                return;
                            }

                            //force manual criteria
                            //result.DefaultLayout = undefined;

                            if (result.DefaultLayout == null || result.DefaultLayout == undefined) {
                                // need select criteria

                                needCriteria = true;

                                settings.widget.store.layout = {
                                    axis_x: [],
                                    axis_y: [],
                                    axis_z: [],
                                    block_axis_x: false,
                                    block_axis_y: false,
                                    block_axis_z: false,
                                };

                                $.each(settings.widget.store.dimension, function (idx, concept) {
                                    if (idx != time_dimension_key) {
                                        settings.widget.store.layout.axis_x.push(idx);
                                    }
                                });
                                settings.widget.store.layout.axis_y.push(time_dimension_key);

                            } else {

                                needCriteria = false;
                                settings.widget.store.layout = result.DefaultLayout;
                            }

                            DrawHTML(settings.$items.container.$getItem());
                        },
                        function (event, status, errorThrown) {
                            clientCloseWaitDialog();
                            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                            //clientAjaxError(event, status);
                            return;
                        },
                        false);

                    /*****************************************************************************/
                }
            },
            function (event, status, errorThrown) {
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                //clientAjaxError(event, status);
                return;
            },
            false);

        /*****************************************************************************/
    }
    function Process_Type_DOTSTAT(data) {
        /*****************************************************************************
        Process request for filter and layout
        *****************************************************************************/

        if (!stub) clientShowWaitDialog(settings.widget.messages.dataset_request_filters);
        clientPostJSON(settings.url.Get_DOTSTAT_StructureObject, clientParseObjectToJson(data),
            function (jsonString) {

                var result = clientParseJsonToObject(jsonString);

                settings.widget.store.dataflow = result.dataflow;
                settings.widget.store.dimension = result.codemap;

                settings.widget.store.hideDimension = {};
                if (result.hasOwnProperty('hideDimension') && result.hideDimension != null)
                    settings.widget.store.hideDimension = result.hideDimension;

                settings.widget.store.criteria = {};
                if (result.hasOwnProperty('costraint') && result.costraint != null)
                    settings.widget.store.criteria = result.costraint;

                settings.widget.store.layout = {
                    axis_x: [],
                    axis_y: [],
                    axis_z: [],
                    block_axis_x: false,
                    block_axis_y: false,
                    block_axis_z: false,
                };
                $.each(settings.widget.store.dimension, function (idx, concept) {
                    if (idx != time_dimension_key) {
                        settings.widget.store.layout.axis_x.push(idx);
                    }
                });
                settings.widget.store.layout.axis_y.push(time_dimension_key);
                DrawHTML(settings.$items.container.$getItem());
            });

        /*****************************************************************************/

    }
    function Process_Type_ECB(data) {

    }
    function Process_Type_ESTAT(data) {

    }
    
    /*********************************************************************************************************
    Private method
    **********************************************************************************************************/

    function isValidDataInput_Configuration(data) {
        if (data == undefined) return false;
        if (!data.hasOwnProperty('dataflow')) return false;
        return true;
    }
    function isValidDataOutput_Filter(data) {
        if (data == undefined) return false;
        if (!data.hasOwnProperty('dataflow')) return false;
        if (!data.hasOwnProperty('codemap')) return false;
        return true;
    }
    function isValidDataOutput_Layout(data) {
        if (data == undefined) return false;
        if (!data.hasOwnProperty('DefaultLayout')) return false;
        //if (!data.DefaultLayout.hasOwnProperty('axis_x')) return false;
        //if (!data.DefaultLayout.hasOwnProperty('axis_y')) return false;
        //if (!data.DefaultLayout.hasOwnProperty('axis_z')) return false;
        return true;
    }
    function isValidDataOutput_Dataset(data) {
        if (data == undefined) return false;
        if (!data.hasOwnProperty('dataflow')) return false;
        if (!data.hasOwnProperty('codemap')) return false;
        if (!data.hasOwnProperty('dataset')) return false;
        if (!data.dataset.hasOwnProperty('series')) return false;

        return true;
    }
    function isValidDataOutput_SaveQuery(data) {
        if (data == undefined) return false;
        if (!data.hasOwnProperty('success')) return false;
        return true;
    }

    function Process_RequestData() {
        $(settings.$items.data_container).empty();
        
        try {
            if (!stub) clientShowWaitDialog(settings.widget.messages.dataset_request_data);

            if (settings.widget.data != undefined)
                settings.widget.data.query = undefined;

            /*****************************************************************************/
            /* Process request for data
            *****************************************************************************/

            var _data = {
                dataflow: settings.widget.store.dataflow,
                configuration: settings.widget.store.configuration,
                criteria: settings.widget.store.filters,
                layout: settings.widget.store.layout,
                WidgetId: (settings.widget.data!=undefined)?
                    (settings.widget.data.widgetId == undefined) ?
                    -1 : settings.widget.data.widgetId : -1
            };

            //sessionStorage.setItem(settings.widget.id, clientParseObjectToJson(settings.widget.store));

            var LoadTableData = function (dest, url, callBack) {

                $(dest).hide();
                $(dest).load(url, callBack);
                $(dest).show();

            };

            clientPostJSON(
                settings.url.GetResults,
                clientParseObjectToJson(_data),
                function (jsonString) {

                    var result = clientParseJsonToObject(jsonString);

                    if (result != null
                        && result.hasOwnProperty('code')) {

                        dataTableID = result.code;

                        Concurrent.Thread.create(LoadTableData,
                            settings.$items.data_container,
                            client.main.config.baseURL + settings.url.GetResultTable + "/" + result.code,
                            function (){

                                if (settings.widget.data != undefined
                                    && settings.widget.data.showObsValue != undefined)

                                    $(settings.$items.data_container).find(".measure").each(function (index) {
                                        $(this).text($(this).data(settings.widget.data.showObsValue));
                                    });

                                if (!stub) clientCloseWaitDialog();
                                else {



                                    $(settings.$items.header_bar).find('.waitInLinee').remove();

                                }
                            }
                            );

                    } else {
                        $(settings.$items.data_container).empty();
                        clientCloseWaitDialog();

                        // Log error if admin loged in show complex message
                        if (sessionStorage.user_role != undefined) {
                            var usRole = JSON.parse(sessionStorage.user_role);
                            if (usRole.RoleId == 1) {
                                $(settings.$items.data_container).html(jsonString);
                            } else {
                                $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
                            }
                        } else {
                            $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
                        }

                        $(settings.$items.header_bar).find('.waitInLinee').remove();
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
    }

    function Process_RequestDataLayout() {
        $(settings.$items.data_container).empty();

        try {
            if (!stub) clientShowWaitDialog(settings.widget.messages.dataset_request_data);

            if (settings.widget.data != undefined)
                settings.widget.data.query = undefined;

            /*****************************************************************************/
            /* Process request for data
            *****************************************************************************/

            var _data = {
                dataflow: settings.widget.store.dataflow,
                configuration: settings.widget.store.configuration,
                criteria: settings.widget.store.filters,
                layout: settings.widget.store.layout,
                WidgetId: (settings.widget.data != undefined) ?
                    (settings.widget.data.widgetId == undefined) ?
                    -1 : settings.widget.data.widgetId : -1
            };

            //sessionStorage.setItem(settings.widget.id, clientParseObjectToJson(settings.widget.store));

            var LoadTableData = function (dest, url, callBack) {

                $(dest).hide();
                $(dest).load(url, callBack);
                $(dest).show();

            };

            clientPostJSON(
                settings.url.GetResultsLayout,
                clientParseObjectToJson(_data),
                function (jsonString) {

                    var result = clientParseJsonToObject(jsonString);

                    if (result != null
                        && result.hasOwnProperty('code')) {

                        dataTableID = result.code;

                        Concurrent.Thread.create(LoadTableData,
                            settings.$items.data_container,
                            client.main.config.baseURL + settings.url.GetResultTable + "/" + result.code,
                            function () {

                                if (settings.widget.data != undefined
                                    && settings.widget.data.showObsValue != undefined)

                                    $(settings.$items.data_container).find(".measure").each(function (index) {
                                        $(this).text($(this).data(settings.widget.data.showObsValue));
                                    });

                                if (!stub) clientCloseWaitDialog();
                                else {



                                    $(settings.$items.header_bar).find('.waitInLinee').remove();

                                }
                            }
                            );

                    } else {
                        $(settings.$items.data_container).empty();
                        clientCloseWaitDialog();

                        // Log error if admin loged in show complex message
                        if (sessionStorage.user_role != undefined) {
                            var usRole = JSON.parse(sessionStorage.user_role);
                            if (usRole.RoleId == 1) {
                                $(settings.$items.data_container).html(jsonString);
                            } else {
                                $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
                            }
                        } else {
                            $(settings.$items.data_container).html(settings.widget.messages.label_error_dataParsing);
                        }

                        $(settings.$items.header_bar).find('.waitInLinee').remove();
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
    }


    function Process_Chart(dest, dataflow, configuration, filters, chart_type, obs_type, dim_axe, customConcept,custom) {

        $(dest).empty();
        try {
            var chart_title_str = "";
            var sep = "";
            for (i in settings.widget.store.dimension) {

                if (i != time_dimension_key
                    && inArray(i, settings.widget.store.hideDimension) < 0) {

                    var codes = Object.keys(filters);

                    if (inArray(i, codes) >= 0){
                        if (filters[i].length == 1) {
                            var code = filters[i][0];
                            chart_title_str += sep + settings.widget.store.dimension[i].codes[code].name;
                            sep = ", ";
                        }
                    } else {
                        for (code in settings.widget.store.dimension[i].codes) {
                            chart_title_str += sep + settings.widget.store.dimension[i].codes[code].name;
                            sep = ", ";
                            break;
                        }
                    }
                }
            }

            /*
            var chart_title = document.createElement('h3');
            $(chart_title).text(chart_title_str);
            $(chart_title).appendTo(dest);
            */

            var chart_container = document.createElement('div');
            $(chart_container).appendTo(dest);

            clientShowWaitDialog();

            configuration.dataflow = dataflow;

            var data = {
                text: [{ title: chart_title_str.toString().replace("'", "&#39;"), locale: settings.widget.locale }],
                criteria: filters,
                chartType: chart_type,
                obsValue: obs_type,
                dimensionAxe: dim_axe,
                customKey: customConcept,
                customChartType: custom
            }
            var str_config = clientParseObjectToJson(configuration);
            var str_data = clientParseObjectToJson(data);

            var html = "<div id='chart_datatable' class='dinamic-widget' " +
                            " data-widget-template='chart'" +
                            " data-widget-stylecss='chart'" +
                            " data-widget-configuration='" + str_config + "'" +
                            " data-widget-data='" + str_data + "'></div>";

            $(chart_container).append(html);

            settings.widget.manager.SetupWidgets($(chart_container).find('.dinamic-widget'), 'en', settings.widget.messages);
        } catch (e) {
            clientCloseWaitDialog();
            return null;
        }
    }

    function ClearArea(dest) {

        settings.widget.store = {};

        $(dest).empty();

        if (!stub) $("#main-dashboard").empty();
    }

    function DrawHTML(dest) {

        if (!stub) clientShowWaitDialog();
        if (!stub) $("#main-dashboard").empty();

        $(dest).empty();

        settings.$items.header_bar = document.createElement('div');
        settings.$items.button_bar = document.createElement('div');
        settings.$items.viewmode_bar = document.createElement('div');
        settings.$items.slice_container = document.createElement('div');
        settings.$items.dataset_container = document.createElement('div');
        settings.$items.data_container = document.createElement('div');
        settings.$items.chart_container = document.createElement('div');
        settings.$items.meta_container = document.createElement('div');
        
        $(settings.$items.header_bar).addClass(settings.cssClass.header_bar);
        $(settings.$items.button_bar).addClass(settings.cssClass.button_bar);
        $(settings.$items.viewmode_bar).addClass(settings.cssClass.viewmode_bar);
        $(settings.$items.slice_container).addClass(settings.cssClass.slice_container);
        $(settings.$items.data_container).addClass(settings.cssClass.data_container);
        $(settings.$items.chart_container).addClass(settings.cssClass.chart_container);
        $(settings.$items.meta_container).addClass(settings.cssClass.meta_container);

        $(settings.$items.header_bar).appendTo(dest);
        $(settings.$items.button_bar).appendTo(dest);
        $(settings.$items.viewmode_bar).appendTo(dest);
        $(settings.$items.slice_container).appendTo(dest);
        $(settings.$items.data_container).appendTo(dest);
        $(settings.$items.chart_container).appendTo(dest);
        $(settings.$items.meta_container).appendTo(dest);


        var local_title = (settings.widget.data) ? GetLocalisedText(settings.widget.data.text, settings.widget.locale) : undefined;
        //fabio 4/11/2015
        var title = (local_title != undefined) ? local_title : settings.widget.store.dataflow.name.replace("'", "&#39;")
        //fine fabio 4/11/2015

        var h3_name = document.createElement('h3');
        if (stub) $(h3_name).append('<i class="waitInLinee icon-spin6 animate-spin"></i> ');
        $(h3_name).append(title);
        $(h3_name).appendTo(settings.$items.header_bar);


        if (!stub){
            
            if (settings.widget.store.enabledCri == true) {
                DrawHTML_Filter(settings.$items.button_bar, settings.widget.store.dimension, settings.widget.store.hideDimension);
                DrawHTML_layout(settings.$items.button_bar, settings.widget.store.layout, settings.widget.store.hideDimension);
            }
            if (settings.widget.store.enabledVar == true) {
                DrawHTML_SelectFormula(settings.$items.button_bar);
            }
            DrawHTML_Download(settings.$items.button_bar);

            if (sessionStorage.user_code != undefined) {
                DrawHTML_ButtonSaveQuery(settings.$items.button_bar);
                var usRole = JSON.parse(sessionStorage.user_role);
                if (usRole.RoleId == 1) DrawHTML_ButtonSaveTemplate(settings.$items.button_bar);
            }

            DrawHTML_TableModeSwitch(settings.$items.viewmode_bar);
            DrawHTML_ChartModeSwitch(settings.$items.viewmode_bar, settings.widget.store.hideDimension);
            DrawHTML_MetadataModeSwitch(settings.$items.viewmode_bar, settings.widget.store.hideDimension);

            
        }
        
        DrawClearBox(settings.$items.button_bar);

        if (!stub) clientCloseWaitDialog();

        if (!needCriteria) {

            UpdateControl();
            Process_RequestData();

        } else {

            if (settings.widget.store.codelist_target == undefined) {
                OpenPopUpFilters(
                    settings.widget.store.dimension,
                    settings.widget.store.criteria,
                    function (args) {

                        UpdateControl();
                        Process_RequestData();
                    },
                    settings.cssClass,
                    settings.widget.messages,
                    time_dimension_key,
                    client.main.maxObs,
                    settings.widget.store.hideDimension);
            } else {
                OpenPopUpFiltersCostraint(
                    settings.widget.store.dataflow,
                    settings.widget.store.configuration,
                    settings.widget.store.dimension,
                    settings.widget.store.codelist_target,
                    settings.widget.store.criteria,
                    function (args) {

                        UpdateControl();
                        Process_RequestData();
                    },
                    settings.cssClass,
                    settings.widget.messages,
                    time_dimension_key,
                    client.main.maxObs,
                    settings.widget.store.hideDimension);
            }
        }

        $(settings.$items.data_container).show();
        $(settings.$items.chart_container).hide();
        $(settings.$items.meta_container).hide();
    }

    function DrawClearBox(dest) {
        var div_clear = document.createElement("div");
        $(div_clear).addClass("clear-box");
        $(div_clear).appendTo(dest);
    }

    function DrawHTML_ButtonSaveQuery(dest) {

        var btn_open_query = document.createElement("a");
        $(btn_open_query).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_query).html("<i class='icon-floppy-1'></i>" + settings.widget.messages.label_save_query);
        $(btn_open_query).click(function (event) {

            var divmes = document.createElement("div");
            var labmes = document.createElement("label");
            $(labmes).css('width', '100%');
            $(labmes).text(settings.widget.messages.label_title_query);
            $(labmes).appendTo(divmes);

            var inputmes = document.createElement("input");
            $(inputmes).attr('type', 'text');
            $(inputmes).css('width', '100%');
            $(inputmes).appendTo(divmes);

            $(divmes).dialog({
                title: settings.widget.messages.label_title_query,
                modal: true,
                buttons: [
                    {
                        text: settings.widget.messages.label_save,
                        click: function () {
                            var check = $(inputmes).val().trim();
                            if ((check != "") && (!(checkLetter(check)))) {
                                $(this).dialog("close");
                                SaveQuery(sessionStorage.user_code,
                                    $(inputmes).val(),
                                    settings.widget.store.dataflow,
                                    settings.widget.store.criteria,
                                    settings.widget.store.layout,
                                    settings.widget.store.configuration);
                            }
                        }
                    },
                    {
                        text: settings.widget.messages.label_cancel,
                        click: function () {
                            $(inputmes).val("");
                            $(this).dialog("close");
                        }
                    }
                ]
            });

        });
        $(btn_open_query).appendTo(dest);
    };
    function DrawHTML_ButtonSaveTemplate(dest) {

        // btn open pop up 
        var btn_save_template = document.createElement("a");
        $(btn_save_template).addClass(settings.cssClass.btn_tool_bar);
        $(btn_save_template).html("<i class='icon-download-alt'></i>" + settings.widget.messages.label_save_template);
        $(btn_save_template).click(function (event) {
            
            // popup
            var div_popup = document.createElement("div");
            var labmes = document.createElement("label");
            $(labmes).css('width', '100%');
            $(labmes).text(settings.widget.messages.titleSaveTemplate);//inserire valore label
            $(labmes).appendTo(div_popup);

            var inputmes = document.createElement("input");
            $(inputmes).attr('type', 'text');
            $(inputmes).css('width', '100%');
            $(inputmes).appendTo(div_popup);

            var div_attribute = document.createElement("div");
            $(div_attribute).addClass("list_attr");
            
            //block
            var div_block = document.createElement("div");
            $(div_block).addClass("block");
            $("<label class='labelBlockInt'>" + settings.widget.messages.labelBlock+ "</label><label class='labelBlock'>x</label><input id='check_x' type='checkbox' value='x'><label class='labelBlock'>y</label><input id='check_y' type='checkbox' value='y'><label class='labelBlock'>z</label><input id='check_z' type='checkbox' value='z'>").appendTo(div_block);
            $(div_block).appendTo(div_popup);

            //lista attributi
            var attr = [];
            var checked = "";//(1 < 2) ? "checked" : "";
            $.each(settings.widget.store.dimension, function (idx, dim) {

                if (dim != null) {

                    var singleCode = false;
                    if (settings.widget.store.criteria.hasOwnProperty(idx)
                        && settings.widget.store.criteria[idx].length == 1) {
                        singleCode = true;
                    }
                    else {
                        var list = $.map(dim.codes, function (el) { return el });
                        if (list.length == 1) singleCode = true;
                    }

                    if (idx == time_dimension_key)
                        singleCode = false;

                    if(singleCode){
                        $("<div class='attribute'><input class='check_attribute' type='checkbox' " + checked + " name='" + idx + "'value='" + idx + "'><label class='label_attribute'>[" + idx + "] " + dim.title + " </label></div>").appendTo(div_attribute);
                        attr.push(dim);
                    }
                }
            });
            
            $("<div class='titleList'>" + settings.widget.messages.labelDimension + "</div>").appendTo(div_popup);
            $(div_attribute).appendTo(div_popup);

            var div_option = document.createElement("div");
            
            $("<div class='attribute'><input class='check_attribute' type='checkbox' " + checked + " name='chk_opt_var' id='chk_opt_var'><label class='label_attribute'>" + settings.widget.messages.labelOptionVar + "</label></div>").appendTo(div_option);
            //$("<div class='attribute'><input class='check_attribute' type='checkbox' " + checked + " name='chk_opt_dec' id='chk_opt_dec' ><label class='label_attribute'>" + settings.widget.messages.labelOptionDec + "</label></div>").appendTo(div_option);
            $("<div class='attribute'><input class='check_attribute' type='checkbox' " + checked + " name='chk_opt_cri' id='chk_opt_cri' ><label class='label_attribute'>" + settings.widget.messages.labelOptionCri + "</label></div>").appendTo(div_option);

            $(div_option).appendTo(div_popup);

            $(div_popup).dialog({
                title: settings.widget.messages.label_save_template,
                modal: true,
                resizable: true,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                autoOpen: false,
                buttons: [
                    {
                        text: settings.widget.messages.label_ok,
                        click: function () {
                            var check = $(inputmes).val().trim();
                            if ((check != "") && (!(checkLetter(check))) ){
                                var cbox_sel = new Array();
                                $(".check_attribute").each(function () {
                                    if (this.checked) {
                                        cbox_sel.push(this.value);
                                    }
                                });
                                
                                var block_sel = [];
                                if ($("#check_x").prop("checked")) {
                                    block_sel.push($("#check_x").val());
                                }
                                if($("#check_y").prop( "checked")){
                                    block_sel.push($("#check_y").val());
                                }
                                if ($("#check_z").prop("checked")) {
                                    block_sel.push($("#check_z").val());
                                }
                                
                                SaveTemplate(sessionStorage.user_code,
                                    $(inputmes).val(),// title
                                    cbox_sel,
                                    block_sel,// dimension selezionate
                                    $("#chk_opt_var").prop('checked'),
                                    //$("#chk_opt_dec").prop('checked'),
                                    false,
                                    $("#chk_opt_cri").prop('checked'),
                                    settings.widget.store.dataflow, 
                                    settings.widget.store.criteria,
                                    settings.widget.store.layout,
                                    settings.widget.store.configuration);
                                $(this).dialog('destroy').remove();
                            }
                        }
                    },
                    {
                        text: settings.widget.messages.label_cancel,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
            $(div_popup).dialog("open");

            });
        $(btn_save_template).appendTo(dest);

    }
    function DrawHTML_ButtonDeleteTemplate(dest) {
        // btn open pop up 
        var btn_load_template = document.createElement("a");
        $(btn_load_template).addClass(settings.cssClass.btn_tool_bar);
        $(btn_load_template).html("<i class='icon-download-alt'></i>" + settings.widget.messages.label_load_template);
        $(btn_load_template).click(function (event) {

            // popup
            var div_popup = document.createElement("div");
            var labmes = document.createElement("label");
            $(labmes).css('width', '100%');
            $(labmes).text(settings.widget.messages.titleLoadTemplate);//inserire valore label
            $(labmes).appendTo(div_popup);

            $(div_popup).dialog({
                title: settings.widget.messages.label_load_template,
                modal: true,
                resizable: true,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                autoOpen: false,
                buttons: [
                    {
                        text: settings.widget.messages.label_ok,
                        click: function () {
                            DeleteTemplate(sessionStorage.user_code,
                                settings.widget.store.dataflow,
                                settings.widget.store.configuration);
                            $(this).dialog('destroy').remove();
                        }
                        
                    },
                    {
                        text: settings.widget.messages.label_cancel,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
            $(div_popup).dialog("open");

        });
        $(btn_load_template).appendTo(dest);
    }
    function DrawHTML_Download(dest) {

        var GetHTML_Download_csv_tabular = function () {
            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_csv_tabular);
            $(p).css('width', '100%');
            $(p).appendTo(div);

            var p_char_sep = document.createElement("p");
            $(p_char_sep).text(settings.widget.messages.label_separator);
            $(p_char_sep).css('width', '100%');
            $(p_char_sep).appendTo(div);

            var char_sep = document.createElement("input");
            $(char_sep).attr('type', 'text');
            $(char_sep).attr('maxlength', '1'); 
            $(char_sep).css('width', '10px');
            $(char_sep).css('float', 'right');
            $(char_sep).appendTo(p_char_sep);

            var p_chk_quote = document.createElement("p");
            $(p_chk_quote).text(settings.widget.messages.label_with_quote);
            $(p_chk_quote).css('width', '100%');
            $(p_chk_quote).appendTo(div);
            var chk_quote = document.createElement("input");
            $(chk_quote).attr('type', 'checkbox');
            $(chk_quote).prop('checked', true);
            $(chk_quote).css('float', 'right');
            $(chk_quote).appendTo(p_chk_quote);
            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                var separator = $(char_sep).val();
                var withQuotation = ($(chk_quote).prop('checked')) ? true : false;

                if (separator.length != 1) {
                    clientShowErrorDialog(settings.widget.messages.label_separator_outlimit);
                } else {
                    location.href = client.main.config.baseURL + "Download/ExportCsvTabular?id=" + dataTableID + "&separator=" + separator + "&withQuotation=" + withQuotation;
                }
            });
            return div;
        };
        var GetHTML_Download_csv_formated = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_csv_not_formatted);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var p_char_sep = document.createElement("p");
            $(p_char_sep).text(settings.widget.messages.label_separator);
            $(p_char_sep).css('width', '100%');
            $(p_char_sep).appendTo(div);
            var char_sep = document.createElement("input");
            $(char_sep).attr('type', 'text');
            $(char_sep).attr('maxlength', '1');
            $(char_sep).css('width', '10px');
            $(char_sep).css('float', 'right');
            $(char_sep).val(';');
            $(char_sep).appendTo(p_char_sep);
            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;
                var separator = $(char_sep).val();

                if (separator.length != 1){
                    clientShowErrorDialog(settings.widget.messages.label_separator_outlimit);
                } else {
                    location.href = client.main.config.baseURL + "Download/ExportCsvModel?id=" + dataTableID + "&separator=" + separator;
                }
            });
            return div;
        };
        var GetHTML_Download_xls = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_xls);
            $(p).css('width', '100%');
            $(p).appendTo(div);

            var p_char_sep = document.createElement("p");
            $(p_char_sep).text(settings.widget.messages.label_separator_decimal);
            $(p_char_sep).css('width', '100%');
            $(p_char_sep).appendTo(div);

            var select_char = document.createElement("select");
            //$(select_char).attr('maxlength', '1');
            $(select_char).css('width', 'auto');
            $(select_char).css('float', 'right');
            $(select_char).append('<option value="."> . </option>');
            $(select_char).append('<option value=","> , </option>');
            $(select_char).appendTo(p_char_sep);

            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;
                var separator = $(select_char).val();
                
                
                location.href = client.main.config.baseURL + "Download/ExportXLS?id=" + dataTableID + "&separator=" + separator;

            });
            return div;
        };
        var GetHTML_Download_html = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_html);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                location.href = client.main.config.baseURL + "Download/ExportHtmlModel?id=" + dataTableID;

            });
            return div;
        };
        var GetHTML_Download_pdf = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_pdf);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var p_size = document.createElement("p");
            $(p_size).text(settings.widget.messages.label_size);
            $(p_size).css('width', '100%');
            $(p_size).appendTo(div);
            var cmb_size = document.createElement('select');
            $(cmb_size).addClass('data-option');
            $(cmb_size).append('<option value="LETTER">LETTER (215mm x 279mm)</option>');
            $(cmb_size).append('<option value="NOTE">NOTE (190mm x 254mm)</option>');
            $(cmb_size).append('<option value="LEGAL">LEGAL (215mm x 355mm)</option>');
            $(cmb_size).append('<option value="TABLOID">TABLOID (279mm x 431mm)</option>');
            $(cmb_size).append('<option value="EXECUTIVE">EXECUTIVE (184mm x 266mm)</option>');
            $(cmb_size).append('<option value="POSTCARD">POSTCARD (99mm x 146mm)</option>');
            $(cmb_size).append('<option value="A0">A0 (841mm x 1188mm)</option>');
            $(cmb_size).append('<option value="A1">A1 (594mm x 841mm)</option>');
            $(cmb_size).append('<option value="A2">A2 (420mm x 594mm)</option>');
            $(cmb_size).append('<option value="A3">A3 (297mm x 420mm)</option>');
            $(cmb_size).append('<option selected="selected" value="A4">A4 (209mm x 297mm)</option>');
            $(cmb_size).append('<option value="A5">A5 (148mm x 209mm)</option>');
            $(cmb_size).append('<option value="A6">A6 (104mm x 148mm)</option>');
            $(cmb_size).append('<option value="A7">A7 (74mm x 104mm)</option>');
            $(cmb_size).append('<option value="A8">A8 (52mm x 74mm)</option>');
            $(cmb_size).append('<option value="A9">A9 (37mm x 52mm)</option>');
            $(cmb_size).append('<option value="A10">A10 (25mm x 37mm)</option>');
            $(cmb_size).append('<option value="B0">B0 (999mm x 1413mm)</option>');
            $(cmb_size).append('<option value="B1">B1 (706mm x 999mm)</option>');
            $(cmb_size).append('<option value="B2">B2 (499mm x 706mm)</option>');
            $(cmb_size).append('<option value="B3">B3 (352mm x 499mm)</option>');
            $(cmb_size).append('<option value="B4">B4 (249mm x 352mm)</option>');
            $(cmb_size).append('<option value="B5">B5 (175mm x 249mm)</option>');
            $(cmb_size).append('<option value="B6">B6 (124mm x 175mm)</option>');
            $(cmb_size).append('<option value="B7">B7 (87mm x 124mm)</option>');
            $(cmb_size).append('<option value="B8">B8 (61mm x 87mm)</option>');
            $(cmb_size).append('<option value="B9">B9 (43mm x 61mm)</option>');
            $(cmb_size).append('<option value="B10">B10 (30mm x 43mm)</option>');
            $(cmb_size).append('<option value="ARCH_E">ARCH_E (914mm x 1219mm)</option>');
            $(cmb_size).append('<option value="ARCH_D">ARCH_D (609mm x 914mm)</option>');
            $(cmb_size).append('<option value="ARCH_C">ARCH_C (457mm x 609mm)</option>');
            $(cmb_size).append('<option value="ARCH_B">ARCH_B (304mm x 457mm)</option>');
            $(cmb_size).append('<option value="ARCH_A">ARCH_A (228mm x 304mm)</option>');
            $(cmb_size).append('<option value="FLSA">FLSA (215mm x 330mm)</option>');
            $(cmb_size).append('<option value="FLSE">FLSE (228mm x 330mm)</option>');
            $(cmb_size).append('<option value="HALFLETTER">HALFLETTER (139mm x 215mm)</option>');
            $(cmb_size).append('<option value="_11X17">_11X17 (279mm x 431mm)</option>');
            $(cmb_size).appendTo(p_size);

            var p_landscape = document.createElement("p");
            $(p_landscape).text(settings.widget.messages.label_landscape);
            $(p_landscape).css('width', '100%');
            $(p_landscape).appendTo(div);
            var chk_landscape = document.createElement("input");
            $(chk_landscape).attr('type', 'checkbox');
            $(chk_landscape).prop('checked', true);
            $(chk_landscape).css('float', 'right');
            $(chk_landscape).appendTo(p_landscape);

            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                var size = $(cmb_size).val();
                var landscape = $(chk_landscape).prop('checked');

                location.href = client.main.config.baseURL + "Download/ExportPdfModel?id=" + dataTableID + "&size=" + size + "&landscape=" + landscape;

            });
            return div;
        };
        var GetHTML_Download_sdmxml_structure = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_sdmx_structure);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var p_version = document.createElement("p");
            $(p_version).text("Version");//(settings.widget.messages.label_version);
            $(p_version).css('width', '100%');
            $(p_version).appendTo(div);
            var cmb_version = document.createElement('select');
            $(cmb_version).css('float', 'right');
            $(cmb_version).addClass('data-option');
            $(cmb_version).append('<option value="V20">V 2.0</option>');
            $(cmb_version).append('<option value="V21">V 2.1</option>');
            $(cmb_version).appendTo(p_version);

            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                var sdmxVersion = $(cmb_version).val();
                location.href = client.main.config.baseURL + "Download/ExportSDMXStructure?id=" + dataTableID + "&sdmxVersion=" + sdmxVersion;
            });
            return div;
        };
        var GetHTML_Download_sdmxml_query = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_sdmx_query);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var p_version = document.createElement("p");
            $(p_version).text("Version");//(settings.widget.messages.label_version);
            $(p_version).css('width', '100%');
            $(p_version).appendTo(div);
            var cmb_version = document.createElement('select');
            $(cmb_version).css('float', 'right');
            $(cmb_version).addClass('data-option');
            $(cmb_version).append('<option value="V20">V 2.0</option>');
            $(cmb_version).append('<option value="V21">V 2.1</option>');
            $(cmb_version).appendTo(p_version);

            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                var sdmxVersion = $(cmb_version).val();
                location.href = client.main.config.baseURL + "Download/ExportSDMXStructure?id=" + dataTableID + "&sdmxVersion=" + sdmxVersion;
            });
            return div;
        };
        var GetHTML_Download_sdmxml_data = function () {

            var div = document.createElement('div');

            var p = document.createElement("p");
            $(p).text(settings.widget.messages.label_desc_save_sdmx_data);
            $(p).css('width', '100%');
            $(p).appendTo(div);
            var p_version = document.createElement("p");
            $(p_version).text("Version");//(settings.widget.messages.label_version);
            $(p_version).css('width', '100%');
            $(p_version).appendTo(div);
            var cmb_version = document.createElement('select');
            $(cmb_version).css('float', 'right');
            $(cmb_version).addClass('data-option');
            $(cmb_version).append('<option value="V20">V 2.0</option>');
            $(cmb_version).append('<option value="V21">V 2.1</option>');
            $(cmb_version).appendTo(p_version);

            var btn_dwn = document.createElement("a");
            $(btn_dwn).addClass('data-option');
            $(btn_dwn).html("<i class='icon-floppy-1'></i>Download");
            $(btn_dwn).css('float', 'right');
            $(btn_dwn).appendTo(div);
            $(btn_dwn).click(function (event) {
                if (dataTableID == null) return;

                var sdmxVersion = $(cmb_version).val();
                location.href = client.main.config.baseURL + "Download/ExportSDMXDataSet?id=" + dataTableID + "&sdmxVersion=" + sdmxVersion;
            });
            return div;
        };
        
        var cmb = document.createElement('select');
        $(cmb).addClass('data-option');
        $(cmb).append('<option value="null">' + settings.widget.messages.label_select_download+ '</option>');
        $(cmb).append('<option value="csv_modular">' + settings.widget.messages.label_save_csv_tabular + '</option>');
        $(cmb).append('<option value="csv_not_formatted">' + settings.widget.messages.label_save_csv_not_formatted + '</option>');
        $(cmb).append('<option value="xls">' + settings.widget.messages.label_save_xls + '</option>');
        $(cmb).append('<option value="html">' + settings.widget.messages.label_save_html + '</option>');
        $(cmb).append('<option value="pdf">' + settings.widget.messages.label_save_pdf + '</option>');
        $(cmb).append('<option value="sdmxml_structure">' + settings.widget.messages.label_save_sdmx_structure + '</option>');
        $(cmb).append('<option value="sdmxml_query">' + settings.widget.messages.label_save_sdmx_query + '</option>');
        $(cmb).append('<option value="sdmxml_data">' + settings.widget.messages.label_save_sdmx_data + '</option>');
        $(cmb).change(function () {
            if (type_dow == "null") return;

            var div_container = document.createElement('div');

            var type_dow = $(this).val();
            switch (type_dow){
                case 'csv_modular': $(div_container).append(GetHTML_Download_csv_tabular()); break;
                case 'csv_not_formatted': $(div_container).append(GetHTML_Download_csv_formated()); break;
                case 'xls': $(div_container).append(GetHTML_Download_xls()); break;
                case 'html': $(div_container).append(GetHTML_Download_html()); break;
                case 'pdf': $(div_container).append(GetHTML_Download_pdf()); break;
                case 'sdmxml_structure': $(div_container).append(GetHTML_Download_sdmxml_structure()); break;
                case 'sdmxml_query': $(div_container).append(GetHTML_Download_sdmxml_query()); break;
                case 'sdmxml_data': $(div_container).append(GetHTML_Download_sdmxml_data()); break;
            }

            $(div_container).dialog({
                title: "Download",
                modal: true,
                resizable: false,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                autoOpen: false,
                buttons: [{
                        text: "Cancel",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
            $(div_container).dialog("open");
        });
        $(cmb).appendTo(dest);
    };
    function DrawHTML_SelectFormula(dest) {

        settings.widget.messages.label_formula_valore = "Valore";
        settings.widget.messages.label_formula_trend = "Tendenza";
        settings.widget.messages.label_formula_cong = "Congiuntura";

        var combox = document.createElement("select");
        $(combox).addClass(settings.cssClass.btn_tool_bar);

        var opt_val = document.createElement("option");
        $(opt_val).val("v");
        $(opt_val).text(settings.widget.messages.label_formula_valore);
        $(opt_val).appendTo(combox);
        var opt_val_2 = document.createElement("option");
        $(opt_val_2).val("vt");
        $(opt_val_2).text(settings.widget.messages.label_formula_trend);
        $(opt_val_2).appendTo(combox);
        var opt_val_3 = document.createElement("option");
        $(opt_val_3).val("vc");
        $(opt_val_3).text(settings.widget.messages.label_formula_cong);
        $(opt_val_3).appendTo(combox);

        $(combox).appendTo(dest);

        $(combox).change(function () {
            Concurrent.Thread.create(
                function (prop) {
                    clientShowWaitDialog();
                    $(".measure").each(function (index) {
                        $(this).text($(this).data(prop));
                    });
                    clientCloseWaitDialog();
                }, $(combox).val()
                );
        });
    }

    function DrawHTML_Filter(dest, codemap, hideDimension) {
        
        // btn open pop up 
        var btn_open_criteria = document.createElement("a");
        $(btn_open_criteria).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_criteria).html("<i class='icon-filter'></i>" + settings.widget.messages.label_open_criteria);
        $(btn_open_criteria).click(function (event) {

            if (!criteriCostraint) {

                OpenPopUpFilters(
                    codemap,
                    settings.widget.store.criteria,
                    function (args) {

                        UpdateControl();
                        Process_RequestData();

                    },
                    settings.cssClass,
                    settings.widget.messages,
                    time_dimension_key,
                    client.main.maxObs,
                    settings.widget.store.hideDimension);

            } else {

                OpenPopUpFiltersCostraint(
                    settings.widget.store.dataflow,
                    settings.widget.store.configuration,
                    codemap,
                    settings.widget.store.codelist_target,
                    settings.widget.store.criteria,
                    function (args) {
                        settings.widget.store = data;
                        Process_Type_RI(data);

                        UpdateControl();
                        Process_RequestData();

                    },
                    settings.cssClass,
                    settings.widget.messages,
                    time_dimension_key,
                    client.main.maxObs,
                    settings.widget.store.hideDimension);
            }
        });
        $(btn_open_criteria).appendTo(dest);

    }
    function DrawHTML_layout(dest, layout, hideDimension) {

        var btn_open_layout = document.createElement("a");
        $(btn_open_layout).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_layout).html("<i class='icon-layout'></i>" + settings.widget.messages.label_open_layout);
        $(btn_open_layout).click(function (event) {

            OpenPopUpLayout(
                settings.widget.store.layout,
                function (args) {

                settings.widget.store.layout = args;

                UpdateControl();
                Process_RequestDataLayout();
                //UpdateControl();

            },
            settings.cssClass,
            settings.widget.messages,
            settings.dataKey,
            hideDimension);

        });
        $(btn_open_layout).appendTo(dest);

    }
    function DrawHTML_Slice(dest, slice, codemap,hideDimension) {

        var tb = document.createElement('table');
        $(tb).addClass(settings.cssClass.table_slice);

        for (var i = 0; i < slice.length; i++) {

            var showDimension = true;
            if (hideDimension != undefined)
                if (inArray(slice[i], hideDimension) >= 0)
                    showDimension = false;

            if (showDimension) {

                var concept = slice[i];

                var tr = document.createElement('tr');
                $(tr).addClass(settings.cssClass.tr_slice);

                var td = document.createElement('td');
                $(td).addClass(settings.cssClass.td_slice_concept);
                $(td).text((concept != time_dimension_key ) ? ((codemap[slice[i]]!=null) ? codemap[slice[i]].title : concept) : time_dimension_name);
                //$(td).text((concept != time_dimension_key) ? conceptnew[slice[i]].title : time_dimension_name);
                $(td).appendTo(tr);
                delete td;

                var td = document.createElement('td');
                $(td).addClass(settings.cssClass.td_slice_code);

                var isSingleCode = true;

                if (concept == time_dimension_key) {

                    if (settings.widget.store.criteria[concept].length == 1) {
                        isSingleCode = false;
                        var codes = $.map(codemap[concept].codes, function (key, el) { return el; });
                        settings.widget.store.criteria[concept] = [];
                        for (kj = 0; kj < codes.length; kj++) {
                            var idx = (codes.length - 1) - kj;
                            if (idx < 0) break;
                            settings.widget.store.criteria[concept].push(codes[idx]);
                        }
                    } else if (settings.widget.store.criteria[concept][0] == settings.widget.store.criteria[concept][1]) {
                        isSingleCode = true;
                    } else {
                        isSingleCode = false;
                    }
                } else {
                    if (settings.widget.store.criteria.hasOwnProperty(concept)) {
                        isSingleCode = (settings.widget.store.criteria[concept].length == 1);
                    } else {
                        //var codes = $.map(codemap[concept].codes, function (key, el) { return el; });
                        //if (settings.widget.store.criteria.hasOwnProperty(codemap[concept])) {
                        //modifica  07/12/2015
                        if (settings.widget.store.dimension.hasOwnProperty(concept)) {
                            var codes = $.map(codemap[concept].codes, function (key, el) { return el; });
                            //isSingleCode = (codes.length == 1);
                            isSingleCode = (codes!=undefined ? (codes.length == 1) : true);
                        }
                        //else { isSingleCode = (settings.widget.store.criteria[concept].length == 1); }
                        else { isSingleCode = true; }
                    }
                }

                var code = undefined;

                if (!isSingleCode) {
                    // draw combo
                    var select = document.createElement('select');
                    var lastKey = undefined;
                    var lastOption = undefined;

                    var time_code_range = false;

                    $.each(codemap[concept].codes, function (key, value) {

                        var isCodeForChoise = false;

                        if (concept == time_dimension_key
                            && settings.widget.store.criteria[concept][0] == key) {
                            isCodeForChoise = true;
                            time_code_range = true;
                        }

                        if (settings.widget.store.criteria[concept] == undefined ||
                            settings.widget.store.criteria[concept][0] == undefined ||
                            inArray(key, settings.widget.store.criteria[concept]) >= 0) {
                            isCodeForChoise = true;
                        }

                        if (isCodeForChoise || time_code_range) {

                            var option = document.createElement('option');

                            $(option).text((concept != time_dimension_key) ? "[" + key + "] " + value.name : value.name);
                            $(option).val(concept + "+" + key);
                            $(option).attr('selected', 'selected');
                            code = key;
                            $(option).appendTo(select);

                            lastKey = key;
                            lastOption = option;
                        }


                        if (concept == time_dimension_key
                            && settings.widget.store.criteria[concept][1] == key) time_code_range = false;

                    });

                    if (code === undefined) {
                        $(lastOption).attr('selected', 'selected');
                        code = lastKey;
                    }

                    $(select).change(function () {
                        var code_key = $(this).val().split('+');

                        settings.widget.store.filters[code_key[0]] = [];

                        if (inArray(code_key[1], settings.widget.store.filters[code_key[0]]) == -1)
                            settings.widget.store.filters[code_key[0]].push(code_key[1]);

                        Process_RequestData();

                    });
                    $(select).appendTo(td);
                    $(td).appendTo(tr);
                } else {

                    if (settings.widget.store.criteria.hasOwnProperty(concept)) {

                        code = settings.widget.store.criteria[concept][0];
                        var name = codemap[concept].codes[code].name;

                        var p = document.createElement('p');
                        $(p).text("[" + code + "] " + name);
                        $(p).appendTo(td);
                    } else {

                        // draw label
                        //commentato da fabio 07/12/2015
                        //if (settings.widget.store.criteria.hasOwnProperty(codemap[concept])) {
                        if (settings.widget.store.dimension.hasOwnProperty(concept)) {
                            var p_code = $.map(codemap[concept].codes, function (value, key) {
                                var p = document.createElement('p');
                                $(p).text((concept != time_dimension_key) ? "[" + key + "] " + value.name : value.name);
                                code = key;
                                return p;
                            });
                        }
                        else {
                            var p_code = document.createElement('p');
                            $(p).text(concept);
                            }
                        $(p_code).appendTo(td);
                    }
                }

                settings.widget.store.filters[concept] = [];
                if (inArray(code, settings.widget.store.filters[concept]) == -1)
                    settings.widget.store.filters[concept].push(code);

                $(td).appendTo(tr);
                delete td;

                $(tr).appendTo(tb);
                delete tr;

            }
        }
        $(tb).appendTo(dest);

        delete tb;

    }

    function DrawHTML_TableModeSwitch(dest) {

        // btn open pop up 
        var btn_open_meta = document.createElement("a");
        $(btn_open_meta).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_meta).html("<i class='icon-table'></i> Table");
        $(btn_open_meta).click(function (event) {

            $(settings.$items.button_bar).show();
            $(settings.$items.data_container).show();
            $(settings.$items.dataset_container).show();
            $(settings.$items.slice_container).show();

            $(settings.$items.chart_container).hide();
            $(settings.$items.meta_container).hide();

            viewMode = 0;

        });
        $(btn_open_meta).appendTo(dest);
    }
    function DrawHTML_ChartModeSwitch(dest, hideDimension) {

        // btn open pop up 
        var btn_open_chart = document.createElement("a");
        $(btn_open_chart).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_chart).html("<i class='icon-chart-pie'></i> Chart mode");
        $(btn_open_chart).click(function (event) {

            $(settings.$items.data_container).hide();
            $(settings.$items.dataset_container).hide();
            $(settings.$items.slice_container).hide();
            $(settings.$items.button_bar).hide();

            $(settings.$items.meta_container).hide();
            $(settings.$items.chart_container).show();

            viewMode = 1;

            ShowChartArea(settings.$items.chart_container, hideDimension);

        });
        $(btn_open_chart).appendTo(dest);
    }
    function DrawHTML_MetadataModeSwitch(dest, hideDimension) {

        // btn open pop up 
        var btn_open_meta = document.createElement("a");
        $(btn_open_meta).addClass(settings.cssClass.btn_tool_bar);
        $(btn_open_meta).html("<i class='icon-book-3'></i> Metadata");
        $(btn_open_meta).click(function (event) {

            $(settings.$items.data_container).hide();
            $(settings.$items.dataset_container).hide();
            $(settings.$items.slice_container).hide();
            $(settings.$items.button_bar).hide();

            $(settings.$items.chart_container).hide();
            $(settings.$items.meta_container).show();

            ShowMetaArea(settings.$items.meta_container, hideDimension);

            viewMode = 2;

        });
        $(btn_open_meta).appendTo(dest);
    }

    function UpdateControl() {

        if (!stub) {

            // move single codede layout x to z
            $.each(settings.widget.store.layout.axis_x, function (idx, concept) {
                if (concept != time_dimension_key) {
                    if (settings.widget.store.dimension[concept] != null) {
                        var codes = $.map(settings.widget.store.dimension[concept].codes, function (el) { return el; });
                        if (codes.length == 1) {
                            delete settings.widget.store.layout.axis_x[idx];
                            settings.widget.store.layout.axis_z.push(concept);
                        }
                    }
                }

            });
            settings.widget.store.layout.axis_x =
                $.map(settings.widget.store.layout.axis_x, function (el) { return el; });

            // move single codede layout y to z
            $.each(settings.widget.store.layout.axis_y, function (idx, concept) {
                if (concept != time_dimension_key) {
                    var codes = $.map(settings.widget.store.dimension[concept].codes, function (el) { return el; });
                    if(codes.length == 1){
                        delete settings.widget.store.layout.axis_y[idx];
                        settings.widget.store.layout.axis_z.push(concept);
                    }
                }

            });
            settings.widget.store.layout.axis_y =
                $.map(settings.widget.store.layout.axis_y, function (el) { return el; });

        }

        if (settings.widget.store.filters == undefined)
            settings.widget.store.filters = {};

        if (settings.widget.store.dimension != undefined) {
            $.each(settings.widget.store.dimension, function (idx, data) {
                if (settings.widget.store.criteria.hasOwnProperty(idx)) {
                    settings.widget.store.filters[idx] = settings.widget.store.criteria[idx];
                } else if (settings.widget.store.dimension[idx] != null) {
                    settings.widget.store.filters[idx] =
                        $.map(settings.widget.store.dimension[idx].codes, function (key, el) {
                            return el;
                        });
                }
            });
        }
        // remove empty array 
        $.each(settings.widget.store.filters,function (idx, data){
            if (settings.widget.store.filters[idx].length == 0)
                delete settings.widget.store.filters[idx];
        });

        $(settings.$items.slice_container).empty();
        
        if (!stub) {
            DrawHTML_Slice(settings.$items.slice_container,
                            settings.widget.store.layout.axis_z,
                            settings.widget.store.dimension,
                            settings.widget.store.hideDimension);
        }

        viewMode = 0;
        $(settings.$items.data_container).show();
        $(settings.$items.chart_container).hide();

    }

    function SaveQuery(cod, title, dataflow, filters, layout, configuration) {

        clientShowWaitDialog();

        var data = {
            UserCode: cod,
            Query: {
                Title: title,
                DataFlow: dataflow,
                Layout: layout,
                Criteria: filters,
                Configuration: configuration
            }
        }


        clientPostJSON(settings.url.SaveQuery, clientParseObjectToJson(data),
        function (jsonString) {

            //var result = clientParseJsonToObject(jsonString);

            if (jsonString != null) {
                //alert(jsonString.indexOf("QueryId"));
                
                //Per verificare il salvataggio della query controllare la presenza della sottostringa "QueryId"
                var ind = jsonString.indexOf("QueryId");

                var textMess = (ind > -1) ? settings.widget.messages.query_saved : settings.widget.messages.query_savError;
                clientCloseWaitDialog();

                var msg_container = document.createElement("div");

                var msg_p = document.createElement("p");

                $(msg_p).text(textMess);

                $(msg_p).appendTo(msg_container);

                $(msg_container).dialog({
                    title: "Message",
                    resizable: false,
                    width: 240,
                    height: 120,
                    modal: true,
                    buttons: [{
                        text:settings.widget.messages.label_ok,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }]
                });
            }

        },
        function (event, status, errorThrown) {
            clientCloseWaitDialog();
            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
            //clientAjaxError(event, status);
            return;
        },
        false);
    }; 
    function SaveTemplate (cod, title, cbox_sel, block_sel,isVar,isDec,isCri,dataflow, filters, layout, configuration) {

        clientShowWaitDialog();

        var data = {
            UserCode: cod,
            //TemplateId:0
            Template: {
                Title: title,
                DataFlow: dataflow,
                Layout: layout,
                Criteria: filters,
                Configuration: configuration,
                HideDimension: cbox_sel,
                BlockXAxe: (inArray('x', block_sel) < 0)?false:true,
                BlockYAxe: (inArray('y', block_sel) < 0)?false:true,
                BlockZAxe: (inArray('z', block_sel) < 0) ? false : true,
                EnableCriteria: isCri,
                EnableDecimal: isDec,
                EnableVaration: isVar,
            }
        }


        clientPostJSON(settings.url.SaveTemplate, clientParseObjectToJson(data),
        function (jsonString) {

            //var result = clientParseJsonToObject(jsonString);

            if (jsonString != null) {

                clientCloseWaitDialog();

                var msg_container = document.createElement("div");

                var msg_p = document.createElement("p");

                $(msg_p).text(settings.widget.messages.template_saved);

                $(msg_p).appendTo(msg_container);

                $(msg_container).dialog({
                    //title: "Message",
                    resizable: false,
                    width: 240,
                    height: 120,
                    modal: true,
                    buttons:[ {
                        text:settings.widget.messages.label_ok,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }]
                });
            }

        },
        function (event, status, errorThrown) {
            clientCloseWaitDialog();
            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
            //clientAjaxError(event, status);
            return;
        },
        false);
    }
    function DeleteTemplate(cod, dataflow,configuration) {

        clientShowWaitDialog();

        var data = {
            UserCode: cod,
            TemplateId:0,
            Template: {
                Title: title,
                DataFlow: dataflow,
                Layout: layout,
                Criteria: filters,
                Configuration: configuration,
                HideDimension: cbox_sel,
                BlockXAxe: (inArray('x', block_sel) < 0) ? false : true,
                BlockYAxe: (inArray('y', block_sel) < 0) ? false : true,
                BlockZAxe: (inArray('z', block_sel) < 0) ? false : true
            }
        }


        clientPostJSON(settings.url.DeleteTemplate, clientParseObjectToJson(data),
        function (jsonString) {

            //var result = clientParseJsonToObject(jsonString);

            if (jsonString != null) {

                clientCloseWaitDialog();

                var msg_container = document.createElement("div");

                var msg_p = document.createElement("p");

                $(msg_p).text(settings.widget.messages.query_delete);

                $(msg_p).appendTo(msg_container);

                $(msg_container).dialog({
                    //title: "Message",
                    resizable: false,
                    width: 240,
                    height: 120,
                    modal: true,
                    buttons: [{
                        text: settings.widget.messages.label_ok,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }]
                });
            }

        },
        function (event, status, errorThrown) {
            clientCloseWaitDialog();
            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
            //clientAjaxError(event, status);
            return;
        },
        false);
    }

    function ShowChartArea(dest, hideDimension) {

        $(dest).empty();
        settings.widget.store.chartCustom = {};
        settings.widget.store.chartFilters = {};
        var setDefaultTypeChartSpline = 'selected="selected"';
        var setDefaultTypeChartColumn = '';

        for (i in settings.widget.store.dimension) {

            settings.widget.store.chartFilters[i] = [];

            if (i == time_dimension_key) {
                settings.widget.store.chartFilters[i] = settings.widget.store.filters[i];
                if (settings.widget.store.filters[i][0] == settings.widget.store.filters[i][1]) {
                    setDefaultTypeChartSpline = '';
                    setDefaultTypeChartColumn = 'selected="selected"';
                }
            }
            else {
                var codes = (settings.widget.store.filters != undefined) ? Object.keys(settings.widget.store.filters) : null;
                if ((inArray(i, codes) >= 0) && (codes != null))
                    settings.widget.store.chartFilters[i].push(settings.widget.store.filters[i][0]);
                else {
                    if (settings.widget.store.criteria.hasOwnProperty(i)) {
                        settings.widget.store.chartFilters[i].push(settings.widget.store.criteria[i][0]);
                    } else if (settings.widget.store.dimension[i] != null)
                        for (code in settings.widget.store.dimension[i].codes) {
                            settings.widget.store.chartFilters[i].push(code);
                            break;
                        }
                }
            }
        }

        var table = document.createElement("table");
        var tr = document.createElement("tr");
        var td_criteria = document.createElement("td");
        $(td_criteria).css('width', 'auto');
        $(td_criteria).appendTo(tr);
        var td_chart = document.createElement("td");
        $(td_chart).css('width', '100%');
        $(td_chart).css('vertical-align', 'top');
        $(td_chart).appendTo(tr);
        $(tr).appendTo(table);

        var criteria_sel = document.createElement("div");
        $(criteria_sel).css('width', '350px');
        $(criteria_sel).appendTo(td_criteria);

        var numDim=0;
        $.each(settings.widget.store.dimension, function (idx, codes) {
            if (inArray(idx, hideDimension)<0){
                if (settings.widget.store.dimension[idx] != null) {
                    var keys = Object.keys(settings.widget.store.dimension[idx].codes);
                    if (keys.length > 1) {
                        var header = document.createElement("h3");
                        $(header).data('sdmx_key', idx);
                        $(header).text(idx + ' - ' + settings.widget.store.dimension[idx].title);
                        $(header).appendTo(criteria_sel);

                        var body = document.createElement("div");
                        var codelist = document.createElement("ul");
                        $(codelist).appendTo(body);
                        $(body).appendTo(criteria_sel);
                        numDim++;
                    }
                }
            }
        });
        var header = document.createElement("h3");
        $(header).text('Option');
        $(header).appendTo(criteria_sel);
        var body = document.createElement("div");
        $(body).appendTo(criteria_sel);

        // Select chart value type
        var obs_type = document.createElement("div");
        $(obs_type).addClass('div_option');
        $(obs_type).append('<p class="title">' + settings.widget.messages.label_obs_value_desc + '</p>');
        $(obs_type).append(
            '<p><input type="checkbox" name="v" checked="checked" />' + settings.widget.messages.label_varValue + '</p>' +
            '<p><input type="checkbox" name="vt"/>' + settings.widget.messages.label_varTrend + '</p>' +
            '<p><input type="checkbox" name="vc"/>' + settings.widget.messages.label_varCyclical + '</p>');
        $(obs_type).appendTo(body);

        // Select chart type
        var chart_type = document.createElement("div");
        $(chart_type).addClass('div_option');
        $(chart_type).append('<p class="title">' + settings.widget.messages.label_chart_type_desc + '</p>');
        var select_chart_type = document.createElement("select");
        $(select_chart_type).append(
            //'<option value="spline" selected="selected" >Spline</option>' +
            '<option value="spline" '+setDefaultTypeChartSpline+' >Spline</option>' +
            '<option value="line">Line</option>' +
            '<option value="area">Area</option>'+
            '<option value="column" ' + setDefaultTypeChartColumn + ' >Column</option>' +
            '<option value="stackedColumn">Stacked Column</option>' +
            '<option value="pie">Pie</option>' +
            '<option value="scatter">Scatter</option>');
            //'<option value="bar">Bar</option>' +
            //'<option value="stackedBar">Stacked Bar</option>' +
            //'<option value="bubble">Bubble</option>' +
        $(select_chart_type).appendTo(chart_type);
        $(chart_type).appendTo(body);

        var dimension_axe = document.createElement("div");
        $(dimension_axe).addClass('div_option');
        $(dimension_axe).append('<p class="title">' + settings.widget.messages.label_dimension_axe + '</p>');
        var select_dimension_axe = document.createElement("select");
        $(select_dimension_axe).append("<option value='"+time_dimension_key+"'>" + settings.widget.messages.label_TIME_PERIOD + "</option>");
        $(select_dimension_axe).appendTo(dimension_axe);
        $(dimension_axe).appendTo(body);

        var requestChart = function () {

            obs_type_sel = [];
            if ($(obs_type).find('input[name=v]').prop('checked')) obs_type_sel.push('v');
            if ($(obs_type).find('input[name=vt]').prop('checked')) obs_type_sel.push('vt');
            if ($(obs_type).find('input[name=vc]').prop('checked')) obs_type_sel.push('vc');

            if (obs_type_sel.length == 0 || obs_type_sel.length > 2) {
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_check_obs_desc);
                return;
            }

            var chart_type_sel = $(select_chart_type).val();
            if (chart_type_sel == undefined || chart_type_sel == "") {
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_check_chart_type_desc);
                return;
            }
            var chart_axe_sel = $(select_dimension_axe).val();
            if (chart_axe_sel == undefined || chart_axe_sel == "") {
                chart_axe_sel = time_dimension_key;
                clientCloseWaitDialog();
                clientShowErrorDialog(settings.widget.messages.label_check_dimension_axe);
                return;
            }

            var dimSelection = 0;
            $.each(settings.widget.store.dimension, function (idx, filter) {

                if (idx != undefined && idx != time_dimension_key) {
                    if (!settings.widget.store.chartFilters.hasOwnProperty(idx)){
                        if (settings.widget.store.filters.hasOwnProperty(idx))
                            settings.widget.store.chartFilters[idx] = settings.widget.store.filters[idx];
                        else
                            settings.widget.store.chartFilters[idx] =
                                $.map(filter.codes, function (Code, idxCode) { return idxCode; });
                    }
                    if (settings.widget.store.chartFilters[idx].length > 1) dimSelection++;
                }
            });
            if (dimSelection > 1) {

                clientShowErrorDialog(settings.widget.messages.label_dimoutlimit_desc);
                return;
            }

            Process_Chart(
                td_chart,
                settings.widget.store.dataflow,
                settings.widget.store.configuration,
                settings.widget.store.chartFilters,
                chart_type_sel,
                obs_type_sel,
                chart_axe_sel,
                settings.widget.store.chartCustom.concept,
                settings.widget.store.chartCustom.codes);

        };

        var btn_chart = document.createElement("span");
        $(btn_chart).css('float','right');
        $(btn_chart).text(settings.widget.messages.label_apply);
        $(btn_chart).appendTo(body);
        $(btn_chart).button().click(requestChart);
        
        var GetCustomSerie = function () {
            var concept;
            var codes;
            var chart_axe_sel = $(select_dimension_axe).val();
            if (chart_axe_sel != time_dimension_key) {
                codes = (settings.widget.store.chartFilters.hasOwnProperty(time_dimension_key)) ?
                        settings.widget.store.chartFilters[time_dimension_key] :
                        Object.keys(settings.widget.store.chartFilters[time_dimension_key]);
                concept = time_dimension_key;
            } else {

                for (i in settings.widget.store.dimension) {
                    if (i != time_dimension_key) {
                        codes = (settings.widget.store.chartFilters.hasOwnProperty(i)) ?
                            settings.widget.store.chartFilters[i] :
                            Object.keys(settings.widget.store.chartFilters[i]);
                        concept = i;
                        if (codes.length > 1) { break; }
                    }
                }
            }
            //settings.widget.store.chartCustom = {};
            CustomChartRender(concept, codes, settings.widget.store.dimension, settings.widget.store.chartCustom);
        };

        var btn_label = document.createElement("span");
        $(btn_label).css('float','left');
        $(btn_label).html('<i class="icon-edit"></i>' + settings.widget.messages.label_serie_label);
        $(btn_label).appendTo(body);
        $(btn_label).button().click(GetCustomSerie);

        $(criteria_sel).accordion({
            active: numDim,
            heightStyle: "content",
            collapsible: true,
            create: function (event, ui) { },
            beforeActivate: function (event, ui) {


                // On opejn Accordion dimension
                var concept = $(ui.newHeader).data('sdmx_key');
                var dest = $(ui.newPanel);
                if (concept == undefined) {

                } else if (concept == time_dimension_key) {
                    var codes = settings.widget.store.dimension[concept].codes;
                    AppendFiltersTime(
                        dest,
                        concept,
                        codes,
                        settings.widget.store.chartFilters,
                        settings.widget.messages,
                        settings.widget.store.dimension,
                        function (count) {

                            var c = 0;
                            if (settings.widget.store.chartFilters[concept].length == 2)
                                c = 1 + parseInt(settings.widget.store.chartFilters[concept][1])- parseInt(settings.widget.store.chartFilters[concept][0]);
                            else c = parseInt(settings.widget.store.chartFilters[concept][0]);

                            if (c == 1){
                                $(ui.newHeader).css('color', '#000000');
                                $(select_dimension_axe).find("option[value='"+time_dimension_key+"']").remove();

                            } else {
                                $(ui.newHeader).css('color', '#FF0000');
                                var exists = 0 != $(select_dimension_axe).find("option[value='" + time_dimension_key + "']").length;
                                if (!exists) $(select_dimension_axe).append("<option value='" + time_dimension_key + "'>" + settings.widget.messages.label_TIME_PERIOD + "</option>");
                            }


                        }, 10000);
                } else {

                    var codes = {};

                    if (settings.widget.store.filters != undefined
                        && settings.widget.store.filters.hasOwnProperty(concept)) {
                        var constraint = settings.widget.store.filters[concept];
                        if(constraint != undefined){
                            $.each(constraint, function (idx, code) {
                                codes[code]=settings.widget.store.dimension[concept].codes[code];
                            });
                        }
                    }else codes = settings.widget.store.dimension[concept].codes;

                    AppendFiltersCoded(
                        dest,
                        concept,
                        codes,
                        settings.widget.store.chartFilters,
                        settings.widget.messages,
                        settings.widget.store.dimension,
                        function (count, sender) {
                            if (settings.widget.store.chartFilters.hasOwnProperty(concept) 
                                && settings.widget.store.chartFilters[concept].length == 1) {
                                $(ui.newHeader).css('color', '#000000');
                                $(select_dimension_axe).find("option[value='" + concept + "']").remove();

                            } else {
                                $(ui.newHeader).css('color', '#FF0000');
                                var exists = 0 != $(select_dimension_axe).find("option[value='" + concept + "']").length;
                                if (!exists) $(select_dimension_axe).append("<option value='" + concept + "'>" + settings.widget.store.dimension[concept].title + "</option>");
                            }
                        }, 10000);
                }
            }
        });

        $(table).appendTo(dest);

        requestChart();
    }
    function ShowMetaArea(dest, hideDimension) {

        clientShowWaitDialog();

        var _data = {
            Artefact: settings.widget.store.dataflow,
            Configuration: settings.widget.store.configuration,
            ArtefactType:"*"
        };
        clientPostJSON(
                settings.url.GetMetadata,
                clientParseObjectToJson(_data),
                function (jsonString) {

                    $(dest).empty();

                    clientCloseWaitDialog();

                    var result = clientParseJsonToObject(jsonString);

                    if (result != null) {

                        var tb = document.createElement('table');
                        $(tb).appendTo(dest);
                        var tr = document.createElement('tr');
                        $(tr).appendTo(tb);

                        var td_left = document.createElement('td');
                        $(td_left).addClass('summaryPanel');
                        $(td_left).appendTo(tr);

                        var td_right = document.createElement('td');
                        $(td_right).addClass('dettailPanel');
                        $(td_right).appendTo(tr);

                        var div_dsd = document.createElement('div');
                        $(div_dsd).appendTo(td_left);
                        var div_concept = document.createElement('div');
                        $(div_concept).appendTo(td_left);
                        var div_codelist = document.createElement('div');
                        $(div_codelist).appendTo(td_left);

                        $(div_dsd).append('<p class="field_desc">Data Structure Definition</p>');

                        var a_dsd = document.createElement('p');
                        $(a_dsd).attr('title',result.Dsd.Id + '+' + result.Dsd.Agency + '+' + result.Dsd.Version);
                        $(a_dsd).text(result.Dsd.Name);
                        $(a_dsd).click(function () { DrawHTML_DettailArtefact($(settings.$items.meta_container).find(".dettailPanel"), 'DSD', result.Dsd.Id, result.Dsd.Agency, result.Dsd.Version, settings.widget.store.configuration,settings.url.GetMetadata); });
                        $(a_dsd).appendTo(div_dsd);

                        $(div_concept).append('<p class="field_desc">Concept</p>');
                        $.each(result.Concept, function (key, concept) {
                            var a_concept = document.createElement('p');
                            $(a_concept).attr('title', concept.Id + '+' + concept.Agency + '+' + concept.Version);
                            $(a_concept).text(concept.Name);
                            $(a_concept).click(function () { DrawHTML_DettailArtefact($(settings.$items.meta_container).find(".dettailPanel"), 'CONCEPTSCHEME', concept.Id, concept.Agency, concept.Version, settings.widget.store.configuration, settings.url.GetMetadata); });
                            $(a_concept).appendTo(div_concept);
                        });

                        $(div_codelist).append('<p class="field_desc">Codelist</p>');
                        $.each(result.Codelist, function (key, codelist) {
                            var a_codelist = document.createElement('p');
                            $(a_codelist).attr('title', codelist.Id + '+' + codelist.Agency + '+' + codelist.Version);
                            $(a_codelist).text(codelist.Name);
                            $(a_codelist).click(function () { DrawHTML_DettailArtefact($(settings.$items.meta_container).find(".dettailPanel"), 'CODELIST', codelist.Id, codelist.Agency, codelist.Version, settings.widget.store.configuration, settings.url.GetMetadata); });
                            $(a_codelist).appendTo(div_codelist);
                        });
                        
                    }

                },
                function (event, status, errorThrown) {
                    clientCloseWaitDialog();
                    clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                    //clientAjaxError(event, status);
                    return;
                },
                false);

    }

};

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

function CustomChartRender(concept,codes,codemap,outCustom) {

    var div_custom_serie_chart = document.createElement("div");

    var p_title = document.createElement("p");
    $(p_title).css("width", "100%");
    $(p_title).text(concept+" - "+codemap[concept].title);
    $(p_title).appendTo(div_custom_serie_chart);

    $.each(codes, function (idx, code) {

        var findCode = (outCustom.hasOwnProperty("concept") && outCustom.concept == concept);
        var title = codemap[concept].codes[code].name;
        var enable=false;
        var chartType = "spline";

        if (findCode) {
            for (i = 0; i < outCustom.codes.length;i++){
                if (outCustom.codes[i].code == code) {
                    enable = true;
                    title = outCustom.codes[i].title;
                    chartType = outCustom.codes[i].chartType;
                }
            }
        }

        var p_select = document.createElement("p");
        $(p_select).css("width", "100%");
        $(p_select).appendTo(div_custom_serie_chart);

        var chk = document.createElement("input");
        $(chk).attr("type", "checkbox");
        $(chk).css("width", "auto");
        $(chk).css("float", "left");
        $(chk).data("sdmx_concept", concept);
        $(chk).data("sdmx_code", code);
        $(chk).val(code);
        $(chk).prop('checked', enable);
        $(chk).appendTo(p_select);

        var select = document.createElement("select");
        $(select).css("width", "auto");
        $(select).css("float", "left");
        $(select).append(
                '<option value="spline" ' + ((chartType == "spline") ? 'selected="selected"' : '') + ' >Spline</option>' +
                '<option value="line"' + ((chartType == "line") ? 'selected="selected"' : '') + '>Line</option>' +
                '<option value="area"' + ((chartType == "area") ? 'selected="selected"' : '') + '>Area</option>'+
                '<option value="column"' + ((chartType == "column") ? 'selected="selected"' : '') + '>Column</option>' +
                '<option value="stackedColumn"' + ((chartType == "stackedColumn") ? 'selected="selected"' : '') + '>Stacked Column</option>');
        $(select).appendTo(p_select);

        $(p_select).append("<span>[" + code + "] " + codemap[concept].codes[code].name + "</span>");

        var txt = document.createElement("input");
        $(txt).attr("type", "text");
        $(txt).css("width", "100%");
        $(txt).css("float", "left");
        $(txt).val(title);
        $(txt).appendTo(p_select);
    });

    $(div_custom_serie_chart).dialog({
        title: client.main.messages.label_serie_label,
        width: 600,
        heigth:400,
        modal: true,
        resizable: false,
        closeOnEscape: false,
        draggable: true,
        position: { my: "center", at: "center", of: window },
        autoOpen: false,
        buttons: [
            {
                text: client.main.messages.label_ok,
                click: function () {


                    outCustom['concept'] = "";
                    outCustom['codes'] = [];

                    $(div_custom_serie_chart).find("input[type=checkbox]").each(function () {
                        if ($(this).prop('checked') == true) {

                            var select = $(this).parent().find("select");
                            var text = $(this).parent().find("input[type=text]");
                            var concept = $(this).data("sdmx_concept");
                            var code = $(this).data("sdmx_code");

                            outCustom['concept'] = concept;
                            outCustom['codes'].push({
                                code: code,
                                chartType: $(select).val(),
                                title: $(text).val()
                            });
                        }
                    });
                    
                    $(this).dialog("close");
                }
            },
            {
                text: client.main.messages.label_cancel,
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });
    $(div_custom_serie_chart).dialog("open");
}

function ShowExtraPopup(index) {

    $("#" + index + "_extra_dialog").dialog({
        title: client.main.messages.label_dialog_attribute,
        //height: 320,
        //Width: 460,
        modal: true,
        resizable: false,
        closeOnEscape: false,
        draggable: true,
        position: { my: "center", at: "center", of: window },
        autoOpen: true,
        buttons: [
            {
                text: "Cancel",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });
}

function OnKeyTitleClick(sender) {

    var mode = parseInt($(sender).attr('display_mode'));

    $.each($(".togglablekeyvalue[sdmx_key='" + $(sender).attr('sdmx_key') + "']"), function (index) {

        if (mode == 0) {
            $(this).text($(this).attr('sdmx_value'));
            $(this).attr('display_mode', mode);
        } else if (mode == 1) {
            $(this).text($(this).attr('sdmx_text'));
            $(this).attr('display_mode', mode);
        } else if (mode == 2) {
            $(this).text("[" + $(this).attr('sdmx_value') + "] " + $(this).attr('sdmx_text'));
            $(this).attr('display_mode', mode);
        }
    });
    mode = (mode == 2) ? 0 :(mode+1);
    $(sender).attr('display_mode', mode);

}
function OnKeyValueClick(sender) {

    if ($(sender).attr('display_mode') == 0) {
        $(sender).text($(sender).attr('sdmx_value'));
        $(sender).attr('display_mode', 1);
    } else if ($(sender).attr('display_mode') == 1) {
        $(sender).text($(sender).attr('sdmx_text'));
        $(sender).attr('display_mode', 2);
    } else if ($(sender).attr('display_mode') == 2) {
        $(sender).text("["+$(sender).attr('sdmx_value')+"] "+$(sender).attr('sdmx_text'));
        $(sender).attr('display_mode', 0);
    }
}

/*******************************************/
// Method for CRITERIA costraint
/*******************************************/
function AppendFiltersCodedCostraint(
    dest,
    dataflow, configuration,
    concept,
    codes,
    outCriteria,
    messages,
    codemap,
    callBack,
    count_max,
    h3) {
    
    $(dest).empty();

    var codelist = $.map(codes, function (node, code) {

        var par = (node.parent != null) ? codes.hasOwnProperty(node.parent)?node.parent:"#":"#";
        var code_id = code;

        var isSingleCode = false;
        for (var i=0;i<codes.length;i++)
            if (i > 1) {
                isSingleCode = true;
                break;
            }

        return {
            id: code_id,
            text: "[" + code_id + "] " + node.name,
            parent: par,
            state: {
                opened: true,
                checked: (inArray(code_id, outCriteria[concept]) >= 0),
                //types: "mandatory",
            },
            icon: false,
            sdmxCode: "_fix_" + code_id.toString(),
        };

    });
    // jstree_filters
    var jstree_filters = document.createElement("div");
    $(jstree_filters)
        .on("check_node.jstree", function (event, data) {
            var currConcept = data.instance.settings.sdmxConcept;
            var currCode = data.node.original.sdmxCode.toString();
            
            if (!outCriteria.hasOwnProperty(currConcept))
                outCriteria[currConcept] = [];

            // remove prefix
            var fix_currCode = currCode.substring(5);
            if (inArray(fix_currCode, outCriteria[currConcept]) == -1)
                outCriteria[currConcept].push(fix_currCode);

            // MODE CRITERIA 2
            var idx_prev_tabs = [];
            var idx_next_tabs = [];
            var idx_tab = 0;
            var idx_next_tab = 0;
            var clearNext = false;
            $.each(codemap, function (concept) {

                if (clearNext) delete outCriteria[concept];
                if (concept == currConcept) {
                    clearNext = true;
                    idx_next_tab = idx_tab + 1;
                }
                if (clearNext) idx_next_tabs.push(idx_tab);
                else idx_prev_tabs.push(idx_tab);

                idx_tab++;
            });

            if(criteriLimit){
                if(outCriteria[currConcept].length > 0){
                    
                }else{

                }
            }

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });

            var count_sel = (outCriteria[currConcept] != undefined) ? outCriteria[currConcept].length : 0
            $(h3).find('a').text(currConcept + " (" + count_sel + "/" + codelist.length + ")");

            event.preventDefault();
            event.stopImmediatePropagation();

            return;
        })
        .on("uncheck_node.jstree", function (event, data) {

            var currConcept = data.instance.settings.sdmxConcept;
            var currCode = data.node.original.sdmxCode.toString();

            // remove prefix
            var fix_currCode = currCode.substring(5);

            if (!outCriteria.hasOwnProperty(currConcept)) delete outCriteria[currConcept];
            outCriteria[currConcept].splice(inArray(fix_currCode, outCriteria[currConcept]),1);

            var clearNext = false;
            $.each(codemap, function (concept) {
                if (clearNext) delete outCriteria[concept];
                if (concept == currConcept) clearNext = true;
            });

            if (outCriteria[currConcept].length > 0) {

            } else {

            }

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });

            var count_sel = (outCriteria[currConcept] != undefined) ? outCriteria[currConcept].length : 0
            $(h3).find('a').text(currConcept + " (" + count_sel + "/" + codelist.length + ")");

            event.preventDefault();
            event.stopImmediatePropagation();

            return;
        })
        .on("check_all.jstree", function (event, data) {
        
            var currConcept = data.instance.settings.sdmxConcept;

            $.each(data.instance._model.data, function (idCode) {
                var node = data.instance.get_node(idCode);
                if (node.original != undefined) {
                    var currCode = node.original.sdmxCode.toString();
                    // remove prefix
                    var fix_currCode = currCode.substring(5);

                    if (!outCriteria.hasOwnProperty(currConcept))
                        outCriteria[currConcept] = [];

                    if (inArray(fix_currCode, outCriteria[currConcept]) == -1)
                        outCriteria[currConcept].push(fix_currCode);
                }
            });

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });

            var count_sel = (outCriteria[currConcept] != undefined) ? outCriteria[currConcept].length : 0
            $(h3).find('a').text(currConcept + " (" + count_sel + "/" + codelist.length + ")");

            event.preventDefault();
            event.stopImmediatePropagation();

            return;
        })
        .on("uncheck_all.jstree", function (event, data) {

            var currConcept = data.instance.settings.sdmxConcept;
            delete outCriteria[currConcept];

            var clearNext = false;
            $.each(codemap, function (concept) {
                if (clearNext) delete outCriteria[concept];
                if (concept == currConcept) clearNext = true;
            });

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });

            var count_sel = (outCriteria[currConcept] != undefined) ? outCriteria[currConcept].length : 0
            $(h3).find('a').text(currConcept + " (" + count_sel + "/" + codelist.length + ")");

            event.preventDefault();
            event.stopImmediatePropagation();

            return;
        })
        .on("loaded.jstree", function (event, data) {

            if (codelist.length > 1) {
                // BUTTON BAR - div > span
                var button_filters = document.createElement("div");

                var btn_check_all = document.createElement("span");
                $(btn_check_all).html('<i class="icon-check-2"></i>');//(messages.label_select_all);
                $(btn_check_all).attr('title', messages.label_select_all);
                $(btn_check_all).button()
                .click(function (event) {
                    $(jstree_filters).jstree("check_all");
                });

                var btn_uncheck_all = document.createElement("span");
                $(btn_uncheck_all).html('<i class="icon-check-outline"></i>');//(messages.label_deselect_all);
                $(btn_uncheck_all).attr('title', messages.label_deselect_all);
                $(btn_uncheck_all).button()
                .click(function (event) {
                    $(jstree_filters).jstree("uncheck_all");
                });

                $(btn_check_all).appendTo(button_filters);
                $(btn_uncheck_all).appendTo(button_filters);

                $(button_filters).appendTo(dest);

            } else {

                outCriteria[concept] = [];
                outCriteria[concept].push(codelist[0].id);
                data.instance._model.data[codelist[0].id].state.checked = true;
                data.instance._model.data[codelist[0].id].state.disabled = true;

            }

            var count_sel = (outCriteria[concept] != undefined) ? outCriteria[concept].length : 0
            $(h3).find('a').text(concept + " (" + count_sel + "/" + codelist.length + ")");

            var div_scroll = document.createElement("div");
            $(div_scroll).addClass("scroller_y");
            $(div_scroll).appendTo(dest);
            $(jstree_filters).appendTo(div_scroll);
            $(jstree_filters).jstree().redraw(true);

        })
        .jstree({
            'plugins': ["themes", "checkbox", "types", "ui"],//, , "state""json_data"],
            sdmxConcept: concept.toString(),
            'checkbox': {
                "tie_selection": false,
                "two_state": true,
                "three_state": false,
                //'keep_selected_style': false
            },
            'ui': { "select_limit": 1 },
            'core': {
                'themes': { "theme": "default", dots: true, "icons": true },
                "data": codelist,
                "progressive_render": true
            }
        });

}

function AppendFiltersTimeCostraint(
    dest,
    dataflow, configuration,
    concept,
    codes,
    outCriteria,
    messages,
    codemap,
    callBack,
    count_max,
    h3) {

    $(dest).empty();

    var isLastPeriod = false;

    if (outCriteria[concept] == undefined) outCriteria[concept] = [];
    else isLastPeriod = (outCriteria[concept].length == 1);

    var times = $.map(codes, function (time) { return time.name; });

    var idx_start = 0;
    var idx_end = times.length - 1;

    var time_min = (outCriteria[concept][0] == undefined) ? outCriteria[concept][0]=times[idx_start] : outCriteria[concept][0];
    var time_max = (outCriteria[concept][1] == undefined) ? (!isLastPeriod) ? outCriteria[concept][1] = times[idx_end] : times[idx_end] :outCriteria[concept][1];

    if (!codes.hasOwnProperty(time_max))
        time_max = time_min;

    // append
    var tb_time = document.createElement("table");

    var tr_time_min = document.createElement("tr");
    var tr_time_max = document.createElement("tr");
    var td_time_min_p = document.createElement("td");
    var td_time_min_txt = document.createElement("td");
    var td_time_max_p = document.createElement("td");
    var td_time_max_txt = document.createElement("td");

    var slc_date_min = document.createElement("select");
    $(slc_date_min).prop('disabled', isLastPeriod);
    var slc_date_max = document.createElement("select");
    $(slc_date_max).prop('disabled', isLastPeriod);

    for (var i = idx_start; i <= idx_end; i++) {
        $("<option value='" + times[i] + "' " + ((time_min === times[i]) ? " selected='selected' " : "") + ">" + times[i] + "</option>").appendTo(slc_date_min);
        $("<option value='" + times[i] + "' " + ((time_max === times[i]) ? " selected='selected' " : "") + ">" + times[i] + "</option>").appendTo(slc_date_max);
    }

    $(slc_date_min).change(function () {
        if (!isLastPeriod) {
            outCriteria[concept][0] = $(this).val();

            if (outCriteria[concept][0] > outCriteria[concept][1]) {
                outCriteria[concept][1] = outCriteria[concept][0];
                $(slc_date_max).val(outCriteria[concept][1]);
            }

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });

        }
    });
    $(slc_date_max).change(function () {
        if (!isLastPeriod) {
            outCriteria[concept][1] = $(this).val();

            if (outCriteria[concept][1] < outCriteria[concept][0]) {
                outCriteria[concept][0] = outCriteria[concept][1];
                $(slc_date_min).val(outCriteria[concept][0]);
            }

            SetCodeCostraint(
                dataflow,
                configuration,
                concept,
                outCriteria,
                codemap,
                function (count) {
                    count_glob = count;
                    if (count > 0) {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + count_max);
                    } else {
                        $(dest).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                    }
                });
        }
    });

    $(td_time_min_p).text(messages.label_time_start);
    $(td_time_min_p).appendTo(tr_time_min);
    $(slc_date_min).appendTo(td_time_min_txt);
    $(td_time_min_txt).appendTo(tr_time_min);

    $(td_time_max_p).text(messages.label_time_end);
    $(td_time_max_p).appendTo(tr_time_max);
    $(slc_date_max).appendTo(td_time_max_txt);
    $(td_time_max_txt).appendTo(tr_time_max);

    $(tr_time_min).appendTo(tb_time);
    $(tr_time_max).appendTo(tb_time);
    if (sessionStorage.user_code != null){

        var usRole = JSON.parse(sessionStorage.user_role);
        if (usRole.RoleId == 1) {

            var tr_year_plus = document.createElement("tr");
            var td_year_plus_p = document.createElement("td");
            var td_year_plus_txt = document.createElement("td");

            var year_plus_txt = document.createElement("input");
            $(year_plus_txt).attr('type', 'text');
            $(year_plus_txt).prop('disabled', !isLastPeriod);
            $(year_plus_txt).attr('name', 'year_plus_txt');
            if (isLastPeriod) $(year_plus_txt).val(time_min);

            $(year_plus_txt).change(function () {
                if (isLastPeriod) {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(this).val();
                    //if (count_max > 0) callBack(CountMaxResults(codemap, outCriteria));
                }
            });
            var year_plus_chk = document.createElement("input");
            $(year_plus_chk).attr('type', 'checkbox');
            $(year_plus_chk).prop('checked', isLastPeriod);
            $(year_plus_chk).attr('name', 'year_plus_chk');
            $(year_plus_chk).click(function () {

                isLastPeriod = $(this).is(':checked');

                if (isLastPeriod) {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(year_plus_txt).val();
                } else {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(slc_date_min).val();
                    outCriteria[concept][1] = $(slc_date_max).val();
                }

                $(slc_date_min).prop('disabled', isLastPeriod);
                $(slc_date_max).prop('disabled', isLastPeriod);
                $(year_plus_txt).prop('disabled', !isLastPeriod);

            });
            var year_plus_p = document.createElement("p");
            $(year_plus_p).text(messages.label_time_plus);
            $(year_plus_p).append(year_plus_chk);

            $(td_year_plus_p).append(year_plus_p);
            $(td_year_plus_p).appendTo(tr_year_plus);
            $(td_year_plus_txt).append(year_plus_txt);
            $(td_year_plus_txt).appendTo(tr_year_plus);
            $(tr_year_plus).appendTo(tb_time);
        }
    }
    $(tb_time).appendTo(dest);

    $(dest).parent().find('.waitInLinee').remove();

}

function OpenPopUpFiltersCostraint(
    dataflow,
    configuration,
    codemap,
    target,
    outCriteria,
    callBack,
    cssClass,
    messages,
    time_dimension_key,
    MaxResults,
    hideDimension) {

    clientShowWaitDialog();

    // popup
    var div_popup = document.createElement("div");
    $(div_popup).attr('id', 'criteria_popup');
    // tabs container
    var div_tabs = document.createElement("div");
    $(div_tabs).addClass(cssClass.tabs_div);
    // tabs header
    var ul = document.createElement("ul");
    $(ul).addClass(cssClass.coded_list);
    $(ul).appendTo(div_tabs);

    var idxConcept = 0;
    var targetConcept = -1;
    $.each(codemap, function (concept) {
        
        if (target == concept)
            targetConcept = idxConcept;
        idxConcept++;

        var showDimension = true;

        if (hideDimension != undefined)
            if (inArray(concept, hideDimension) >= 0)
                showDimension = false;

        if (showDimension) {

            // Header Tabs
            var li = document.createElement("li");
            $(li).appendTo(ul);

            var ancor_filter = document.createElement("a");
            $(ancor_filter).addClass(cssClass.tab_header);
            $(ancor_filter).text(concept);
            //$(ancor_filter).attr('title', (target==concept)? codemap[concept].title:"???");
            $(ancor_filter).attr('href', '#tab-' + concept);
            $(ancor_filter).appendTo(li);

            // TAB - div
            var div_tab = document.createElement("div");
            $(div_tab).addClass(cssClass.tab_div);
            $(div_tab).attr('id', 'tab-' + concept);

            $(div_tab).appendTo(div_tabs);
        }
    });

    AppendCodes = function (destination,title_h3) {
        $(destination).empty();

        var concept = $(destination).attr('id').toString().substring(4);// remove #tabs_
        var _data = {
            dataflow: dataflow,
            configuration: configuration,
            Codelist: concept,
            PreviusCostraint: outCriteria
        }

        clientShowWaitDialog();

        clientPostJSON(
            "Main/GetSpecificCodemap",
            clientParseObjectToJson(_data),
            function (jsonString) {

                var result = clientParseJsonToObject(jsonString);

                if (result != null && !result.hasOwnProperty('error')) {

                    clientCloseWaitDialog();

                    // sovrascrivo i codici se non ha criteri
                    if (!outCriteria.hasOwnProperty(concept) || codemap[concept]==null)
                        codemap[concept] = result.codemap[concept];

                    // Title concept - h3
                    var h3 = document.createElement("h3");
                    $(h3).addClass(cssClass.coded_title);
                    $(h3).html(codemap[concept].title);
                    $(h3).appendTo(destination);

                    var div_codes = document.createElement("div");
                    $(div_codes).appendTo(destination);

                    var codeCount = 0;
                    $.each(codemap[concept].codes, function (code) {
                        codeCount++;
                    });

                    // se La prima codelist ha un solo valore richiedo il conteggio delle osservazioni subito
                    if (codeCount == 1) {
                        SetCodeCostraint(
                        dataflow,
                        configuration,
                        concept,
                        outCriteria,
                        codemap,
                        function (count) {
                            count_glob = count;
                            if (count > 0) {
                                $(destination).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + MaxResults);
                            } else {
                                $(destination).parents('#criteria_popup').dialog("option", "title", messages.label_dialog_criteria + " - NaN");
                            }
                        });
                    }

                    if (concept == time_dimension_key) {
                        AppendFiltersTimeCostraint(
                             div_codes,
                             dataflow,
                             configuration,
                             concept,
                             codemap[concept].codes,
                             outCriteria,
                             messages,
                             codemap,
                             null,
                             MaxResults,
                             title_h3);
                    }
                    else {
                        AppendFiltersCodedCostraint(
                            div_codes,
                            dataflow,
                            configuration,
                            concept,
                            codemap[concept].codes,
                            outCriteria,
                            messages,
                            codemap,
                            null,
                            MaxResults,
                            title_h3);
                    }
                } else {
                    clientCloseWaitDialog();
                }
            },
            function (event, status, errorThrown) {
                clientCloseWaitDialog();
                //clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                return;
            },
            false);
    };

    //setup tabs
    $(div_tabs).tabs({
        active: targetConcept,
        create: function (event, ui) {

            var destination = ui.panel;
            AppendCodes(destination, ui.tab);
            
        },
        beforeActivate: function (event, ui) {

            var destination = ui.newPanel;
            AppendCodes(destination, ui.newTab);

        },
    });
    $(div_tabs).appendTo(div_popup);

    // setup Dialog
    $(div_popup).dialog({
        title: messages.label_dialog_criteria,
        width: 900,
        height: 550,
        modal: true,
        resizable: true,
        closeOnEscape: false,
        draggable: true,
        position: { my: "center", at: "center", of: window },
        autoOpen: false,
        buttons: [
            {
                text: messages.label_ok,
                click: function () {
                    //alert(criteriLimit);
                    //alert(MaxResults);
                    //alert(count_glob);
                    /*fabio modifica 18/11/2015*/
                    if (count_glob < MaxResults) {
                        if (!criteriLimit) {
                            //CLIENT POSTJSON PER I CRITERI MANCANTI IN MODALITA' COSTRAINT_NO_LIMIT
                            var allCriteriaSet = true;

                            var str_concept = "";
                            var str_sep = "";
                            $.each(codemap, function (key, concept) {
                                if (inArray(key, hideDimension) < 0) {
                                    if (outCriteria.hasOwnProperty(key)) {
                                        if (outCriteria[key].length < 1) {
                                            allCriteriaSet = false;
                                            str_concept += str_sep + key;
                                            str_sep = ", ";
                                        }
                                    } else {
                                        allCriteriaSet = false;
                                        str_concept += str_sep + key;
                                        str_sep = ", ";
                                    }
                                }
                            });
                            if (!allCriteriaSet) {
                                var _data = {
                                    dataflow: dataflow,
                                    configuration: configuration
                                }
                                //Process_Type_RI_CNL(_data);
                                clientPostJSON(
                                    "Main/GetCodemapCostraintNoLimit",
                                    clientParseObjectToJson(_data),
                                    function (jsonString) {
                                        //alert(jsonString);
                                        var result = clientParseJsonToObject(jsonString);
                                        codemap = result.codemap;
                                    },
                                    function (event, status, errorThrown) {
                                        clientCloseWaitDialog();
                                        //clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
                                        //clientAjaxError(event, status);
                                        return;
                                    },
                                    false);


                                $(this).dialog("close");
                                callBack(outCriteria);

                                /*****************************************************************************/
                            }
                            //FINE 
                        } else {
                            var allCriteriaSet = true;

                            var str_concept = "";
                            var str_sep = "";
                            $.each(codemap, function (key, concept) {
                                if (inArray(key, hideDimension) < 0) {
                                    if (outCriteria.hasOwnProperty(key)) {
                                        if (outCriteria[key].length < 1) {
                                            allCriteriaSet = false;
                                            str_concept += str_sep + key;
                                            str_sep = ", ";
                                        }
                                    } else {
                                        allCriteriaSet = false;
                                        str_concept += str_sep + key;
                                        str_sep = ", ";
                                    }
                                }
                            });

                            if (!allCriteriaSet) {

                                var div_alert = document.createElement('div');
                                //$(div_alert).html("Please choise a selection for tab: " + str_concept);
                                $(div_alert).html("Please choise a selection for tab: " + str_concept);
                                $(div_alert).dialog({
                                    title: messages.label_dialog_criteria,
                                    modal: true,
                                    resizable: true,
                                    closeOnEscape: false,
                                    draggable: true,
                                    position: { my: "center", at: "center", of: window },
                                    autoOpen: false,
                                    buttons: [
                                        {
                                            text: messages.label_ok,
                                            click: function () { $(this).dialog("close"); }
                                        }
                                    ]
                                });
                                $(div_alert).dialog("open");
                            } else {

                                if (CountMaxResults(codemap, outCriteria, hideDimension) < MaxResults) {
                                    $(this).dialog("close");
                                    callBack(outCriteria);
                                } else {
                                    alert(CountMaxResults(codemap, outCriteria, hideDimension));
                                }

                            }
                        }

                    }//end if globale
                    else { alert('sono stati superati il numero di record max visualizzabili'); }

                }
            },
            {
                text: messages.label_cancel,
                click: function (){
                    
                    $(this).dialog("close");
                    outCriteria = [];
                }
            }
        ]
    });

    clientCloseWaitDialog();

    $(div_popup).dialog("open");
}

function SetCodeCostraint(dataflow,configuration, concept, criteria, codemap,callBack) {

    var _data = {
        Dataflow: dataflow,
        Configuration:configuration,
        Codelist: concept,
        PreviusCostraint: criteria
    }
    clientShowWaitDialog();

    clientPostJSON(
        "Main/SetCostraint",
        clientParseObjectToJson(_data),
        function (jsonString) {

            var result = clientParseJsonToObject(jsonString);
            if (!result.hasOwnProperty("error")) {
                if (callBack != undefined && callBack != null)
                    callBack(result);
            }
            clientCloseWaitDialog();
        },
        function (event, status, errorThrown) {
            clientCloseWaitDialog();
            clientShowErrorDialog(settings.widget.messages.label_error_dataParsing);
            //clientAjaxError(event, status);
            return;
        },
        false);

}

/*******************************************/
// Method for CRITERIA standard
/*******************************************/
function OpenPopUpFilters(
    codemap,
    outCriteria,
    callBack,
    cssClass,
    messages,
    time_dimension_key,
    MaxResults,
    hideDimension) {

    clientShowWaitDialog();

    // popup
    var div_popup = document.createElement("div");
    
    // tabs container
    var div_tabs = document.createElement("div");
    $(div_tabs).addClass(cssClass.tabs_div);
    // tabs header
    var ul = document.createElement("ul");
    $(ul).addClass(cssClass.coded_list);
    $(ul).appendTo(div_tabs);

    var countCodemap = -1;
    $.each(codemap, function (concept) {
        countCodemap++;
        var showDimension = true;
        
        if (hideDimension != undefined)
            if (inArray(concept, hideDimension)>=0)
                showDimension = false;

        if (showDimension){

                // Header Tabs
                var li = document.createElement("li");
                $(li).appendTo(ul);
                
                var ancor_filter = document.createElement("a");
                $(ancor_filter).addClass(cssClass.tab_header);
                $(ancor_filter).text(concept);
                $(ancor_filter).attr('title', codemap[concept].title);
                $(ancor_filter).attr('href', '#tab-' + concept);
                $(ancor_filter).appendTo(li);

                // TAB - div
                var div_tab = document.createElement("div");
                $(div_tab).addClass(cssClass.tab_div);
                $(div_tab).attr('id', 'tab-' + concept);

                $(div_tab).appendTo(div_tabs);
            }
    });

    //setup tabs
    $(div_tabs).tabs({
        active:countCodemap,
        create: function (event, ui) {
            var destination = ui.panel;
            $(destination).empty();
            var concept = $(destination).attr('id').toString().substring(4);// remove #tabs_

            // Title concept - h3
            var h3 = document.createElement("h3");
            $(h3).addClass(cssClass.coded_title);
            $(h3).html(codemap[concept].title);
            $(h3).appendTo(destination);

            var div_codes = document.createElement("div");
            $(div_codes).appendTo(destination);

            if (concept == time_dimension_key) {
                AppendFiltersTime(
                     div_codes,
                     concept,
                     codemap[concept].codes,
                     outCriteria,
                     messages,
                     codemap,
                     function (count) {
                         $(div_popup).dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + MaxResults);
                     },
                     MaxResults);
            }
            else {

                AppendFiltersCoded(
                    div_codes,
                    concept,
                    codemap[concept].codes,
                    outCriteria,
                    messages,
                    codemap,
                    function (count) {
                        $(div_popup).dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + MaxResults);
                    },
                    MaxResults);
            }
        },
        beforeActivate: function (event, ui) {
            var destination = ui.newPanel;
            $(destination).empty();
            var concept = $(destination).attr('id').toString().substring(4);// remove #tabs_

            // Title concept - h3
            var h3 = document.createElement("h3");
            $(h3).addClass(cssClass.coded_title);
            $(h3).html(codemap[concept].title);
            $(h3).appendTo(destination);

            var div_codes = document.createElement("div");
            $(div_codes).appendTo(destination);

            if (concept == time_dimension_key) {
                AppendFiltersTime(
                     div_codes,
                     concept,
                     codemap[concept].codes,
                     outCriteria,
                     messages,
                     codemap,
                     function (count) {
                         $(div_popup).dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + MaxResults);
                     },
                     MaxResults);
            }
            else {
                
                AppendFiltersCoded(
                    div_codes,
                    concept,
                    codemap[concept].codes,
                    outCriteria,
                    messages,
                    codemap,
                    function (count) {
                        $(div_popup).dialog("option", "title", messages.label_dialog_criteria + " - " + count + " / " + MaxResults);
                    },
                    MaxResults);
            }
        }
    });

    $(div_tabs).appendTo(div_popup);
    
    $(div_popup).dialog({
        title: messages.label_dialog_criteria + " - " + CountMaxResults(codemap, outCriteria) + " / " + MaxResults,
        width: 900,
        height: 550,
        modal: true,
        resizable: true,
        closeOnEscape: false,
        draggable: true,
        position: { my: "center", at: "center", of: window },
        autoOpen: false,
        buttons: [
            {
                text: messages.label_ok,
                click: function () {

                    $(this).dialog("close");
                    callBack(outCriteria);

                }
            },
            {
                text: messages.label_cancel,
                click: function () {

                    $(this).dialog("close");
                    outCriteria = [];
                }
            }
        ]
    });

    clientCloseWaitDialog();

    $(div_popup).dialog("open");
}

function AppendFiltersCoded(
    dest,
    concept,
    codes,
    outCriteria,
    messages,
    codemap,
    callBack,
    count_max) {

    $(dest).empty();

    var codelist = $.map(codes, function (node, code) {

        var par = (node.parent != null) ? codes.hasOwnProperty(node.parent) ? node.parent : "#" : "#";
        var code_id = code;

        var isSingleCode = false;
        for (var i = 0; i < codes.length; i++)
            if (i > 1) {
                isSingleCode = true;
                break;
            }

        return {
            id: code_id,
            text: "[" + code_id + "] " + node.name,
            parent: par,
            state: {
                opened: true,
                checked: (inArray(code_id, outCriteria[concept]) >= 0),
                //types: "mandatory",
            },
            icon: false,
            //prefisso per fix parsing di interi
            sdmxCode: "_fix_" + code_id.toString(),
        };

    });

    // jstree_filters
    var jstree_filters = document.createElement("div");
    $(jstree_filters)
    .on("check_node.jstree", function (event, data) {
        var currConcept = data.instance.settings.sdmxConcept;
        var currCode = data.node.original.sdmxCode.toString();

        if (!outCriteria.hasOwnProperty(currConcept))
            outCriteria[currConcept] = [];

        // remove prefix
        var fix_currCode = currCode.substring(5);
        if (inArray(fix_currCode, outCriteria[currConcept]) == -1)
            outCriteria[currConcept].push(fix_currCode);

        callBack(CountMaxResults(codemap, outCriteria), data.node);

        event.preventDefault();
        event.stopImmediatePropagation();

        return;
    })
    .on("uncheck_node.jstree", function (event, data) {

        var currConcept = data.instance.settings.sdmxConcept;
        var currCode = data.node.original.sdmxCode.toString();

        // remove prefix
        var fix_currCode = currCode.substring(5);

        if (!outCriteria.hasOwnProperty(currConcept)) delete outCriteria[currConcept];
        outCriteria[currConcept].splice(inArray(fix_currCode, outCriteria[currConcept]), 1);

        callBack(CountMaxResults(codemap, outCriteria), data.node);

        event.preventDefault();
        event.stopImmediatePropagation();

        return;
    })
    .on("check_all.jstree", function (event, data) {

        var currConcept = data.instance.settings.sdmxConcept;

        $.each(data.instance._model.data, function (idCode) {
            var node = data.instance.get_node(idCode);
            if (node.original != undefined) {
                var currCode = node.original.sdmxCode.toString();
                // remove prefix
                var fix_currCode = currCode.substring(5);

                if (!outCriteria.hasOwnProperty(currConcept))
                    outCriteria[currConcept] = [];

                if (inArray(fix_currCode, outCriteria[currConcept]) == -1)
                    outCriteria[currConcept].push(fix_currCode);
            }
        });

        callBack(CountMaxResults(codemap, outCriteria));

        event.preventDefault();
        event.stopImmediatePropagation();

        return;
    })
    .on("uncheck_all.jstree", function (event, data) {

        var currConcept = data.instance.settings.sdmxConcept;
        delete outCriteria[currConcept];

        callBack(CountMaxResults(codemap, outCriteria));

        event.preventDefault();
        event.stopImmediatePropagation();

        return;
    })
    .on("loaded.jstree", function (event, data) {

        if (codelist.length > 1) {
            // BUTTON BAR - div > span
            var button_filters = document.createElement("div");

            var btn_check_all = document.createElement("span");
            $(btn_check_all).html('<i class="icon-check-2"></i>');//(messages.label_select_all);
            $(btn_check_all).attr('title', messages.label_select_all);
            $(btn_check_all).button()
            .click(function (event) {
                $(jstree_filters).jstree("check_all");
            });

            var btn_uncheck_all = document.createElement("span");
            $(btn_uncheck_all).html('<i class="icon-check-outline"></i>');//(messages.label_deselect_all);
            $(btn_uncheck_all).attr('title', messages.label_deselect_all);
            $(btn_uncheck_all).button()
            .click(function (event) {
                $(jstree_filters).jstree("uncheck_all");
            });

            $(btn_check_all).appendTo(button_filters);
            $(btn_uncheck_all).appendTo(button_filters);

            $(button_filters).appendTo(dest);
        } else {

            outCriteria[concept] = [];
            outCriteria[concept].push(codelist[0].id);
            data.instance._model.data[codelist[0].id].state.checked = true;
            data.instance._model.data[codelist[0].id].state.disabled = true;

        }

        var div_scroll = document.createElement("div");
        $(div_scroll).addClass("scroller_y");
        $(div_scroll).appendTo(dest);
        $(jstree_filters).appendTo(div_scroll);
        $(jstree_filters).jstree().redraw(true);

    })
    .jstree({
        'plugins': ["themes", "checkbox", "types", "ui"],// "state", "json_data"],
        sdmxConcept: concept.toString(),

        'checkbox': {
            "tie_selection": false,
            "two_state": true,
            "three_state": false,
            //'keep_selected_style': false
        },
        'ui': { "select_limit": 1 },
        'core': {
            'themes': { "theme": "default", dots: true, "icons": true },
            "data": codelist,
            "progressive_render": true
        }
    });

}

function AppendFiltersTime(
    dest,
    concept,
    codes,
    outCriteria,
    messages,
    codemap,
    callBack,
    count_max) {

    $(dest).empty();

    var isLastPeriod = false;

    if (outCriteria[concept] == undefined) outCriteria[concept] = [];
    else isLastPeriod = (outCriteria[concept].length == 1);

    var times = $.map(codes, function (time) { return time.name; });

    var idx_start = 0;
    var idx_end = times.length - 1;

    var time_min = (outCriteria[concept][0] == undefined) ? outCriteria[concept][0] = times[idx_start] : outCriteria[concept][0];
    var time_max = (outCriteria[concept][1] == undefined) ? (!isLastPeriod) ? outCriteria[concept][1] = times[idx_end] : times[idx_end] : outCriteria[concept][1];

    if (!codes.hasOwnProperty(time_max))
        time_max = time_min;

    // append
    var tb_time = document.createElement("table");

    var tr_time_min = document.createElement("tr");
    var tr_time_max = document.createElement("tr");
    var td_time_min_p = document.createElement("td");
    var td_time_min_txt = document.createElement("td");
    var td_time_max_p = document.createElement("td");
    var td_time_max_txt = document.createElement("td");

    var slc_date_min = document.createElement("select");
    $(slc_date_min).prop('disabled', isLastPeriod);
    var slc_date_max = document.createElement("select");
    $(slc_date_max).prop('disabled', isLastPeriod);

    for (var i = idx_start; i <= idx_end; i++) {
        $("<option value='" + times[i] + "' " + ((time_min === times[i]) ? " selected='selected' " : "") + ">" + times[i] + "</option>").appendTo(slc_date_min);
        $("<option value='" + times[i] + "' " + ((time_max === times[i]) ? " selected='selected' " : "") + ">" + times[i] + "</option>").appendTo(slc_date_max);
    }

    $(slc_date_min).change(function () {
        if (!isLastPeriod) {
            outCriteria[concept][0] = $(this).val();
            if (count_max > 0)
                callBack(CountMaxResults(codemap, outCriteria));
        }
    });
    $(slc_date_max).change(function () {
        if (!isLastPeriod) {
            outCriteria[concept][1] = $(this).val();
            if (count_max > 0)
                callBack(CountMaxResults(codemap, outCriteria));
        }
    });

    $(td_time_min_p).text(messages.label_time_start);
    $(td_time_min_p).appendTo(tr_time_min);
    $(slc_date_min).appendTo(td_time_min_txt);
    $(td_time_min_txt).appendTo(tr_time_min);

    $(td_time_max_p).text(messages.label_time_end);
    $(td_time_max_p).appendTo(tr_time_max);
    $(slc_date_max).appendTo(td_time_max_txt);
    $(td_time_max_txt).appendTo(tr_time_max);

    $(tr_time_min).appendTo(tb_time);
    $(tr_time_max).appendTo(tb_time);
    if (sessionStorage.user_code != null) {

        var usRole = JSON.parse(sessionStorage.user_role);
        if (usRole.RoleId == 1) {

            var tr_year_plus = document.createElement("tr");
            var td_year_plus_p = document.createElement("td");
            var td_year_plus_txt = document.createElement("td");

            var year_plus_txt = document.createElement("input");
            $(year_plus_txt).attr('type', 'text');
            $(year_plus_txt).prop('disabled', !isLastPeriod);
            $(year_plus_txt).attr('name', 'year_plus_txt');
            if (isLastPeriod) $(year_plus_txt).val(time_min);

            $(year_plus_txt).change(function () {
                if (isLastPeriod) {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(this).val();
                    if (count_max > 0)
                        callBack(CountMaxResults(codemap, outCriteria));
                }
            });
            var year_plus_chk = document.createElement("input");
            $(year_plus_chk).attr('type', 'checkbox');
            $(year_plus_chk).prop('checked', isLastPeriod);
            $(year_plus_chk).attr('name', 'year_plus_chk');
            $(year_plus_chk).click(function () {

                isLastPeriod = $(this).is(':checked');

                if (isLastPeriod) {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(year_plus_txt).val();
                    if (count_max > 0)
                        callBack(CountMaxResults(codemap, outCriteria));
                } else {
                    outCriteria[concept] = [];
                    outCriteria[concept][0] = $(slc_date_min).val();
                    outCriteria[concept][1] = $(slc_date_max).val();
                    if (count_max > 0)
                        callBack(CountMaxResults(codemap, outCriteria));
                }

                $(slc_date_min).prop('disabled', isLastPeriod);
                $(slc_date_max).prop('disabled', isLastPeriod);
                $(year_plus_txt).prop('disabled', !isLastPeriod);

            });
            var year_plus_p = document.createElement("p");
            $(year_plus_p).text(messages.label_time_plus);
            $(year_plus_p).append(year_plus_chk);

            $(td_year_plus_p).append(year_plus_p);
            $(td_year_plus_p).appendTo(tr_year_plus);
            $(td_year_plus_txt).append(year_plus_txt);
            $(td_year_plus_txt).appendTo(tr_year_plus);
            $(tr_year_plus).appendTo(tb_time);
        }
    }
    $(tb_time).appendTo(dest);

    $(dest).parent().find('.waitInLinee').remove();

}


function OpenPopUpLayout(
    layout, 
    callBack, 
    cssClass, 
    messages, 
    dataKey,
    hideDimension) {

    // Create html table container for 3 list sortable 
    var table_list = document.createElement("table");
    var tr_z = document.createElement("tr");
    var tr_xy = document.createElement("tr");
    var td_z = document.createElement("td");
    var td_x = document.createElement("td");
    var td_y = document.createElement("td");
    var td_help = document.createElement("td");

    var p_z = document.createElement("p");
    var p_x = document.createElement("p");
    var p_y = document.createElement("p");
    var p_help = document.createElement("p");

    var lst_z = document.createElement("ul");
    var lst_x = document.createElement("ul");
    var lst_y = document.createElement("ul");

    $(p_z).html(messages.label_fixed_data);
    $(p_x).html(messages.label_axe_hor);
    $(p_y).html(messages.label_axe_ver);
    $(p_help).html(messages.label_data_grid);

    // Build the table
    $(p_z).appendTo(td_z);
    $(lst_z).appendTo(td_z);
    $(p_x).appendTo(td_x);
    $(lst_x).appendTo(td_x);
    $(p_y).appendTo(td_y);
    $(lst_y).appendTo(td_y);
    $(p_help).appendTo(td_help);

    $(td_z).appendTo(tr_z);
    $(td_x).appendTo(tr_z);
    $(td_y).appendTo(tr_xy);
    $(td_help).appendTo(tr_xy);
    $(tr_z).appendTo(table_list);
    $(tr_xy).appendTo(table_list);
    // Apply style to table

    $(table_list).addClass(cssClass.table_layout);

    $(lst_z).addClass(cssClass.list_layout);
    if (!layout.block_axis_z)
        $(lst_z).addClass(cssClass.list_layout_connected);

    $(lst_x).addClass(cssClass.list_layout);
    if (!layout.block_axis_x)
        $(lst_x).addClass(cssClass.list_layout_connected);

    $(lst_y).addClass(cssClass.list_layout);
    if (!layout.block_axis_y)
        $(lst_y).addClass(cssClass.list_layout_connected);


    // Fill list sortable
    $.each(layout.axis_z, function (idx, item) {
        var showDim=true;
        if (hideDimension != undefined 
            && inArray(item, hideDimension) >= 0)showDim=false;
        if(showDim)
        {
            var li = document.createElement("li");
            $(li).addClass(cssClass.list_item_layout);
            $(li).html(item);
            $(li).data(dataKey.sdmxKey, item);
            $(li).appendTo(lst_z);
        }
    });
    $.each(layout.axis_x, function (idx, item) {
        var showDim=true;
        if (hideDimension != undefined 
            && inArray(item, hideDimension) >= 0)showDim=false;
        if(showDim)
        {
            var li = document.createElement("li");
            $(li).addClass(cssClass.list_item_layout);
            $(li).html(item);
            $(li).data(dataKey.sdmxKey, item);
            $(li).appendTo(lst_x);
        }
    });
    $.each(layout.axis_y, function (idx, item) {
        var showDim=true;
        if (hideDimension != undefined 
            && inArray(item, hideDimension) >= 0)showDim=false;
        if(showDim)
        {
            var li = document.createElement("li");
            $(li).addClass(cssClass.list_item_layout);
            $(li).html(item);
            $(li).data(dataKey.sdmxKey, item);
            $(li).appendTo(lst_y);
        }
    });
    // create UI- Sortable

    $(lst_z).sortable({
        cursor: "move",
        forcePlaceholderSize: true,
        connectWith: (layout.block_axis_z)?".none":"."+cssClass.list_layout_connected
    }).disableSelection();
    $(lst_x).sortable({
        cursor: "move",
        forcePlaceholderSize: true,
        connectWith: (layout.block_axis_x) ? ".none" : "." + cssClass.list_layout_connected
    }).disableSelection();
    $(lst_y).sortable({
        cursor: "move",
        forcePlaceholderSize: true,
        connectWith: (layout.block_axis_y) ? ".none" : "." + cssClass.list_layout_connected
    }).disableSelection();

    // popup
    var div_popup = document.createElement("div");
    $(table_list).appendTo(div_popup);

    $(div_popup).dialog({
        title: messages.label_dialog_layout,
        height: 460,
        minWidth: 620,
        modal: true,
        resizable: true,
        closeOnEscape: false,
        draggable: true,
        position: { my: "center", at: "center", of: window },
        autoOpen: false,
        buttons: [
            {
                text: messages.label_ok,
                click: function () {

                    var _axis = {
                        axis_x: $(lst_x).children().map(function (i) { return $(this).data(dataKey.sdmxKey); }).get(),
                        axis_y: $(lst_y).children().map(function (i) { return $(this).data(dataKey.sdmxKey); }).get(),
                        axis_z: $(lst_z).children().map(function (i) { return $(this).data(dataKey.sdmxKey); }).get()
                    };
                    
                    if (_axis.axis_x.length < 1) {
                        clientShowErrorDialog(messages.label_layout_x_missing);
                        return;
                    }
                    if (_axis.axis_y.length < 1){
                        clientShowErrorDialog(messages.label_layout_y_missing);
                        return;
                    }

                    $(this).dialog("close");

                    callBack(_axis);

                }
            },
            {
                text: messages.label_cancel,
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });
    $(div_popup).dialog("open");
}

function CountMaxResults(codemap, criteria, hideDimension) {

    var count = 1;
    $.each(codemap, function (idx, concept) {
        var c = 1;

        if (idx == "TIME_PERIOD") {


                var codes = $.map(codemap[idx].codes, function (el) { return el.name; });

                if (!criteria.hasOwnProperty(idx)) {
                    criteria[idx] = [];
                    criteria[idx].push(codes[0]);
                    criteria[idx].push(codes[codes.length - 1]);
                }

                if (criteria[idx].length == 1)
                    c = criteria[idx][0];
                else c = (inArray(criteria[idx][1], codes) - inArray(criteria[idx][0], codes)) + 1;
            
            if (c < 1) c = 1;

        } else if (criteria[idx] == undefined || criteria[idx].length == 0) {
            
            if (hideDimension != undefined && inArray(idx, hideDimension) > 0)
                c = 1;
            else {
                if (codemap[idx] != null) {
                    var codes = $.map(codemap[idx].codes, function (el) { return el; });
                    c = codes.length;
                } else {
                    c = 1;
                }
            }
            
        }else if (criteria[idx].length > 0)
            c=criteria[idx].length;

        count *= c;

    });

    return count;

};

