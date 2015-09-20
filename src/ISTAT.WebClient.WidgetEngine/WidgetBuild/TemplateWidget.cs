using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class TemplateWidget
    {
        public SqlConnection Sqlconn { get; set; }
        private const string ErrorOccured = "{\"error\" : true }";
        private const string ErrorOccuredMess = "{\"error\" : true, \"message\" : {0} }";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryWidget));

        public TemplateWidget(string connectionString)
        {
            Sqlconn = new SqlConnection(connectionString);
        }

        public List<TemplateObject> Get(GetTemplateObject PostDataArrived)
        {

            try
            {

                //if (PostDataArrived == null

                //    || PostDataArrived.Template == null)

                //    throw new Exception("Input Error");



                Sqlconn.Open();

                try
                {

                    List<TemplateObject> tmpListTemplateObjects = new List<TemplateObject>();

                    string sqlquery;

                    SqlDataReader dtReader;

                    sqlquery = "Select * from Template";

                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {

                        dtReader = comm.ExecuteReader();

                        while (dtReader.Read())
                        {

                            TemplateObject tmpObj = new TemplateObject();

                            tmpObj.TemplateId = dtReader["TemplateId"].ToString();

                            tmpObj.Title = (string)dtReader["Title"];

                            tmpObj.Configuration = new JavaScriptSerializer().Deserialize<EndpointSettings>((string)dtReader["Configuration"]);

                            tmpObj.Dataflow = new JavaScriptSerializer().Deserialize<MaintenableObj>((string)dtReader["Dataflow"]);

                            tmpObj.Criteria = new JavaScriptSerializer().Deserialize<Dictionary<string, List<string>>>((string)dtReader["Criteria"]);

                            tmpObj.Layout = new JavaScriptSerializer().Deserialize<LayoutObj>((string)dtReader["Layout"]);

                            tmpObj.HideDimension = new JavaScriptSerializer().Deserialize<List<string>>((string)dtReader["HideDimension"]);

                            tmpObj.BlockXAxe = (bool)dtReader["BlockXAxe"];

                            tmpObj.BlockYAxe = (bool)dtReader["BlockYAxe"];

                            tmpObj.BlockZAxe = (bool)dtReader["BlockZAxe"];

                            tmpObj.EnableCriteria = (bool)dtReader["EnableCriteria"];
                            tmpObj.EnableVaration = (bool)dtReader["EnableVaration"];
                            tmpObj.EnableDecimal = (bool)dtReader["EnableDecimal"];
                            


                            tmpListTemplateObjects.Add(tmpObj);

                        }



                    }

                    dtReader.Close();

                    return tmpListTemplateObjects;

                }

                catch (Exception) { throw; }

                finally
                {

                    Sqlconn.Close();

                }

            }

            catch (Exception ex)
            {

                Logger.Warn(ex.Message, ex);

                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));

            }

        }

        public TemplateObject GetSingle(GetTemplateObject PostDataArrived)
        {

            try
            {
                Sqlconn.Open();
                try
                {
                    TemplateObject tmpObj = null;
                    SqlDataReader dtReader = null;

                    string sqlquery = string.Format("Select * from Template where [tmplKey]='{0}'",
                        new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        PostDataArrived.Template.Dataflow.id
                        + "+" + PostDataArrived.Template.Dataflow.agency
                        + "+" + PostDataArrived.Template.Dataflow.version
                        + "+" + PostDataArrived.Template.Configuration.EndPoint).Replace("'", "''"));
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        dtReader = comm.ExecuteReader();
                        if(dtReader.Read())
                        {
                            tmpObj = new TemplateObject();
                            tmpObj.TemplateId = dtReader["TemplateId"].ToString();
                            tmpObj.Title = (string)dtReader["Title"];
                            tmpObj.Configuration = new JavaScriptSerializer().Deserialize<EndpointSettings>((string)dtReader["Configuration"]);
                            tmpObj.Dataflow = new JavaScriptSerializer().Deserialize<MaintenableObj>((string)dtReader["Dataflow"]);
                            tmpObj.Criteria = new JavaScriptSerializer().Deserialize<Dictionary<string, List<string>>>((string)dtReader["Criteria"]);
                            tmpObj.Layout = new JavaScriptSerializer().Deserialize<LayoutObj>((string)dtReader["Layout"]);
                            tmpObj.HideDimension = new JavaScriptSerializer().Deserialize<List<string>>((string)dtReader["HideDimension"]);
                            tmpObj.BlockXAxe = (bool)dtReader["BlockXAxe"];
                            tmpObj.BlockYAxe = (bool)dtReader["BlockYAxe"];
                            tmpObj.BlockZAxe = (bool)dtReader["BlockZAxe"];
                            tmpObj.EnableCriteria = (bool)dtReader["EnableCriteria"];
                            tmpObj.EnableVaration = (bool)dtReader["EnableVaration"];
                            tmpObj.EnableDecimal = (bool)dtReader["EnableDecimal"];
                        }
                    }
                    dtReader.Close();
                    return tmpObj;
                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }

            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }

        }


        public TemplateObject Add(GetTemplateObject PostDataArrived)
        {
            try
            {
                if (PostDataArrived == null
                    || PostDataArrived.Template == null)
                    throw new Exception("Input Error");

                Sqlconn.Open();
                try
                {
                    string sqlquery;
                    int templateId = -1;
                    string sqlqueryCount = string.Format("Select [TemplateId] from Template where [tmplKey]='{0}'",
                        new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        PostDataArrived.Template.Dataflow.id
                        + "+" + PostDataArrived.Template.Dataflow.agency
                        + "+" + PostDataArrived.Template.Dataflow.version
                        + "+" + PostDataArrived.Template.Configuration.EndPoint).Replace("'", "''"));
                        
                    using (SqlCommand comm = new SqlCommand(sqlqueryCount, Sqlconn))
                    {
                        var tmplId = comm.ExecuteScalar();
                        if (tmplId != null)
                            templateId = (int)tmplId;
                    }

                    if (templateId >= 0)
                    {
                        // update
                        sqlquery = string.Format(@"UPDATE Template SET [Title]='{0}' ,[Dataflow]='{1}' ,[Criteria]='{2}' ,[Layout]='{3}', [Configuration]='{4}',[tmplKey]='{5}',[HideDimension]='{6}',[BlockXAxe]={7},[BlockYAxe]={8},[BlockZAxe]={9},[EnableCriteria]={10},[EnableDecimal]={11},[EnableVaration]={12} WHERE [TemplateId]={13}",
                                    PostDataArrived.Template.Title.Replace("'", "''"),
                                    PostDataArrived.Template._DataflowString,
                                    PostDataArrived.Template._CriteriaString,
                                    PostDataArrived.Template._LayoutString,
                                    PostDataArrived.Template._ConfigurationString,
                                    PostDataArrived.Template._QueryUniqueKeyString,
                                    PostDataArrived.Template._HideDimensionString,
                                    (PostDataArrived.Template.BlockXAxe) ? 1 : 0,
                                    (PostDataArrived.Template.BlockYAxe) ? 1 : 0,
                                    (PostDataArrived.Template.BlockZAxe) ? 1 : 0,
                                    (PostDataArrived.Template.EnableCriteria) ? 1 : 0,
                                    (PostDataArrived.Template.EnableDecimal) ? 1 : 0,
                                    (PostDataArrived.Template.EnableVaration) ? 1 : 0,      
                                    templateId);

                        using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                        {
                            int resAdd = comm.ExecuteNonQuery();
                            if (resAdd == 0)
                                throw new Exception("Template not update");
                        }
                    }
                    else {
                        // new     
                        sqlquery = string.Format(@"INSERT INTO Template ([Title] ,[Dataflow] ,[Criteria] ,[Layout], [Configuration],[HideDimension],[BlockXAxe],[BlockYAxe],[BlockZAxe],[EnableCriteria],[EnableDecimal],[EnableVaration],[tmplKey])
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},{9},{10},{11},'{12}')",
                                    PostDataArrived.Template.Title.Replace("'", "''"),
                                    PostDataArrived.Template._DataflowString,
                                    PostDataArrived.Template._CriteriaString,
                                    PostDataArrived.Template._LayoutString,
                                    PostDataArrived.Template._ConfigurationString,
                                    PostDataArrived.Template._HideDimensionString,
                                    (PostDataArrived.Template.BlockXAxe)?1:0,
                                    (PostDataArrived.Template.BlockYAxe) ? 1 : 0,
                                    (PostDataArrived.Template.BlockZAxe) ? 1 : 0,
                                    (PostDataArrived.Template.EnableCriteria) ? 1 : 0,
                                    (PostDataArrived.Template.EnableDecimal) ? 1 : 0,
                                    (PostDataArrived.Template.EnableVaration) ? 1 : 0,   
                                    PostDataArrived.Template._QueryUniqueKeyString
                                    );

                        using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                        {
                            int resAdd = comm.ExecuteNonQuery();
                            if (resAdd == 0)
                                throw new Exception("Template not insert");
                        }
                    }
                    sqlquery = string.Format("Select [TemplateId] from Template where [tmplKey]='{0}'",
                        PostDataArrived.Template._QueryUniqueKeyString.Replace("'", "''"));
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        PostDataArrived.Template.TemplateId = comm.ExecuteScalar().ToString();
                    }
                    return PostDataArrived.Template;
                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }

        public string Delete(GetTemplateObject PostDataArrived)
        {
            try
            {
                if (string.IsNullOrEmpty(PostDataArrived.TemplateId))
                    throw new Exception("Input Error");

                string sqlquery = string.Format(@"DELETE FROM Template WHERE [TemplateId]={0}", PostDataArrived.TemplateId);

                Sqlconn.Open();
                try
                {
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        int resAdd = comm.ExecuteNonQuery();
                        if (resAdd == 0)
                            throw new Exception("Template not delete");
                    }
                    return "{\"DeleteResult\" : true,  }";
                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }

    }
}
