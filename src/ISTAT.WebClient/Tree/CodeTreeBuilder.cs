// -----------------------------------------------------------------------
// <copyright file="CodeTreeBuilder.cs" company="EUROSTAT">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;

    /// <summary>
    /// Build a JSTree from a codelist
    /// </summary>
    public class CodeTreeBuilder : JSTreeBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The id prefix.
        /// </summary>
        private const string IDPrefix = "CLV_";

        /// <summary>
        /// The set of codes that are checked.
        /// </summary>
        private readonly Dictionary<ICode, JsTreeNode> _checkedNodes;

        /// <summary>
        /// This field holds the codelist
        /// </summary>
        private readonly ICodelistObject _codeList;

        /// <summary>
        /// This field holds a map between a code and a tree node
        /// </summary>
        private readonly Dictionary<ICode, JsTreeNode> _idNodeMap;

        /// <summary>
        /// This field holds the list of root tree nodes
        /// </summary>
        private readonly List<JsTreeNode> _rootNodes;

        /// <summary>
        /// The dirty flag. When true the jstree should be <see cref="RefreshNodeMap"/>
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// This field holds the last culture the <see cref="_idNodeMap"/> and <see cref="_rootNodes"/> was build with.
        /// </summary>
        private CultureInfo _prevCulture;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeTreeBuilder"/> class.
        /// </summary>
        /// <param name="codelist">
        /// The codelist.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// codelist is null
        /// </exception>
        public CodeTreeBuilder(ICodelistObject codelist)
        {
            if (codelist == null)
            {
                throw new ArgumentNullException("codelist");
            }

            this._codeList = codelist;
            this._idNodeMap = new Dictionary<ICode, JsTreeNode>(codelist.Items.Count);
            this._rootNodes = new List<JsTreeNode>(codelist.Items.Count);
            this._checkedNodes = new Dictionary<ICode, JsTreeNode>(codelist.Items.Count);
            this.BuildIdNodeMap();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check the code with the specified id
        /// </summary>
        /// <param name="codes">
        /// The code ids to check
        /// </param>
        public void CheckCodes(ICollection<ICode> codes)
        {
            this._checkedNodes.Clear();
            this._dirty = true;
            if (codes == null)
            {
                return;
            }

            foreach (ICode code in codes)
            {
                this._checkedNodes.Add(code, null);

                // JsTreeNode node = null;
                // if (idNodeMap.TryGetValue(code.Id, out node)) {
                // string clazz = (node.attr["class"] as String) ?? String.Empty;
                // node.attr["class"] = String.Format("{0} jstree-checked", clazz);
                // }
            }
        }

        /// <summary>
        /// Get the code tree starting from the specified <paramref name="parentCodeId"/> 
        /// </summary>
        /// <param name="parentCodeId">
        /// The parent code id
        /// </param>
        /// <returns>
        /// The code tree starting from the specified parentCode 
        /// </returns>
        public List<JsTreeNode> GetJSTree(string parentCodeId)
        {
            List<JsTreeNode> nodes = null;
            this._dirty = this._dirty || !this._prevCulture.Equals(Thread.CurrentThread.CurrentUICulture);
            if (this._dirty)
            {
                this.RefreshNodeMap();
                this._dirty = false;
                this._prevCulture = Thread.CurrentThread.CurrentUICulture;
            }

            if (string.IsNullOrEmpty(parentCodeId) || parentCodeId.Equals("-1"))
            {
                nodes = this._rootNodes;
            }
            else
            {
                parentCodeId = parentCodeId.Substring(IDPrefix.Length);
                JsTreeNode node;
                var code = (ICode)this._codeList.GetCodeById(parentCodeId);
                if (this._idNodeMap.TryGetValue(code, out node))
                {
                    nodes = node.children;
                }

                if (nodes == null)
                {
                    nodes = new List<JsTreeNode>();
                }
            }

            return nodes;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build the  <see cref="_idNodeMap"/>
        /// </summary>
        private void BuildIdNodeMap()
        {
            this._idNodeMap.Clear();
            this._rootNodes.Clear();
            this._prevCulture = Thread.CurrentThread.CurrentUICulture;
            var needParent = new Queue<ICode>();
            foreach (ICode code in this._codeList.Items)
            {
                var node = new JsTreeNode();
                node.SetId(string.Format(CultureInfo.InvariantCulture, "{0}{1}", IDPrefix, code.Id));
                SetupNode(node, code);
                this._idNodeMap.Add(code, node);
                node.SetLeaf(true);
                if (!string.IsNullOrEmpty(code.ParentCode))
                {
                    needParent.Enqueue(code);
                }
                else
                {
                    this._rootNodes.Add(node);
                }
            }

            while (needParent.Count > 0)
            {
                ICode code = needParent.Dequeue();
                JsTreeNode child;
                if (this._idNodeMap.TryGetValue(code, out child))
                {
                    var parentCode = (ICode)this._codeList.GetCodeById(code.ParentCode);
                    JsTreeNode parent;
                    if (this._idNodeMap.TryGetValue(parentCode, out parent))
                    {
                        parent.state = JSTreeConstants.OpenState;
                        parent.SetLeaf(false);
                        parent.children.Add(child);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the <see cref="_idNodeMap"/>
        /// </summary>
        private void RefreshNodeMap()
        {
            foreach (KeyValuePair<ICode, JsTreeNode> kv in this._idNodeMap)
            {
                kv.Value.Unchecked();
                if (this._checkedNodes.ContainsKey(kv.Key))
                {
                    kv.Value.Check();
                }

                ////kv.Value.data.Clear();
                SetupNode(kv.Value, kv.Key);
            }
        }

        #endregion
    }
}