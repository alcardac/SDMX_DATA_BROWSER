function WidgetDataflows(options) {
    
    var isOpen = true;
    var automation = false;

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
            }
        },
        $items: {
            container: new clientItem(options.idCSS)
        },
        baseURL:"/",
        events: {
            expandAll: ExpandAll,
            expandCategories: ExpandCategories,
            collapseAll: CollapseAll,
            clickDataflow: OnTreeNodeSelected
        },
        url: {
            GetDataflows: "Main/GetTree",
           
        },
        options:{
        }
    };

    //$.extend(settings.widget, options);

    this.Setup = function () {

        settings.$items.container.$getItem().empty();
        $("<h3><i class='waitInLinee icon-spin6 animate-spin'></i>" + client.main.messages.text_wait + "</h3>").appendTo(settings.$items.container.$getItem());

        if (settings.widget.configuration == undefined) {
            settings.$items.container.$getItem().empty();
            return;
        }
        var _widget = this;

        var configuration = {};

        // if array of 1
        if (settings.widget.configuration.hasOwnProperty('length')) {
            if (sessionStorage.getItem("select-ws") == undefined || sessionStorage.getItem("select-ws") == null)
            {
                configuration = { configuration: settings.widget.configuration[0] };
            }
            else
            {
                for (var i = 0, len = settings.widget.configuration.length; i < len; i++) {
                    if (settings.widget.configuration[i].Title == sessionStorage.getItem("select-ws"))
                        configuration = { configuration: settings.widget.configuration[i] };
                }
                //alert(configuration);
            }
        } else {
            // if no array
            configuration = { configuration: settings.widget.configuration };
        }

        // if more ws
        if (settings.widget.configuration.length > 1) {
            if (settings.widget.data != undefined){
                
                var menu = settings.widget.data.ContainerMenuSelect;
                var selectedItem = settings.widget.data.CurrentSelectItem;

                // for combo box
                DrawHTML_Ws(
                    menu,
                    settings.widget.configuration,
                    //selectedItem,
                    sessionStorage.getItem("select-ws"), 
                    function (config) {
                        var configuration = { configuration: config };
                        _widget.Bind(configuration);
                    },
                    settings.widget.target);
            }
        }
        _widget.Bind(configuration);

        return;

    };

    this.Bind = function (data) {        
        var dest = settings.$items.container.$getItem();

        var conf = {
            configuration: {
                DecimalSeparator: data.configuration.DecimalSeparator,
                Active: false,
                EndPoint: data.configuration.EndPoint,
                EndPointType: data.configuration.EndPointType,
                EndPointV20: data.configuration.EndPointV20,
                EndPointSource: data.configuration.EndPointSource,
                Domain: data.configuration.Domain,
                EnableHTTPAuthentication: data.configuration.EnableHTTPAuthentication,
                EnableProxy: data.configuration.EnableProxy,
                Password: data.configuration.Password,
                Prefix: data.configuration.Prefix,
                ProxyPassword: data.configuration.ProxyPassword,
                ProxyServer: data.configuration.ProxyServer,
                ProxyServerPort: data.configuration.ProxyServerPort,
                ProxyUserName: data.configuration.ProxyUserName,
                UseSystemProxy: data.configuration.UseSystemProxy,
                UserName: data.configuration.UserName,
                Wsdl: data.configuration.Wsdl,
                UseUncategorysed: data.configuration.UseUncategorysed,
                UseVirtualDf: data.configuration.UseVirtualDf
            }
        };


        LoadJsTree(
            dest,
            settings.url.GetDataflows,
            conf,
            settings.widget.data.query,
            settings.widget.target,null);
        
    }
};

