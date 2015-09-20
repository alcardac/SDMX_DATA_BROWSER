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
    public class QueryWidget
    {
        public SqlConnection Sqlconn { get; set; }
        private const string ErrorOccured = "{\"error\" : true }";
        private const string ErrorOccuredMess = "{\"error\" : true, \"message\" : {0} }";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryWidget));

        public QueryWidget(string connectionString)
        {
            Sqlconn = new SqlConnection(connectionString);
        }

        public List<QueryObject> Get(GetQueryObject PostDataArrived)
        {
            try
            {
                if (PostDataArrived == null || string.IsNullOrEmpty(PostDataArrived.UserCode))
                    throw new Exception("Input Error");

                string sqlquery = string.Format("Select * from Query where UserCode='{0}'", PostDataArrived.UserCode.Replace("'", "''"));
                if (!string.IsNullOrEmpty(PostDataArrived.QueryId))
                    sqlquery += string.Format(" and QueryId={0}", PostDataArrived.QueryId);

                Sqlconn.Open();
                try
                {
                    DataTable dtres = new DataTable();
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            da.Fill(dtres);
                        }
                    }

                    List<QueryObject> queries = new List<QueryObject>();
                    foreach (DataRow query in dtres.Rows)
                    {
                        queries.Add(new QueryObject()
                        {
                            QueryId = query["QueryId"].ToString(),
                            Title = query["Title"].ToString(),
                            _DataflowString = query["Dataflow"].ToString(),
                            _CriteriaString = query["Criteria"].ToString(),
                            _ConfigurationString = query["Configuration"].ToString(),
                            _LayoutString = query["Layout"].ToString(),
                        });
                    }
                    return queries;
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

        public QueryObject Add(GetQueryObject PostDataArrived)
        {
            try
            {
                if (PostDataArrived == null || string.IsNullOrEmpty(PostDataArrived.UserCode) || PostDataArrived.Query == null)
                    throw new Exception("Input Error");

                string sqlquery = string.Format(@"INSERT INTO Query ([UserCode] ,[Title] ,[Dataflow] ,[Criteria] ,[Layout], [Configuration])
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                            PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.Query.Title.Replace("'", "''"), PostDataArrived.Query._DataflowString, PostDataArrived.Query._CriteriaString, PostDataArrived.Query._LayoutString, PostDataArrived.Query._ConfigurationString);

                Sqlconn.Open();
                try
                {
                    string sqlqueryCount = string.Format("Select count(*) from Query where UserCode='{0}' And Title='{1}'", PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.Query.Title.Replace("'", "''"));
                    using (SqlCommand comm = new SqlCommand(sqlqueryCount, Sqlconn))
                    {
                        int count = (int)comm.ExecuteScalar();
                        if (count > 0)
                            throw new Exception("Already exist Query for this user with this title");
                    }
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        int resAdd = comm.ExecuteNonQuery();
                        if (resAdd == 0)
                            throw new Exception("Query not insert");
                    }
                    sqlquery = string.Format("Select QueryId from Query where UserCode='{0}' And Title='{1}'", PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.Query.Title.Replace("'", "''"));
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        PostDataArrived.Query.QueryId = comm.ExecuteScalar().ToString();
                    }
                    return PostDataArrived.Query;
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

        public string Delete(GetQueryObject PostDataArrived)
        {
            try
            {
                if (string.IsNullOrEmpty(PostDataArrived.QueryId))
                    throw new Exception("Input Error");

                string sqlquery = string.Format(@"DELETE FROM Query WHERE QueryId={0}", PostDataArrived.QueryId);

                Sqlconn.Open();
                try
                {

                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        int resAdd = comm.ExecuteNonQuery();
                        if (resAdd == 0)
                            throw new Exception("Query not insert");
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
