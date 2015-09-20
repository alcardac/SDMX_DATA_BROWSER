<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script type="text/javascript" src="<%=Url.Content("~/Scripts/jquery/canvasjs-1.6.2/jquery.canvasjs.min.js")%>"></script>
    <script type="text/javascript" src="<%=Url.Content("~/Scripts/pages/dashboard.js")%>"></script>

    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
    };
    </script>

</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-cogs"></i> <%= Messages.footer_settings %></h1>
    <div class="D1" id="D1">
        <div class="D-Left">
            <div id="D1-Container-left" class="div-container" title="Basic dialog">
                <div id="D1-chart-left"></div>
            </div>
        </div>
        <!-- SPLIT VERTICAL -->
        <div class="D-Right">
            <div id="D1-Container-right" class="div-container" title="Basic dialog">
                <div id="D1-chart-right"></div>
            </div>
        </div>
        <div class="clear-box"></div>
    </div>
    <!-- SPLIT HORIZZONTAL -->
    <div class="D2" id="D2">
        <div class="D-Left">
            <div id="D2-Container-left" class="div-container" title="Basic dialog">
                <div id="D2-chart-left"></div>
            </div>
        </div>
        <!-- SPLIT VERTICAL -->
        <div class="D-Right">
            <div id="D2-Container-right" class="div-container" title="Basic informations">
                <div class="div-heading"><%=Messages.footer_settings %></div>
                <!-- Vertical List -->
                <%--<ul id="info-voices">    
                    <li>
                        <a href="#" class="info-link">
                            <i class="icon-user profile"></i>
                        </a>
                        <span class="info-description">Profile</span>
                    </li>

                    <li>
                        <a href="#" class="info-link">
                            <i class="icon-doc-text-inv queries"></i>
                        </a>
                        <span class="info-description">Queries</span>
                    </li>

                    <li>
                        <a href="#" class="info-link">
                            <i class="icon-users users"></i>
                        </a>
                        <span class="info-description">Registered Users</span>
                    </li>

                    <li>
                        <a href="#" class="info-link">
                            <i class="icon-chart-line visits"></i>
                        </a>
                        <span class="info-description">Visits</span>
                    </li>

                    <li>
                        <a href="#" class="info-link">
                            <i class="icon-info-circled infos"></i>
                        </a>
                        <span class="info-description">Informations</span>
                    </li>
                </ul>--%>

                <div class="dash-info">
                    <div class="dash-info-box">
                        <div class="dash-info-box-title">
                            <%=Messages.dashboard_Profile %>
                        </div>
                        <div class="dash-info-box-icon">
                            <a href="<%=Url.Content("~/WebClient/Profile")%>" class="info-link">
                                <i class="icon-info-4 profile"></i>
                            </a>
                        </div>
                    </div>
                    <div class="dash-info-box">
                        <div class="dash-info-box-title">
                            <%=Messages.dashboard_Queries %>
                        </div>
                        <div class="dash-info-box-icon">
                            <a href="<%=Url.Content("~/WebClient/Queries")%>" class="info-link">
                                <i class="icon-doc-circled queries"></i>
                            </a>
                        </div>
                    </div>
                    <div class="clear-box"></div>
                </div>
                <div class="dash-info">
                    <div class="dash-info-box">
                        <div class="dash-info-box-title">
                            <%=Messages.dashboard_Themes %>
                        </div>
                        <div class="dash-info-box-icon">
                            <a href="<%=Url.Content("~/WebClient/Templates")%>" class="info-link">
                                <i class="icon-photo-circled"></i>
                            </a>
                        </div>
                    </div>
                    <div class="dash-info-box" id="home-page-setting">
                        <div class="dash-info-box-title">
                            <%=Messages.dashboard_Profiles %>
                        </div>
                        <div class="dash-info-box-icon">
                            <a href="<%=Url.Content("~/WebClient/Administration")%>" class="info-link">
                                <i class="icon-wrench-circled home"></i>
                            </a>
                        </div>
                    </div>
                    <div class="clear-box"></div>
                </div>

            </div>

        </div>
        <div class="clear-box"></div>
    </div>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
