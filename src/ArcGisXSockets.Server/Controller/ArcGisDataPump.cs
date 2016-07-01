using System;
using System.Text;
using System.Threading.Tasks;
using ArcGisXSockets.Server.Helpers;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;
using XSockets.Plugin.Framework;
using XSockets.Plugin.Framework.Attributes;


namespace ArcGisXSockets.Server.Controller
{

    /// <summary>
    /// Fake geometry data to ArcGis clients
    /// </summary>
    [XSocketMetadata("ArcGisDataPump", PluginRange.Internal)]
    public class ArcGisDataPump : XSocketController
    {
        public ArcGisDataPump()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    Random random = new Random();
                    string feature = $"{{\"geometry\" : {{\"x\" : {random.NextDouble(8.40, 8.95).ToString(new System.Globalization.CultureInfo("en-US"))}, \"y\" : {random.NextDouble(45.23, 45.85).ToString(new System.Globalization.CultureInfo("en-US"))} }}, \"attributes\" : {{\"ObjectId\" : {random.Next(1, Int32.MaxValue)}, \"RouteID\" : 1, \"DateTimeStamp\" : {DateTime.Now.UnixTicks().ToString(new System.Globalization.CultureInfo("en-US"))} }}}}";
                    await this.InvokeToAll<ArcGis>(feature, "gis");
                    await Task.Delay(100);
                }
            });
        }
    }
}

