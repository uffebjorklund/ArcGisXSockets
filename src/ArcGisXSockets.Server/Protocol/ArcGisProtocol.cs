using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XSockets.Core.Common.Protocol;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.XSocket.Model;
using XSockets.Plugin.Framework;
using XSockets.Plugin.Framework.Attributes;
using XSockets.Protocol.Rfc6455;

namespace ArcGisXSockets.Server.Protocol
{    
    /// <summary>
    /// This is actually just a vanilla RFC6455 protocol... If Esri provide the possibility ro add sub-protocols to the websocket connection this will be much better & easier
    /// </summary>
    [Export(typeof(IXSocketProtocol), Rewritable = Rewritable.No)]
    public class ArcGisProtocol : Rfc6455Protocol
    {
        public ArcGisProtocol()
        {
            this.ProtocolProxy = new ArcGisProtocolProxy();
        }
        public override async Task<bool> Match(IList<byte> handshake)
        {
            var s = Encoding.UTF8.GetString(handshake.ToArray());
            return
                Regex.Match(s, @"(^Sec-WebSocket-Version:\s13)", RegexOptions.Multiline).Success
                &&
                !Regex.Match(s, @"(^Sec-WebSocket-Protocol:)", RegexOptions.Multiline).Success;
        }

        public override IXSocketProtocol NewInstance()
        {
            return new ArcGisProtocol();
        }

        public override async Task SetUri()
        {
            try
            {
                await base.SetUri();
            }
            catch (Exception)
            {
                //Allow anything...
                this.ConnectionContext.RequestUri = new Uri("http://localhost");
            }
        }

        public override async Task Opened()
        {
            await this.VerifyController(new Message { Controller = "arcgis" });
        }

        public override IMessage OnIncomingFrame(IEnumerable<byte> payload, MessageType messageType)
        {
            return this.ProtocolProxy.In(payload, messageType);
        }

        public override byte[] OnOutgoingFrame(IMessage message)
        {
            return this.ProtocolProxy.Out(message);
        }
    }
}
