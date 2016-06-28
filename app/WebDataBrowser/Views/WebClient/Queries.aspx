<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>
    
<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=ResolveClientUrl("~/Scripts/istat-widget-manager.js")%>"></script>
    <script src="<%=ResolveClientUrl("~/Scripts/istat-widget-dataset.js")%>"></script>
    <script src="<%=ResolveClientUrl("~/Scripts/istat-client.js")%>"></script>
    <script src="<%=ResolveClientUrl("~/Scripts/pages/dashboardElements.js")%>"></script>
    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=ResolveClientUrl("~/")%>WebClient/Login";
        };
    </script>
    <link href="<%=ResolveClientUrl("~/Content/style/query/List.css")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-doc-text-inv"></i><%= Messages.dashboard_Queries%></h1>
    <div id="queryContainer" class="queryContainer">
    </div>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript">

        var curIdxPaging = 0;
        var elemnPaging = <%=ConfigurationManager.AppSettings["ElementInList"].ToString() %>;
        //var endpoints = [];

        function removeQuery(queryId) {
            var div_dialog = document.createElement("div");
            $(div_dialog).attr("title",'<%= Messages.label_queryDelete %>');
            $(div_dialog).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.text_confirmDelQuery %></p>");
            $(div_dialog).dialog({
                resizable: false,
                height: 140,
                position: { my: "center", at: "center", of: window },
                modal: true,
                buttons: {
                    '<%=Messages.label_save%>': function () {
                        var dati = '{ "QueryId":"' + queryId + '"}';
                        var urlRemoveQuery = "Query/Del";
                        clientPostJSON(urlRemoveQuery, dati,
                                function (jsonString) {
                                    var div_response = document.createElement("div");
                                    $(div_response).attr("title", '<%= Messages.label_queryDelete %>');
                                    $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.text_remove_query %></p>");
                                    $(div_response).dialog({
                                        resizable: false,
                                        height: 140,
                                        position: { my: "center", at: "center", of: window },
                                        modal: true,
                                        buttons: {
                                            'Ok': function () {
                                                $(this).dialog("close");
                                                location.reload(true);
                                            }
                                        }
                                    });
                        },
                        function (event, status, errorThrown) {
                            clientShowErrorDialog('<%= Messages.label_error_data %>');
                            //clientAjaxError(event, status);
                            return;
                        },
                        false);
                        $(this).dialog("close");
                    },
                    '<%=Messages.label_cancel%>': function () {
                        $(this).dialog("close");
                    }
                }
            });
        }

        function openQuery(queryId,code) {
            location.href = "<%=ResolveClientUrl("~/WebClient/Index")%>?QueryId=" + queryId + "&UserCode=" + code;
        }

        function DrawHTML_ListQueries(dest) {
            $(dest).empty();

            //var endpoint = "";
            var lista_vuota = '<%= Messages.label_queryList_empty %>';

            try {
                var dati = '{ "UserCode":"' + sessionStorage.user_code + '"}';
                var urlGetQuery = "Query/Get";
                clientPostJSON(urlGetQuery, dati,
                        function (jsonString) {
                            var query = clientParseJsonToObject(jsonString);
                            
                            if (query == "") {
                                $(dest).append("<div class='query'><div class='query_title' >" + '<%= Messages.emptylistQuery %>' + "</div></div>");
                            } else {
                                var div_pag = DrawHTML_Pagging(
                                    curIdxPaging,
                                    query.length,
                                    elemnPaging,
                                    function (idx) {
                                        curIdxPaging = idx;
                                        DrawHTML_ListQueries(dest);
                                    }
                                );

                                $(div_pag).appendTo(dest);
                                $(div_pag).buttonset();

                                var pagFrom = (curIdxPaging * elemnPaging);
                                var pagTo = pagFrom + elemnPaging;
                                var idxPag = -1;
                               
                                //endpoints = _sett_obj.endpoints;

                                

                                var i = 0;
                                $.each(query, function () {
                                    var hideDiv = false;
                                    idxPag++;
                                    if (!(idxPag < pagFrom || idxPag > (pagTo - 1))) {
                                        strjson = JSON.stringify(this);
                                        var query_id = this.QueryId;

                                        var div = document.createElement('div');
                                        $(div).addClass('query');
                                        $(div).append("<div class='query_title'>" + this.Title + "</div><div class='buttons'><span class='button'><i class='icon-upload-2'></i><input class='button' title='execute query' type='button' id='id_button_O_" + i + "'/></span><span class='button'><i class='icon-trash'></i><input class='button' title='delete query' type='button' id='id_button_R_" + i + "'/></span></div></div>");
                                        $(dest).append(div);

                                        //$(dest).append("<div class='query'><div class='query_title'>" + this.Title + "</div><div class='buttons'><span class='button'><i class='icon-upload-2'></i><input class='button' title='execute query' type='button' id='id_button_O_" + i + "'/></span><span class='button'><i class='icon-trash'></i><input class='button' title='delete query' type='button' id='id_button_R_" + i + "'/></span></div></div></div>");
                                        $("#id_button_O_" + i).val('<%= Messages.label_open_query %>');
                                        $("#id_button_R_" + i).val('<%= Messages.label_remove_query %>');

                                        $("#id_button_O_" + i).click(function () {
                                            openQuery(query_id, sessionStorage.user_code);
                                        });

                                        $("#id_button_R_" + i).click(function () {
                                            removeQuery(query_id);
                                        });
                                    } 
                                    i++;

                                });
                                
                            }

                        },
                        function (event, status, errorThrown) {
                            clientShowErrorDialog('<%= Messages.label_error_data %>');
                            //clientAjaxError(event, status);
                            return;
                        },
                        false);

            }
            catch (ex) {

            }

        }

        jQuery(document).ready(function () {
            DrawHTML_ListQueries("#queryContainer");

            
        });
    </script>
</asp:Content>
