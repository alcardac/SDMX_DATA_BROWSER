// -----------------------------------------------------------------------
// <copyright file="SessionQueryManager.cs" company="EUROSTAT">
//   Date Created : 2010-11-11
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
namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    using System;
    using System.Threading;
    using System.Web;
    using System.Web.SessionState;
    //using Estat.Nsi.Client;
    //using ISTAT.WebClient.NSIWC;
    using ISTAT.WebClient.WidgetComplements.Model.NSIWC;
    /// <summary>
    /// The session query manager.
    /// </summary>
    public static class SessionQueryManager
    {
        #region Public Methods

        /// <summary>
        /// Apply query culture to current thread.
        /// </summary>
        /// <param name="query">
        /// The session query containing the culture
        /// </param>
        public static void ApplyCultureToThread(SessionQuery query)
        {
            if (query != null && query.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = query.CurrentCulture;
            }
        }

        /// <summary>
        /// Retrieves the query saved into specified HTTP session.
        /// </summary>
        /// <param name="session">
        /// the HTTP session
        /// </param>
        /// <returns>
        /// the query instance, which will never be <c>null</c> if the method succeeds
        /// </returns>
        public static SessionQuery GetSessionQuery(HttpSessionStateBase session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            SessionQuery query = (SessionQuery)session[Constants.HTTPSessionQueryAttr];

            //getting EndpointType
            //NSIClientSettings settings = NSIClientSettings.Instance;
            //EndpointType endPointType = (EndpointType)Enum.Parse(typeof(EndpointType), settings.EndPointType);
            //query.EndPointType = Enum.IsDefined(typeof(EndpointType), endPointType) ? endPointType : EndpointType.V20;

            ApplyCultureToThread(query);

            // if (query == null)
            // {
            // throw new ArgumentException("No query instance found into HTTP session");
            // }
            return query;
        }

        /// <summary>
        /// Save the specified session query to the specified Session
        /// </summary>
        /// <param name="session">
        /// The HTTP Session
        /// </param>
        /// <param name="query">
        /// The new session query
        /// </param>
        /// <exception cref="ArgumentNullException">session is null</exception>
        /// <returns>
        /// The saved <paramref name="query"/>
        /// </returns>
        public static SessionQuery SaveSessionQuery(HttpSessionStateBase session, SessionQuery query)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            session[Constants.HTTPSessionQueryAttr] = query;
            return query;
        }

        public static SessionQuery SaveSessionQuery(HttpSessionState session, SessionQuery query)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            session[Constants.HTTPSessionQueryAttr] = query;
            return query;
        }


        /// <summary>
        /// Clear session
        /// </summary>
        /// <param name="session">
        /// The current <see cref="HttpSessionState"/>
        /// </param>
        public static void ResetSession(HttpSessionStateBase session)
        {
            if (session != null)
            {
                if (SessionQueryExistsAndIsValid(session))
                {
                    GetSessionQuery(session).Reset();
                    GetSessionQuery(session).ResetDsds();
                }

                session.Clear();
            }
        }

        /// <summary>
        /// Tests to see if a query instance is saved into the HTTP session associated with specified context.
        /// </summary>
        /// <param name="context">
        /// the HTTP context
        /// </param>
        /// <returns>
        /// <c>true</c> if session exists and is valid, <c>false</c> otherwise
        /// </returns>
        public static bool SessionQueryExistsAndIsValid(HttpContext context)
        {
            return context.Session != null && context.Session[Constants.HTTPSessionQueryAttr] != null;
        }

        /// <summary>
        /// Tests to see if a query instance is saved into the HTTP session associated with specified context.
        /// </summary>
        /// <param name="session">
        /// the current session
        /// </param>
        /// <returns>
        /// <c>true</c> if session exists and is valid, <c>false</c> otherwise
        /// </returns>
        public static bool SessionQueryExistsAndIsValid(HttpSessionStateBase session)
        {
            return session != null && session[Constants.HTTPSessionQueryAttr] != null;
        }

        #endregion
    }
}