function DrawHTML_ButtonControl(dest) {

    $(dest).empty();

    var val_Expand = "<i class='icon-angle-circled-down'></i> " + client.main.messages.label_expand_all;
    var val_Close = "<i class='icon-angle-circled-up'></i> " + client.main.messages.label_collapse_all;
    var val_ExpandCat = "<i class='icon-angle-circled-down'></i> " + client.main.messages.label_expand_categories;
    var val_CloseCat = "<i class='icon-angle-circled-up'></i> " + client.main.messages.label_collapse_categories;

    var jstree_btn_container = document.createElement("div");
    var btn_expandAll = document.createElement("a");
    $(btn_expandAll).addClass('tree-button');
    $(btn_expandAll).html(val_Expand);
    $(btn_expandAll).appendTo(jstree_btn_container);

    var btn_expandCategories = document.createElement("a");
    $(btn_expandCategories).addClass('tree-button');
    $(btn_expandCategories).html(val_ExpandCat);
    $(btn_expandCategories).appendTo(jstree_btn_container);

    $(jstree_btn_container).appendTo(dest);

    $(btn_expandCategories).bind("click", function () {
        clientShowWaitDialog();

        ExpandCategories('#' + $(dest).attr('id'));

        clientCloseWaitDialog();

    });
    $(btn_expandAll).bind("click", function () {

        clientShowWaitDialog();

        isOpen = $(btn_expandAll).data('isOpen');

        if (isOpen == undefined || isOpen == false) {

            ExpandAll('#' + $(dest).attr('id'));
            $(btn_expandAll).html(val_Close);
            $(btn_expandAll).data('isOpen', true);

        } else {
            CollapseAll('#' + $(dest).attr('id'));
            $(btn_expandAll).html(val_Expand);
            $(btn_expandAll).data('isOpen', false);

        }

        clientCloseWaitDialog();

    });
}
function DrawHTML_Ws(dest, configurations, selectedItem, callBack,target) {

    var menuSelect = document.createElement('select');
    $(menuSelect).attr('name', 'select-ws');
    $(menuSelect).attr('id', 'select-ws');

//    alert(selectedItem);

    
    $.each(configurations, function (idx, config) {
        var attrSelect = '';
        if (selectedItem == config.Title) { attrSelect = 'selected="selected"'; }
        var _value = clientParseObjectToJson(config);
        $(menuSelect).append("<option value='" + _value + "' " + attrSelect + ">" + config.Title + "</option>");
    });
    $(menuSelect).on("selectmenuchange", function (event, ui) {

        sessionStorage.setItem("select-ws", $("#select-ws option:selected").text());

        $("#main-dashboard").empty();

        if (target != undefined)
            $(target).empty();


        callBack(clientParseJsonToObject($(this).val()));
    });
    $(menuSelect).appendTo($(dest));
    $(menuSelect).selectmenu();
}

// Private method
function ExpandAll(container) {

    jstree_container = $(container).children(".jstree");
    jstree_container.jstree('open_all');

}
function CollapseAll(container) {

    var jstree_container = $(container).children(".jstree");
    jstree_container.jstree('close_all');

}
function ExpandCategories(jstree_container) {

    $(jstree_container).find("li").each(
        function (index) {
            var category = $(this);
            var children = category.find("li[rel='category']");
            if (children.length > 0) {
                $(jstree_container).jstree('open_node', category);
            }
            else {
                var selectedDataflow = category.find("ul > li > a.jstree-clicked");
                if (selectedDataflow.length == 0) {
                    $(jstree_container).jstree('close_node', category);
                }
            }
        }
    );

}
function ExpandFirstCategories(jstree_container) {


    $(jstree_container).find("li[rel='category-scheme']").each(
        function (index) {
            var category = $(this);
            $(jstree_container).jstree('open_node', category);
        }
    );

    //if selector is defined open default on change language
    if (sessionStorage.getItem("select-jstree") != null && sessionStorage.getItem("select-jstree") != undefined)
    { $(jstree_container).jstree('select_node', sessionStorage.getItem("select-jstree")); }

}

