<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=Url.Content("~/Scripts/pages/dashboardElements.js")%>"></script>
    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
        };
    </script>
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-chart-line"></i> <%= Messages.dashboard_Statistics%></h1>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
