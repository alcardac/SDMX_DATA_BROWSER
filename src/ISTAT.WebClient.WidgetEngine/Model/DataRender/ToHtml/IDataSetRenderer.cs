namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    using System.IO;

    /// <summary>
    /// Interface for all renderers
    /// </summary>
    public interface IDataSetRenderer
    {
        #region Public Properties

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        string StandardFileExtension { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Render all data add and writes it to the specified writer
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        void RenderAllTables(IDataSetModel model, TextWriter os);

        /// <summary>
        /// Render all data add and writes it to the specified stream
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        void RenderAllTables(IDataSetModel model, Stream os);

        #endregion
    }
}