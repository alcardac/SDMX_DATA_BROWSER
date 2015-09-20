using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using ISTAT.WebClient.WidgetComplements.Model.Properties;

namespace ISTAT.WebClient.WidgetComplements.Model
{
 public  static class TextTypeHelper
    {
      public static string GetText(IList<ITextTypeWrapper> values, string lang)
      {
          string result = string.Empty;

          if (string.IsNullOrEmpty(lang))
          {
              lang = Resources.defaultLanguage;
          }

          foreach (ITextTypeWrapper value in values)
          {
              if (!string.IsNullOrEmpty(value.Value))
              {
                  if (lang.Equals(value.Locale))
                  {
                      return value.Value;
                  }

                  if (Resources.defaultLanguage.Equals(value.Locale))
                  {
                      result = value.Value;
                  }
                  else if (result.Length == 0)
                  {
                      result = value.Value;
                  }
              }
          }
          return result;
      }
    }
}