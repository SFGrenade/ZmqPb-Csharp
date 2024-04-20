using System;
using Google.Protobuf;
using ZeroMQ;

namespace ZmqPb;
/*
public class RadioDish : ZmqWrap
{
    private bool isServer_ = false;

    public RadioDish(string host, bool isServer, string[] joinGroups, ZContext zmqContext = null) : base(host, isServer ? ZSocketType.RADIO : ZSocketType.DISH, zmqContext)
    {
        isServer_ = isServer;
        if (isServer_)
        {
            zmqSocket_.Bind(host_);
        }
        else
        {
            foreach( string joinGroup in joinGroups ) {
                zmqSocket_.Join( joinGroup );
            }
            zmqSocket_.Connect(host_);
        }
    }

    ~RadioDish()
    {
    }

    public void SendMessage(IMessage message, string group)
    {
        Proto.Wrapper wrappedMessage = new Proto.Wrapper();
        wrappedMessage.ProtoName = message.Descriptor.FullName;
        wrappedMessage.ProtoContent = message.ToByteString();
        ZMessage newMessage = new ZMessage(new[] { new ZFrame(wrappedMessage.ToByteArray()) });
        newMessage.SetGroup(group);
        queueToSend_.Enqueue(newMessage);
    }

    public override void SendMessage(IMessage message)
    {
        throw new NotImplementedException("RadioDish.SendMessage - overloaded function without group not implemented!");
    }

    protected override bool CanSend()
    {
        return !isServer_;
    }

    protected override void DidSend()
    {
    }

    protected override bool CanRecv()
    {
        return isServer_;
    }

    protected override void DidRecv()
    {
    }
}
*/