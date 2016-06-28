<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginWidget.aspx.cs" Inherits="ISTAT.SingleSignON.LoginWidget.Widget.LoginWidget" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript">
        var baseUrl = "http://localhost:22920/service";
        function Parti(nomefunzione, dati) {
            $.ajax({
                type: "POST",
                url: baseUrl + nomefunzione,
                async: true,
                data: JSON.stringify(dati),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var strjson = JSON.stringify(msg);
                    $('#txtRes').val(strjson);
                },
                error: function (msg) {
                    $('#txtRes').val('Error --> ' + msg.statusText + '\n\rDescription --> ' + msg.responseText);
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
            <input type="button" value="GETUsers" onclick="Parti('/GetUsers', null)" />
            <input type="button" value="ADDUser" onclick="Parti('/AddUser', JSON.parse($('#txtRes').val()))" />
            <input type="button" value="MODUser" onclick="Parti('/ModUser', JSON.parse($('#txtRes').val()))" />
            <input type="button" value="DELUser" onclick="Parti('/DelUser', JSON.parse($('#txtRes').val()))" />
            <input type="button" value="GETMetadata" onclick="Parti('/GetMetadata', 'it')" /><br />
            <br />
            <textarea id="txtRes" style="width: 700px; height: 500px">pippo</textarea>
            <br />
            Mail:<input type="text" id="txtMail" />
            Password:<input type="password" id="txtPass" />
            <input type="button" value="Login" onclick="Parti('/Login', { Email: $('#txtMail').val(), Password: $('#txtPass').val() })" /><br />
            <br />
            Codice Utente:<input type="text" id="CodUsertxt" /><br />
            OLD Password:<input type="password" id="OLDPasswordtxt" /><br />
            NEW Password:<input type="password" id="NEWPasswordtxt" /><br />
            <input type="button" value="ChangePassword" onclick="Parti('/ChangePassword', { UserCode: $('#CodUsertxt').val(), OldPassword: $('#OLDPasswordtxt').val(), NewPassword: $('#NEWPasswordtxt').val() })" /><br />
            <input type="button" value="ResetPassword" onclick="Parti('/ResetPassword', { UserCode: $('#CodUsertxt').val() })" /><br />


        </div>
    </form>
</body>
</html>
