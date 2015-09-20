
namespace ISTAT.WebClient.WidgetComplements.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Header;

    using Estat.Sri.SdmxStructureMutableParser.Model;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
    using ISTAT.WebClient.WidgetComplements.Model.Settings;
    using System.IO;

    /// <summary>
    /// Description of Utils.
    /// </summary>
    public static class Utils
    {
        #region Constants and Fields

        /// <summary>
        /// This regex object holds the regular expression that validates the time dimension contrains
        /// </summary>
        private static readonly Regex _timePeriodValidate =
            new Regex("^[12][09][0-9]{2}(-((Q[1-4])|(W[1-5][0-9])|(H[12])|([01][0-9])|([01][0-9]-[0-3][0-9])))?$");

        #endregion

        #region Public Methods

        /// <summary>
        /// The contents from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// The source collection.
        /// </param>
        /// <param name="destination">
        /// The destination collection.
        /// </param>
        /// <typeparam name="T">
        /// The type of the <paramref name="source"/> and <paramref name="destination"/> items
        /// </typeparam>
        public static void CopyCollection<T>(ICollection<T> source, ICollection<T> destination)
        {
            foreach (var value in source)
            {
                destination.Add(value);
            }
        }

        /// <summary>
        /// Get a concept from the specified map from the specified component
        /// </summary>
        /// <param name="component">
        /// The component
        /// </param>
        /// <param name="conceptMap">
        /// The concept id to concept bean map
        /// </param>
        /// <returns>
        /// The concept of the specified component or null
        /// </returns>
        public static IConceptObject GetCachedConcept(IComponent component, Dictionary<string, IConceptObject> conceptMap)
        {
            IConceptObject concept;
            conceptMap.TryGetValue(MakeKeyForConcept(component), out concept);
            return concept;
        }

        /// <summary>
        /// Get the Dimension or TimeDimension that uses the specified concept id inside a KeyFamily
        /// </summary>
        /// <param name="keyFamily">
        /// The KeyFamily to search
        /// </param>
        /// <param name="concept">
        /// The concept id e.g. "FREQ"
        /// </param>
        /// <returns>
        /// The Dimension or TimeDimension as ComponentBean if found, else null 
        /// </returns>
        public static IDimension GetComponentByName(IDataStructureObject keyFamily, string concept)
        {
            IDimension component;
            if (keyFamily.TimeDimension != null && keyFamily.TimeDimension.Id.Equals(concept))
            {
                component = keyFamily.TimeDimension;
            }
            else
            {
                component = keyFamily.GetDimension(concept);
            }

            return component;
        }

        /// <summary>
        /// Get localised name for the specified artefact
        /// </summary>
        /// <param name="identifiable">
        /// The artefact
        /// </param>
        /// <returns>
        /// The localized name if it exists or the id 
        /// </returns>
        public static string GetLocalizedName(INameableObject identifiable)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;
            string lang = cultureInfo.TwoLetterISOLanguageName;
            string ret = TextTypeHelper.GetText(identifiable.Names, lang);
            if (string.IsNullOrEmpty(ret))
            {
                ret = identifiable.Id;
            }

            return ret;
        }

        /// <summary>
        /// Check if the specified <paramref name="keyFamily"/> can do Time Series data
        /// </summary>
        /// <param name="keyFamily">
        /// The <see cref="KeyFamilyBean"/>
        /// </param>
        /// <returns>
        /// True if it is a time series DSD else false
        /// </returns>
        public static bool IsTimeSeries(IDataStructureObject keyFamily)
        {
            return string.IsNullOrEmpty(Validator.ValidateForCompact(keyFamily));
        }

        /// <summary>
        /// This is an utility method that verify if a string text equals "true" constant value
        /// The check is done in invariant culture and the string case is ignored.
        /// In case the input is null or empty the result is false
        /// </summary>
        /// <param name="input">
        /// The string that needs to be check
        /// </param>
        /// <returns>
        /// The returned values are:
        /// <list type="bullet">
        /// <item>
        /// true in case the string is "true"
        /// </item>
        /// <item>
        /// false in case the string is null or empty or any other string
        /// </item>
        /// </list>
        /// </returns>
        public static bool IsTrueString(string input)
        {
            bool ret = false;

            if (!string.IsNullOrEmpty(input))
            {
                ret = input.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return ret;
        }

        /// <summary>
        /// Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="artefact">
        /// The artefact to generate the key for
        /// </param>
        /// <returns>
        /// The key
        /// </returns>
        public static string MakeKey(IMaintainableObject artefact)
        {
            return MakeKey(artefact.Id, artefact.AgencyId, artefact.Version);
        }

        /// <summary>
        /// Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        /// The artefact reference to generate the key for
        /// </param>
        /// <returns>
        /// The key
        /// </returns>
        public static string MakeKey(IStructureReference reference)
        {
            return MakeKey(reference.MaintainableReference.MaintainableId, reference.MaintainableReference.AgencyId, reference.MaintainableReference.Version);
        }

        /// <summary>
        /// Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        /// The artefact reference to generate the key for
        /// </param>
        /// <returns>
        /// The key
        /// </returns>
        public static string MakeKey(ICrossReference reference)
        {
            return MakeKey(reference.MaintainableReference.MaintainableId, reference.MaintainableReference.AgencyId, reference.MaintainableReference.Version);
        }

        /// <summary>
        /// Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        /// The artefact reference to generate the key for
        /// </param>
        /// <returns>
        /// The key
        /// </returns>
        public static string MakeKey(IMaintainableRefObject reference)
        {
            return MakeKey(reference.MaintainableId, reference.AgencyId, reference.Version);
        }

        /// <summary>
        /// Create a key for the <paramref name="item"/> that belongs to <paramref name="itemScheme"/>
        /// </summary>
        /// <param name="item">
        /// The <see cref="ItemBean"/>
        /// </param>
        /// <param name="itemScheme">
        /// The <see cref="ItemSchemeBean"/>
        /// </param>
        /// <returns>
        /// A string that uniquely identifies the <paramref name="item"/>
        /// </returns>
        public static string MakeKey(IItemObject item, IItemSchemeObject<IConceptObject> itemScheme)
        {
            return MakeKey(item.Id, itemScheme.Id, itemScheme.AgencyId, itemScheme.Version);
        }

        /// <summary>
        /// Make a key from id, agency and version
        /// </summary>
        /// <param name="id">
        /// The ID
        /// </param>
        /// <param name="agency">
        /// The Agency
        /// </param>
        /// <param name="version">
        /// The version
        /// </param>
        /// <returns>
        /// A key id
        /// </returns>
        public static string MakeKey(string id, string agency, string version)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}+{1}+{2}", agency, id, version);
        }

        /// <summary>
        /// Make a key from concept scheme id, agency and version and concept id
        /// </summary>
        /// <param name="conceptId">
        /// The <see cref="ConceptBean"/> Id
        /// </param>
        /// <param name="conceptSchemeId">
        /// The <see cref="ConceptSchemeBean"/> id
        /// </param>
        /// <param name="conceptSchemeAgency">
        /// The <see cref="ConceptSchemeBean"/> Agency
        /// </param>
        /// <param name="conceptSchemeVersion">
        /// The <see cref="ConceptSchemeBean"/> Version
        /// </param>
        /// <returns>
        /// The make key.
        /// </returns>
        public static string MakeKey(
            string conceptId, string conceptSchemeId, string conceptSchemeAgency, string conceptSchemeVersion)
        {
            return string.Format(
                CultureInfo.InvariantCulture, "{0}:{1}", MakeKey(conceptSchemeId, conceptSchemeAgency, conceptSchemeVersion), conceptId);
        }

        /// <summary>
        /// Make a key for <see cref="ConceptBean"/>
        /// </summary>
        /// <param name="component">
        /// The <see cref="ComponentBean"/> containing the concept reference
        /// </param>
        /// <returns>
        /// The key for concept.
        /// </returns>
        public static string MakeKeyForConcept(IComponent component)
        {
            IStructureReference concept = component.ConceptRef;
            return MakeKey(
                concept.ChildReference.Id,
                concept.MaintainableReference.MaintainableId,
                concept.MaintainableReference.AgencyId,
                concept.MaintainableReference.Version);
        }

        /// <summary>
        /// Populate the specified map from ConceptSchemes contained in the specified structure
        /// </summary>
        /// <param name="structure">
        /// The structure containing the ConceptSchemes
        /// </param>
        /// <param name="conceptMap">
        /// The concept id to concept bean map
        /// </param>
        public static void PopulateConceptMap(ISdmxObjects structure, Dictionary<string, IConceptObject> conceptMap)
        {
            conceptMap.Clear();
            foreach (IConceptSchemeObject conceptScheme in structure.ConceptSchemes)
            {
                foreach (IConceptObject concept in conceptScheme.Items)
                {
                    string key = MakeKey(concept, conceptScheme);
                    if (!conceptMap.ContainsKey(key))
                    {
                        conceptMap.Add(key, concept);
                    }
                }
            }
        }

        /// <summary>
        /// Validate time period
        /// </summary>
        /// <param name="time">
        /// The time period to validate
        /// </param>
        /// <returns>
        /// <c>True</c> if it is valid; otherwise <c>False</c>
        /// </returns>
        public static bool ValidateTimePeriod(string time)
        {
            if (!string.IsNullOrEmpty(time) && !_timePeriodValidate.IsMatch(time))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Populate the given headerbean object from the apllication settings
        /// </summary>
        /// <param name="header">
        /// The headerBean to populate
        /// </param>
        public static void PopulateHeaderFromSettings(IHeader header)
        {
            header.Id = string.IsNullOrEmpty(HeaderSettings.Default.id) ? Resources.DefaultHeaderId : HeaderSettings.Default.id;
            header.Test = bool.Parse(HeaderSettings.Default.test);

            if (!string.IsNullOrEmpty(HeaderSettings.Default.name))
            {
                header.AddName(new TextTypeWrapperImpl(HeaderSettings.Default.lang, HeaderSettings.Default.name, null));
            }
            //TODO this propery has no setter - please add it
            //  header.Prepared = DateTime.Now.ToString(Resources.SdmxHeaderPreparedDateTimeFormat, CultureInfo.InvariantCulture);

            IList<ITextTypeWrapper> textTypeWrapperSender = new List<ITextTypeWrapper>();
            IContactMutableObject senderContact = new ContactMutableObjectCore();

            if (!string.IsNullOrEmpty(HeaderSettings.Default.sendername))
            {
                textTypeWrapperSender.Add(new TextTypeWrapperImpl(HeaderSettings.Default.lang, HeaderSettings.Default.sendername, null));
            }

            if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactname))
            {
                senderContact.AddName(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.sendercontactname));

                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactdepartment))
                {
                    senderContact.AddDepartment(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.sendercontactdepartment));
                }

                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactrole))
                {
                    senderContact.AddRole(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.sendercontactrole));
                }

                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontacttelephone))
                {
                    senderContact.AddTelephone(HeaderSettings.Default.sendercontacttelephone);
                }
                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactfax))
                {
                    senderContact.AddFax(HeaderSettings.Default.sendercontactfax);
                }
                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactx400))
                {
                    senderContact.AddX400(HeaderSettings.Default.sendercontactx400);
                }
                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontacturi))
                {
                    senderContact.AddUri(HeaderSettings.Default.sendercontacturi);
                }
                if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactemail))
                {
                    senderContact.AddEmail(HeaderSettings.Default.sendercontactemail);
                }
            }
            IContact contactImmutable = new ContactCore(senderContact);
            IList<IContact> contacts = new List<IContact>();
            contacts.Add(contactImmutable);
            // SENDER
            var sender = new PartyCore(textTypeWrapperSender, HeaderSettings.Default.senderid, contacts, null);
            header.Sender = sender;

            IList<ITextTypeWrapper> textTypeWrapperReceiver = new List<ITextTypeWrapper>();
            IContactMutableObject receiverContact = new ContactMutableObjectCore();

            if (!string.IsNullOrEmpty(HeaderSettings.Default.receiverid))
            {

                if (!string.IsNullOrEmpty(HeaderSettings.Default.receivername))
                {
                    textTypeWrapperReceiver.Add(new TextTypeWrapperImpl(HeaderSettings.Default.lang, HeaderSettings.Default.receivername, null));
                }

                if (!string.IsNullOrEmpty(HeaderSettings.Default.receivercontactname))
                {
                    receiverContact.AddName(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.receivercontactname));

                    if (!string.IsNullOrEmpty(HeaderSettings.Default.receivercontactdepartment))
                    {
                        receiverContact.AddDepartment(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.receivercontactdepartment));
                    }

                    if (!string.IsNullOrEmpty(HeaderSettings.Default.receivercontactrole))
                    {
                        receiverContact.AddRole(new TextTypeWrapperMutableCore(HeaderSettings.Default.lang, HeaderSettings.Default.receivercontactrole));
                    }
                    if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontacttelephone))
                    {
                        receiverContact.AddTelephone(HeaderSettings.Default.sendercontacttelephone);
                    }
                    if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactfax))
                    {
                        receiverContact.AddFax(HeaderSettings.Default.sendercontactfax);
                    }
                    if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactx400))
                    {
                        receiverContact.AddX400(HeaderSettings.Default.sendercontactx400);
                    }
                    if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontacturi))
                    {
                        receiverContact.AddUri(HeaderSettings.Default.sendercontacturi);
                    }
                    if (!string.IsNullOrEmpty(HeaderSettings.Default.sendercontactemail))
                    {
                        receiverContact.AddEmail(HeaderSettings.Default.sendercontactemail);
                    }

                }
                // RECEIVER
                IContact contactImmutableReceiver = new ContactCore(receiverContact);
                IList<IContact> contactsReceiver = new List<IContact>();
                contactsReceiver.Add(contactImmutableReceiver);
                IParty receiver = new PartyCore(textTypeWrapperReceiver, HeaderSettings.Default.receiverid, contactsReceiver, null);

                header.AddReciever(receiver);
            }

            if (!string.IsNullOrEmpty(HeaderSettings.Default.source))
            {
                header.AddSource(new TextTypeWrapperImpl(HeaderSettings.Default.lang, HeaderSettings.Default.source, null));
            }
        }


        /// <summary>
        /// The Virtual path in IIS
        /// </summary>
        private const string VirtualPath = "~/";

        public static string GetRootApp()
        {
            return System.Web.HttpContext.Current.Server.MapPath(VirtualPath);
        }

        public static string App_Data_Path { get; set; }
        public static string GetAppPath()
        {
            string pathDB = Path.Combine(App_Data_Path, "ISTAT.WebClient");
            if (!Directory.Exists(pathDB))
                Directory.CreateDirectory(pathDB);
            return pathDB;
        }

        public static string GetTreeCachePath()
        {
            string pathDB = Path.Combine(App_Data_Path, "ISTAT.WebClient", "Tree");
            if (!Directory.Exists(pathDB))
                Directory.CreateDirectory(pathDB);

            return pathDB;
        }
        #endregion
    }


}
