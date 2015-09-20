<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>
  
<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=Url.Content("~/Scripts/istat-widget-manager.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/istat-widget-dataset.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/istat-client.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/pages/redirectLogin.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/pages/dashboardElements.js")%>"></script>
     
    <link href="<%=Url.Content("~/Content/style/widgets/UserList.css")%>" rel="stylesheet" />
    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
        };
    </script>
</asp:Content>
    
<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-users-2"></i> <%= Messages.dashboard_userList%></h1>
    <!-- HTML Widget START -->
    <div id="DivLogin" class="queryContainer">
         
    </div>
    <!-- HTML Widget END -->
</asp:Content>
  

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript">
        var _endPoint = "<%=ConfigurationManager.AppSettings["SingleSignOnUrl"].ToString() %>";
        var curIdxPaging = 0;
        var elemnPaging = <%=ConfigurationManager.AppSettings["ElementInList"].ToString() %>;
        var users = [];
        
        jQuery(document).ready(function () {

            CheckIsLogin();

            DrawHTML_UserLIst("#DivLogin");

        });

        function removeUser(userCode, _endPoint) {
            var user_obj = "";
            $.ajax({
                type: "POST",
                url: _endPoint+"/service/GetUser",
                async: true,
                data: JSON.stringify(userCode),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg_o) {
                    var us = msg_o;
                    var age = us.Age;
                    var agency = us.Agency;
                    var country = us.Country;
                    var email = us.Email;
                    var lang = us.Lang;
                    var name = us.Name;
                    var position = us.Position;
                    var sex = us.Sex;
                    var study = us.Study;
                    var surname = us.Surname;
                    var theme = us.Themes;
                    var themes = new Array();
                    for (ii = 0; ii < theme.length; ii++) {
                        themes[ii] = '"' + theme[ii] + '"';
                    }
                    var codice = us.UserCode;
                    
                    var dati = '{"Age":"' + age + '",' +
                                '"Agency":"' + agency + '",' +
                                '"Country":"' + country + '",' +
                                '"Email":"' + email + '",' +
                                '"Lang":"' + lang + '",' +
                                '"Name":"' + name + '",' +
                                '"Position":"' + position + '",' +
                                '"Sex":"' + sex + '",' +
                                '"Study":"' + study + '",' +
                                '"Surname":"' + surname + '",' +
                                '"Themes":[' + themes + '],' +
                                '"UserCode":"' + codice + '"}';
                    
                    var div_dialog = document.createElement("div");
                    $(div_dialog).attr("title", '<%=Messages.form_Message%>'); 
                    $(div_dialog).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.text_confirmDelUser %></p>");
                    $(div_dialog).dialog({
                        modal: true,
                        resizable: false,
                        closeOnEscape: false,
                        draggable: false,
                        position: { my: "center", at: "center", of: window },
                        buttons: {
                            '<%=Messages.label_save%>': function () {
                                $.ajax({
                                    type: "POST",
                                    url: _endPoint + "/service/DelUser",
                                    async: true,
                                    data: dati,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        var div_response = document.createElement("div");
                                        $(div_response).attr("title", '<%=Messages.form_Message%>');
                                        $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.text_userRemoved %></p>");
                                        $(div_response).dialog({
                                            modal: true,
                                            resizable: false,
                                            closeOnEscape: false,
                                            draggable: false,
                                            position: { my: "center", at: "center", of: window },
                                            buttons: {
                                                '<%=Messages.label_save%>': function () {
                                                    $(this).dialog("close");
                                                    DrawHTML_UserLIst("#DivLogin");
                                                    //location.reload(true);
                                                }
                                            }
                                        });
                                    },
                                    error: function (msg) {
                                        var div_response = document.createElement("div");
                                        $(div_response).attr("title", "");
                                        $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_error_data %></p>");
                                        $(div_response).dialog({
                                            modal: true,
                                            resizable: false,
                                            closeOnEscape: false,
                                            draggable: false,
                                            position: { my: "center", at: "center", of: window },
                                            buttons: {
                                                'Ok': function () {
                                                    $(this).dialog("close");
                                                    DrawHTML_UserLIst("#DivLogin");
                                                    //location.reload(true);
                                                }
                                            }
                                        });
                                    }
                                });
                    
                                $(this).dialog("close");
                            },
                            '<%=Messages.label_cancel%>': function () {
                                $(this).dialog("close");
                            }
                        }
                    });
                    
                },
                error: function (msg) {
                    var div_response = document.createElement("div");
                    $(div_response).attr("title", "");
                    $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_error_data %></p>");
                    $(div_response).dialog({
                        modal: true,
                        resizable: false,
                        closeOnEscape: false,
                        draggable: false,
                        position: { my: "center", at: "center", of: window },
                        buttons: {
                            'Ok': function () {
                                $(this).dialog("close");
                                DrawHTML_UserLIst("#DivLogin");
                                //location.reload(true);
                            }
                        }
                    });
                }
            });
        }

        function changeUser(code) { 
            $("#DivLogin").load(_endPoint + "/Widget/widgetChangeUser.html", null,
                function (responseTxt, statusTxt, xhr) {
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
        }

        function OnSuccessLogin_changeUser(msg) {
            //alert(JSON.stringify(msg));
            //alert("modifiche  effettuate correttamente");
           
            var div_response = document.createElement("div");
            $(div_response).attr("title", '<%=Messages.form_Message%>');
            $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_changeSuccess %></p>");
            $(div_response).dialog({
                modal: true,
                resizable: false,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                        DrawHTML_UserLIst("#DivLogin");
                        //location.reload(true);
                    }
                }
            });
        }

        function OnFailLogin(msg) {
            //alert(msg);

            var div_response = document.createElement("div");
            $(div_response).attr("title", '<%=Messages.form_Message%>');
            $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + msg + "</p>");
            $(div_response).dialog({
                modal: true,
                resizable: false,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                    }
                }
            });
        }
        function OnSuccess_changePsw(msg) {
            //alert("modifiche  effettuate correttamente");

            var div_response = document.createElement("div");
            $(div_response).attr("title", '<%=Messages.form_Message%>');
            $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_changePswOk %></p>");
            $(div_response).dialog({
                modal: true,
                resizable: false,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                        DrawHTML_UserLIst("#DivLogin");
                        //location.reload(true);
                    }
                }
            });
        }

        function OnFail_changePsw(msg) {
            //alert(msg);
            var div_response = document.createElement("div");
            $(div_response).attr("title", '<%=Messages.form_Message%>');
            $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>"+msg+"</p>");
            $(div_response).dialog({
                modal: true,
                resizable: false,
                closeOnEscape: false,
                draggable: false,
                position: { my: "center", at: "center", of: window },
                buttons: {
                    'Ok': function () {
                        $(this).dialog("close");
                    }
                }
            });
        }

        function changePDW(code) {
            $("#DivLogin").load(_endPoint + "/Widget/widgetChangePassword.html", null, function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success") {
                    var data = {
                        endpoint: _endPoint,
                        code_user: code,
                        local_msg: {
                            operazione_riuscita: "<%= Messages.label_changePswOk%>",
                            operazione_non_riuscita: "<%= Messages.label_changePswKO%>",
                            compila_i_campi: "<%= Messages.text_msgCampiObbl%>",
                            length_psw: "<%= Messages.text_msglengthPsw%>",
                            conf_psw: '<%= Messages.label_confPsw%>',
                        },
                        local_interface: {
                            psw: '<%= Messages.label_psw%>',
                            conferma_psw: '<%= Messages.label_confirmPsw%>',
                            old_psw: '<%= Messages.label_oldPsw%>',
                            button: '<%= Messages.label_sendButton%>',
                        }
                    };
                    SetupWidget_widgetChangePassword(data, OnSuccess_changePsw, OnFail_changePsw);
                } else if (statusTxt == "error") {
                    alert("Error: " + xhr.status + ": " + xhr.statusText);
                }
            });
        }

        function saved(code, nome, cognome, user_email, role) {
            var _role = "";
            
            if(role == 1){
                _role =  "Administrator";
            } else {
                _role = "User";
            }

            var obj_role = '{"RoleId":"'+role+'","Role":"'+ _role+'"}';
            
            var dati = {
                UserCode: code,
                Nome: nome,
                Cognome: cognome,
                Email: user_email,
                UserRole: JSON.parse(obj_role)
            }
            
            var urlGetQuery = "Profile/ModUserRole";
            clientPostJSON(urlGetQuery, clientParseObjectToJson(dati),
                    function (jsonString) {
                        var div_response = document.createElement("div");
                        $(div_response).attr("title", "");
                        $(div_response).html("<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span><%= Messages.label_changeUserOk %></p>");
                        $(div_response).dialog({
                            title: "<%=Messages.form_Message%>",
                            modal: true,
                            resizable: false,
                            closeOnEscape: false,
                            draggable: false,
                            position: { my: "center", at: "center", of: window },
                            buttons: {
                                'Ok': function () {
                                    $(this).dialog("close");
                                    DrawHTML_UserLIst("#DivLogin");
                                    //location.reload(true);
                                }
                            }
                        });
                    },
                    function (event, status, errorThrown) {
                        clientShowErrorDialog('<%= Messages.label_error_data %>');
                        clientAjaxError(event, status);
                        return;
                    },
                    false);
        }

        function dynamicSort(property) {
            return function (a, b) {
                return (a[property] > b[property]) ? -1 : (a[property] < b[property]) ? 1 : 0;
            }
        }

        function CheckIsLogin() {
            var dati = {
                UserCode: sessionStorage.user_code,
                Nome: sessionStorage.user_name,
                Cognome: sessionStorage.user_surname,
                Email: sessionStorage.email,
                UserRole: JSON.parse(sessionStorage.user_role)
            }

            clientPostJSON("Profile/IsLogin",
                clientParseObjectToJson(dati),
                function (jsonString) { },
                function (event, status, errorThrown) {
                    clientShowErrorDialog("<%= Messages.label_error_data %>");
                    return;
                },
                false);
        }

        function DrawHTML_UserLIst(dest) {
            $(dest).empty();
            $(dest).append("<i class='icon-spin6 animate-spin'></i>"+client.main.messages.text_wait);
            var dati = _endPoint;

            clientPostJSON(
                "Profile/GetUserList",
                dati,
                function (jsonString) {
                    
                    var user_container=document.createElement("div");

                    var disabledIdRole=500;

                    var dat = clientParseJsonToObject(jsonString);
                    var idUs = JSON.parse(sessionStorage.user_isSA);

                    dat.Roles.sort(dynamicSort('RoleId'));

                    if (dat == "" || dat==undefined) { 
                        $(user_container).append("<div class='lista_vuota'><%= Messages.label_queryList_empty %></div>");
                    } else {

                        var cont = 0;
                        var obj_SA = "";
                        $.each(dat.UserList, function () {
                            if (this.IsSuperAdmin) 
                                obj_SA = dat.UserList.splice(cont, 1);
                            cont++;
                        });

                        var fon = obj_SA.concat(dat.UserList);

                        var div_pag = DrawHTML_Pagging(
                            curIdxPaging,
                            fon.length,
                            elemnPaging,
                            function (idx) {
                                curIdxPaging = idx;
                                DrawHTML_UserLIst(dest);
                            }
                        );
                        
                        $(div_pag).appendTo(user_container);
                        $(div_pag).buttonset();

                        var pagFrom = (curIdxPaging * elemnPaging);
                        var pagTo = pagFrom + elemnPaging;
                        var idxPag = -1;

                        /////////////////////////////////////////////////////////////////
                        var i = 0;
                        var ii = 0;

                        $.each(fon, function () {

                            idxPag++;
                            
                            var hideDiv = (idxPag < pagFrom || idxPag > (pagTo - 1));

                            if(hideDiv) return;

                            var contSel = ii;

                            var nome = this.Nome;
                            var cognome = this.Cognome;
                            var isSA = this.IsSuperAdmin;
                            var user_code = this.UserCode;
                            var user_email = this.Email;
                            var role = this.UserRole.RoleId;
                            var user_name = nome + " " + cognome;

                            var optionRole = "";
                            var checked = "";

                            var cls = (isSA == true) ? "superAdmin" : "";
                            var but = ((isSA == true) || (user_code == sessionStorage.user_code)) ? "disabled" : "";
                            var dis = ((isSA == true) || (user_code == sessionStorage.user_code)) ? "disableb-button" : "";
                            if (role == disabledIdRole) {
                                optionRole = "<select disabled id='id_Role_" + ii + "' ><option value='"+disabledIdRole+"'>Disabled</option></select>";
                                checked = "";
                            } else {
                                var sel = "";
                                $.each(dat.Roles, function () {
                                    if (this.RoleId != disabledIdRole) {
                                        if (this.RoleId == role) {
                                            sel = sel + "<option value='" + this.RoleId + "' selected >" + this.Role + "</option>";
                                        } else {
                                            sel = sel + "<option value='" + this.RoleId + "'>" + this.Role + "</option>";
                                        }
                                    }
                                });
                                optionRole = "<select " + but + " id='id_Role_" + ii + "' >" + sel + "</select>";
                                checked = "checked";
                            }

                            var select_role = "<div class=\"select_role\">" + optionRole + "</div>";
                            var enable = '<div class=\"enable\"><input ' + but + '  id="id_Enable_' + i + '"' + checked + ' type="checkbox"  name="enable"/><label class=' + dis + '>is active</label></div>';
                            var save = "<div class=\"save\"><i class=' icon-floppy-1 " + dis + "'></i><input class='button' " + but + " id='id_Save_" + i + "' type='button' value='<%= Messages.label_buttonSave%>'></div>";

                            var id_use=document.createElement("div");
                            $(id_use).attr('id','id_use');
                            $(id_use).addClass('user');
                            $(id_use).addClass(cls);

                            var user_main=document.createElement("div");
                            $(user_main).addClass('user_main');
                            $(user_main).append("<div class='user_mail'>" + user_email + "</div>");
                            $(user_main).append("<div class='user_name'>" + user_name + "</div>");
                            $(user_main).append("<div class='clear-box'>");
                            $(user_main).appendTo(id_use);

                            var user_role=document.createElement("div");
                            $(user_role).addClass('user_role');
                            $(user_role).append(select_role + enable + save);
                            $(user_role).append("<div class='clear-box'>");
                            $(user_role).appendTo(id_use);
                            
                            $(user_role).find("#id_Enable_" + i).click(function () {
                                if ($(this).is(":not(:checked)")) {
                                    $("#id_Role_" + contSel).empty();
                                    $("#id_Role_" + contSel).append("<option value="+disabledIdRole+">Disabled</option>");
                                    $("#id_Role_" + contSel).attr('disabled', 'disabled');
                                } else {
                                    $("#id_Role_" + contSel).removeAttr('disabled');
                                    $("#id_Role_" + contSel).empty();

                                    var sel = "";
                                    $.each(dat.Roles, function () {
                                        if (this.RoleId != disabledIdRole)
                                            sel = sel + "<option value='" + this.RoleId + "'>" + this.Role + "</option>";
                                    });

                                    $("#id_Role_" + contSel).append(sel);
                                }

                            });
                            $(user_role).find("#id_Save_" + i).click(function () { saved(user_code, nome, cognome, user_email, $("#id_Role_" + contSel).val());});
                            
                            if (idUs == true) {

                                var buttons=document.createElement("div");
                                $(buttons).addClass('buttons');
                                $(buttons).append("<span class='button'><i class='icon-upload-2'></i><input class='button' title='execute query' type='button' id='id_button_O_" + i + "' value='<%=Messages.button_userListChange %>'/></span>");
                                $(buttons).append("<span class='button'><i class='icon-key-inv'></i><input class='button' title='delete query' type='button' id='id_button_C_" + i + "' value='<%=Messages.button_userListChangePsw %>'/></span>");
                                $(buttons).append("<span class='button'><i class='icon-trash " + dis + "'></i><input " + but + " class='button' title='delete query' type='button' id='id_button_R_" + i + "' value='<%=Messages.button_userListRemove %>'/></span>");
                                $(buttons).append("<div class='clear-box'>");
                                $(buttons).appendTo(id_use);

                                $(buttons).find("#id_button_O_" + i).click(function () { changeUser(user_code); });
                                $(buttons).find("#id_button_C_" + i).click(function () { changePDW(user_code); });
                                $(buttons).find("#id_button_R_" + i).click(function () { removeUser(user_code, _endPoint); });

                                $(id_use).appendTo(user_container);

                            } else {

                                $(id_use).appendTo(user_container);

                                $(user_main).css('width', '50%');
                                $(user_role).css('width', '45%');
                                $(user_role).css('float', 'right');
                            }

                            i++;
                            ii++;

                        });
                        
                        $(dest).empty();
                        $(user_container).appendTo(dest);

                    }
                },
                function (event, status, errorThrown) {
                    clientShowErrorDialog('<%= Messages.label_error_data %>');
                    //clientAjaxError(event, status);
                    return;
                },
                false);
        }

    </script>
</asp:Content>
