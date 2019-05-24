﻿// =====================================================================
//  This file is part of the Microsoft Dynamics Accelerator code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Dynamics.Health.Samples
{
    public class Location
    {
        #region Class Level Members

        /// <summary>
        /// Stores the organization service proxy.
        /// </summary>
        private OrganizationServiceProxy _serviceProxy;

        #endregion Class Level Members

        #region How To Sample Code
        /// <summary>
        /// Create and configure the organization service proxy.
        /// Initiate the method to create any data that this sample requires.
        /// Create a location. 
        /// </summary>

        public void Run(ServerConnection.Configuration serverConfig, bool promptforDelete)
        {
            try
            {
                //<snippetMarketingAutomation1>
                // Connect to the Organization service. 
                // The using statement assures that the service proxy will be properly disposed.
                using (_serviceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials))
                {
                    // This statement is required to enable early-bound type support.
                    _serviceProxy.EnableProxyTypes();
                    
                    Entity location = new Entity("msemr_location");
                    
                    location["msemr_name"] = "Noble Hospital Center";

                    location["msemr_addresstype"] = new OptionSetValue(935000001); //Physical
                    location["msemr_addressuse"] = new OptionSetValue(935000001); //Work
                    location["msemr_addresscity"] = "Albuquerque";
                    location["msemr_addresscountry"] = "US";                   
                    location["msemr_addressline1"] = "1770";
                    location["msemr_addressline2"] = "Byrd";
                    location["msemr_addressline3"] = "Lane";
                    location["msemr_addresspostalcode"] = "87107";
                    location["msemr_addressstate"] = "New Mexico";
                    location["msemr_addresstext"] = "Primary Location";
                    location["msemr_addressperiodend"] = DateTime.Now;
                    location["msemr_addressperiodstart"] = DateTime.Now.AddDays(20);
                    
                    Guid accountId = Organization.GetAccountId(_serviceProxy, "Galaxy Corp");
                    if (accountId != Guid.Empty)
                    {
                        location["msemr_managingorganization"] = new EntityReference("account", accountId);
                    }

                    Guid partOfLocationId = SDKFunctions.GetLocationId(_serviceProxy, "FHIR Organizational Unit");
                    if (partOfLocationId != Guid.Empty)
                    {
                        location["msemr_partof"] = new EntityReference("msemr_location", partOfLocationId);
                    }

                    Guid physicaltypeCodeableConceptId = SDKFunctions.GetCodeableConceptId(_serviceProxy, "si", 935000072);
                    if (physicaltypeCodeableConceptId != Guid.Empty)
                    {
                        location["msemr_physicaltype"] = new EntityReference("msemr_codeableconcept", physicaltypeCodeableConceptId);
                    }

                    Guid typeCodeableConceptId = SDKFunctions.GetCodeableConceptId(_serviceProxy, "Dx", 935000131);
                    if (typeCodeableConceptId != Guid.Empty)
                    {
                        location["msemr_type"] = new EntityReference("msemr_codeableconcept", typeCodeableConceptId);
                    }
                    
                    location["msemr_mode"] = new OptionSetValue(935000001); //Kind

                    location["msemr_status"] = new OptionSetValue(935000000); //Active

                    location["msemr_operationalstatus"] = new OptionSetValue(935000004); //Occupied
                    
                    location["msemr_description"] = "Primary Location";

                    location["msemr_locationnumber"] = "L442";
                    location["msemr_locationalias1"] = "General Hospital";
                    location["msemr_locationpositionaltitude"] = (decimal) 18524.5265;
                    location["msemr_locationpositionlatitude"] = (decimal) 24.15;
                    location["msemr_locationpositionlongitude"] = (decimal) 841.12;

                    Guid locationId = _serviceProxy.Create(location);

                    // Verify that the record has been created.
                    if (locationId != Guid.Empty)
                    {
                        Console.WriteLine("Succesfully created {0}.", locationId);
                    }
                }
            }
            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        #endregion How To Sample Code

        #region Main Method

        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        /// <param name="args"></param>
        static public void Main(string[] args)
        {
            try
            {
                // Obtain the target organization's Web address and client logon 
                // credentials from the user.
                ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration config = serverConnect.GetServerConfiguration();

                Location app = new Location();
                app.Run(config, true);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
        #endregion Main method
    }
}
