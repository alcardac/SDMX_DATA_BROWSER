<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentHeader" runat="server">

    <script src="<%=Url.Content("~/Scripts/jquery/Concurrent.Thread-full-20090713.js")%>"></script>
    <link href="<%=Url.Content("~/Scripts/jquery/jstree-master/dist/themes/default/style.min.css")%>" rel="stylesheet" />
    <script src="<%=Url.Content("~/Scripts/jquery/jstree-master/dist/jstree.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/jquery/canvasjs-1.6.2/jquery.canvasjs.min.js")%>"></script>

    <script src="<%=Url.Content("~/Scripts/pages/index.js")%>"></script>

    <% if (ViewBag.query != null) {%>
    <script type="text/javascript">

        $(document).ready(function () {
            client.main.manager.widget.query = {
                "#main-treeview": {
                    dataflow: {
                        id: "<%= ViewBag.query.Dataflow.id %>",
                        agency: "<%= ViewBag.query.Dataflow.agency %>",
                        version: "<%= ViewBag.query.Dataflow.version %>"
                    }
                },
                "#main-table-dataset":{
                    dataflow: {
                        id: "<%= ViewBag.query.Dataflow.id %>",
                        agency: "<%= ViewBag.query.Dataflow.agency %>",
                        version: "<%= ViewBag.query.Dataflow.version %>",
                        configuration: {
                            IDNode: "<%= (ViewBag.query.Configuration.IDNode!=null)?ViewBag.query.Configuration.IDNode:"" %>",
                            Locale: "<%= (ViewBag.query.Configuration.Locale!=null)?ViewBag.query.Configuration.Locale:"" %>",
                            Title: "<%= (ViewBag.query.Configuration.Title!=null)?ViewBag.query.Configuration.Title:"" %>",
                            EndPoint: "<%= (ViewBag.query.Configuration.EndPoint!=null)?ViewBag.query.Configuration.EndPoint:"" %>",
                            EndPointType: "<%= (ViewBag.query.Configuration.EndPointType!=null)?ViewBag.query.Configuration.EndPointType:""%>",
                            EndPointV20: "<%= (ViewBag.query.Configuration.EndPointV20!=null)?ViewBag.query.Configuration.EndPointV20:"" %>",
                            EndPointSource: "<%= (ViewBag.query.Configuration.EndPointSource!=null)?ViewBag.query.Configuration.EndPointSource:"" %>",
                            DecimalSeparator: "<%= (ViewBag.query.Configuration.DecimalSeparator!=null)?ViewBag.query.Configuration.DecimalSeparator:"" %>"

                        }
                    },
                    key_time_dimension: "TIME_PERIOD",
                    layout: {
                        axis_x: [
                            <% foreach (var criterio in ViewBag.query.Layout.axis_x)
                               { %>
                            "<%= criterio %>",
                        <% } %>],
                        axis_y: [
                            <% foreach (var criterio in ViewBag.query.Layout.axis_y)
                               { %>
                            "<%= criterio %>",
                        <% } %>],
                        axis_z: [
                            <% foreach (var criterio in ViewBag.query.Layout.axis_z)
                               { %>
                            "<%= criterio %>",
                        <% } %>],
                    },
                    criteria: {
                        <% foreach (var criterio in ViewBag.query.Criteria){ %>
                            "<%= criterio.Key %>": [
                            <% foreach (var value in criterio.Value){ %>
                                "<%= value %>",
                            <% } %>
                            ],
                        <% } %>
                    }
                }
            };
        });
    </script>
    <% } %>

    <script type="text/javascript">
        criteriaMode="<%=ConfigurationManager.AppSettings["GuestCriteriaMode"].ToString() %>";
    </script>
    
</asp:Content>

<asp:Content ID="MainContainer" ContentPlaceHolderID="MainContainer" runat="server">
    
    <div id="split-left">

        <!-- tree view -->
        <div id="main-treeview" class="dinamic-widget"
            data-widget-template="treeview"
            data-widget-stylecss="treeview-dataflows"
            data-widget-data='{"ContainerMenuSelect":"#menu-left","CurrentSelectItem":"avana2012"}'
            data-widget-target="#main-table-dataset">
        </div>
        
    </div>

    <div id="split-toggler">
        <div id="close-left" class="close-left">
            <i class=" icon-left-dir"></i>
        </div>
    </div>
        
    <div id="split-right">
        
        <!-- dashboard -->
        <div id="main-dashboard" class="dinamic-widget"
            data-widget-template="dashboard"
            data-widget-stylecss="dashboard-style">
        </div>
        <!-- table -->
        <div id="main-table-dataset" class="dinamic-widget"
            data-widget-template="table"
            data-widget-stylecss="table-dataset">
        </div>
        
    </div>
    
    <div id="panel_info_extra">
        <div id="panel_info_extra_toggler">
            <div id="panel_info_extra_btn" class="close-right"  onclick="CloseInfoExtraDF(this);">
                <i class=" icon-right-dir"></i>
            </div>
        </div>
        <div class="content"></div>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