function LoadJsTree(dest, url, data, query, target, callBack) {

    dest.empty();
    

    var conf = {
        configuration: {
            Title: data.configuration.Title,
            DecimalSeparator: data.configuration.DecimalSeparator,
            EndPoint: data.configuration.EndPoint,
            EndPointV20: data.configuration.EndPointV20,
            EndPointType: data.configuration.EndPointType,
            EndPointSource: data.configuration.EndPointSource,
            UseUncategorysed: data.configuration.UseUncategorysed,
            UseVirtualDf: data.configuration.UseVirtualDf
        }
    };
    clientPostJSON(
        url, clientParseObjectToJson(conf),
        function (jsonString) {

            var jsonData = clientParseJsonToObject(jsonString);

            if (jsonData != null
                && !jsonData.hasOwnProperty('error')) {

                DrawHTML_ButtonControl(dest);

                SetupJsTree(dest, jsonData, query, target, callBack);

            } else {
                dest.empty();
                var error_report = document.createElement("p");
                $(error_report).append('<i class="icon-attention-1"></i>');
                $(error_report).append(client.main.messages.label_error_dataParsing);
                $(error_report).appendTo(dest);
            }
        },
        function (event, status, errorThrown) {
            errorThrown = 'SetupJsonTree';
            clientAjaxError(event, status, errorThrown);
            return;
        },
        true
    );
    
}
function SetupJsTree(dest, data, query, target, callBack) {
   

    var options = {
        "plugins": ["themes", "types", "ui"], //"state",
        "themes": {
            "theme": "default",
            "dots": false,
            "icons": false
        },
        "core": {
            "data": data,
            'multiple': false,
            "progressive_render": true
        },
        "ui": {
            "select_limit": 1,
           // "selected_parent_close": false
        },
        "types": {
            "category-scheme": {
                "icon": false,//"icon-database-1" ,
                //"select_node": OnCategoryTreeNodeClicked
            },
            "category": {
                "icon": false,//"icon-folder" ,
                //"select_node": OnCategoryTreeNodeClicked
            },
            "dataflow": {
                "icon":"icon-table",
                //"select_node": OnTreeNodeSelected,
                "leaf": true
            },
            "xs-dataflow": {
                "icon": "icon-table",
                //"select_node": OnTreeNodeSelected,
                "leaf": true
            },
            "virtual-dataflow": {
                "icon": "icon-table",
                //"select_node": OnTreeNodeSelected,
                "leaf": true
            },
            "default": { "select_node": false }
        }
    };

    var jstree_container = document.createElement("div");

    $(jstree_container).data('target', target);
    $(jstree_container).data('callBack', callBack);
    
    $(jstree_container)
    .on('activate_node.jstree', function (e, data) {
        data.instance.toggle_node(data.node);
        e.preventDefault();
        e.stopImmediatePropagation();
    })
    .on('loaded.jstree', function (e, data) {

        //$(jstree_container).children("li > i.jstree-icon.jstree-ocl").remove();
        //-------Query select df
        if (query != undefined) {

            var id = query.dataflow.id;
            var agency = query.dataflow.agency;
            var version = query.dataflow.version.replace('.', '_');

            var selector = "#" + agency + "-" + id + "-" + version;
            $(selector).children('a').addClass('jstree-clicked');
        }

    })
    .on("changed.jstree", function (e, data) {
        if (data.selected.length) {
            //session storage id selector to need open
            sessionStorage.setItem("select-jstree", data.node.id);

            OnTreeNodeSelected(
                jstree_container,
                data.instance.get_node(data.selected[0]),
                $(jstree_container).data('target'),
                $(jstree_container).data('callBack'));

        }
    })
    .on("ready.jstree", function (e, data) {
        ExpandFirstCategories(jstree_container);
    })
    .jstree(options);
    $(jstree_container).appendTo(dest);
}

