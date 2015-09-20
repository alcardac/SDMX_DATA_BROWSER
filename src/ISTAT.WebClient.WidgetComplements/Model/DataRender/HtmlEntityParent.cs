// --------------------------------------------------------------------------------------------------------------------
namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    /// <summary>
    /// A <see cref="HtmlEntity"/> with children
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="HtmlEntity"/> based children of this <see cref="HtmlEntityParent{T}"/>
    /// </typeparam>
    public abstract class HtmlEntityParent<T> : HtmlEntity
        where T : HtmlEntity
    {

        #region Constants and Fields

        /// <summary>
        /// The element children
        /// </summary>
        private readonly HtmlEntityCollection<T> _children = new HtmlEntityCollection<T>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntityParent{T}"/> class with the specific <paramref name="tag"/>. 
        /// </summary>
        /// <param name="tag">
        /// The element tag
        /// </param>
        protected HtmlEntityParent(string tag)
            : base(tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntityParent{T}"/> class with the specific <paramref name="tag"/> and <paramref name="child"/>. 
        /// </summary>
        /// <param name="tag">
        /// The element tag
        /// </param>
        /// <param name="child">
        /// The element child
        /// </param>
        protected HtmlEntityParent(string tag, T child)
            : base(tag)
        {
            this.Children.Add(child);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntityParent{T}"/> class with the specific <paramref name="tag"/> and <paramref name="text"/>. 
        /// </summary>
        /// <param name="tag">
        /// The element tag
        /// </param>
        /// <param name="text">
        /// The element text
        /// </param>
        protected HtmlEntityParent(string tag, string text)
            : base(tag, text)
        {
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the element children
        /// </summary>
        public HtmlEntityCollection<T> Children
        {
            get
            {
                return this._children;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add the <paramref name="children"/> to the <see cref="Children"/>
        /// </summary>
        /// <param name="children">
        /// The children.
        /// </param>
        public void Add(IEnumerable<T> children)
        {
            foreach (var child in children)
            {
                this._children.Add(child);
            }
        }

        /// <summary>
        /// Add a child element
        /// </summary>
        /// <param name="child">
        /// The child element
        /// </param>
        public void AddElement(T child)
        {
            this.Children.Add(child);
        }

        /// <summary>
        /// Write this html entity and it's children as html
        /// </summary>
        /// <param name="writer">
        /// The output <see cref="TextWriter"/>
        /// </param>
        public override void Write(TextWriter writer)
        {
            writer.Write("<{0}", this.Tag);

            string id = string.Empty;

            foreach (KeyValuePair<string, string> entry in this.Attributes)
            {
                if (entry.Key == "ID") id = HttpUtility.HtmlAttributeEncode(entry.Value);
                writer.Write(" {0}=\"{1}\"", entry.Key, HttpUtility.HtmlAttributeEncode(entry.Value));
            }

            writer.Write(">");

            if (this.HasClass(HtmlClasses.ExtraInfoWrapper))
            {
                string jsClass = HtmlClasses.ExtraInfoBtn;// +" ui-icon ui-icon-info";
                string jsFunc = "ShowExtraPopup('" + id + "');";
                writer.Write(string.Format("<span class=\"{0}\" onClick=\"{1}\"></span>", jsClass, jsFunc));

            }

            if (this._children.Count == 0 && string.IsNullOrEmpty(this.Text))
            {
                writer.Write("</{0}>", this.Tag);
            }
            else
            {

                if (!string.IsNullOrEmpty(this.Text))
                {
                    writer.Write(HttpUtility.HtmlEncode(this.Text));
                }

                foreach (var child in this._children)
                {
                    child.Write(writer);
                }

                writer.Write("</{0}>", this.Tag);
            }
        }
        #endregion
    }
}