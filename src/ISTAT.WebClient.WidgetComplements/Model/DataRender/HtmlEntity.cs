// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlEntity.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This class is a base for html like elements, currently only table td and tr
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;

    /// <summary>
    /// This class is a base for html like elements, currently only table td and tr 
    /// </summary>
    public abstract class HtmlEntity
    {
        #region Constants and Fields

        /// <summary>
        /// The attributes of the element
        /// </summary>
        private readonly Dictionary<string, string> _attr = new Dictionary<string, string>();

        /// <summary>
        /// The class attribute contents
        /// </summary>
        private readonly StringBuilder _classContents = new StringBuilder();

        /// <summary>
        /// The set of classes inside the class attribute
        /// </summary>
        private readonly Dictionary<string, object> _classes = new Dictionary<string, object>();

        /// <summary>
        /// The element tag
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// The element text
        /// </summary>
        private string _text = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntity"/> class with the specified <paramref name="tag"/>. 
        /// </summary>
        /// <param name="tag">
        /// The element tag
        /// </param>

        protected HtmlEntity(string tag)
        {
            this._tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntity"/> class with the specified <paramref name="tag"/> and <paramref name="text"/>. 
        /// </summary>
        /// <param name="tag">
        /// The element tag
        /// </param>
        /// <param name="text">
        /// The element text
        /// </param>
        protected HtmlEntity(string tag, string text)
        {
            this._tag = tag;
            this.Text = text;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the map of attributes name (as key) and attribute value (as value)
        /// </summary>
        public IDictionary<string, string> Attributes
        {
            get
            {
                this.AddClassToAttr();
                return this._attr;
            }
        }

        /// <summary>
        /// Gets the element tag
        /// </summary>
        public string Tag
        {
            get
            {
                return this._tag;
            }
        }

        /// <summary>
        /// Gets or sets the element text
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }

            set
            {
                this._text = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an attribute
        /// </summary>
        /// <param name="name">
        /// The attribute name
        /// </param>
        /// <param name="value">
        /// The attribute value
        /// </param>
        public void AddAttribute(string name, string value)
        {
            this._attr[name] = value;
        }

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
        /// Check if the specified class value is set
        /// </summary>
        /// <param name="classValue">
        /// The class value
        /// </param>
        /// <returns>
        /// True if is set or else false
        /// </returns>
        public bool HasClass(string classValue)
        {
            return classValue != null && this._classes.ContainsKey(classValue);
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
        /// Returns a string representation of this html entity
        /// </summary>
        /// <returns>
        /// A string representation of this html entity
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            {
                this.Write(writer);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Write this html entity and it's children as html
        /// </summary>
        /// <param name="writer">
        /// The output <see cref="TextWriter"/>
        /// </param>
        public virtual void Write(TextWriter writer)
        {
            writer.Write("<{0}", this.Tag);
            foreach (KeyValuePair<string, string> entry in this.Attributes)
            {
                writer.Write(" {0}=\"{1}\"", entry.Key, HttpUtility.HtmlAttributeEncode(entry.Value));
            }

            if (string.IsNullOrEmpty(this._text))
            {
                writer.Write("/>");
            }
            else
            {
                writer.Write(">");
                if (!string.IsNullOrEmpty(this._text))
                {
                    writer.Write(HttpUtility.HtmlEncode(this._text));
                }

                writer.Write("</{0}>", this.Tag);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add the contents of <see cref="_classes"/> to <see cref="_attr"/>
        /// </summary>
        private void AddClassToAttr()
        {
            this._classContents.Length = 0;

            foreach (string key in this._classes.Keys)
            {
                this._classContents.Append(key);
                this._classContents.Append(' ');
            }

            if (this._classContents.Length > 0)
            {
                this._classContents.Length--;
                this._attr["class"] = this._classContents.ToString();
            }
            else
            {
                this._attr.Remove("class");
            }
        }

        #endregion
    }
}