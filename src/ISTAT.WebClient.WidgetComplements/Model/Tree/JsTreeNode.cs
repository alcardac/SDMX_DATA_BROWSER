// -----------------------------------------------------------------------
// <copyright file="JsTreeNode.cs" company="EUROSTAT">
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
namespace ISTAT.WebClient.WidgetComplements.Model.Tree
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class represents a JSTree node
    /// </summary>
    public class JsTreeNode
    {
        #region Constants and Fields

        /// <summary>
        /// The _attributes.
        /// </summary>
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

        /// <summary>
        /// The _children.
        /// </summary>
        private readonly List<JsTreeNode> _children = new List<JsTreeNode>();

        /// <summary>
        /// The _classes.
        /// </summary>
        private readonly Dictionary<string, object> _classes = new Dictionary<string, object>();

        /// <summary>
        /// The class buffer
        /// </summary>
        private readonly StringBuilder _classesBuffer = new StringBuilder();

        /////// <summary>
        /////// The _data.
        /////// </summary>
        ////private List<Data> _data = new List<Data>();

        /// <summary>
        /// The _metadata.
        /// </summary>
        private JSTreeMetadata _metadata;

        /// <summary>
        /// The _state.
        /// </summary>
        private string _state = JSTreeConstants.CloseState; // change to "open" to expand all by default

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the tree node attributes. The attributes apply to the <c>li</c> tag
        /// </summary>
        public Dictionary<string, object> attr
        {
            get
            {
                this.AddClassToAttr();
                return this._attributes;
            }
        }

        /// <summary>
        /// Gets the list of children of this tree node
        /// </summary>
        public List<JsTreeNode> children
        {
            get
            {
                return this._children;
            }
        }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string data { get; set; }

        /////// <summary>
        /////// The list of data. Each data item reflects a different language/locale.
        /////// </summary>
        ////public List<Data> data
        ////{
        ////    get
        ////    {
        ////        if (this._data == null)
        ////        {
        ////            this._data = new List<Data>();
        ////        }

        ////        return this._data;
        ////    }

        ////    set
        ////    {
        ////        this._data = value;
        ////    }
        ////}

        /// <summary>
        /// Gets or sets metadata. This can be used to store various things lile id, version and other information
        /// </summary>
        public JSTreeMetadata metadata
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

        /// <summary>
        /// Gets or sets the current state. <see cref="JSTreeConstants.OpenState"/> or <see cref="JSTreeConstants.CloseState"/>. Default is <see cref="JSTreeConstants.CloseState"/>s
        /// </summary>
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
        /// Check the node (used with checkbox plugin)
        /// </summary>
        public void Check()
        {
            this.AddClass("jstree-checked");
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
        /// Set leaf class
        /// </summary>
        /// <param name="isLeaf">
        /// If it is true add jstree-leaf class else remove it
        /// </param>
        public void SetLeaf(bool isLeaf)
        {
            if (isLeaf)
            {
                this.AddClass("jstree-leaf");
            }
            else
            {
                this.RemoveClass("jstree-leaf");
            }
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

        /// <summary>
        /// Uncheck the node (used with checkbox plugin)
        /// </summary>
        public void Unchecked()
        {
            this.RemoveClass("jstree-checked");
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

    /*
    /// <summary>
    /// The language specific data of each node. 
    /// </summary>
    internal class Data
    {
        #region Constants and Fields

        /////// <summary>
        /////// The _icon.
        /////// </summary>
        ////private string _icon; // was "folder";


        /////// <summary>
        /////// The _language.
        /////// </summary>
        ////private string _language;

        /// <summary>
        /// The _title.
        /// </summary>
        private string _title;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the language specific icon. TODO this probably can be removed.
        /// </summary>
        public string icon
        {
            get
            {
                return this._icon;
            }

            set
            {
                this._icon = value;
            }
        }

        /// <summary>
        /// Gets the language of this data
        /// </summary>
        public string language
        {
            get
            {
                return this._language;
            }

            set
            {
                this._language = value;
            }
        }

        /// <summary>
        /// Gets the language specific title
        /// </summary>
        public string title
        {
            get
            {
                return this._title;
            }

            set
            {
                this._title = value;
            }
        }

        #endregion
    }
*/
}