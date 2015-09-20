<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>
<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-contacts"></i> <%= Messages.footer_contacts %></h1>
    <div class="contacts-left" id="inner-left" tabindex="6">
        <div>
            <div>
            </div>
            <div id="contact-form" class="form">
                <div class="fieldset">
                    <div>
                        <label for="name"><%=Messages.form_Name %></label>
                    </div>
                    <div>
                        <input id="name" name="name" type="text" class="input-field" title="<%=Messages.form_Name_tp %>" required="required"/>
                        <label id="err-name" class="msg-error"><%=Messages.form_Name_tp %></label>
                    </div>
                </div>
                <div class="fieldset">
                    <div>
                        <label for="email"><%=Messages.form_Email %></label>
                    </div>
                    <div>
                        <input id="email" name="email" class="input-field" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$" title="<%=Messages.form_Email_tp %>" required="required"/>
                        <label id="err-email" class="msg-error"><%=Messages.form_Email_tp %></label>
                    </div>
                </div>
                <div class="fieldset">
                    <div>
                        <label for="subject"><%=Messages.form_Subject %></label>
                    </div>
                    <div>
                        <input id="subject" name="subject" type="text" class="input-field" title="<%=Messages.form_Subject_tp %>" required="required"/>
                        <label id="err-subject" class="msg-error"><%=Messages.form_Subject_tp %></label>
                    </div>
                </div>
                <div class="fieldset">
                    <div>
                        <label for="message"><%=Messages.form_Message %></label>
                    </div>
                    <div>
                        <textarea id="message" name="message" class="input-field" title="<%=Messages.form_Message_tp %>" required="required"></textarea>
                        <label id="err-message" class="msg-error"><%=Messages.form_Message_tp %></label>
                    </div>
                </div>
                <div class="fieldset">
                    <div class="form-checkbox">
                        <div>
                            <input id="agree_terms" name="agree_terms" type="checkbox" title="<%=Messages.form_Agree_tp %>" class="input-field" required="required"/>
                            <span><%=Messages.form_Agree %></span>
                        </div>
                        <label id="err-agree_terms" class="msg-error"><%=Messages.form_Agree_tp %></label>
                    </div>
                </div>
                <div class="buttonset">
                    <input type="submit" title="<%=Messages.form_Send_tp %>" value="<%=Messages.form_Send %>"/>
                </div>
            </div>
        </div>
    </div>
    <div class="contacts-right" id="inner-right" tabindex="7">
        <div class="P1">
            <h2><%=Messages.contact_Numbers %></h2>
            <div>
                <i class="icon-phone-1"></i><span class="e-title"><%=Messages.contact_Phone %></span>
                <span class="e-desc">+39 06 46731</span>
            </div>
            <div>
                <i class="icon-fax"></i><span class="e-title"><%=Messages.contact_Fax %></span>
                <span class="e-desc">xx-xxxx</span>
            </div>
        </div>
        <div class="P2">
            <h2><%=Messages.contact_Details %></h2>
            <div>
                <i class="icon-email"></i><span class="e-title"><%=Messages.contact_Email %></span>
                <span class="e-desc">comunica@istat.it</span>
            </div>
            <div>
                <i class="icon-location"></i><span class="e-title"><%=Messages.contact_Address %></span>
                <span class="e-desc">Via Cesare Balbo 16 00184 - Roma</span>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
    <script src="<%=Url.Content("~/Scripts/pages/contacts.js")%>"></script>
</asp:Content>
