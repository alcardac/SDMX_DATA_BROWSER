/* Classe per descrivere i contenitori delle pagine 
* @id ID Css dell'elemento
*/
function clientItem(id) {
    this.$id = null; // The html id prefixed with '#'. To use inside Jquery $(...$id) 
    this.id = null; // The html id
    this.$item = null; // The jquery function $($id). Use $getItem getter 
    // reset the Jquery item
    this.reset = function () {
        this.$item = null;
    };
    // get the id and reset the getItem
    this.getId = function () {
        this.reset();
        //        console.log("Setting ID : " + this.id);
        return this.id;
    };
    // set the ID 
    this.setId = function (id) {
        this.id = id;
        this.$id = "#" + id;
    };
    // get the Jquery function for this id. 
    this.$getItem = function () {
        if (this.$item == null) {
            this.$item = $(this.$id);
        }
        return this.$item;
    };
    this.setId(id);
}
/* Oggetto comune nelle pagine */

var client = {
    main: {
        config: {
            baseURL: location.protocol + "//" + location.hostname + ":" + location.port + "/" + location.pathname.substring(1, location.pathname.indexOf("/", 2) + 1),
            locale: "en"
        },
        events: {
            ajaxStart: clientShowWaitDialog,
            ajaxStop: clientCloseWaitDialog,
            ajaxError: clientAjaxError,
        },
        manager:{
            widget: new WidgetManager()
        },
        maxObs:10000,
        messages:{
        },
        url:{
            getMessages:"Settings/GetMessages" 
        }
    }
};

function clientSetup(url,locale,maxObs) {

    if (url != undefined) client.main.config.baseURL = url;

    client.main.maxObs = maxObs;
    client.main.config.locale = locale;

    clientRetrieveMessages();

}

function clientRetrieveMessages() {
    clientPostJSON(
        client.main.url.getMessages,
        null,
        function (jsonString) {

            client.main.messages = clientParseJsonToObject(jsonString);
            client.main.manager.widget.SetupWidgets($(".dinamic-widget"),client.main.config.locale, client.main.messages);

        },
        null,
        true);
}

function clientShowWaitDialog(msg) {
    $.unblockUI({ fadeOut: 0 });
    $.blockUI({
        message: "<i class='icon-spin6 animate-spin'></i>" + ((msg != undefined) ? msg : (client.main.messages.text_wait != undefined)?client.main.messages.text_wait:""),
        fadeIn: 0,
        fadeOut: 100,
        showOverlay: true,
        css: { border: 0 }
    });
}
function clientCloseWaitDialog() {
    $.unblockUI();
}

function clientShowErrorDialog(msg) {

    client.main.events.ajaxStop();

    var msg_container = document.createElement("div");

    $(msg_container).html((msg != undefined) ? msg : client.main.messages.error_ajax_generic);

    $(msg_container).dialog({
        title: "Error",//client.main.messages.error_ajax_generic,
        resizable: false,
        //width: 360,
        //height: 240,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}

function clientParseJsonToObject(json) {
    
    try {
        var obj = null;
        if (json == null) return null;
        if (typeof (json.d) !== 'undefined' && json.d != null && json.d) {
            obj = $.parseJSON(json.d);
        }
        else {
            obj = $.parseJSON(json);
        }
        return obj;
    } catch (ex) { return null; }
   
}
function clientParseObjectToJson(obj) {
    var json = JSON.stringify(obj);
    return json;
}
function clientStringArraytoJSON(a) {
    var json = "[";
    for (var i = 0, j = a.length; i < j; i++) {
        if (i > 0) {
            json = json + ",";
        }
        json = json + "'" + a[i] + "'";
    }
    json = json + "]";
    return json;
}

function clientPostJSON(url, data, callback, errorCallback,useWait) {
    if (useWait) client.main.events.ajaxStart();

    if (!errorCallback) {
        errorCallback = clientAjaxError;
    }

    $.ajax({
        type: "POST",
        url: client.main.config.baseURL + url,
        async: true,
        timeout: 400000,
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (jsonString) {

            var res = clientParseJsonToObject(jsonString);

            if (res != null && res.hasOwnProperty('Msg')) {

                clientShowErrorDialog(res.Msg);

            } else if (callback) callback(jsonString);

            if (useWait) client.main.events.ajaxStop();
        },
        error: errorCallback
    });
}

function clientAjaxError(event, req, options, error) {
    clientShowErrorDialog(error);
}

function clientGetTemplate(tpl) {

    if (tpl != null) {
        return client.main.manager.template.getTemplate(tpl);
    }
    return false;
}
function clientLoadTemplate(tpl, data, dest){
    dest.empty();
    $.tmpl(clientGetTemplate(tpl), data).appendTo(dest);
}

function inArray(elem, array) {
    if (array == null) return -1;
    if (array == undefined) return -1;
    var len = array.length;
    for (var i = 0 ; i < len; i++) {
        if (array[i] == elem) { return i; }
    }
    return -1;
}

function GetLocalisedText(texts, locale) {
    var title = undefined;
    if (texts != undefined) {
        for (var l_idx = 0; l_idx < texts.length; l_idx++) {
            title = texts[l_idx].title;
            if (texts[l_idx].locale == locale
                && texts[l_idx].title != undefined
                && texts[l_idx].title != "")
                break;
        }
    }
    return title;
}