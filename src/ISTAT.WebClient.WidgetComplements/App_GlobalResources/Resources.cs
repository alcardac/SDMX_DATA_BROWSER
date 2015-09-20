using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.Properties
{
    public class Resources
    {
        public static string DefaultHeaderId = "NSICLIENT_NET";
        public static string defaultLanguage = "en";
        public static string ErrorAccessDllFormat1 = "Could not access the {0}. Please check the permissions";
        public static string ErrorApplicationError = "An application error occurred";
        public static string ErrorApplicationErrorFormat2 = "Requested URL:{0} Exception details: {1}";
        public static string ErrorAssemblyNotConfiguredFormat1 = "Assembly '{0}' was requested but is not configured.";
        public static string ErrorCannotFindLocaleFormat2 = "Could not find locale '{0}'. Available locale '{1}'";
        public static string ErrorDllBadFormat1 = "The DLL '{0}' seems to be for a different architecture";
        public static string ErrorInvalidAxisValueFormat1 = "Invalid defaultAxis value : '{0}'";
        public static string ErrorInvalidTimeDimensionCodelist = "NSI WS responded with an invalid code list for Time Dimension";
        public static string ErrorLayoutFileMissingFormat1 = "The configured default layout file doesn't exist. File : '{0}'";
        public static string ErrorLoadingDllFormat1 = "An error occurred while trying to pre-load DLL. Exception :\n\r'{0}'";
        public static string ErrorLoadingSqlite = "Could not load SQLite.";
        public static string ErrorLoadingSqliteConfig = "Could not load SQLite due to configuration issues";
        public static string ErrorMaxJsonLength = "Please try increasing MaxJsonLength at web.config";
        public static string ErrorMissingDllFormat1 = "DLL '{0}' doesn't exist.";
        public static string ErrorRecursionLimit = "Please try increasing RecursionLimit at web.config";
        public static string ExceptionCannotLoadWsdlFormat2 = "CONFIG ERROR:Cannot load the WSDL '{0}' for endpoint '{1}'";
        public static string ExceptionEndpointNotSet = "Endpoint is not set";
        public static string ExceptionErrorGettingCategorySchemes = "Error getting GetCategorySchemes";
        public static string ExceptionExecuteQuery = "Error while ExecuteQuery";
        public static string ExceptionGetCodelistFormat2 = "Error getting GetCodelist for {0}. Error : {1}";
        public static string ExceptionGettingDataflow = "Error getting GetDataflows";
        public static string ExceptionGettingStructure = "Error getting GetStructure";
        public static string ExceptionInvalidNumberOfCodeListsFormat1 = "Zero or more than one codelist returned by the server, for {0}";
        public static string ExceptionKeyFamilyCountNot1 = "SERVER RESPONSE getStructure ERROR: The number of keyFamilies is not 1";
        public static string ExceptionMissingConceptSchemeFormat1 = "SERVER RESPONSE getStructure ERROR: Missing referenced concept  scheme : {0}";
        public static string ExceptionMissingResponse = "SERVER RESPONSE ERROR: Missing response";
        public static string ExceptionMissingStructure = "SERVER RESPONSE ERROR: Missing Structure from response";
        public static string ExceptionNameTable_is_null = "XmlReader.NameTable is null";
        public static string ExceptionNoTimeDimension = "Cannot add a time component because the key family does not have a time dimension";
        public static string ExceptionParsingCountCodelistFormat0 = "Error parsing the count codelist for {0}";
        public static string ExceptionReadingConfiguration = "Could not read the NSIClient configuration .\n\rReason :";
        public static string ExceptionReceivedSoapFault = "Received a SOAP FAULT:";
        public static string ExceptionServerResponse = "SERVER RESPONSE ERROR:";
        public static string ExceptionServerResponseInvalidKeyFamily = "SERVER RESPONSE getStructure ERROR: The KeyFamily is not the same as the dataflow referenced keyfamily";
        public static string ExceptionUnsupported_operation = "Unsupported operation";
        public static string ExceptionZeroCodesFormat1 = "Zero codes in codelist returned by the server, for {0}";
        public static string FailureFromServer = "SERVER RESPONSE Failure";
        public static string InfoAssemblyLoadSuccessFormat1 = "Assembly {0} was loaded successfully";
        public static string InfoConfiguredPath = "Configured path:{0}";
        public static string InfoCountFormat2 = "Dataflow {0}, Codelist : {1}";
        public static string InfoCreatingNsiClient = "Creating new  NSI Client";
        public static string InfoGettingCategorySchemes = "Getting all category schemes";
        public static string InfoGettingCodelistFormat1 = "Getting codelist for {0}";
        public static string InfoGettingDataFlows = "Getting all dataflows";
        public static string InfoGettingStructureFormat3 = "Getting structure for {0}+{1}+{2}";
        public static string InfoGetWSDL = "Retrieve/Load WSDL...";
        public static string InfoLoadingAssemblyFormat1 = "Loading assembly {0}...";
        public static string InfoPartialCodelistFormat3 = "Dataflow {0}, Component : {1} , Codelist : {2}";
        public static string InfoPlatformSettingsInitiated = "Platform Settings initiated";
        public static string InfoSoapResponse = "Soap RESPONSE :";
        public static string InfoSuccess = "|--Success";
        public static string InfoWebConfigLocation = "{0}\n\rExpected location of web.config : {1}";
        public static string InfoWSDLSuccess = "`-- WSDL loaded successfully!";
        public static string LogMaintainableValidationErrors = "{0} has validation errors";
        public static string NoResultsFound = "No results found";
        public static string SdmxHeaderPreparedDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        public static string StatusMessageFormat2 = "`-- {0}: {1}";
        public static string Unauthorized = "Unauthorized";
        public static string WarningFromServer = "SERVER RESPONSE has warnings:";
        public static string WarningOverMaximumObs = "Requested {0} observations for dataflow {1} but maximum is {2}";
    }
}
