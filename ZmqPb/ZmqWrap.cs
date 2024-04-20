using System;
using Google.Protobuf;
using ZeroMQ;

namespace ZmqPb;

public class Subscription
{
    public IMessage message { get; set; } = null;
    public System.Action<IMessage> callback { get; set; } = null;

    public Subscription()
    {
    }

    public Subscription(IMessage message, System.Action<IMessage> callback)
    {
        this.message = message;
        this.callback = callback;
    }
}

public abstract class ZmqWrap
{
    protected string host_ = "";

    protected bool ownsContext_ = false;
    protected ZContext zmqContext_ = null;
    protected ZSocket zmqSocket_ = null;

    protected System.Collections.Concurrent.ConcurrentQueue<ZMessage> queueToSend_ = new();
    protected System.Collections.Generic.Dictionary<string, Subscription> subscribedMessages_ = new();

    public ZmqWrap(string host, ZSocketType socketType, ZContext zmqContext = null)
    {
        host_ = host;
        ownsContext_ = zmqContext == null;
        zmqContext_ = ownsContext_ ? new ZContext() : zmqContext;
        zmqSocket_ = new ZSocket(zmqContext_, socketType);
    }

    ~ZmqWrap()
    {
        while (!queueToSend_.IsEmpty)
        {
            queueToSend_.TryDequeue(out ZMessage tmp);
        }

        subscribedMessages_.Clear();
        zmqSocket_.Close();
        if (ownsContext_)
        {
            zmqContext_.Shutdown();
        }
    }

    public void Subscribe(IMessage message, System.Action<IMessage> callback)
    {
        string messageType = message.Descriptor.FullName;
        if (subscribedMessages_.ContainsKey(messageType))
        {
            subscribedMessages_[messageType].callback = callback;
        }
        else
        {
            subscribedMessages_[messageType] = new Subscription(message, callback);
        }
    }

    public virtual void SendMessage(IMessage message)
    {
        Proto.Wrapper wrappedMessage = new Proto.Wrapper();
        wrappedMessage.ProtoName = message.Descriptor.FullName;
        wrappedMessage.ProtoContent = message.ToByteString();
        ZMessage newMessage = new ZMessage(new[] { new ZFrame(wrappedMessage.ToByteArray()) });
        queueToSend_.Enqueue(newMessage);
    }

    public virtual void Run()
    {
        if (CanSend() && !queueToSend_.IsEmpty)
        {
            queueToSend_.TryPeek(out ZMessage msgToSend);
            zmqSocket_.SendMessage(msgToSend, ZSocketFlags.DontWait, out ZError sendResult);
            if (Equals(sendResult, ZError.None))
            {
                DidSend();
                queueToSend_.TryDequeue(out msgToSend);
            }
            else
            {
            }
        }
        else if (CanRecv())
        {
            ZMessage receivedReply = zmqSocket_.ReceiveMessage(ZSocketFlags.DontWait, out ZError recvResult);
            if (Equals(recvResult, ZError.None))
            {
                DidRecv();
                Proto.Wrapper receivedWrapper = Proto.Wrapper.Parser.ParseFrom(receivedReply.Pop());
                if (subscribedMessages_.ContainsKey(receivedWrapper.ProtoName))
                {
                    subscribedMessages_[receivedWrapper.ProtoName].message = subscribedMessages_[receivedWrapper.ProtoName].message.Descriptor.Parser
                        .ParseFrom(receivedWrapper.ProtoContent);
                    subscribedMessages_[receivedWrapper.ProtoName].callback(subscribedMessages_[receivedWrapper.ProtoName].message);
                }
                else
                {
                    throw new FormatException("Topic '" + receivedWrapper.ProtoName + "' not subscribed!");
                }
            }
            else
            {
            }
        }
    }

    protected virtual bool CanSend()
    {
        return false;
    }

    protected virtual void DidSend()
    {
    }

    protected virtual bool CanRecv()
    {
        return false;
    }

    protected virtual void DidRecv()
    {
    }
}