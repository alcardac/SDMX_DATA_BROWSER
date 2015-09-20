using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model
{
    public class SessionImplObject
    {
        public ISdmxObjects SdmxObject { get; set; }

        public Dictionary<string, Dictionary<string, ICodelistObject>> CodelistConstrained { get; set; }
        public Dictionary<string, List<DataChacheObject>> DataCache { get; set; }
        public Dictionary<string, LayoutObj> DafaultLayout { get; set; }

        public string SavedTree { get; set; }
        public string SavedDefaultLayout { get; set; }
        public string SavedCodemap { get; set; }
        public string SavedData { get; set; }
        public string SavedChart { get; set; }
                             
        public void MergeObject(SessionImplObject ret)
        {
            if (this.SdmxObject == null) this.SdmxObject = ret.SdmxObject;
            else this.SdmxObject.Merge(ret.SdmxObject);

            if (ret.CodelistConstrained != null) this.CodelistConstrained = ret.CodelistConstrained;
            if (ret.DafaultLayout != null) this.DafaultLayout = ret.DafaultLayout;
            if (ret.DataCache != null) this.DataCache = ret.DataCache;

            if (string.IsNullOrEmpty(ret.SavedTree))
                this.SavedTree = ret.SavedTree;
            if (string.IsNullOrEmpty(ret.SavedCodemap))
                this.SavedCodemap = ret.SavedCodemap;
            if (string.IsNullOrEmpty(ret.SavedData))
                this.SavedData = ret.SavedData;
        }

        public void ClearCache()
        {
          
            if(this.DataCache!=null)
                this.DataCache.Values.ToList().ForEach(dc => dc.ForEach(findCache =>
                {
                    try
                    {
                        FileInfo cachedDB = new FileInfo(findCache.DBFileName);
                        if (cachedDB.Exists)
                            cachedDB.Delete();
                    }
                    catch (Exception)
                    {
                    }
                }));
            this.DataCache = null;
            this.SdmxObject = null;
            this.CodelistConstrained = null;
            this.DafaultLayout = null;
        }
    }

    public class DataChacheObject
    {
        public Dictionary<string, List<string>> Criterias { get; set; }
        public string DBFileName { get; set; }

    }
}
