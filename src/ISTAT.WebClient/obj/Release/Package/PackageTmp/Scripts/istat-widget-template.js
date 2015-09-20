// widget base
function widget_template(options) {

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

    };
    this.Bind = function (data) {

    }
};