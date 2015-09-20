using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ISTAT.SingleSignON.Service.Model
{
    public class Configuration
    {
        public static string ConnString { get; set; }
        public Configuration()
        {
            try
            {
                string FileConfig = "Service.Config";
                DirectoryInfo MapPath = new DirectoryInfo(AppDomain.CurrentDomain.RelativeSearchPath);
                if (!File.Exists(Path.Combine(MapPath.FullName,FileConfig)))
                    throw new Exception("File Service.Config not found");

                XElement XFileConfig = XElement.Parse(File.ReadAllText(Path.Combine(MapPath.FullName, FileConfig)));
                string connStr = (from f in XFileConfig.Descendants()
                                  where f.Attribute("name") != null && f.Attribute("name").Value.Trim().ToLower().Equals("usermodel")
                                  && f.Attribute("connectionString") != null
                                  select new
                                  {
                                      ConnStr = f.Attribute("connectionString").Value
                                  }).FirstOrDefault().ConnStr;

                ConnString = connStr.Replace("|DataDirectory|", Path.Combine(MapPath.FullName,"App_Data"));
                //ConnString = connStr;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Parsing file Service.Config: " + ex.Message );
            }
            

        }
    }
}
