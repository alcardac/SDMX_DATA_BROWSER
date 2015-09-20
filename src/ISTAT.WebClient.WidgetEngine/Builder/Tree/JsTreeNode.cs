// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsTreeNode.cs" company="Eurostat">
//   Date Created : 2011-09-28
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This class represents a JSTree node
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetEngine.Builder.Tree
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class represents a JSTree node
    /// </summary>
    public class JsTreeNode
    {
        #region Constants and Fields

        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

        private readonly List<JsTreeNode> _children = new List<JsTreeNode>();

        private readonly Dictionary<string, object> _classes = new Dictionary<string, object>();

        private readonly StringBuilder _classesBuffer = new StringBuilder();

        private JSTreeMetadata _metadata;
        private Dictionary<string, object> _metadata_li;

        private string _state = JSTreeConstants.CloseState; // change to "open" to expand all by default

        private string _type;

        #endregion

        #region Public Properties

        public Dictionary<string, object> li_attr
        {
            get
            {
                this.AddClassToAttr();
                return this._attributes;
            }
        }
        public string type { get { return _type; } set { _type = value; } }

        public List<JsTreeNode> children
        {
            get
            {
                return this._children;
            }
        }

        public string text { get; set; }

        public JSTreeMetadata a_attr
        {
            get
            {
                return this._metadata;
            }

            set
            {
                this._metadata = value;
            }
        }

        public string state
        {
            get
            {
                return this._state;
            }

            set
            {
                this._state = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a class value from class attribute
        /// </summary>
        /// <param name="classValue">
        /// The class value
        /// </param>
        public void AddClass(string classValue)
        {
            this._classes[classValue] = null;
        }

        /// <summary>
        /// Remove a class value from class attribute
        /// </summary>
        /// <param name="classValue">
        /// The class value
        /// </param>
        public void RemoveClass(string classValue)
        {
            this._classes.Remove(classValue);
        }

        /// <summary>
        /// Set the Node ID attribute
        /// </summary>
        /// <param name="id">
        /// The ID
        /// </param>
        public void SetId(string id)
        {
            this._attributes["id"] = id;
        }

        /// <summary>
        /// Set the Node rel attribute (used in types plugin)
        /// </summary>
        /// <param name="rel">
        /// The value
        /// </param>
        public void SetRel(string rel)
        {
            this._attributes["rel"] = rel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add the contents of <see cref="_classes"/> to <see cref="_attributes"/>
        /// </summary>
        private void AddClassToAttr()
        {
            this._classesBuffer.Length = 0;

            foreach (string key in this._classes.Keys)
            {
                this._classesBuffer.Append(key);
                this._classesBuffer.Append(' ');
            }

            if (this._classesBuffer.Length > 0)
            {
                this._classesBuffer.Length--;
                this._attributes["class"] = this._classesBuffer.ToString();
            }
            else
            {
                this._attributes.Remove("class");
            }
        }

        #endregion
    }

   
}