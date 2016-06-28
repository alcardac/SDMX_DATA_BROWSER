// -----------------------------------------------------------------------
// <copyright file="TextTypeHelper.cs" company="EUROSTAT">
//   Date Created : 2013-03-29
//   Copyright (c) 2009, 2015 by the European Commission, represented by Eurostat.   All rights reserved.
// 
// Licensed under the EUPL, Version 1.1 or – as soon they
// will be approved by the European Commission - subsequent
// versions of the EUPL (the "Licence");
// You may not use this work except in compliance with the
// Licence.
// You may obtain a copy of the Licence at:
// 
// https://joinup.ec.europa.eu/software/page/eupl 
// 
// Unless required by applicable law or agreed to in
// writing, software distributed under the Licence is
// distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the Licence for the specific language governing
// permissions and limitations under the Licence.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Estat.Nsi.Client.Properties;

using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace Estat.Nsi.Client
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
