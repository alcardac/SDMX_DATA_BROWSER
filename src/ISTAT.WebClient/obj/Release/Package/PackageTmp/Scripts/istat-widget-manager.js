function WidgetManager() {

    var widgets = new Array();

    this.query = undefined;

    this.SetupWidgets = function (selector, locale, messages) {

        var q=client.main.manager.widget.query;

        var _manager = this;

        var _messages = messages;
        var _locale = locale;

        $.each(selector, function (i, widget) {

            var _idCSS=$(widget).attr('id');
            var _classCSS = $(widget).data('widgetStylecss');
            var _template = $(widget).data('widgetTemplate');
            var _target = $(widget).data('widgetTarget');
            var _data = $(widget).data('widgetData');
            var _configuration = $(widget).data('widgetConfiguration');

            if (q != undefined && q["#" + _idCSS] != undefined) {
                if (_data == undefined)_data = {};
                _data.query = q["#" + _idCSS];
            }

            switch (_template){
                case "dashboard":
                    widgets["#" + _idCSS] =
                        new WidgetDashBoard({
                            managerWidgets: _manager,
                            idCSS: _idCSS,
                            classCSS: _classCSS,
                            template: _template,
                            target: _target,
                            configuration: _configuration,
                            messages: _messages,
                            locale: _locale,
                            data: _data
                        });
                    if (q == undefined) widgets["#" + _idCSS].Setup();
                    break;
                case "treeview":
                    widgets["#" + _idCSS] =
                        new WidgetDataflows({
                            managerWidgets: _manager,
                            idCSS: _idCSS,
                            classCSS: _classCSS,
                            template: _template,
                            target: _target,
                            configuration: _configuration,
                            messages: _messages,
                            locale: _locale,
                            data: _data
                        });
                    widgets["#" + _idCSS].Setup();
                    break;
                case "table":
                    widgets["#" + _idCSS] =
                        new WidgetDataset({
                            managerWidgets: _manager,
                            idCSS: _idCSS,
                            classCSS: _classCSS,
                            template: _template,
                            target: _target,
                            configuration: _configuration,
                            messages: _messages,
                            locale: _locale,
                            data: _data,
                        });
                    widgets["#" + _idCSS].Setup();
                    break;
                case "chart":
                    widgets["#" + _idCSS] =
                        new WidgetChart({
                            managerWidgets: _manager,
                            idCSS: _idCSS,
                            classCSS: _classCSS,
                            template: _template,
                            target: _target,
                            configuration: _configuration,
                            messages: _messages,
                            locale: _locale,
                            data: _data
                        });
                    widgets["#" + _idCSS].Setup();
                    break;
                case "text":
                    widgets["#" + _idCSS] =
                        new WidgetText({
                            managerWidgets: _manager,
                            idCSS: _idCSS,
                            classCSS: _classCSS,
                            template: _template,
                            target: _target,
                            configuration: _configuration,
                            messages: _messages,
                            locale:_locale,
                            data: _data
                        });
                    widgets["#" + _idCSS].Setup();
                    break; 
            }
             
        });
    }

    this.GetWidget = function (id) {
        try{
            return  widgets[id];
        }catch(e){
            return undefined;
        }
    }

}