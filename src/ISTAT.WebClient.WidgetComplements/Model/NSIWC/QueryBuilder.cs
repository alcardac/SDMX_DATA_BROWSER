// -----------------------------------------------------------------------
// <copyright file="QueryBuilder.cs" company="EUROSTAT">
//   Date Created : 2011-09-28
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
namespace ISTAT.WebClient.WidgetComplements.Model.NSIWC
{
    using System;
    using System.Collections.Generic;

    using Estat.Sri.CustomRequests.Constants;

    using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;

    /// <summary>
    /// This class is responsible for building SDMX Query
    /// </summary>
    public class QueryBuilder
    {
        /// <summary>
        /// The current Session query
        /// </summary>
        private readonly SessionQuery _sessionQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="sessionQuery">
        /// The session query.
        /// </param>
        public QueryBuilder(SessionQuery sessionQuery)
        {
            this._sessionQuery = sessionQuery;
        }

        /// <summary>
        /// Create a SDMX Model Query for <see cref="SessionQuery.Dataflow"/> from the criteria at <see cref="SessionQuery._queryComponentIndex"/>
        /// </summary>
        /// <returns>
        /// A QueryBean object
        /// </returns>
        public IDataQuery CreateQueryBean()
        {
            if (!this._sessionQuery.IsDataflowSet)
            {
                throw new InvalidOperationException("Dataflow is not set");
            }

            ISet<IDataQuerySelection> selections = new HashSet<IDataQuerySelection>();
            string startTime = String.Empty;
            string endTime = String.Empty;

            // Under the DataWhere only one child MUST reside.
            foreach (var queryComponent in this._sessionQuery.GetQueryComponents())
            {
                if (queryComponent != null)
                {
                    IComponent component = queryComponent.GetKeyFamilyComponent();
                    var dimension = component as IDimension;

                    if (dimension != null && !dimension.TimeDimension)
                    {
                        IDataQuerySelection selection = this.AddDimensionToQuery(queryComponent, component);
                        selections.Add(selection);
                    }
                    else if (dimension != null && dimension.TimeDimension)
                    {
                        if (!string.IsNullOrEmpty(queryComponent.StartDate))
                        {
                            startTime = queryComponent.StartDate;
                            endTime = queryComponent.EndDate;
                        }
                    }
                }
            }
            IDataQuerySelectionGroup sel = new DataQuerySelectionGroupImpl(selections, null, null);
            if ((string.IsNullOrEmpty(startTime)) && (!string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, null, new SdmxDateCore(endTime));
            }
            else if ((!string.IsNullOrEmpty(startTime)) && (string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime), null);
            }
            else if ((!string.IsNullOrEmpty(startTime)) && (!string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime), new SdmxDateCore(endTime));
            }
            IList<IDataQuerySelectionGroup> selGroup = new List<IDataQuerySelectionGroup>();
            selGroup.Add(sel);
            IDataQuery query;
            if (_sessionQuery.EndPointType == EndpointType.REST)
            {
                query = new DataQueryFluentBuilder().Initialize(_sessionQuery.KeyFamily, _sessionQuery.Dataflow).
                      WithDataQuerySelectionGroup(selGroup).Build();
            }
            else
            {
                query = new DataQueryFluentBuilder().Initialize(_sessionQuery.KeyFamily, _sessionQuery.Dataflow).
                       WithOrderAsc(true).
                       WithMaxObservations(_sessionQuery.MaximumObservations).
                       WithDataQuerySelectionGroup(selGroup).Build();
            }
            return query;
        }

        public IDataQuerySelection AddDimensionToQuery(QueryComponent queryComponent, IComponent component)
        {
            ISet<string> valuern = new HashSet<string>();
            foreach (ICode c in queryComponent.RetrieveCodes())
            {
                if (!string.IsNullOrEmpty(c.Id))
                {
                    valuern.Add((c.Id));
                }
            }
            IDataQuerySelection selection = new DataQueryDimensionSelectionImpl(component.Id, valuern);

            return selection;
        }

        /// <summary>
        /// Create a new Constrain from the <see cref="SessionQuery._queryComponentIndex"/>
        /// </summary>
        /// <returns>
        /// A constrain bean
        /// </returns>
        public IContentConstraintMutableObject CreateConstraintBean()
        {
            return this.CreateConstraintBean(null);
        }

        /// <summary>
        /// Create a new Constrain from the <see cref="SessionQuery._queryComponentIndex"/> and optionally for the specified component
        /// </summary>
        /// <param name="currentComponent">
        /// The current component. Normally this component should be belong to the AvailableComponents set. Set to null if there isn't any current component
        /// </param>
        /// <returns>
        /// A constrain bean
        /// </returns>
        public IContentConstraintMutableObject CreateConstraintBean(string currentComponent)        
        {
            if (!this._sessionQuery.IsDataflowSet)
            {
                throw new InvalidOperationException("Dataflow is not set");
            }
            IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
            criteria.Id = currentComponent ?? "SPECIAL";
            criteria.AddName("en", "english");
            criteria.AgencyId = "agency";
            ICubeRegionMutableObject region = new CubeRegionMutableCore();

            if (currentComponent != null)
            {
                IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                keyValue.Id = currentComponent;
                keyValue.AddValue(SpecialValues.DummyMemberValue);
                region.AddKeyValue(keyValue);
            }

            foreach (var queryComponent in this._sessionQuery.GetQueryComponents())
            {
                if (queryComponent != null && !queryComponent.GetKeyFamilyComponent().ConceptRef.ChildReference.Id.Equals(currentComponent))
                {
                    var setComponents = new KeyValuesMutableImpl();
                    region.AddKeyValue(setComponents);
                    setComponents.Id = queryComponent.GetKeyFamilyComponent().ConceptRef.ChildReference.Id;

                    var dimension = queryComponent.GetKeyFamilyComponent() as IDimension;
                    if (dimension != null && dimension.TimeDimension)
                    {
                        // there is another way to handle this with ReferencePeriod but
                        // ReferencePeriod accepts {xs:Date,xs:DateTime}. Using this way, MemberValue, we can use anything we want.

                        // must be first
                        setComponents.KeyValues.Add(queryComponent.StartDate);

                        if (!string.IsNullOrEmpty(queryComponent.EndDate))
                        {
                            setComponents.KeyValues.Add(queryComponent.EndDate);
                        }
                    }
                    else if (queryComponent.GetKeyFamilyComponent().Representation.Representation.MaintainableReference.MaintainableId != null)
                    {
                        // java has different API for MemberValue
                        foreach (ICode code in queryComponent.RetrieveCodes())
                        {
                            setComponents.KeyValues.Add(code.Id);
                        }
                        if (setComponents.KeyValues.Count == 0)
                        {
                            setComponents.AddValue(SpecialValues.DummyMemberValue);
                        }
                    }
                    else
                    {
                        setComponents.KeyValues.Add(queryComponent.TextValue);
                    }
                }
            }

            if (region.KeyValues.Count > 0)
            {
                criteria.IncludedCubeRegion = region;
            }

            return criteria;
        }
    }
}