<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewMasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources" %>

<asp:Content ID="Headcontainer" ContentPlaceHolderID="ContentHeader" runat="server">
</asp:Content>

<asp:Content ID="Maincontainer" ContentPlaceHolderID="MainContainer" runat="server">
    <h1><i class="icon-bookmark-3"></i> <%= Messages.footer_links %></h1>
<div class="linksContainer Whalf PageCentered">
    <p><a href="#int">1. <%= Messages.label_linkIO %></a><br/>
    <a href="#est">2. <%= Messages.label_linkOC %></a><br/>
    <a href="#naz">3. <%= Messages.label_linkAPE %></a><br/>
    <a href="#siti">4. <%= Messages.label_linkOS %></a></p>
    <h3>1. <%= Messages.label_linkIO %></h3>
    <p><a href="http://ec.europa.eu/eurostat/web/ess/latest-news" target="_blank">European Statistical System (ESS)</a><br>
    <a href="http://ec.europa.eu/eurostat">The Statistical Office of the European Communities (EUROSTAT)</a><br>
    <a href="http://www.imf.org/" target="_blank">International Monetary Fund (IMF)</a><a href="http://www.istat.it/fmi/ITALY-NSDP.html"></a><br>
    <a href="http://www.istat.it/fmi/ITALY-NSDP.html" target="_blank">Pagina multicontribuita IMF-ISTAT</a><br>
    <a href="http://www.worldbank.org/" target="_blank">The World Bank</a><br>
    <a href="http://www.ecb.int/" target="_blank">Banca Centrale Europea</a><br>
    <a href="http://www.cbs.nl/isi/" target="_blank">International Statistical Institute (ISI)</a><br>
    <a href="http://www.oecd.org/" target="_blank">Organisation for Economic Cooperation and Development (OECD)</a><br>
    <a href="http://www.un.org/" target="_blank">United Nations (ONU)</a><br>
    <a href="http://www.unece.org/" target="_blank">United Nations Economic Commission for Europe (UNECE)</a><br>
    <a href="http://data.un.org/" target="_blank">United Nations Global Data Dissemination Infrastructure (UNdata)</a><br>
    <a href="http://www.eclac.cl/default.asp?idioma=IN" target="_blank">United Nations Economic Commission for Latin America and the Caribbean (UN/ECLAC)</a><br>
    <a href="http://www.unesco.org/" target="_blank">United Nations Educational, Scientific and Cultural Organization (UNESCO)</a><br>
    <a href="http://www.unicef.org/" target="_blank">United Nations Children's Fund (UNICEF)</a>; <a href="http://www.ucw-project.org/" target="_blank">Understanding Children's Work (UCW)</a><br>
    <a href="http://www.fao.org/" target="_blank">United Nations Food and Agriculture Organization (FAO)</a><br>
    <a href="http://www.un.org/popin/" target="_blank">United Nations Population Information Network (POPIN)</a><br>
    <a href="http://www.who.int/en/" target="_blank">World Health Organization (WHO)</a><br>
    <a href="http://www.wto.org/english/thewto_e/thewto_e.htm" target="_blank">World Trade Organization (WTO)</a></p>
    <table>
    <tbody>
    <tr>
    <td>
    <h3>2. <%= Messages.label_linkOC %></h3>
    </td>
    <td>
    <h3><a href="#eu">EU</a> <a href="#am">AM</a> <a href="#af">AF</a> <a href="#as">AS</a> <a href="#oc">OC</a></h3>
    </td>
    </tr>
    <tr>
    <td colspan="2"><a name="eu" id="eu"></a><strong>EUROPA</strong></td>
    </tr>
    <tr>
    <td><a href="http://www.statistik.at/" target="_blank">Statistik Austria</a></td>
    <td>Austria</td>
    </tr>
    <tr>
    <td><a href="http://www.statbel.fgov.be/" target="_blank">National Institute of Statistics</a></td>
    <td>Belgio</td>
    </tr>
    <tr>
    <td><a href="http://www.bhas.ba/" target="_blank">Agency for Statistics of Bosnia and Herzegovina</a></td>
    <td>Bosnia-Erzegovina</td>
    </tr>
    <tr>
    <td><a href="http://www.fzs.ba/Eng/index.htm" target="_blank">Federal Office of Statistic of the Federation</a></td>
    <td>Bosnia-Erzegovina</td>
    </tr>
    <tr>
    <td><a href="http://www.rzs.rs.ba/" target="_blank">Republika Srpska Institute for statistics</a></td>
    <td>Bosnia-Erzegovina</td>
    </tr>
    <tr>
    <td><a href="http://www.nsi.bg/Index_e.htm" target="_blank">National Statistical Institute (NSI)</a></td>
    <td>Bulgaria</td>
    </tr>
    <tr>
    <td><a href="http://www.mof.gov.cy/mof/cystat/statistics.nsf/index_en/index_en?OpenDocument" target="_blank">Statistical Service of Cyprus</a></td>
    <td>Cipro</td>
    </tr>
    <tr>
    <td><a href="http://www.dzs.hr/default_e.htm" target="_blank">Central Bureau of Statistics</a></td>
    <td>Croazia</td>
    </tr>
    <tr>
    <td><a href="http://www.dst.dk/HomeUK.aspx" target="_blank">Statistics Denmark</a></td>
    <td>Danimarca</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.ee/" target="_blank">Statistical Office of Estonia</a></td>
    <td>Estonia</td>
    </tr>
    <tr>
    <td><a href="http://tilastokeskus.fi/index_en.html" target="_blank">Statistics Finland</a></td>
    <td>Finlandia</td>
    </tr>
    <tr>
    <td><a href="http://www.insee.fr/" target="_blank">Institut National de la Statistique et des Études Économiques (INSÉÉ)</a></td>
    <td>Francia</td>
    </tr>
    <tr>
    <td><a href="http://www.ined.fr/" target="_blank">Institut National d'études démographiques (INED)</a></td>
    <td>Francia</td>
    </tr>
    <tr>
    <td><a href="https://www.destatis.de/DE/Startseite.html" target="_blank">Statistisches Bundesamt Deutschland</a></td>
    <td>Germania</td>
    </tr>
    <tr>
    <td><a href="http://www.statistics.gr/" target="_blank">Greek Statistics</a></td>
    <td>Grecia</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.gl/" target="_blank">Statistics Greenland</a></td>
    <td>Groenlandia</td>
    </tr>
    <tr>
    <td><a href="http://www.cso.ie/" target="_blank">Central Statistics Office</a></td>
    <td>Irlanda</td>
    </tr>
    <tr>
    <td><a href="http://www.statice.is/" target="_blank">Statistics Iceland</a></td>
    <td>Islanda</td>
    </tr>
    <tr>
    <td><a href="http://www.csb.lv//avidus.cfm" target="_blank">Central Statistical Bureau of Latvia</a></td>
    <td>Lettonia</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.gov.lt/lt/" target="_blank">Department of Statistics (StD)</a></td>
    <td>Lituania</td>
    </tr>
    <tr>
    <td><a href="http://www.statec.public.lu/en/index.html" target="_blank">STATEC</a></td>
    <td>Lussemburgo</td>
    </tr>
    <tr>
    <td><a href="http://www.nso.gov.mt/" target="_blank">National Statistics Office</a></td>
    <td>Malta</td>
    </tr>
    <tr>
    <td><a href="http://www.statistica.md/" target="_blank">Department of Statistics and Sociological Analysis</a></td>
    <td>Moldova</td>
    </tr>
    <tr>
    <td><a href="http://www.ssb.no/" target="_blank">Statistics Norway (SSB)</a></td>
    <td>Norvegia</td>
    </tr>
    <tr>
    <td><a href="http://www.cbs.nl/" target="_blank">Statistics Netherlands (CBS)</a></td>
    <td>Paesi Bassi</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.gov.pl/" target="_blank">Central Statistical Office (CSO/P)</a></td>
    <td>Polonia</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.pt/" target="_blank">Instituto Nacional de Estatística</a></td>
    <td>Portogallo</td>
    </tr>
    <tr>
    <td><a href="http://www.nrscotland.gov.uk/" target="_blank">National records of Scotland</a></td>
    <td>Regno Unito</td>
    </tr>
    <tr>
    <td><a href="http://www.statisticsauthority.gov.uk/" target="_blank">UK Statistics Authority</a></td>
    <td>Regno Unito</td>
    </tr>
    <tr>
    <td><a href="http://www.ons.gov.uk/ons/index.html" target="_blank">Office for National Statistics (ONS)</a></td>
    <td>Regno Unito</td>
    </tr>
    <tr>
    <td><a href="http://www.czso.cz/" target="_blank">Czech Statistical Office (CzSO)</a></td>
    <td>Repubblica Ceca</td>
    </tr>
    <tr>
    <td><a href="http://portal.statistics.sk/showdoc.do?docid=359" target="_blank">Statistical Office of the Slovak Republic</a></td>
    <td>Repubblica Slovacca</td>
    </tr>
    <tr>
    <td><a href="http://webrzs.stat.gov.rs/WebSite/" target="_blank">Statistical Office of the Republic of Serbia</a></td>
    <td>Repubblica di Serbia</td>
    </tr>
    <tr>
    <td><a href="http://www.insse.ro/" target="_blank">National Institute of Statistics</a></td>
    <td>Romania</td>
    </tr>
    <tr>
    <td><a href="http://www.insse.ro/cms/en" target="_blank">Federal State Statistics Service</a></td>
    <td>Russia</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.si/" target="_blank">Statistical Office of The Republic of Slovenia</a></td>
    <td>Slovenia</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.es/" target="_blank">Instituto Nacional de Estadística (INE)</a></td>
    <td>Spagna</td>
    </tr>
    <tr>
    <td><a href="http://www.juntadeandalucia.es/index.html" target="_blank">Instituto de Estadística de Andalucía (IEA)</a></td>
    <td>Spagna</td>
    </tr>
    <tr>
    <td><a href="http://www.idescat.es/" target="_blank">Institut d'Estadistica de Catalunya</a></td>
    <td>Spagna</td>
    </tr>
    <tr>
    <td><a href="http://www.scb.se/default____2154.aspx" target="_blank">Statistics Sweden (SCB)</a></td>
    <td>Svezia</td>
    </tr>
    <tr>
    <td><a href="http://www.admin.ch/bfs/" target="_blank">Bundesamt für Statistik</a></td>
    <td>Svizzera</td>
    </tr>
    <tr>
    <td><a href="http://www.turkstat.gov.tr/Start.do;jsessionid=h2JTNTNKJ0kgk8MGLMGFZXk8B22K2hNyJY4pYFNvTn1gG69m5LVn!-1793253831" target="_blank">State Institute of Statistics</a></td>
    <td>Turchia</td>
    </tr>
    <tr>
    <td><a href="http://www.ksh.hu/" target="_blank">Hungarian Central Statistical Office (KSH)</a></td>
    <td>Ungheria</td>
    </tr>
    <tr>
    <td colspan="2"></td>
    </tr>
    <tr>
    <td colspan="2"><a name="am" id="am"></a><strong>AMERICA</strong></td>
    </tr>
    <tr>
    <td><a href="http://www.indec.mecon.ar/" target="_blank">Instituto Nacional de Estadística y Censos (INDEC)</a></td>
    <td>Argentina</td>
    </tr>
    <tr>
    <td><a href="http://www.cbs.aw/cbs/home.do" target="_blank">Central Bureau of Statistics</a></td>
    <td>Aruba</td>
    </tr>
    <tr>
    <td><a href="http://www.barstats.gov.bb/" target="_blank">Statistical Services</a></td>
    <td>Barbados</td>
    </tr>
    <tr>
    <td><a href="http://www.ihsn.org/home/" target="_blank">National Institute of Statistics (INE)</a></td>
    <td>Bolivia</td>
    </tr>
    <tr>
    <td><a href="http://www.ibge.gov.br/english/default.php" target="_blank">Brazilian Statistical and Geographic Foundation (IBGE)</a></td>
    <td>Brasile</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.gouv.qc.ca/" target="_blank">Institut de la statistique du Québec</a></td>
    <td>Canada</td>
    </tr>
    <tr>
    <td><a href="http://www.statcan.gc.ca/" target="_blank">Statistics Canada</a></td>
    <td>Canada</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.cl/" target="_blank">National Institute of Statistics (INE)</a></td>
    <td>Cile</td>
    </tr>
    <tr>
    <td><a href="http://www.dane.gov.co/" target="_blank">Departamento Administrativo Nacional de Estadística</a></td>
    <td>Colombia</td>
    </tr>
    <tr>
    <td><a href="http://www.inec.go.cr/" target="_blank">National Institute of Statistics and Censuses</a></td>
    <td>Costa Rica</td>
    </tr>
    <tr>
    <td><a href="http://ghdx.healthdata.org/organizations/national-institute-statistics-and-censuses-ecuador" target="_blank">National Institute of Statistics and Census (INEC)</a></td>
    <td>Ecuador</td>
    </tr>
    <tr>
    <td><a href="http://www.digestyc.gob.sv/">General Directorate of Statistics and Censuses</a></td>
    <td>El Salvador</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.gob.gt/" target="_blank">Instituto Nacional de Estadística</a></td>
    <td>Guatemala</td>
    </tr>
    <tr>
    <td><a href="http://statinja.gov.jm/" target="_blank">Statistical Institute of Jamaica</a></td>
    <td>Giamaica</td>
    </tr>
    <tr>
    <td><a href="http://www.inegi.gob.mx/inegi/default.aspx" target="_blank">National Statistics, Geography and Informatics Institute (INEGI)</a></td>
    <td>Messico</td>
    </tr>
    <tr>
    <td><a href="http://www.dgeec.gov.py/" target="_blank">General Directorate of Statistics, Surveys and Censuses</a></td>
    <td>Paraguay</td>
    </tr>
    <tr>
    <td><a href="http://www.inei.gob.pe/" target="_blank">Instituto Nacional de Estadistica e Informatica (INEI)</a></td>
    <td>Perù</td>
    </tr>
    <tr>
    <td><a href="http://www.one.gob.do/" target="_blank">National Statistics Office</a></td>
    <td>Repubblica Dominicana</td>
    </tr>
    <tr>
    <td><a href="http://www.census.gov/" target="_blank">U.S. Bureau of the Census</a></td>
    <td>Stati Uniti</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.gub.uy/" target="_blank">Statistical Department</a></td>
    <td>Uruguay</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.gov.ve/">Instituto Nacional de Estadìstica</a></td>
    <td>Venezuela</td>
    </tr>
    <tr>
    <td colspan="2"></td>
    </tr>
    <tr>
    <td colspan="2"><a name="af" id="af"></a><strong>AFRICA</strong></td>
    </tr>
    <tr>
    <td><a href="http://www.ons.dz/" target="_blank">Office National de Statistiques (ONS)</a></td>
    <td>Algeria</td>
    </tr>
    <tr>
    <td><a href="http://www.mof.gov.eg/english/Pages/Home.aspx" target="_blank">Ministry of Economy</a></td>
    <td>Egitto</td>
    </tr>
    <tr>
    <td><a href="http://www.nsomalawi.mw/" target="_blank">National Statistical Office</a></td>
    <td>Malawi</td>
    </tr>
    <tr>
    <td><a href="http://www.statistic-hcp.ma/" target="_blank">Statistics Directorate</a></td>
    <td>Marocco</td>
    </tr>
    <tr>
    <td><a href="http://www.ine.gov.mz/" target="_blank">National Institute of Statistics</a></td>
    <td>Mozambico</td>
    </tr>
    <tr>
    <td><a href="http://www.statssa.gov.za/" target="_blank">Central Statistical Service (CSS)</a></td>
    <td>Sud Africa</td>
    </tr>
    <tr>
    <td><a href="http://www.ins.nat.tn/" target="_blank">National Statistics Institute</a></td>
    <td>Tunisia</td>
    </tr>
    <tr>
    <td><a href="http://www.nigerianstat.gov.ng/" target="_blank">National Bureau of Statistics (NBS)</a></td>
    <td>Nigeria</td>
    </tr>
    <tr>
    <td colspan="2"></td>
    </tr>
    <tr>
    <td colspan="2"><a name="as" id="as"></a><strong>ASIA</strong></td>
    </tr>
    <tr>
    <td><a href="http://www.armstat.am/" target="_blank">Ministry of Statistics</a></td>
    <td>Armenia</td>
    </tr>
    <tr>
    <td><a href="http://www.stats.gov.cn/enGliSH/" target="_blank">National Bureau of Statistics</a></td>
    <td>Cina</td>
    </tr>
    <tr>
    <td><a href="http://www.kostat.go.kr/eng/" target="_blank">National Statistical Office</a></td>
    <td>Corea del Sud</td>
    </tr>
    <tr>
    <td><a href="http://www.census.gov.ph/" target="_blank">National Statistical Office</a></td>
    <td>Filippine</td>
    </tr>
    <tr>
    <td><a href="http://www.nscb.gov.ph/" target="_blank">National Statistical Coordination Board (NSCB)</a></td>
    <td>Filippine</td>
    </tr>
    <tr>
    <td><a href="http://www.info.gov.hk/censtatd/" target="_blank">Census and Statistics Department (C&amp;SD)</a></td>
    <td>Hong Kong</td>
    </tr>
    <tr>
    <td><a href="http://www.epa.gov/" target="_blank">Economic Planning Agency (EPA)</a></td>
    <td>Giappone</td>
    </tr>
    <tr>
    <td><a href="http://www.jetro.go.jp/" target="_blank">Japan External Trade Organization (JETRO)</a></td>
    <td>Giappone</td>
    </tr>
    <tr>
    <td><a href="http://www.stat.go.jp/1.htm" target="_blank">Japan Statistics Bureau and Statistics Center</a></td>
    <td>Giappone</td>
    </tr>
    <tr>
    <td><a href="http://www.dos.gov.jo/" target="_blank">Department of Statistics (DOS)</a></td>
    <td>Giordania</td>
    </tr>
    <tr>
    <td><a href="http://censusindia.gov.in/" target="_blank">Census India</a></td>
    <td>India</td>
    </tr>
    <tr>
    <td><a href="http://mospi.nic.in/Mospi_New/site/home.aspx" target="_blank">Department of Statistics (General Statistics)</a></td>
    <td>India</td>
    </tr>
    <tr>
    <td><a href="http://www.bps.go.id/" target="_blank">Biro Pusat Statistik (BPS)</a></td>
    <td>Indonesia</td>
    </tr>
    <tr>
    <td><a href="http://www.amar.org.ir/Default.aspx" target="_blank">Statistical Centre of Iran</a></td>
    <td>Iran</td>
    </tr>
    <tr>
    <td><a href="http://www.cbs.gov.il/" target="_blank">Central Bureau of Statistics (CBS)</a></td>
    <td>Israele</td>
    </tr>
    <tr>
    <td><a href="http://www.ecosn.org/econsos/kazakhstan.aspx#1.%20LINK%20TO%20NATIONAL%20STATISTICAL%20OFFICE%20(NSO)%20WEBSITE">National Statistical Office of Kazakhstan</a></td>
    <td>Kazakhstan</td>
    </tr>
    <tr>
    <td><a href="http://ghdx.healthdata.org/organizations/national-statistical-committee-kyrgyz-republic">National Statistical Committee</a></td>
    <td>Kyrgyzstan</td>
    </tr>
    <tr>
    <td><a href="http://www.csb.gov.kw/">Statistics and Information Sector</a>
    <p><a href="http://www.nsc.gov.la/" target="_blank">Lao Statistic Bureau</a></p>
    </td>
    <td>Kuwait
    <p>Laos</p>
    </td>
    </tr>
    <tr>
    <td><a href="http://www.cas.gov.lb/" target="_blank">Administration Centrale de la Statistique</a></td>
    <td>Libano</td>
    </tr>
    <tr>
    <td><a href="http://www.dsec.gov.mo/" target="_blank">Census and Statistics Department</a></td>
    <td>Macau</td>
    </tr>
    <tr>
    <td><a href="http://www.statistics.gov.my/" target="_blank">Department of Statistics</a></td>
    <td>Malesia</td>
    </tr>
    <tr>
    <td><a href="http://statsmauritius.govmu.org/English/Pages/default.aspx">Central Statistical Office</a></td>
    <td>Mauritius</td>
    </tr>
    <tr>
    <td><a href="http://www.nso.mn/v3/index2.php" target="_blank">National Statistical Office</a></td>
    <td>Mongolia</td>
    </tr>
    <tr>
    <td><a href="http://www.ncsi.gov.om/NCSI_website/N_default.aspx">National Centre for Statistics &amp; Information</a></td>
    <td>Oman</td>
    </tr>
    <tr>
    <td><a href="http://www.pcbs.gov.ps/" target="_blank">Palestinian Central Bureau of Statistics (PCBS)</a></td>
    <td>Palestina</td>
    </tr>
    <tr>
    <td><a href="http://www.singstat.gov.sg/" target="_blank">Statistics Singapore</a></td>
    <td>Singapore</td>
    </tr>
    <tr>
    <td><a href="http://www.statistics.gov.lk/" target="_blank">Department of Census and Statistics</a></td>
    <td>Sri Lanka</td>
    </tr>
    <tr>
    <td><a href="http://web.nso.go.th/" target="_blank">National Statistical Office</a></td>
    <td>Tailandia</td>
    </tr>
    <tr>
    <td><a href="http://eng.stat.gov.tw/mp.asp?mp=5" target="_blank">National Statistics</a></td>
    <td>Taiwan</td>
    </tr>
    <tr>
    <td><a href="http://www.gov.uz/mms100fr.html" target="_blank">Ministry of Macroeconomy and Statistics</a></td>
    <td>Uzbekistan</td>
    </tr>
    <tr>
    <td colspan="2"></td>
    </tr>
    <tr>
    <td colspan="2"><a name="oc" id="oc"></a><strong>OCEANIA</strong></td>
    </tr>
    <tr>
    <td><a href="http://www.abs.gov.au/" target="_blank">Australian Bureau of Statistics (ABS)</a></td>
    <td>Australia</td>
    </tr>
    <tr>
    <td><a href="http://www.stats.govt.nz/" target="_blank">Statistics New Zealand</a></td>
    <td>Nuova Zelanda</td>
    </tr>
    </tbody>
    </table>
    <h3>3. <%= Messages.label_linkAPE %></h3>
    <p><strong>Amministrazioni centrali dello Stato</strong><br>
    <a href="http://www.quirinale.it/" target="_blank"><strong>Presidenza della Repubblica</strong></a><br>
    <a href="http://www.senato.it/" target="_blank"><strong>Senato della Repubblica</strong></a><br>
    <a href="http://www.camera.it/index.asp" target="_blank"><strong>Camera dei Deputati</strong></a><br>
    <strong><a href="http://www.governo.it/">Presidenza del Consiglio dei Ministri</a></strong><br></p>
    <p><strong>Ministeri</strong></p>
    <p><a href="http://www.esteri.it/">Affari Esteri</a><br>
    <a href="http://www.interno.it/">Interno</a><br>
    <a href="http://www.giustizia.it/">Giustizia</a><br>
    <a href="http://www.difesa.it/">Difesa</a><br>
    <a href="http://www.mef.gov.it/">Economia e Finanze</a><br>
    <a href="http://www.sviluppoeconomico.gov.it/">Sviluppo economico</a><br>
    <a href="http://www.mincomes.it/">Commercio internazionale</a><br>
    <a href="http://www.politicheagricole.it/">Politiche Agricole, Alimentari e Forestali</a><br>
    <a href="http://www.minambiente.it/">Ambiente, Tutela del Territorio e del Mare</a><br>
    <a href="http://www.mit.gov.it/mit/site.php">Infrastrutture e Trasporti</a><br>
    <a href="http://www.lavoro.gov.it/lavoro/">Lavoro e Politiche Sociali</a><br>
    <a href="http://www.istruzione.it/web/hub">Istruzione, Università e Ricerca</a><br>
    <a href="http://www.beniculturali.it/">Beni e Attività Culturali</a><br>
    <a href="http://www.salute.gov.it/">Salute</a></p>
    <p><strong>Ministeri senza Portafoglio</strong></p>
    <p><a href="http://www.affariregionali.it/">Rapporti con le Regioni</a><br>
    <a href="http://www.attuazione.it/">Attuazione del programma</a><br>
    <a href="http://www.innovazionepa.gov.it/">Pubblica amministrazione e innovazione</a><br>
    <a href="http://www.pariopportunita.gov.it/">Pari opportunità</a> Rapporti con il Parlamento<br>
    <a href="http://www.politichecomunitarie.it/">Politiche Europee</a> Riforme per il federalismo<br>
    <a href="http://www.politichegiovaniliesport.it/">Politiche per i giovani</a> Semplificazione normativa</p>
    <p><strong>Enti e istituti di ricerca</strong></p>
    <blockquote>
    <p><a href="http://www.cnr.it/" target="_blank">Consiglio Nazionale delle Ricerche (CNR)</a><br>
    <a href="http://www.enea.it/" target="_blank">Ente per le Nuove tecnologie, l'Energia e l'Ambiente (ENEA)</a><br>
    <a href="http://www.inea.it/" target="_blank">Istituto Nazionale di Economia Agraria (INEA)</a><br>
    <a href="http://www.inran.it/" target="_blank">Istituto Nazionale di Ricerca per gli Alimenti e la Nutrizione (INRAN)</a><br>
    <a href="http://www.isfol.it/" target="_blank">Istituto per lo Sviluppo dei Lavoratori (ISFOL)</a><br>
    <a href="http://www.iss.it/">Istituto Superiore di Sanità (ISS)</a><br>
    <a href="http://www.eim.gov.it/" target="_blank">Ente Italiano della Montagna (EIM)</a></p>
    </blockquote>
    <p><strong>Altri enti</strong></p>
    <blockquote>
    <p><a href="http://www.aci.it/" target="_blank">Automobil Club d'Italia (ACI)</a><br>
    <a href="http://www.agea.gov.it/" target="_blank">Agenzia per le Erogazioni in Agricoltura (AGEA)</a><br>
    <a href="http://www.apat.gov.it/" target="_blank">Agenzia per la protezione dell'ambiente e per i servizi tecnici (ISPRA)</a><br>
    <a href="http://www.cnel.it/" target="_blank">Consiglio nazionale dell'economia e del lavoro (CNEL)</a><br>
    <a href="http://www.coni.it/" target="_blank">Comitato olimpico nazionale italiano (CONI)</a><br>
    <a href="http://www.enasarco.it/" target="_blank">Fondazione Enasarco - Ente nazionale di assistenza Agenti e Rappresentanti di commercio</a><br>
    <a href="http://www.grtn.it/" target="_blank">Gestore della Rete di Trasmissione Nazionale S.p.A.</a><br>
    <a href="http://www.ice.it/" target="_blank">Istituto nazionale per il Commercio Estero (ICE)</a><br>
    <a href="http://www.inail.it/" target="_blank">Istituto Nazionale Assicurazione Infortuni sul Lavoro (INAIL)</a><br>
    <a href="http://www.inpdap.it/" target="_blank">Istituto Nazionale di Previdenza per i dipendenti dell'Amministrazione Pubblica (INPDAP)</a><br>
    <a href="http://www.inps.it/" target="_blank">Istituto Nazionale di Previdenza Sociale (INPS)</a><br>
    <a href="http://www.ismea.it/" target="_blank">Istituto di servizi per il mercato agricolo alimentare (ISMEA)</a><br>
    <a href="http://www.isvap.it/" target="_blank">Istituto per la Vigilanza sulle Assicurazioni Private e di Interesse Collettivo (ISVAP)</a><br>
    <a href="http://www.aranagenzia.it/" target="_blank">Agenzia per la rappresentanza negoziale nella Pubblica Amministrazione (ARAN)</a><br>
    <a href="http://www.cnipa.gov.it/" target="_blank">Centro nazionale per l'informatica nella Pubblica Amministrazione (CNIPA)</a><br>
    <a href="http://www.autoritalavoripubblici.it/" target="_blank">Autorità per la vigilanza sui lavori pubblici</a><br>
    <a href="http://www.garanteprivacy.it/" target="_blank">Garante per la protezione dei dati personali</a></p>
    </blockquote>
    <p><strong>Amministrazioni locali</strong></p>
    <blockquote>
    <p><a href="http://www.regione.abruzzo.it/" target="_blank">Regione Abruzzo</a><br>
    <a href="http://www.regione.basilicata.it/" target="_blank">Regione Basilicata</a><br>
    <a href="http://www.regione.calabria.it/" target="_blank">Regione Calabria</a><br>
    <a href="http://www.regione-campania.com/" target="_blank">Regione Campania</a><br>
    <a href="http://www.regione.emilia-romagna.it/" target="_blank">Regione Emilia Romagna</a><br>
    <a href="http://www.regione.fvg.it/" target="_blank">Regione Friuli-Venezia Giulia</a><br>
    <a href="http://www.regione.lazio.it/" target="_blank">Regione Lazio</a><br>
    <a href="http://www.regione.liguria.it/" target="_blank">Regione Liguria</a><br>
    <a href="http://www.regione.lombardia.it/" target="_blank">Regione Lombardia</a><br>
    <a href="http://www.regione.marche.it/" target="_blank">Regione Marche</a><br>
    <a href="http://www.molisedati.it/regione.htrm" target="_blank">Regione Molise</a><br>
    <a href="http://www.regione.piemonte.it/" target="_blank">Regione Piemonte</a><br>
    <a href="http://power.peg.it/taras/regio.html" target="_blank">Regione Puglia</a><br>
    <a href="http://www.regione.sardegna.it/" target="_blank">Regione Sardegna</a><br>
    <a href="http://www.regione.sicilia.it/" target="_blank">Regione Sicilia</a><br>
    <a href="http://www.regione.toscana.it/" target="_blank">Regione Toscana</a><br>
    <a href="http://www.tqs.it/regione/index.html" target="_blank">Regione Trentino-Alto Adige</a><br>
    <a href="http://www.regione.umbria.it/" target="_blank">Regione Umbria</a><br>
    <a href="http://www.regione.vda.it/" target="_blank">Regione Valle d'Aosta</a><br>
    <a href="http://www.regione.veneto.it/" target="_blank">Regione Veneto</a><br>
    <a href="http://www.provincia.bz.it/" target="_blank">Provincia autonoma di Bolzano</a><br>
    <a href="http://www.provincia.tn.it/" target="_blank">Provincia autonoma di Trento</a><br>
    <a href="http://www.provincia.mantova.it/statistica/sisp/sthome.htm" target="_blank">Provincia di Mantova (Servizio Statistica)</a></p>
    </blockquote>
    <p><strong>Camere di Commercio</strong></p>
    <blockquote>
    <p><a href="http://www.cameradicommercio.it/" target="_blank">Portale delle Camere di commercio</a><br>
    <a href="http://www.al.camcom.it/" target="_blank">Alessandria</a><br>
    <a href="http://www.ats.it/camcom/" target="_blank">Arezzo</a><br>
    <a href="http://www.rinascita.it/PROVIAP/cameic/camcomap.htm" target="_blank">Ascoli Piceno</a><br>
    <a href="http://www.at.camcom.it/" target="_blank">Asti</a><br>
    <a href="http://www.netcafe.interbusiness.it/unioncamere/ba/pagba2.html" target="_blank">Bari</a><br>
    <a href="http://www.bg.camcom.it/" target="_blank">Bergamo</a><br>
    <a href="http://www.bi.camcom.it/" target="_blank">Biella</a><br>
    <a href="http://www.hk-cciaa.bz.it/" target="_blank">Bolzano</a><br>
    <a href="http://www.bs.camcom.it/" target="_blank">Brescia</a><br>
    <a href="http://www.netcafe.interbusiness.it/unioncamere/br/pagbr1.html" target="_blank">Brindisi</a><br>
    <a href="http://www.ce.camcom.it/" target="_blank">Caserta</a><br>
    <a href="http://www.co.camcom.it/" target="_blank">Como</a><br>
    <a href="http://www.cr.camcom.it/" target="_blank">Cremona</a><br>
    <a href="http://www.cn.camcom.it/" target="_blank">Cuneo</a><br>
    <a href="http://www.fe.camcom.it/" target="_blank">Ferrara</a><br>
    <a href="http://www.fi.camcom.it/" target="_blank">Firenze</a><br>
    <a href="http://www.netcafe.interbusiness.it/unioncamere/fg/pagfg1.html" target="_blank">Foggia</a><br>
    <a href="http://www.fo.camcom.it/" target="_blank">Forli-Cesena</a><br>
    <a href="http://www.ouverture.it/camcom-grosseto/" target="_blank">Grosseto</a><br>
    <a href="http://www.sp.camcom.it/" target="_blank">La Spezia</a><br>
    <a href="http://www.netcafe.interbusiness.it/unioncamere/le/pagle1.html" target="_blank">Lecce</a><br>
    <a href="http://www.lc.camcom.it/" target="_blank">Lecco</a><br>
    <a href="http://www.lo.camcom.it/" target="_blank">Lodi</a><br>
    <a href="http://www.lunet.it/enti/cciaa/welcome.html" target="_blank">Lucca</a><br>
    <a href="http://www.mi.camcom.it/" target="_blank">Milano</a><br>
    <a href="http://mo.nettuno.it/fiera/cciaamo" target="_blank">Modena</a><br>
    <a href="http://www.no.camcom.it/" target="_blank">Novara</a><br>
    <a href="http://www.iperv.it/CCIAA/index.html" target="_blank">Padova</a><br>
    <a href="http://www.pv.camcom.it/" target="_blank">Pavia</a><br>
    <a href="http://www.rgn.it/~CCIAA/home.html" target="_blank">Pescara</a><br>
    <a href="http://www.rn.camcom.it/" target="_blank">Rimini</a><br>
    <a href="http://www.rm.camcom.it/" target="_blank">Roma</a><br>
    <a href="http://www.adigecolli.it/soci/cciaaro/polinn.htm" target="_blank">Rovigo</a><br>
    <a href="http://www.so.camcom.it/" target="_blank">Sondrio</a><br>
    <a href="http://www.netcafe.interbusiness.it/unioncamere/ta/pagta1.html" target="_blank">Taranto</a><br>
    <a href="http://www.to.camcom.it/" target="_blank">Torino</a><br>
    <a href="http://www.ts.camcom.it/" target="_blank">Trieste</a><br>
    <a href="http://www.ud.camcom.it/" target="_blank">Udine</a><br>
    <a href="http://www.va.camcom.it/" target="_blank">Varese</a><br>
    <a href="http://www.vb.camcom.it/" target="_blank">Verbania-Cusio-Ossola</a><br>
    <a href="http://www.vc.camcom.it/" target="_blank">Vercelli</a></p>
    </blockquote>
    <p><strong>UnionCamere regionali</strong></p>
    <blockquote>
    <p><a href="http://www.unioncamere.it/" target="_blank">UnionCamere Nazionale</a><br>
    <a href="http://www.rer.camcom.it/" target="_blank">Unioncamere Emilia Romagna</a><br>
    <a href="http://www.lig.camcom.it/" target="_blank">Unioncamere Liguria</a><br>
    <a href="http://www.lom.camcom.it/" target="_blank">Unioncamere Lombardia</a><br>
    <a href="http://www.pie.camcom.it/" target="_blank">Unioncamere Piemonte</a><br>
    <a href="http://www.unioncamerepuglia.it/" target="_blank">Unioncamere Puglia</a><br>
    <a href="http://www.ven.camcom.it/" target="_blank">Unioncamere Veneto</a><br>
    <a href="http://www.camcom.it/">Portale del sistema delle camere di commercio</a></p>
    </blockquote>
    <h3>4. <%= Messages.label_linkOS %></h3>
    <p><a href="http://www.sistan.it/index.php">Sistan</a><br>
    <a href="http://www.dati.gov.it">Open data della PA</a><br>
    <a href="http://www.agid.gov.it">Agenzia per l'Italia digitale</a><br>
    <a href="http://www.magellanopa.it/bussola">Bussola della trasparenza dei siti web della PA</a><br>
    <a href="http://www.bancaditalia.it/">Banca d'Italia</a><br>
    <a href="http://www.europarl.europa.eu/">Parlamento europeo</a><br>
    <a href="http://www.consilium.europa.eu/">Consiglio dell'Unione Europea</a><br>
    <a href="http://ec.europa.eu/">Commissione Europea</a><br>
    <a href="http://europa.eu/">Unione Europea</a><a href="http://epp.eurostat.ec.europa.eu/portal/page/portal/pgp_ess/ess/ess_news"></a><br>
    <a href="http://eu2014.istat.it/" target="_blank">Presidenza italiana del Consiglio dell'UE 2014</a><br>
    <a href="http://portale.ancitel.it/" target="_blank">Rete telematica dei comuni italiani (ANCITEL</a><br>
    <a href="http://www.upinet.it/" target="_blank">Unione delle province italiane (UPINET)</a><br>
    <a href="http://www.gazzettaufficiale.it/" target="_blank">Gazzetta Ufficiale</a></p>
</div>
</asp:Content>

<asp:Content ID="Footercontainer" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
