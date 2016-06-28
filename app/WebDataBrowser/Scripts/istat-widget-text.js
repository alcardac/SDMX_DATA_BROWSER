// widget base
function WidgetText(options) {

    var settings = {
        widget: {
            manager: options.managerWidgets,
            classCSS: options.classCSS,
            template: options.template,
            endpoint: options.endpoint,
            target: options.target,
            configuration: options.configuration,
            data: options.data,
            locale: options.locale,
            messages: options.messages,

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

            if (settings.widget.data == undefined) return;

            this.Bind(settings.widget.data);

        } catch (e) {
            return null;
        }

    };
    this.Bind = function (data) {

        settings.$items.container.$getItem().empty();

        var title = data.text[0].title;
        for (i = 0; i < data.text.length; i++)
            if (data.text[i].locale == settings.widget.locale)
                if (data.text[i].title != "")
                    title = data.text[i].title;

        var content = data.text[0].content;
        for (i = 0; i < data.text.length; i++)
            if (data.text[i].locale == settings.widget.locale)
                if (data.text[i].content != "")
                    content = data.text[i].content;

        var h3 = document.createElement('h3');
        $(h3).html(title);

        var div = document.createElement('div');
        $(div).html(content);

        $(h3).appendTo(settings.$items.container.$getItem());
        $(div).appendTo(settings.$items.container.$getItem());

    }
};
