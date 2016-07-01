using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XSockets.Core.Common.Protocol;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.XSocket.Model;
using XSockets.Protocol.Rfc6455;

namespace ArcGisXSockets.Server.Protocol
{
    public class ArcGisProtocolProxy : IProtocolProxy
    {
        public IMessage In(IEnumerable<byte> payload, MessageType messageType)
        {
            //Map the incoming data to the arcgis controller (not used in this demo)
            return new Message(Encoding.UTF8.GetString(payload.ToArray()), "message", "arcgis");
        }

        public byte[] Out(IMessage message)
        {
            return GetFrame(message).ToBytes();
        }

        protected Rfc6455DataFrame GetFrame(IMessage message, bool isMasked = false)
        {
            var frameType = FrameType.Text;
            if (message.MessageType != MessageType.Text)
                frameType = FrameType.Binary;
            var rnd = new Random().Next(0, 34298);
            var frame = new Rfc6455DataFrame
            {
                FrameType = frameType,
                IsFinal = true,
                IsMasked = isMasked,
                MaskKey = rnd,
                Payload = Encoding.UTF8.GetBytes(message.Data)
            };
            return frame;
        }
    }
}
