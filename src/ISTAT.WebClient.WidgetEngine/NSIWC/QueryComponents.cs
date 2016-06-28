// -----------------------------------------------------------------------
// <copyright file="QueryComponent.cs" company="EUROSTAT">
//   Date Created : 2010-11-11
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
namespace ISTAT.WebClient.WidgetEngine.NSIWC
{
    using System.Collections.Generic;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;

    /// <summary>
    /// Used for storing the data about a key family component used for filtering the data.
    /// </summary>
    /// <remarks>
    /// This is a lazy implementation, as probably there should be one interface and three implementations, one for a
    /// dimension component with a codelist, one for a dimension component without a codelist and one for time component.
    /// </remarks>
    public class QueryComponent
    {
        #region Constants and Fields

        /// <summary>
        /// The selected codes.
        /// </summary>
        private readonly Dictionary<string, ICode> _codeMap;

        /// <summary>
        /// The concept of the component
        /// </summary>
        private readonly IConceptObject _concept;

        /// <summary>
        /// The end date, <c>null</c> if not set. Used for time period only.
        /// </summary>
        private readonly string _endDate;

        /// <summary>
        /// The key family component.
        /// </summary>
        private readonly IComponent _keyFamilyComponent;

        /// <summary>
        /// The start date, <c>null</c> if not set. Used for time period only.
        /// </summary>
        private readonly string _startDate;

        /// <summary>
        /// Plain text value, used only if the component is not associated with a codelist, otherwise is <c>null</c>.
        /// </summary>
        private readonly string _textValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryComponent"/> class. 
        /// Initialize a new instance of the QueryComponent class for a coded component
        /// </summary>
        /// <param name="keyFamilyComponent">
        /// The component, value for <see cref="KeyFamilyComponent"/> property
        /// </param>
        /// <param name="concept">
        /// The concept used by this component, value for <see cref="Concept"/> property
        /// </param>
        /// <param name="codes">
        /// The selected code used by this component, value for <see cref="Concept"/> property
        /// </param>
        public QueryComponent(IComponent keyFamilyComponent, IConceptObject concept, List<ICode> codes)
        {
            this._keyFamilyComponent = keyFamilyComponent;
            this._concept = concept;
            this._codeMap = new Dictionary<string, ICode>(codes.Count);
            foreach (ICode code in codes)
            {
                this._codeMap.Add(code.Id, code);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryComponent"/> class. 
        /// Initialize a new instance of the QueryComponent class for a uncoded component
        /// </summary>
        /// <param name="keyFamilyComponent">
        /// The component, value for <see cref="KeyFamilyComponent"/> property
        /// </param>
        /// <param name="concept">
        /// The concept used by this component, value for <see cref="Concept"/> property
        /// </param>
        /// <param name="value">
        /// The value for <see cref="TextValue"/> property
        /// </param>
        public QueryComponent(IComponent keyFamilyComponent, IConceptObject concept, string value)
        {
            this._keyFamilyComponent = keyFamilyComponent;
            this._concept = concept;
            this._textValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryComponent"/> class. 
        /// Initialize a new instance of the QueryComponent class for the TimeDimension component
        /// </summary>
        /// <param name="keyFamilyComponent">
        /// The component, value for <see cref="KeyFamilyComponent"/> property
        /// </param>
        /// <param name="concept">
        /// The concept used by this component, value for <see cref="Concept"/> property
        /// </param>
        /// <param name="startYear">
        /// The start year, value for <see cref="StartYear"/> property
        /// </param>
        /// <param name="endYear">
        /// The end year, value for <see cref="EndYear"/> property
        /// </param>
        /// <param name="startPeriod">
        /// The start period, value for <see cref="StartPeriod"/> property
        /// </param>
        /// <param name="endPeriod">
        /// The end period , value for <see cref="EndPeriod"/> property
        /// </param>
        public QueryComponent(
            IComponent keyFamilyComponent,
            IConceptObject concept,
            string startDate,
            string endDate
           )
        {
            this._keyFamilyComponent = keyFamilyComponent;
            this._concept = concept;
            this._startDate = startDate;

            this._endDate = endDate;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the concept of the component
        /// </summary>
        public string Concept
        {
            get
            {
                return this._concept.Id;
            }
        }

        /// <summary>
        /// Gets the end date, <c>null</c> if not set. Used for time period only.
        /// </summary>
        public string EndDate
        {
            get
            {
                return this._endDate;
            }
        }


        /// <summary>
        /// Gets the key family component.
        /// </summary>
        public IComponent GetKeyFamilyComponent()
        {
            return this._keyFamilyComponent;
        }

        /// <summary>
        /// Gets the start period, <c>null</c> if not set. Used for time period only.
        /// </summary>
        public string StartDate
        {
            get
            {
                return this._startDate;
            }
        }


        /// <summary>
        /// Gets the plain text value, used only if the component is not associated with a codelist, otherwise is <c>null</c>.
        /// </summary>
        public string TextValue
        {
            get
            {
                return this._textValue;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the codes.
        /// </summary>
        /// <returns>
        /// The codes
        /// </returns>
        public ICollection<ICode> RetrieveCodes()
        {
            return this._codeMap.Values;
        }

        #endregion
    }
}