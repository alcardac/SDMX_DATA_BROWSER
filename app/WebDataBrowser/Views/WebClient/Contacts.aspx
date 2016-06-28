<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-contacts"></i> <%= Messages.footer_contacts %></h1>
    <div class="Icontainer Whalf PageCentered">
        <h2><%=Messages.label_ISTAT %></h2>
        <div class="Paragraph">
            <div>
                <i class="icon-phone-1"></i><span class="e-title"><%=Messages.contact_Phone %></span>
                <span class="e-desc">+39 06 46731</span>
            </div>
            <div>
                <i class="icon-location"></i><span class="e-title"><%=Messages.contact_Address %></span>
                <span class="e-desc">Via Cesare Balbo 16 00184 - Roma</span>
            </div>
        </div>
        <div class="Description">
            <p>Il <b><a href="https://contact.istat.it//Index.php">contact centre</a></b> è il servizio a cui rivolgersi per <b>richiedere dati</b>, pubblicazioni, file di microdati, cartografie, ricerche storiche ed elaborazioni personalizzate, nonché per informazioni su dati europei armonizzati.</p>
            <p>Per contattare la <b>redazione del sito</b>, chiedere informazioni sulla reperibilità dei documenti, fornire un feedback su come migliorare il servizio - ma non per richiedere dati o pubblicazioni - scrivere a <b><a href="mailto:comunica@istat.it">comunica@istat.it</a></b>.</p>
            <p>Per segnalare criticità o <b>motivi di insoddisfazione</b> compilare il modulo in versione <a href="http://www.istat.it/it/files/2012/06/modulo_di_reclamo_istat_def.pdf">pdf</a> o in versione <a href="http://www.istat.it/it/files/2012/06/modulo_di_reclamo_istat_def.odt">odt</a> e inviarlo a <b><a href="mailto:comunica@istat.it">comunica@istat.it</a></b>.</p>
            <p>Presso i <b><a href="http://www.istat.it/it/informazioni/per-gli-utenti/sportelli-sul-territorio">Centri di informazione statistica</a></b>, strutture presenti in ogni sede regionale dell'Istat, è possibile usufruire di una vasta gamma di servizi, ricevere assistenza qualificata nella ricerca di dati statistici, consultare e acquistare tutti i prodotti editoriali dell'Istituto.</p>
            <p>Per richiedere informazioni sui volumi in <b>edizione cartacea</b> e ordini, inviare una e-mail a <a href="mailto:editoria.acquisti@istat.it"><b>editoria.acquisti@istat.it</b></a>.</p>
            <p>Per <b>informazioni sulle rilevazioni</b> in corso si consiglia di visionare la <a href="http://www.istat.it/it/informazioni/per-i-rispondenti"><b>sezione dedicata ai rispondenti</b></a> e selezionare l'argomento di interesse.</p>
            <p>Per contattare direttamente altri uffici consultare l'<b><a href="http://www.istat.it/it/istituto-nazionale-di-statistica/organizzazione/organigramma">organigramma</a></b> dell'Istituto.</p>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
