<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=ResolveClientUrl("~/Scripts/pages/dashboardElements.js")%>"></script>
    <link href="<%=ResolveClientUrl("~/Content/style/widgets/Login.css")%>" rel="stylesheet" />
    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=ResolveClientUrl("~/")%>WebClient/Login";
        };
    </script>
</asp:Content>
      
<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-user-3"></i> <%= Messages.dashboard_Profile%></h1>
    <div id="userProfile">
           
    </div>
    <script>
        var code = sessionStorage.user_code;
        var _endPoint = "<%=ConfigurationManager.AppSettings["SingleSignOnUrl"].ToString() %>";
        function OnSuccessLogin(msg) {
            alert("dal client cancellazione effettuata :" + msg);
        }

        function OnSuccessLogin_changeUser(msg) {
            var div_dialog = document.createElement("div");
            $(div_dialog).attr("title", '<%= Messages.dashboard_Profile%>');
            $(div_dialog).html("<p><span class='ui-icon ui-icon-circle-check' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_changeSuccess%></p>");
            $(div_dialog).dialog({
                resizable: false,
                height: 140,
                position: { my: "center", at: "center", of: window },
                modal: true,
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                }
            }
            });
        }
        function OnSuccessLogin_changePsw(msg) {
            alert("dal client modifiche  effettuate correttamente");
        }
        function OnFailLogin(msg) {
            var div_fail = document.createElement("div");
            $(div_fail).attr("title", '<%= Messages.label_loginGenError %>');
            $(div_fail).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + msg + "</p>");
            $(div_fail).dialog({
                resizable: false,
                height: 140,
                position: { my: "center", at: "center", of: window },
                modal: true,
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                    }
                }
            });
        }
        $("#userProfile").load(_endPoint + "/Widget/widgetChangeUser.html", null, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {
                var data = {
                    endpoint: _endPoint,
                    code_user: code,
                    local_msg: {
                        error_caricamento_dati: '<%= Messages.label_errorloadDat%>',
                            error_utente_non_presente: '<%= Messages.label_utenteNoPres%>',
                            campi_obbligatori: '<%= Messages.text_fieldsChangeUser%>',
                            operazione_effettuata: '<%= Messages.label_changeUserOk%>',
                            operazione_fallita: '<%= Messages.label_changeUserKO%>',
                        },
                        local_interface: {
                            nome: '<%= Messages.label_userName%>',
                            cognome: '<%= Messages.label_userSurname%>',
                            sesso: '<%= Messages.label_userSex%>',
                            età: '<%= Messages.label_userAge%>',
                            residenza: '<%= Messages.label_userResidence%>',
                            titolo_studio: '<%= Messages.label_userEduQual%>',
                            cond_prof: '<%= Messages.label_userProfStatus%>',
                            settore_econ: '<%= Messages.label_userEconomicSector%>',
                            lingua: '<%= Messages.label_userLang%>',
                            temi_interesse: '<%= Messages.label_userThemes%>',
                            indirizzo_email: '<%= Messages.label_userEmail%>',
                            password: '<%= Messages.label_userPsw%>',
                            conferma_password: '<%= Messages.label_userConfPsw%>',
                            informativa_privacy: "<%= Messages.label_userPrivacy%>",
                            label_autoriz_trattamento_dati: '<%= Messages.label_userTreatDat%>',
                            buttonM: '<%= Messages.button_userChange%>',
                        }
                    };
                    SetupWidget_widgetChangeUser(data, OnSuccessLogin_changeUser, OnFailLogin);
                } else if (statusTxt == "error") {
                    alert("Error: " + xhr.status + ": " + xhr.statusText);
                }
        });
    </script>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
