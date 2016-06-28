// -----------------------------------------------------------------------
// <copyright file="ComponentArtefactCache.cs" company="EUROSTAT">
//   Date Created : 2011-01-29
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
    using System.Reflection;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

    /// <summary>
    /// Helper class that caches artefacts such as codelists and concepts and maps them by component
    /// </summary>
    /// <typeparam name="T">
    /// The <c>IdentifiableArtefactBean</c> type to cache
    /// </typeparam>
    public class ComponentArtefactCache<T, TI>
        where TI : IItemObject
        where T :  IItemSchemeObject<TI>
    {
        #region Constants and Fields

        /// <summary>
        /// Artefact cache.
        /// </summary>
        private readonly IDictionary<IComponent, T> _componentMap = new Dictionary<IComponent, T>();
        private readonly IDictionary<IComponent, TI> _componentMapItemObject = new Dictionary<IComponent, TI>();


        /// <summary>
        /// Cache of merged lists. Only used for ItemSchemes
        /// </summary>
        private readonly List<T> _mergedList = new List<T>();

        #endregion

        #region Methods

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void Clear()
        {
            this._componentMap.Clear();
            this._mergedList.Clear();
        }

        /// <summary>
        /// Get the artefact for the specified component
        /// </summary>
        /// <param name="component">
        /// The component
        /// </param>
        /// <returns>
        /// The artefact if it exists or null
        /// </returns>
        public T GetArtefact(IComponent component)
        {
            T artefact;
            this._componentMap.TryGetValue(component, out artefact);
            return artefact;
        }

        public TI GetArtefactItemObject(IComponent component)
        {
            TI artefact;
            this._componentMapItemObject.TryGetValue(component, out artefact);
            return artefact;
        }

        /// <summary>
        /// Get the list of all cached ItemScheme artefacts. Duplicate ItemSchemes are merged.
        /// </summary>
        /// <returns>
        /// The merged list
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The T is not an <c>ItemSchemeBean</c>
        /// </exception>
        public List<T> GetMergedItemScheme()
        {
            if (this._mergedList.Count > 0)
            {
                return this._mergedList;
            }

            this.BuildMergedList();

            return this._mergedList;
        }

        /// <summary>
        /// Remove the specified component and it's cached artefact
        /// </summary>
        /// <param name="component">
        /// The <see cref="ComponentBean"/> to remove
        /// </param>
        public void Remove(IComponent component)
        {
            this._componentMap.Remove(component);
            this._mergedList.Clear();
        }

        /// <summary>
        /// Add/Update the specified component with the specified artefact. 
        /// If there is already a cached artefact for this component it is removed, and then the specified artefact is added
        /// </summary>
        /// <param name="component">
        /// The component
        /// </param>
        /// <param name="artefact">
        /// The new artefact to cache
        /// </param>
        public void Update(IComponent component, T artefact)
        {
            this._componentMap.Remove(component);
            this._componentMap.Add(component, artefact);
            this._mergedList.Clear();
        }

        public void UpdateItemObject(IComponent component, TI artefact)
        {
            this._componentMapItemObject.Remove(component);
            this._componentMapItemObject.Add(component, artefact);
            this._mergedList.Clear();
        }

        /// <summary>
        /// Make a shallow copy of the specified <c>ItemSchemeBean</c> to avoid changing the original item list
        /// </summary>
        /// <param name="from">
        /// The source <see cref="ItemSchemeBean"/>
        /// </param>
        /// <returns>
        /// The output <see cref="ItemSchemeBean"/>
        /// </returns>
        private static T CopyItemScheme(IItemSchemeObject<TI> from)
        {
            return (T)from.MutableInstance.ImmutableInstance;
        }

        /// <summary>
        /// Build the merged item scheme list
        /// </summary>
        private void BuildMergedList()
        {
            var idMap = new Dictionary<string, IItemSchemeObject<TI>>();
            var itemSet = new Dictionary<string, object>();
            foreach (T artefact in this._componentMap.Values)
            {
                var itemScheme = artefact as IItemSchemeObject<TI>;
                if (itemScheme != null)
                {
                    string key = Estat.Nsi.Client.Utils.MakeKey(itemScheme);
                    IItemSchemeObject<TI> otherScheme;

                    if (idMap.TryGetValue(key, out otherScheme))
                    {
                        itemSet.Clear();
                        foreach (TI item in otherScheme.Items)
                        {
                            itemSet.Add(item.Id, null);
                        }

                        foreach (TI item in itemScheme.Items)
                        {
                            if (!itemSet.ContainsKey(item.Id))
                            {
                                otherScheme.Items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        T output = CopyItemScheme(itemScheme);
                        var merged = output as IItemSchemeObject<TI>;
                        idMap.Add(key, merged);
                        this._mergedList.Add(output);
                    }
                }
            }
        }

        #endregion
    }
}