using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class ChartResponseObject
    {
        public string series_title; //nome dataflow
        public List<serieType> series;// colonne
        public string primary_name;
        public decimal primary_max;
        public decimal primary_min;
        public string secondary_name;
        public decimal secondary_max;
        public decimal secondary_min;
        public string x_name;
        
    }

    public class serieType
    {

        public string indexLabel;
        public string name; // tutti i nomi colonne (descrizioni) 
        public string serieKey; 
        public string type;  //mi arriva da fuori "spline"
        public bool showInLegend; //mi arriva da fuori "true"
        public List<dataPointType> dataPoints;
        public string axisYType;// secondary or empty 
        public float lineThickness;
        public string markerType;// secondary or empty 
        public float markerSize;// secondary or empty 
        
    };
    public class dataPointType
    {
        public string legendText; 
        public string label; 
        public object y; // OBJValue
        public object x; // indice progressivo dell'anno (salva in codemap) in base al criterio tempo
    };
   
}
