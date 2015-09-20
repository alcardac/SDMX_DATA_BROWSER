<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <link href="<%=Url.Content("~/Content/style/widgets/Login.css")%>" rel="stylesheet" />
</asp:Content>
 
<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1 id="WidgetTitle"><i class="icon-login"></i> <%= Messages.label_login %></h1>
    <!-- HTML Widget START here-->
    <div id="widgetContainer">
        <div class="containerT" id="containerT">
            <div id="DivLogin">
            </div>
            <div class="nuovo_utente" id="id_nuovo_utente">
                <i class="icon-user-add-1"></i><a  id="id_link_nuovo_utente"><%= Messages.label_loginNew %></a>
            </div>
            <div class="reset_psw" id="id_reset_psw">
                <i class=" icon-key-inv"></i><a  id="id_link_reset_psw"><%= Messages.label_loginPwd %></a>
            </div>
        </div>
    </div>
    <!-- HTML Widget END -->
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
    <!-- JS Widget START -->
    <script src="<%= Url.Content("~/Scripts/pages/login.js")%>"></script>
    <script type="text/javascript">

        var _endPoint = "<%=ConfigurationManager.AppSettings["SingleSignOnUrl"].ToString() %>";
        
        $(document).ready(function () {

        var data = {
            endpoint: _endPoint,
            local_msg: {
                login_effettuato: '<%=Messages.log_OK%>',
                login_fallito: '<%=Messages.log_KO%>',
            },
            local_interface: {
                utente: '<%=Messages.label_loginUser%>',
                password: '<%=Messages.label_loginPassword%>',
            }
        };

        $("#DivLogin").load(_endPoint + "/Widget/widgetLogin.html", null, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {

                SetupWidget_widgetLogin(data, OnSuccessLogin, OnFailLogin);

            } else if (statusTxt == "error") {
                clientShowErrorDialog("Error: " + xhr.status + ": " + xhr.statusText);
            }
        });


        $("#id_link_nuovo_utente").click(function () {
            var data = {
                endpoint: _endPoint,
                local_msg: {
                    errore_caricamento_dati: '<%= Messages.label_errorloadDat%>',
                    registrazione_effettuata: '<%= Messages.label_changeUserOk%>',//'Registrazione Effettuata', //client.main.messages
                    registrazione_fallita: '<%= Messages.label_changeUserKO%>',//'Operazione Non Riuscita',
                    compila_campi: '<%= Messages.text_msgCampiObbl%>',//'Compila i Campi Obbligatori',
                    length_psw: '<%= Messages.text_msglengthPsw%>',//'La password deve contenere almeno 8 caratteri',
                    conf_psw: '<%= Messages.label_confPsw%>',//'Il campo "Conferma Password" deve essere uguale al campo "Password"',
                    privacy: '<%= Messages.text_msgCampiObbl%>',//'Devi accettare il trattamento dei dati',
                },
                local_interface: {
                    nome: '<%= Messages.label_userName%> *',
                    cognome: '<%= Messages.label_userSurname%> *',
                    sesso: '<%= Messages.label_userSex%> *',
                    eta: '<%= Messages.label_userAge%> *',
                    residenza: '<%= Messages.label_userResidence%> *',
                    titolo_studio: '<%= Messages.label_userEduQual%> *',
                    cond_prof: '<%= Messages.label_userProfStatus%> *',
                    settore_econ: '<%= Messages.label_userEconomicSector%> *',
                    lingua: '<%= Messages.label_userLang%> *',
                    temi_interesse: '<%= Messages.label_userThemes%>',
                    indirizzo_email: '<%= Messages.label_userEmail%> *',
                    password: '<%= Messages.label_userPsw%> *',
                    conferma_password: '<%= Messages.label_userConfPsw%> *',
                    informativa_privacy: "<%= Messages.label_userPrivacy%> *",
                    label_autoriz_trattamento_dati: '<%= Messages.label_userTreatDat%> *',
                    button: '<%= Messages.form_Send%>'
                }
            };

            $("#WidgetTitle").empty();
            $("#WidgetTitle").html("<i class='icon-user-add-1'></i> Nuovo Utente");
            $("#containerT").load(_endPoint + "/Widget/widgetRegistration.html", null, function (responseTxt, statusTxt, xhr) {

                if (statusTxt == "success") {

                    SetupWidget_widgetRegistration(data, OnSuccessLoginR, OnFailLogin);

                } else if (statusTxt == "error") {
                    clientShowErrorDialog("Error: " + xhr.status + ": " + xhr.statusText);
                }
            });
        });

        

    });

    function OnSuccessLogin(msg) {
        var strjson = JSON.stringify(msg);
        var obj = JSON.parse(strjson);
       
        var data = {
            UserCode: obj.UserCode,
            IsSuperAdmin: obj.IsSA,
        }
        //--//
        var urlGetProfile = "Profile/Login";
        clientPostJSON(urlGetProfile, clientParseObjectToJson(data),
                function (jsonString) {
                    var objProf = JSON.parse(jsonString);
                    if (objProf.UserRole.RoleId == 500) {
                        var mess = "Nome Utente o password non corretti";
                        OnFailLogin(mess);
                    } else {
                        sessionStorage.setItem("user_code", obj.UserCode);
                        sessionStorage.setItem("user_name", obj.Name );
                        sessionStorage.setItem("user_surname", obj.Surname);
                        sessionStorage.setItem("email", obj.Email);
                        sessionStorage.setItem("user_isSA", objProf.IsSuperAdmin);
                        sessionStorage.setItem("user_role", JSON.stringify(objProf.UserRole));
                        window.location.href = "<%=Url.Content("~/")%>WebClient/Index";
                    }
                    
                },
                function (event, status, errorThrown) {
                    clientShowErrorDialog('<%= Messages.label_error_data %>');
                    //clientAjaxError(event, status);
                    return;
                },
                false);

        //--//
        
    }

    function OnSuccessLoginR(msg) {

        var div_dialog = document.createElement("div");
        $(div_dialog).attr("title",'<%= Messages.label_loginRegistered %>');
        $(div_dialog).html("<p><span class='ui-icon ui-icon-circle-check' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_registrationSuccess %></p>");
        $(div_dialog).dialog({
            resizable: false,
            height: 140,
            position: { my: "center", at: "center", of: window },
            modal: true,
            buttons: {
                'Ok': function () {
                    $(this).dialog("close");
                    var strjson = JSON.stringify(msg);
                    var obj = JSON.parse(strjson);
                    sessionStorage.setItem("user_code", obj.UserCode);
                    sessionStorage.setItem("user_name", obj.Name);
                    sessionStorage.setItem("user_surname", obj.Surname);
                    sessionStorage.setItem("email", obj.Email);
                    sessionStorage.setItem("user_isSA", false);
                    sessionStorage.setItem("user_role", JSON.stringify({ RoleId: "<%= (int)ISTAT.WebClient.WidgetComplements.Model.Enum.UserRolesEnum.User %>", Role: "<%=ISTAT.WebClient.WidgetComplements.Model.Enum.UserRolesEnum.User.ToString() %>" }));
                        

                    window.location.href = "<%=Url.Content("~/")%>WebClient/Index";
                }
            }
        });
    }

    $("#id_link_reset_psw").click(function () {
        
        var data = {
            endpoint: _endPoint,
            local_msg: {
                reset_effettuato: '<%= Messages.label_reset_pdw_Ok%>',
                reset_fallito: '<%= Messages.label_reset_pdw_Ko%>',
                sender: '<%= Messages.label_sender_mail%>',
                mailTemplate: '<div class="mail_template"><p class="mail_template"><%= Messages.label_text_mail%>##NEWPASSWORD## </p></div>',
                subject_mail: '<%= Messages.label_subject_reset_pdw%>',
                mailSMTPServer: '<%= Messages.label_mailSMTPServer%>',
                compila: '<%= Messages.label_insert_mail%>',
            },
            local_interface: {
                label: '<%= Messages.label_mail%>',//"Email",
            }
        };

        $("#WidgetTitle").html("");
        $("#WidgetTitle").html("<i class=' icon-key-inv'></i> Reset Password");
        $("#containerT").load(_endPoint + "/Widget/widgetResetPassword.html", null, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {
                SetupWidget_widgetResetPassword(data, OnSuccessLoginRP, OnFailLogin);
            } else if (statusTxt == "error") {
                clientShowErrorDialog("Error: " + xhr.status + ": " + xhr.statusText);
            }
        });
    });

    function OnSuccessLoginRP(msg) {
        //clientShowErrorDialog("dal client " + msg);

        var div_dialog = document.createElement("div");
        $(div_dialog).attr("title", '<%= Messages.form_Message %>');
        $(div_dialog).html("<p><span class='ui-icon ui-icon-circle-check' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_reset_pdw_Ok %></p>");
        $(div_dialog).dialog({
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

    </script>
    <!-- JS Widget END -->
</asp:Content>
