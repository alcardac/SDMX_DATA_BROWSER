using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetComplements.Model
{
    public class CacheTree
    {
        private SqlConnection Sqlconn { get; set; }
        private EndpointSettings conf { get; set; }
        private ISdmxObjects sdmxOBJ { get; set; }

        public CacheTree(string ConnectionString, EndpointSettings config)
        {
            Sqlconn = new SqlConnection(ConnectionString);
            conf = config;
        }

        #region Cache
        public string GetCachedTree()
        {
            lock (Sqlconn)
            {
                var ser = new JavaScriptSerializer();
                try
                {
                    string ConfStr = ser.Serialize(conf);
                    string sqlquery = string.Format("Select SavedTreeJson from SavedTree where Configuration='{0}'", ConfStr);
                    Sqlconn.Open();
                    DataTable dtres = new DataTable();
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            da.Fill(dtres);
                        }
                    }
                    if (dtres == null || dtres.Rows.Count == 0 || dtres.Rows[0][0] == null)
                        return null;

                    string JsonTree = dtres.Rows[0][0].ToString();

                    //ISdmxObjects ret = GetSdmxOBJ(dtres.Rows[0][0].ToString());
                    try
                    {
                        string sqlupd = string.Format("Update SavedTree set LastRequest='{1}' where Configuration='{0}'", ConfStr, DateTime.Now.ToString("yyyyMMdd HHmm"));
                        using (SqlCommand commupd = new SqlCommand(sqlupd, Sqlconn))
                            commupd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    return JsonTree;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    Sqlconn.Close();
                }
            }

        }
        public void SaveCachedTree(string JsonTree)
        {
            Thread tt = new Thread(new ParameterizedThreadStart(ThreadSaveCachedTree));
            tt.Name = "SaveCachedTree";
            tt.Start(JsonTree);

        }

        public void ThreadSaveCachedTree(object TObject)
        {
            string JsonTree = (string)TObject;
            lock (Sqlconn)
            {
                var ser = new JavaScriptSerializer();
                try
                {
                    Sqlconn.Open();
                    string ConfStr = ser.Serialize(conf);

                    //string TreeName = string.Format("tree_{0}.json", Guid.NewGuid());
                    //SaveSdmxOBJ(SdmxOBJ, TreeName);
                    string sqlcount = string.Format("Select count(*) from SavedTree where Configuration='{0}'", ConfStr.Replace("'", "''"));
                    int conta = 0;
                    using (SqlCommand commconta = new SqlCommand(sqlcount, Sqlconn))
                        conta = Convert.ToInt32(commconta.ExecuteScalar());
                    
                    if (conta == 0)
                    {
                        string sqlupd = string.Format("INSERT INTO SavedTree (Configuration, SavedTreeJson, LastUpdate, LastRequest) VALUES ('{0}', '{1}', '{2}', '{2}')"
                            , ConfStr.Replace("'", "''"), JsonTree.Replace("'", "''"), DateTime.Now.ToString("yyyyMMdd HHmm"));
                        using (SqlCommand commupd = new SqlCommand(sqlupd, Sqlconn))
                            commupd.ExecuteNonQuery();
                    }

                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    Sqlconn.Close();
                }
            }
        }
        #endregion



        private ISdmxObjects GetSdmxOBJ(string FileName)
        {

            ISdmxObjects structureObjects = new SdmxObjectsImpl();
            IStructureParsingManager parsingManager = new StructureParsingManager(SdmxSchemaEnumType.VersionTwo);

            string FullNamePath = Path.Combine(Utils.GetTreeCachePath(), FileName);

            if (!File.Exists(FullNamePath))
                return null;
            using (var dataLocation = new FileReadableDataLocation(FullNamePath))
            {
                IStructureWorkspace structureWorkspace = parsingManager.ParseStructures(dataLocation);
                structureObjects = structureWorkspace.GetStructureObjects(false);
            }

            return structureObjects;
        }
        private void SaveSdmxOBJ(ISdmxObjects SdmxOBJ, string FileName)
        {
            if (SdmxOBJ == null)
                return;

            StructureWriterManager swm = new StructureWriterManager();
            StructureOutputFormat sofType = StructureOutputFormat.GetFromEnum(StructureOutputFormatEnumType.SdmxV2RegistryQueryResponseDocument);
            SdmxStructureFormat sof = new SdmxStructureFormat(sofType);
            string FullNamePath = Path.Combine(Utils.GetTreeCachePath(), FileName);
            using (Stream ms = File.Create(FullNamePath))
            {
                swm.WriteStructures(SdmxOBJ, sof, ms);
            }
        }
    }
}
