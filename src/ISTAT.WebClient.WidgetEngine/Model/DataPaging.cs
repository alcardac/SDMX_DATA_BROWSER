using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetEngine.Model
{
    public class DataPaging
    {

        public int PageNumber { get; set; }
        public int CurrentPageIndex { get; set; }
        private DirectoryInfo PagingFileDirectory { get; set; }
        private string FilePageFormat = "{0}.Pag{1}.page";


        public DataPaging(DataChacheObject findCache)
        {
            CurrentPageIndex = 1;
            PageNumber = 0;
            PagingFileDirectory = new DirectoryInfo(findCache.PagingDataDirectory);
            if (!PagingFileDirectory.Exists)
                PagingFileDirectory.Create();
            else
            {
                FileInfo[] pagine = PagingFileDirectory.GetFiles();
                PageNumber = pagine.Length;
            }
        }



        public DatasetJsonObj ParseAllData(IDataSetStore store, List<DataCriteria> Criterias, LayoutObj layObj, IDataStructureObject kf, ISet<ICodelistObject> codelists)
        {
            if (PageNumber > 0)
                return ParseAllDataPage(1);

            string DominantFreq = CalculateDominantFrequency(Criterias, kf, codelists);


            CurrentPageIndex = 1;
            int NObservationForPage = WebClientSettings.Instance.NObservationForPage;
            DatasetJsonObj dataset = new DatasetJsonObj() { series = new Dictionary<string, Dictionary<string, string>>() };
            List<string> sort = new List<string>();
            sort.AddRange(layObj.axis_y);
            sort.AddRange(layObj.axis_x);
            store.SetSort(sort);
            store.SetCriteria(Criterias);
            IDataReader datareader = store.CreateDataReader(false);
            try
            {
                int ActualRecordRegistred = 0;
                PageNumber = 0;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;
                while (datareader.Read())
                {
                    List<string> Colonne = new List<string>();
                    layObj.axis_x.ForEach(axisX => Colonne.Add(GetFromReader(datareader, axisX, DominantFreq, kf.TimeDimension.Id)));

                    List<string> Righe = new List<string>();
                    layObj.axis_y.ForEach(axisY => Righe.Add(GetFromReader(datareader, axisY, DominantFreq, kf.TimeDimension.Id)));

                    string OBSVal = (string)datareader[kf.PrimaryMeasure.Id];

                    string serieString = string.Join("+", Righe);

                    if (!dataset.series.ContainsKey(serieString))
                    {
                        if (ActualRecordRegistred >= NObservationForPage)
                        {
                            //NormalizeDataset(dataset, layObj.axis_x, kf, codelists);
                            FileInfo fi = new FileInfo(Path.Combine(PagingFileDirectory.FullName, string.Format(FilePageFormat, PagingFileDirectory.Name, PageNumber + 1)));
                            File.WriteAllText(fi.FullName, ser.Serialize(dataset));

                            ActualRecordRegistred = 0;
                            PageNumber++;
                            dataset = null;
                            dataset = new DatasetJsonObj() { series = new Dictionary<string, Dictionary<string, string>>() };
                        }

                        //dataset.series[serieString] = AllPossibleValues(layObj.axis_x, kf, codelists);
                        dataset.series[serieString] = new Dictionary<string, string>();
                    }
                    dataset.series[serieString][string.Join("+", Colonne)] = OBSVal;
                    ActualRecordRegistred++;


                }

                FileInfo fiLastPage = new FileInfo(Path.Combine(PagingFileDirectory.FullName, string.Format(FilePageFormat, PagingFileDirectory.Name, PageNumber + 1)));
                File.WriteAllText(fiLastPage.FullName, ser.Serialize(dataset));
                PageNumber++;
                if (PageNumber == 1)
                    return dataset;

                return ParseAllDataPage(1);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                store.Commit();
            }

        }


        private string CalculateDominantFrequency(List<DataCriteria> Criterias, IDataStructureObject kf, ISet<ICodelistObject> codelists)
        {
            string DominantFreq = null;
            if (kf.FrequencyDimension.HasCodedRepresentation() && !string.IsNullOrEmpty(kf.FrequencyDimension.Representation.Representation.MaintainableReference.MaintainableId))
            {
                ICodelistObject codes = codelists.FirstOrDefault(c => c.Id == kf.FrequencyDimension.Representation.Representation.MaintainableReference.MaintainableId &&
                        c.AgencyId == kf.FrequencyDimension.Representation.Representation.MaintainableReference.AgencyId &&
                        c.Version == kf.FrequencyDimension.Representation.Representation.MaintainableReference.Version);
                if (codes.Items != null && codes.Items.Count > 1)
                {
                    List<string> allFreq = new List<string>();
                    codes.Items.ToList().ForEach(c => allFreq.Add(c.Id));

                    DataCriteria filter = Criterias.Find(cri => cri.component.Id == kf.FrequencyDimension.Id);
                    if (filter != null)
                        allFreq = filter.values;
                    if (allFreq.Count > 1)
                    {
                        if (allFreq.Contains("A")) DominantFreq = "A";
                        if (allFreq.Contains("S")) DominantFreq = "S";
                        if (allFreq.Contains("Q")) DominantFreq = "Q";
                        if (allFreq.Contains("M")) DominantFreq = "M";
                    }
                }
            }
            return DominantFreq;
        }

        private string GetFromReader(IDataReader datareader, string axisX, string DominantFreq, string TimeDimension)
        {
            if (string.IsNullOrEmpty(DominantFreq) || TimeDimension != axisX)
                return (string)datareader[axisX];

            string TimePeriod = ((string)datareader[axisX]).Trim();
            string ActualFreq;
            if (TimePeriod.Length == 4)
                ActualFreq = "A";
            else if (TimePeriod.Contains("-S"))
                ActualFreq = "S";
            else if (TimePeriod.Contains("-Q"))
                ActualFreq = "Q";
            else if (TimePeriod.Length >= 7)
                ActualFreq = "M";
            else
                return TimePeriod;
            if (ActualFreq == DominantFreq)
                return TimePeriod;
            switch (DominantFreq)
            {
                case "A":
                    //Il timeperiod può essere solo A
                    return TimePeriod.Substring(0, 4);
                case "S":
                    //Il timeperiod può essere A o S
                    if (ActualFreq == "A")
                        return TimePeriod + "-S2";
                    else
                        return TimePeriod;
                case "Q":
                    if (ActualFreq == "A")
                        return TimePeriod + "-Q4";
                    else if (ActualFreq == "S")
                    {
                        if (TimePeriod.EndsWith("1"))
                            return TimePeriod.Substring(0, 4) + "-Q2";
                        else
                            return TimePeriod.Substring(0, 4) + "-Q4";
                    }
                    else
                        return TimePeriod;
                //Il timeperiod può essere A o S o Q
                case "M":
                    if (ActualFreq == "A")
                        return TimePeriod + "-12";
                    else if (ActualFreq == "S")
                    {
                        if (TimePeriod.EndsWith("1"))
                            return TimePeriod.Substring(0, 4) + "-06";
                        else
                            return TimePeriod.Substring(0, 4) + "-12";
                    }
                    else if (ActualFreq == "Q")
                    {
                        if (TimePeriod.EndsWith("1"))
                            return TimePeriod.Substring(0, 4) + "-03";
                        else if (TimePeriod.EndsWith("2"))
                            return TimePeriod.Substring(0, 4) + "-06";
                        else if (TimePeriod.EndsWith("3"))
                            return TimePeriod.Substring(0, 4) + "-09";
                        else
                            return TimePeriod.Substring(0, 4) + "-12";
                    }
                    else
                        return TimePeriod;
                //Il timeperiod può essere A o S o Q o M

                default:
                    return TimePeriod;
            }




        }

        private void NormalizeDataset(DatasetJsonObj dataset, List<string> dimensions, IDataStructureObject kf, ISet<ICodelistObject> codelists)
        {
            if (dataset.series.Count < 2)
                return;

            string FirstSerie = dataset.series.Keys.First();

            List<string> TemplateRow = AllPossibleValues(dimensions, kf, codelists);
            Dictionary<string, string> primaRiga = dataset.series[FirstSerie];
            dataset.series[FirstSerie] = new Dictionary<string, string>();

            TemplateRow.ForEach(t =>
                {
                    if (primaRiga.ContainsKey(t))
                        dataset.series[FirstSerie][t] = primaRiga[t];
                    else
                        dataset.series[FirstSerie].Add(t, "null");
                });


            foreach (var serie in dataset.series)
            {
                if (serie.Key == FirstSerie)
                    continue;
                foreach (string rigaK in serie.Value.Keys)
                {
                    if (dataset.series[FirstSerie][rigaK] == "null")
                        dataset.series[FirstSerie][rigaK] = "NaN";
                }
            }
            List<string> Keys = dataset.series[FirstSerie].Keys.ToList();
            foreach (string rigaK in Keys)
            {
                if (dataset.series[FirstSerie][rigaK] == "null")
                    dataset.series[FirstSerie].Remove(rigaK);
            }

        }

        private Dictionary<string, List<string>> NewLines = new Dictionary<string, List<string>>();
        private List<string> AllPossibleValues(List<string> dimensions, IDataStructureObject kf, ISet<ICodelistObject> codelists)
        {
            if (NewLines.ContainsKey(string.Join("+", dimensions)))
                return NewLines[string.Join("+", dimensions)];
            Dictionary<string, string> allVals = new Dictionary<string, string>();
            List<string> BuilderSeries = new List<string>();

            foreach (var dim in dimensions)
            {
                if (dim == kf.TimeDimension.Id)
                    return new List<string>();
                IComponent component = kf.Components.FirstOrDefault(c => c.Id == dim);
                if (component == null || !component.HasCodedRepresentation() || string.IsNullOrEmpty(component.Representation.Representation.MaintainableReference.MaintainableId))
                    return new List<string>();
                ICodelistObject codelist = codelists.First(c => c.Id == component.Representation.Representation.MaintainableReference.MaintainableId);
                if (codelist == null)
                    continue;

                List<string> InternalBuilderSeries = new List<string>();

                if (BuilderSeries.Count == 0)
                {
                    foreach (var code in codelist.Items)
                        InternalBuilderSeries.Add(code.Id);
                }
                else
                {
                    BuilderSeries.ForEach(bs =>
                     {
                         foreach (var code in codelist.Items)
                             InternalBuilderSeries.Add(bs + "+" + code.Id);
                     });
                }
                BuilderSeries = InternalBuilderSeries;
            }


            NewLines.Add(string.Join("+", dimensions), BuilderSeries);
            return BuilderSeries;
        }


        public DatasetJsonObj ParseAllDataPage(int NumberPage)
        {
            FileInfo fi = new FileInfo(Path.Combine(PagingFileDirectory.FullName, string.Format(FilePageFormat, PagingFileDirectory.Name, NumberPage)));
            if (!fi.Exists)
                return null;
            CurrentPageIndex = NumberPage;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            return ser.Deserialize<DatasetJsonObj>(File.ReadAllText(fi.FullName));
        }


    }
}
