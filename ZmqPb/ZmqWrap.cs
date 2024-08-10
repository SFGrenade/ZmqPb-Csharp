using System;
using System.Linq;
using System.IO;
using Google.Protobuf;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ZeroMQ;

namespace ZmqPb;

public class Subscription
{
    public IMessage message { get; set; } = null;
    public Action<IMessage> callback { get; set; } = null;

    public Subscription()
    {
    }

    public Subscription(IMessage message, Action<IMessage> callback)
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

    protected ConcurrentQueue<KeyValuePair<string, string>> queueToSend_ = new();
    protected Dictionary<string, Subscription> subscribedMessages_ = new();

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
            queueToSend_.TryDequeue(out KeyValuePair<string, string> tmp);
        }

        subscribedMessages_.Clear();
        zmqSocket_.Close();
        if (ownsContext_)
        {
            zmqContext_.Shutdown();
        }
    }

    public void Subscribe(IMessage message, Action<IMessage> callback)
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

    private static byte[] ConvertToByteArray(string input)
    {
        return input.Select(Convert.ToByte).ToArray();
    }

    private static string ConvertToString(byte[] bytes)
    {
        return new string(bytes.Select(Convert.ToChar).ToArray());
    }

    public virtual void SendMessage(IMessage message)
    {
        byte[] myBuffer;
        using (MemoryStream stream = new MemoryStream())
        {
            message.WriteTo(stream);
            myBuffer = stream.ToArray();
        }

        queueToSend_.Enqueue(new KeyValuePair<string, string>(message.Descriptor.FullName, ConvertToString(myBuffer)));
    }

    public virtual void Run()
    {
        if (CanSend() && !queueToSend_.IsEmpty)
        {
            queueToSend_.TryPeek(out KeyValuePair<string, string> msgToSend);
            zmqSocket_.SendMessage(new ZMessage(new[] { new ZFrame(ConvertToByteArray(msgToSend.Key)) }), ZSocketFlags.DontWait, out ZError sendResultPartOne);
            zmqSocket_.SendMessage(new ZMessage(new[] { new ZFrame(ConvertToByteArray(msgToSend.Value)) }), ZSocketFlags.DontWait,
                out ZError sendResultPartTwo);
            if (Equals(sendResultPartOne, ZError.None) && Equals(sendResultPartTwo, ZError.None))
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
            ZMessage receivedReplyPartOne = zmqSocket_.ReceiveMessage(ZSocketFlags.DontWait, out ZError recvResultPartOne);
            ZMessage receivedReplyPartTwo = zmqSocket_.ReceiveMessage(ZSocketFlags.DontWait, out ZError recvResultPartTwo);
            if (Equals(recvResultPartOne, ZError.None) && Equals(recvResultPartTwo, ZError.None))
            {
                DidRecv();
                string messageType = ConvertToString(receivedReplyPartOne.Pop().Read());
                if (subscribedMessages_.ContainsKey(messageType))
                {
                    subscribedMessages_[messageType].message = subscribedMessages_[messageType].message.Descriptor.Parser
                        .ParseFrom(receivedReplyPartTwo.Pop());
                    subscribedMessages_[messageType].callback(subscribedMessages_[messageType].message);
                }
                else
                {
                    throw new FormatException("Topic '" + messageType + "' not subscribed!");
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