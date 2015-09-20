<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>
<%@ Import Namespace="System.Globalization" %>
<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=Url.Content("~/Scripts/istat-widget-manager.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/istat-widget-dataset.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/istat-client.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/pages/redirectLogin.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/pages/dashboardElements.js")%>"></script>
    <link href="<%=Url.Content("~/Scripts/jquery/jstree-master/dist/themes/default/style.min.css")%>" rel="stylesheet" />
    <script src="<%=Url.Content("~/Scripts/jquery/jstree-master/dist/jstree.js")%>"></script>
    <link href="<%=Url.Content("~/Scripts/jquery/jQuery-TE_v.1.4.0/jquery-te-1.4.0.css")%>" rel="stylesheet" />
    <script src="<%=Url.Content("~/Scripts/jquery/jQuery-TE_v.1.4.0/jquery-te-1.4.0.min.js")%>"></script>

    <link href="<%=Url.Content("~/Content/style/widgets/UserList.css")%>" rel="stylesheet" />
    <link href="<%=Url.Content("~/Content/style/widgets/DashboardsManag.css")%>" rel="stylesheet" />
    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
        };
    </script>
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-desktop-2"></i> <%=Messages.label_dashManagement %></h1>
    <!-- HTML Widget START -->
    <div id="div_Dashboard" class="Container">
    </div>
    <!-- HTML Widget END -->
</asp:Content>
 
<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">

    <script type="text/javascript">
        selectedLang = '<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName%>';
        systemLang = { <% foreach(CultureInfo c in Messages.AvailableLocales()){ %> "<%=c.TwoLetterISOLanguageName%>":"<%=c.DisplayName%>", <% } %> };
        
        $.each(systemLang, function (value, display) { objLang[value] = ""; });
        objTitle = { title: objLang, };
        objContent = { content: objLang, };

        maxNumObservation=<%=ISTAT.WebClient.WidgetComplements.Model.Settings.WebClientSettings.Instance.MaxResultObs %>;

        //var dashboards;
        _endPoint = "<%=ConfigurationManager.AppSettings["SingleSignOnUrl"].ToString() %>";
        
        elemnPaging = <%=ConfigurationManager.AppSettings["ElementInList"].ToString() %>;
        
        criteriaMode="<%=ConfigurationManager.AppSettings["AdminCriteriaMode"].ToString() %>";

    </script>

</asp:Content>