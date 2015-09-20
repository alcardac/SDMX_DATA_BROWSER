<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
        <script type="text/javascript">
            // redirect to login page if access denied
            if (sessionStorage.user_code == null) {
                window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
        };
    </script>
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-wrench-2"></i> <%= Messages.label_dashAdministration %></h1>
    <div class="administrationContainer">
        <div class="dash-info">
            <div class="dash-info-box">
                <div class="dash-info-box-title">
                    <%=Messages.label_dashProfiles%>
                </div>

                <div class="dash-info-box-icon">
                    <a href="<%=Url.Content("~/WebClient/Profiles")%>" class="info-link">
                        <i class="icon-group-circled profile"></i>
                    </a>
                </div>
            </div>
            <div class="dash-info-box">
                <div class="dash-info-box-title">
                    <%=Messages.label_dashManagement %>
                </div>
                <div class="dash-info-box-icon">
                    <a href="<%=Url.Content("~/WebClient/DashboardsManagement")%>" class="info-link">
                        <i class="icon-desktop-circled visits"></i>
                    </a>
                </div>
            </div>
            <div class="clear-box"></div>
        </div>
        <div class="dash-info">
            <div class="dash-info-box">
                <div class="dash-info-box-title">
                    <%=Messages.label_dashLayout %>
                </div>
                <div class="dash-info-box-icon">
                    <a href="<%=Url.Content("~/WebClient/Layout")%>" class="info-link">
                        <i class="icon-website-circled home"></i>
                    </a>
                </div>
            </div>
            <div class="dash-info-box">
                <div class="dash-info-box-title">
                    <%=Messages.dashboard_ManagementTemplate %>
                </div>
                <div class="dash-info-box-icon">
                    <a href="<%=Url.Content("~/WebClient/TemplatesManagement")%>" class="info-link">
                        <i class="icon-edit-circled queries"></i>
                    </a>
                </div>
            </div>
            <div class="clear-box"></div>
            <div class="dash-info-box" id="home-page-setting">
            </div>
            <div class="clear-box"></div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
