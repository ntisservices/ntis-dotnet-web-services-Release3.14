﻿using System;
using log4net;
using SubscriberWebService.Services;

namespace SubscriberWebService
{

    public class WebServiceSubscriber : supplierPushInterface
    {
        // syncLock object, used to lock the code block
        private static object syncLock = new object();

        protected static readonly ILog log = LogManager.GetLogger(typeof(WebServiceSubscriber));
        protected static IFusedFvdAndSensorTrafficDataService fusedFvdAndSensorTrafficDataService = new FusedFvdAndSensorTrafficDataService();
        protected static IMidasTrafficDataService midasTrafficDataService = new MidasTrafficDataService();
        protected static INtisModelNotificationDataService ntisModelNotificationService = new NtisModelNotificationDataService();
        protected static IFusedSensorOnlyTrafficDataService fusedSensorOnlyTrafficDataService = new FusedSensorOnlyTrafficDataService();
        protected static ITMUTrafficDataService tmuTrafficDataService = new TMUTrafficDataService();
        protected static IVMSDataService vmsDataService = new VMSDataService();
        protected static IEventDataService eventDataService = new EventDataService();

        public WebServiceSubscriber()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        // This method implements an interface method in the supplierPushInterface class (in autogenerated supplierPush.cs file)
        //
        // Ideally, this method should not return anything, but the  supplierPushInterface in the autogenerated supplierPush.cs demands it
        //
        public putDatex2DataResponse putDatex2Data(putDatex2DataRequest request)
        {

            lock (syncLock)
            {
                try
                {
                    D2LogicalModel d2LogicalModel = request.d2LogicalModel;
                    string feedType = d2LogicalModel.payloadPublication.feedType;
                    if (feedType.Contains("MIDAS Loop Traffic Data"))
                    {
                        midasTrafficDataService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("TMU Loop Traffic Data"))
                    {
                        tmuTrafficDataService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("Fused Sensor-only PTD"))
                    {
                        fusedSensorOnlyTrafficDataService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("Fused FVD and Sensor PTD"))
                    {
                        fusedFvdAndSensorTrafficDataService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("VMS and Matrix Signal Status Data"))
                    {
                        vmsDataService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("NTIS Model Update Notification"))
                    {
                        ntisModelNotificationService.Handle(d2LogicalModel);
                    }
                    else if (feedType.Contains("Event Data"))
                    {
                        eventDataService.Handle(d2LogicalModel);
                    }
                    else
                    {
                        throw new Exception("Unrecognised feed type");
                    }
                }
                catch (Exception e)
                {
                    log.Info(e.Message);
                    throw e;
                }

            }

            // Return the message sent
            return new putDatex2DataResponse(request.d2LogicalModel);

        }

    }

}