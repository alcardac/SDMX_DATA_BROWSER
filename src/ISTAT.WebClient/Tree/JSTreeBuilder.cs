// -----------------------------------------------------------------------
// <copyright file="JSTreeBuilder.cs" company="EUROSTAT">
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
namespace ISTAT.WebClient.Tree
{
    using System.Globalization;
    using System.Threading;

    using ISTAT.WebClient.Tree;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

    /// <summary>
    /// An abstract class, base for <a href="http://jstree.com">JSTree</a> builders
    /// </summary>
    public abstract class JSTreeBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The artefact format. Used with Dataflows
        /// </summary>
        private const string ArtefactFormat = "{0} - {{0}}";

        /// <summary>
        /// The default format used when setting up a node
        /// </summary>
        private const string DefaultFormat1 = "{0}";

        #endregion

        #region Methods

        /// <summary>
        /// Setup a <see cref="JsTreeNode"/> from a <c>CodeBean</c>
        /// </summary>
        /// <param name="node">
        /// The <see cref="JsTreeNode"/>
        /// </param>
        /// <param name="artefact">
        /// The <c>CodeBean</c>
        /// </param>
        protected static void SetupNode(JsTreeNode node, ICode artefact)
        {
            string entitle = "[" + artefact.Id + "]";
            string format = string.Format(CultureInfo.InvariantCulture, ArtefactFormat, entitle);
            SetupNode(node, artefact, entitle, format);
        }

        /// <summary>
        /// Setup a <see cref="JsTreeNode"/> from a <c>DataflowBean</c>
        /// </summary>
        /// <param name="node">
        /// The <see cref="JsTreeNode"/>
        /// </param>
        /// <param name="artefact">
        /// The <c>DataflowBean</c>
        /// </param>
        protected static void SetupNode(JsTreeNode node, IDataflowObject artefact)
        {
            string entitle = artefact.Id;
            string format = string.Format(CultureInfo.InvariantCulture, ArtefactFormat, entitle);
            SetupNode(node, artefact, entitle, format);
        }

        /// <summary>
        /// Setup a <see cref="JsTreeNode"/> from a <c>IdentifiableArtefactBean</c>
        /// </summary>
        /// <param name="node">
        /// The <see cref="JsTreeNode"/>
        /// </param>
        /// <param name="artefact">
        /// The <c>IdentifiableArtefactBean</c>
        /// </param>
        protected static void SetupNode(JsTreeNode node, INameableObject artefact)
        {
            string entitle = artefact.Id;
            SetupNode(node, artefact, entitle, DefaultFormat1);
        }

        /// <summary>
        /// Setup a <see cref="JsTreeNode"/> from a <c>IdentifiableArtefactBean</c>
        /// </summary>
        /// <param name="node">
        /// The <see cref="JsTreeNode"/>
        /// </param>
        /// <param name="artefact">
        /// The <c>IdentifiableArtefactBean</c>
        /// </param>
        /// <param name="defaultString">
        /// The text to use in case there is no name or description
        /// </param>
        /// <param name="format">
        /// The format that will be used with <see cref="System.String.Format(string,object)"/> and artefact name or description
        /// </param>
        protected static void SetupNode(
            JsTreeNode node, INameableObject artefact, string defaultString, string format)
        {
            string lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            /*string result = TextTypeHelper.GetText(artefact.Names, lang);
            string title = string.Format(CultureInfo.CurrentCulture, format, result.Length == 0 ? TextTypeHelper.GetText(artefact.Descriptions, lang) : result);
             * */
            string result;
            string title="";

            if (string.IsNullOrEmpty(title))
            {
                title = defaultString;
            }

            ////Data data = new Data { title = title };
            ////node.data.Add(data);
            node.data = title;
        }

        #endregion
    }
}