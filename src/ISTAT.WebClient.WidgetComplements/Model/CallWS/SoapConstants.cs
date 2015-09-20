namespace ISTAT.WebClient.WidgetComplements.Model.CallWS
{
    /// <summary>
    /// A list of SOAP related constants
    /// </summary>
    internal static class SoapConstants
    {
        /// <summary>
        /// SOAP 1.1 namespace
        /// </summary>
        internal const string Soap11Ns = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// This field holds a template for soap 1.1 request envelope
        /// </summary>
        internal const string SoapRequest =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:{0}=\"{1}\"><soap:Header/><soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// SOAP Body tag
        /// </summary>
        internal const string Body = "Body";

        /// <summary>
        /// SOAPAction HTTP header
        /// </summary>
        internal const string SoapAction = "SOAPAction";
    }
}