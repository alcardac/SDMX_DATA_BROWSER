namespace ISTAT.WebClient.WidgetComplements.Model.DataRenderNSI
{
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using System.Collections.Generic;

    /// <summary>
    /// This class holds the display mode status of a dimensions and it's codes
    /// </summary>
    public class DisplayModeStatus
    {
        #region Constants and Fields

        /// <summary>
        /// The default display mode
        /// </summary>
        private const DisplayMode DefaultModeConst = DisplayMode.Description;

        /// <summary>
        /// Code value to display mode map
        /// </summary>
        private readonly Dictionary<string, DisplayMode> _codeDisplayMode = new Dictionary<string, DisplayMode>();

        /// <summary>
        /// The dimension wide display mode
        /// </summary>
        private DisplayMode _mode = DefaultModeConst;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the default display mode
        /// </summary>
        public static DisplayMode DefaultMode
        {
            get
            {
                return DefaultModeConst;
            }
        }

        /// <summary>
        /// Gets the code value to display mode map
        /// </summary>
        public Dictionary<string, DisplayMode> CodeDisplayMode
        {
            get
            {
                return this._codeDisplayMode;
            }
        }

        /// <summary>
        /// Gets or sets the dimension wide display mode
        /// </summary>
        public DisplayMode Mode
        {
            get
            {
                return this._mode;
            }

            set
            {
                this._mode = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the display mode of a code
        /// </summary>
        /// <param name="code">
        /// The code value
        /// </param>
        /// <returns>
        /// The display mode of a code if it exists or dimension wide display mode
        /// </returns>
        public DisplayMode GetDisplayMode(string code)
        {
            DisplayMode mode;
            return this._codeDisplayMode.TryGetValue(code, out mode) ? mode : this._mode;
        }

        /// <summary>
        /// Toggle the display mode dimension wide
        /// </summary>
        public void ToggleDisplayMode()
        {
            this._mode = Toggle(this._mode);
            this.ToggleAllCodesDisplayMode();
        }

        /// <summary>
        /// Toggle the display mode of a code
        /// </summary>
        /// <param name="code">
        /// The code value
        /// </param>
        public void ToggleDisplayMode(string code)
        {
            DisplayMode mode;
            if (this._codeDisplayMode.TryGetValue(code, out mode))
            {
                mode = Toggle(mode);
                if (mode == this._mode)
                {
                    this._codeDisplayMode.Remove(code);
                }
                else
                {
                    this._codeDisplayMode[code] = mode;
                }
            }
            else
            {
                mode = Toggle(this._mode);
                this._codeDisplayMode.Add(code, mode);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Toggle the <paramref name="mode"/>
        /// </summary>
        /// <param name="mode">
        /// The current <see cref="Mode"/>
        /// </param>
        /// <returns>
        /// The toggled <see cref="Mode"/>
        /// </returns>
        private static DisplayMode Toggle(DisplayMode mode)
        {
            return (DisplayMode)(((int)mode + 1) % 3);
        }

        /// <summary>
        /// Toggle the display mode of a code
        /// </summary>
        private void ToggleAllCodesDisplayMode()
        {
            var keys = new List<string>(this._codeDisplayMode.Keys);
            foreach (string key in keys)
            {
                DisplayMode mode = Toggle(this._codeDisplayMode[key]);
                if (mode == this._mode)
                {
                    this._codeDisplayMode.Remove(key);
                }
                else
                {
                    this._codeDisplayMode[key] = mode;
                }
            }
        }

        #endregion
    }
}