function OnTreeNodeSelected(jstree_container, anchorElm, target, callBack) {

    if (anchorElm.type == "xs-dataflow"
        || anchorElm.type == "dataflow"
        || anchorElm.type == "virtual-dataflow") {

        var agency = anchorElm.original.a_attr.DataflowAgency;
        var id = anchorElm.original.a_attr.DataflowID;
        var version = anchorElm.original.a_attr.DataflowVersion;
        var name = anchorElm.original.a_attr.DataflowName;

        var configuration = {
            Title: anchorElm.original.a_attr.Title,
            active: true,
            DecimalSeparator: anchorElm.original.a_attr.DataflowDecimalCulture,
            EndPoint: anchorElm.original.a_attr.DataflowUrl,
            EndPointType: anchorElm.original.a_attr.DataflowUrlType,
            EndPointV20: anchorElm.original.a_attr.DataflowUrlV20,
            EndPointSource: anchorElm.original.a_attr.DataflowSource,
            /*Domain: "",
            EnableHTTPAuthentication: false,
            EnableProxy: false,
            Password: "",
            Prefix: "",
            ProxyPassword: "",
            ProxyServer: "",
            ProxyServerPort: "",
            ProxyUserName: "",
            UseSystemProxy: false,
            UserName: "",
            Wsdl: "",
            */
        };

        var ret = false;
        if ((agency.length > 0) && (id.length > 0) && (version.length > 0))
            ret = OnCurrentChanged(agency, id, version, name, configuration, target, callBack);

    }

    return ret;
}
function OnCurrentChanged(_agency, _id, _version,_name, _configuration, target, callBack) {

    try {

        clientPostJSON(
        "Main/ClearSession",null,
        function (jsonString) {

            var jsonData = clientParseJsonToObject(jsonString);

            if (jsonData.hasOwnProperty('success')) {

                var data = {
                    dataflow: { id: _id, agency: _agency, version: _version, name: _name },
                    configuration: _configuration,
                    layout: {
                        axis_x: [],
                        axis_y: [],
                        axis_z: [],
                    },
                    criteria: {}
                }

                var widget_target = client.main.manager.widget.GetWidget(target);
                if (widget_target != undefined)
                    widget_target.Bind(data);

                $('#main-dashboard').hide();

                if (callBack != undefined) {
                    callBack(data);
                }

            } else {
                dest.empty();
                var error_report = document.createElement("p");
                $(error_report).append('<i class="icon-attention-1"></i>');
                $(error_report).append(client.main.messages.label_error_dataParsing);
                $(error_report).appendTo(dest);
            }
        },
        function (event, status, errorThrown) {
            errorThrown = 'SetupJsonTree';
            clientAjaxError(event, status, errorThrown);
            return;
        },
        false);

    } catch (e) {

    }
}

function ShowInfoExtraDF(sender) {

    
    var div = $('#panel_info_extra .content'); //document.createElement('div');
    $(div).empty();

    var desc = clientParseJsonToObject($(sender).parent().find('a').attr('DataflowDesc'));
    $.each(desc, function (idx, el) {
        $(div).append('<p style="float:left; margin-bottom:10px;">' + el + '</p>');
    });

    var urls = clientParseJsonToObject($(sender).parent().find('a').attr('DataflowUrls'));
    $.each(urls, function (idx, el) {
        //$(div).append('<p style="float:left; margin-bottom:10px;">' + el.Title + '</br> <a style="float:left" href="' + el.URL + '"  target="_blank">link</a></p>');
        //fabio 4/11/2015
        $(div).append('<p style="float:left; margin-bottom:10px;"><a href="' + el.URL + '" target="_blank">' + el.Title + '</a></p>');
        //fine fabio 4/11/2015
    });

    $('#panel_info_extra').css('display','block');

    return false;
}

function CloseInfoExtraDF(sender) {

    //var div = $('#panel_info_extra'); //document.createElement('div');
    //$(div).empty();
    $('#panel_info_extra').css('display', 'none');

    return false;

}
