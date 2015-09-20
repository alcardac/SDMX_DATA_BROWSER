<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=Url.Content("~/Scripts/pages/dashboardElements.js")%>"></script>
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-users"></i> <%= Messages.dashboard_RegUsers + Messages.dashboard_menu%></h1>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
