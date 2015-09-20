using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Estat.Sri.CustomRequests.Constants;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class DOTSTAT_Widget
    {
        private GetCodemapObject CodemapObj { get; set; }
        internal SessionImplObject SessionObj { get; set; }
        private IGetSDMX GetSDMXObject = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CodemapWidget));
        private const string ErrorOccured = "{\"error\" : true }";


        public DOTSTAT_Widget(GetCodemapObject codemapObj, SessionImplObject sessionObj)
        {

            Org.Sdmxsource.Sdmx.Api.Exception.SdmxException.SetMessageResolver(new Org.Sdmxsource.Util.ResourceBundle.MessageDecoder());

            CodemapObj = codemapObj;
            SessionObj = sessionObj;
            GetSDMXObject = ISTAT.WebClient.WidgetEngine.Model.WebServiceSelector.GetSdmxImplementation(this.CodemapObj.Configuration);
        }

        public SessionImplObject GetCodemap()
        {
            try
            {
                return this.SessionObj;
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn(Resources.ErrorMaxJsonLength);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(Resources.ErrorRecursionLimit);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
        }
        public SessionImplObject Get_DOTSTAT_CodemapAndLayout(ConnectionStringSettings connectionStringSetting)
        {
            try
            {
                ISdmxObjects structure = GetDsd();
                //IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");

                Dictionary<string, ICodelistObject> ConceptCodelists = GetCodeMap(structure,kf, false);

                string costraint = string.Empty; //the code save in template
                string hideDimension = string.Empty;

                System.Data.SqlClient.SqlConnection Sqlconn = new System.Data.SqlClient.SqlConnection(connectionStringSetting.ConnectionString);
                Sqlconn.Open();
                string sqlquery = string.Format("Select * from Template where [tmplKey]='{0}'",
                    new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    CodemapObj.Dataflow.id + "+" + CodemapObj.Dataflow.agency + "+" + CodemapObj.Dataflow.version + "+" + CodemapObj.Configuration.EndPoint).Replace("'", "''"));
                using (System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand(sqlquery, Sqlconn))
                {
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        costraint = reader.GetString(reader.GetOrdinal("Criteria"));
                        hideDimension = reader.GetString(reader.GetOrdinal("HideDimension"));
                    }
                }
                Sqlconn.Close();

                Dictionary<string, List<string>> _costraint = (Dictionary<string, List<string>>)new JavaScriptSerializer().Deserialize(costraint, typeof(Dictionary<string, List<string>>));
                List<string> _hideDimension = (List<string>)new JavaScriptSerializer().Deserialize(hideDimension, typeof(List<string>));
                CodemapResponseObject codemapret = new CodemapResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists),
                    costraint = _costraint,
                    hideDimension = _hideDimension,
                    key_time_dimension = kf.TimeDimension.Id,
                    freq_dimension = null,
                    dataflow = new MaintenableObj()
                    {
                        id = kf.Id,
                        agency = kf.AgencyId,
                        version = kf.Version,
                        name = TextTypeHelper.GetText(kf.Names, this.CodemapObj.Configuration.Locale),
                        description = TextTypeHelper.GetText(kf.Descriptions, this.CodemapObj.Configuration.Locale)
                    }
                };

                this.SessionObj.SavedCodemap = new JavaScriptSerializer().Serialize(codemapret);
                
                return this.SessionObj;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
                                          
            }
        }
        public Dictionary<string, ICodelistObject> GetCodeMap(ISdmxObjects sdmxObjects, IDataStructureObject kf, bool withAttribute)
        {
            Dictionary<string, ICodelistObject> Conceptcodelist = new Dictionary<string, ICodelistObject>();
            if (kf != null)
            {
                foreach (IDimension component 
                    in kf.DimensionList.Dimensions.Where(c => c.HasCodedRepresentation() 
                        && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                {
                    var codelist = (from c in sdmxObjects.Codelists where c.Id == component.Representation.CrossReferences.First().MaintainableId select c).FirstOrDefault();
                    if (codelist != null)
                        Conceptcodelist.Add(component.Id, codelist);
                }

                var time_period = (from c in kf.DimensionList.Dimensions where c.TimeDimension == true select c).FirstOrDefault();
                var time_period_codelist = (from c in sdmxObjects.Codelists where c.CrossReferences.FirstOrDefault().MaintainableId == time_period.CrossReferences.FirstOrDefault().MaintainableId select c).FirstOrDefault();
                            
                

                if (withAttribute && kf.AttributeList != null)
                {
                    foreach (IComponent component in kf.AttributeList.Attributes.Where(c => c.HasCodedRepresentation() && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                    {
                        var codelist = (from c in sdmxObjects.Codelists where c.Id == component.Representation.CrossReferences.First().MaintainableId select c).FirstOrDefault();
                        if (codelist != null)
                            Conceptcodelist.Add(component.Id, codelist);
                    }

                }

                if (this.SessionObj == null) this.SessionObj = new SessionImplObject();
                if (this.SessionObj.CodelistConstrained == null) this.SessionObj.CodelistConstrained = new Dictionary<string, Dictionary<string, ICodelistObject>>();
                this.SessionObj.CodelistConstrained[Utils.MakeKey(kf)] = Conceptcodelist;
            }
            return Conceptcodelist;
        }

        public ISdmxObjects GetDsd()
        {
            try
            {

                var Structure = GetSDMXObject.GetDsd(this.CodemapObj.Dataflow.id, this.CodemapObj.Dataflow.agency, this.CodemapObj.Dataflow.version);

                return Structure;

            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);

            }
            return null;
        }

        public Dictionary<string, CodemapObj> ParseCodelist(Dictionary<string, ICodelistObject> Codemap)
        {
            Dictionary<string, CodemapObj> drcodemap = new Dictionary<string, CodemapObj>();
            foreach (string conceptId in Codemap.Keys)
            {
                List<string> criteri = new List<string>();

                ICodelistObject codelist = Codemap[conceptId];
                CodemapObj codemap = new CodemapObj() { title = TextTypeHelper.GetText(codelist.Names, this.CodemapObj.Configuration.Locale), codes = new Dictionary<string, CodeObj>() };

                foreach (ICode codeItem in codelist.Items.Where(ci => (criteri.Count > 0 ? criteri.Contains(ci.Id) : true)))
                    codemap.codes.Add(
                        codeItem.Id.ToString(),
                        new CodeObj()
                        {
                            name = TextTypeHelper.GetText(codeItem.Names, this.CodemapObj.Configuration.Locale),
                            parent = codeItem.ParentCode
                        });

                codemap.codes = codemap.codes.OrderBy(c => c.Value.parent == null).ToDictionary(c => c.Key, c => c.Value);
                drcodemap.Add(conceptId, codemap);
            }
            return drcodemap;
        }

        private ICodelistObject GetTimeCodeList(ISdmxObjects sdmxObjects, ICodelistObject FreqCodelist, IDataStructureObject kf)
        {

            if (this.SessionObj.CodelistConstrained != null && this.SessionObj.CodelistConstrained.ContainsKey(Utils.MakeKey(kf))
                && this.SessionObj.CodelistConstrained[Utils.MakeKey(kf)].ContainsKey(kf.TimeDimension.Id))
            {
                ICodelistObject codes = this.SessionObj.CodelistConstrained[Utils.MakeKey(kf)][kf.TimeDimension.Id];
                if (codes != null)
                    return codes;
            }
            var codelist = (from c in sdmxObjects.Codelists where c.Id == kf.TimeDimension.Id select c).FirstOrDefault();
            ICodelistObject CL_TIME_MA = codelist;
            if (CL_TIME_MA == null 
                || CL_TIME_MA.Items == null 
                || CL_TIME_MA.Items.Count != 2)
                return CL_TIME_MA;

            //string format_time = (CL_TIME_MA.Items[0].Id.Contains("-")) ? "yyyy-MM-dd" : "yyyy";
            string time_min_normal = CL_TIME_MA.Items[0].Id;
            string time_max_normal = CL_TIME_MA.Items[1].Id;

            string FrequencyDominant = "A";
            if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "S") > 0) FrequencyDominant = "S";
            if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "Q") > 0) FrequencyDominant = "Q";
            if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "M") > 0) FrequencyDominant = "M";

            if (!CL_TIME_MA.Items[0].Id.Contains("-"))
            {
                time_min_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[0].Id, "01", "01");
                time_max_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[1].Id, "01", "01");
            }
            else
            {
                var time_p_c = CL_TIME_MA.Items[0].Id.Split('-');
                if (time_p_c.Length == 2)
                {
                    int mul =
                        (FrequencyDominant == "M") ? 1 :
                        (FrequencyDominant == "Q") ? 3 :
                        (FrequencyDominant == "S") ? 6 : 0;

                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    time_p_c = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("S"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("Q"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("M"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
            }


            DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
            DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

            ICodelistMutableObject CL_TIME = new CodelistMutableCore();
            CL_TIME.Id = CL_TIME_MA.Id;
            CL_TIME.AgencyId = CL_TIME_MA.AgencyId;
            CL_TIME.Version = CL_TIME_MA.Version;

            CL_TIME_MA.Names.ToList().ForEach(n => CL_TIME.AddName(n.Locale, n.Value));

            DateTime ActualDate = MinDate;
            switch (FrequencyDominant)
            {
                case "A":
                    #region Aggiungo gli Annual
                    while (ActualDate.CompareTo(MaxDate) < 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddYears(1);
                    }
                    #endregion
                    break;
                case "S":
                    #region Aggiungo gli Semestrali
                    while (ActualDate.CompareTo(MaxDate) < 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-S" + (ActualDate.Month < 6 ? "1" : "2");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(6);
                    }
                    #endregion
                    break;
                case "Q":
                    #region Aggiungo i Quartely
                    while (ActualDate.CompareTo(MaxDate) < 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-Q" + ((ActualDate.Month - 1) / 3 + 1).ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(3);
                    }
                    #endregion
                    break;
                case "M":
                    #region Aggiungo i Mensili
                    while (ActualDate.CompareTo(MaxDate) < 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.ToString("yyyy-MM");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(1);
                    }
                    #endregion
                    break;
                default:
                    break;
            }

            return CL_TIME.ImmutableInstance;
        }

    }
}
