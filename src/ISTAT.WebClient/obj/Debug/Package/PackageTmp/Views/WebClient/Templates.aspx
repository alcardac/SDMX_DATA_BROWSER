<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>
<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
    <script src="<%=Url.Content("~/Scripts/jquery/colorpicker/colpick.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/pages/dashboardElements.js")%>"></script>

    <script type="text/javascript">
        // redirect to login page if access denied
        if (sessionStorage.user_code == null) {
            window.location.href = "<%=Url.Content("~/")%>WebClient/Login";
        };
    </script>

</asp:Content>
<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-photo"></i> <%= Messages.dashboard_Themes %></h1>
    <div class="settings-left" id="inner-left" tabindex="6">
        <div class="P1">
            <div>
                <h2><%= Messages.label_CustomFonts %></h2>
            </div>
<%--            
            <div>
                Body
            </div>
--%>
            <div>
                <select name="select-font-family" id="select-font-family">
                    <option selected="selected" value="Arial">Arial</option>
                    <option value="Tahoma">Tahoma</option>
                    <option value="Verdana">Verdana</option>
                </select>
                <select name="select-font-size" id="select-font-size">
                    <option value="10">10</option>
                    <option selected="selected" value="12">12</option>
                    <option value="14">14</option>
                </select>
            </div>
<%--        HEADER/FOOTER FONTS    
            <div>
                Header
            </div>
            <div>
                <select name="select-font-family-header" id="select-font-family-header">
                    <option selected="selected" value="Arial">Arial</option>
                    <option value="Tahoma">Tahoma</option>
                    <option value="Verdana">Verdana</option>
                </select>
                <select name="select-font-size" id="select-font-size-header">
                    <option selected="selected" value="12">12</option>
                    <option value="14">14</option>
                    <option value="18">18</option>
                </select>
            </div>
            <div>
                Footer
            </div>
            <div>
                <select name="select-font-family-footer" id="select-font-family-footer">
                    <option selected="selected" value="Arial">Arial</option>
                    <option value="Tahoma">Tahoma</option>
                    <option value="Verdana">Verdana</option>
                </select>
                <select name="select-font-size" id="select-font-size-footer">
                    <option selected="selected" value="12">12</option>
                    <option value="14">14</option>
                    <option value="18">18</option>
                </select>
            </div>
--%>
        </div>
<%--        <div class="P2">
            <div>
                <h2><%= Messages.label_CustomScreen %></h2>
            </div>
            <div>
                <select name="select-screen-size" id="select-screen-size">
                    <option selected="selected" value="100%">max</option>
                    <option value="950px">950</option>
                    <option value="1024px">1024</option>
                    <option value="1280px">1280</option>
                </select>
            </div>
        </div>--%>
    </div> 
    <div class="settings-right" id="inner-right" tabindex="7">
        <div class="P2">
            <div>
                <h2><%= Messages.label_CustomColors %></h2>
            </div>
            <div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_standard" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc" for="admin_color_standard">Predefinito</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #666666">
                                    <div class="td-blank">&nbsp;</div>
                                </td>
                                <td style="background-color: #666666">&nbsp;</td>
                                <td style="background-color: #959595">&nbsp;</td>
                                <td style="background-color: #DA0D14">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
                </div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_blue" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="Content/style/custom/blue.css">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc">Blue</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #3A5795">&nbsp;</td>
                                <td style="background-color: #272a30">&nbsp;</td>
                                <td style="background-color: #99A7C5 ">&nbsp;</td>
                                <td style="background-color: #D4DCED">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
                </div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_green" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="Content/style/custom/green.css">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc">Green</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #3a946d">&nbsp;</td>
                                <td style="background-color: #0B4521">&nbsp;</td>
                                <td style="background-color: #83C59D ">&nbsp;</td>
                                <td style="background-color: #D2EDDD">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
                </div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_sistan" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="Content/style/custom/sistan.css">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc">Sistan</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #F39700">
                                    <div class="td-blank">&nbsp;</div>
                                </td>
                                <td style="background-color: #F39700">&nbsp;</td>
                                <td style="background-color: #00295A">&nbsp;</td>
                                <td style="background-color: #E6E6E6">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
                </div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_sport" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="Content/style/custom/sport.css">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc">Sport</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #DF4D44">&nbsp;</td>
                                <td style="background-color: #3A8BBE">&nbsp;</td>
                                <td style="background-color: #1174B3">&nbsp;</td>
                                <td style="background-color: #FAF8D1">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
                </div>
                <div class="color-option">
				    <input name="admin_color" id="admin_color_coffee" type="radio" value="fresh" class="tog">
				    <input type="hidden" class="css_url" name="css_url" value="Content/style/custom/coffee.css">
				    <input type="hidden" class="icon_colors" value="{&quot;icons&quot;:{&quot;base&quot;:&quot;#999&quot;,&quot;focus&quot;:&quot;#2ea2cc&quot;,&quot;current&quot;:&quot;#fff&quot;}}">
				    <label class="color-desc">Coffee</label>
				    <table class="color-palette">
					    <tbody>
                            <tr>
                                <td style="background-color: #46403c">&nbsp;</td>
                                <td style="background-color: #9ea476">&nbsp;</td>
                                <td style="background-color: #7C6D5F ">&nbsp;</td>
                                <td style="background-color: #c7a589">&nbsp;</td>
						    </tr>
				        </tbody>
				    </table>
			    </div>
            </div>
        </div>
    </div>
    <div class="clear-box"></div>
    <div class="saveContainer">
        <div class="saveBox">
            <input type="submit" id="saveSettings" value="<%=Messages.label_buttonSave %>">
        </div>
    </div>
    <div id="dialog-message-settings-ok" title="<%=Messages.label_settingsDialogTitle %>">
        <p>
            <span class="ui-icon ui-icon-circle-check"></span>
            <%=Messages.label_settingsDialogMsg %>
        </p>
    </div>
</asp:Content>
<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript" src="<%=Url.Content("~/Scripts/pages/settings.js")%>"></script>
</asp:Content>
