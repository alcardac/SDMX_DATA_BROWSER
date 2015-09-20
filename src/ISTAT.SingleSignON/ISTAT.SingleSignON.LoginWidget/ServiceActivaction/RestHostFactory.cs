using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace ISTAT.SingleSignON.LoginWidget.ServiceActivaction
{
    /// <summary>
    /// The sdmx rest service host factory.
    /// </summary>
    public class RestHostFactory : WebServiceHostFactory
    {


        #region Fields

        /// <summary>
        /// The _type.
        /// </summary>
        private readonly Type _type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SdmxRestServiceHostFactory"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public RestHostFactory(Type type)
        {
            this._type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create service host.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceHost"/>.
        /// </returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            try
            {
                ServiceHost serviceHost = base.CreateServiceHost(serviceType, baseAddresses);

                var webBehavior = new WebHttpBehavior { AutomaticFormatSelectionEnabled = false, HelpEnabled = true, FaultExceptionEnabled = false, DefaultBodyStyle = WebMessageBodyStyle.Bare };

                var webExtBehavior = new EnableCrossOriginResourceSharingBehavior();


                var binding = new WebHttpBinding { TransferMode = TransferMode.Streamed };

                //serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
                //serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
                var endpoint = serviceHost.AddServiceEndpoint(this._type, binding, baseAddresses[0]);

                endpoint.Behaviors.Add(webBehavior);
                //endpoint.Behaviors.Add(webExtBehavior);

                return serviceHost;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
