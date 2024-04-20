using ZeroMQ;

namespace ZmqPb;

public class PubSub : ZmqWrap
{
    private bool isServer_ = false;

    public PubSub(string host, bool isServer, ZContext zmqContext = null) : base(host, isServer ? ZSocketType.PUB : ZSocketType.SUB, zmqContext)
    {
        isServer_ = isServer;
        if (isServer_)
        {
            zmqSocket_.Bind(host_);
        }
        else
        {
            zmqSocket_.SetOption(ZSocketOption.SUBSCRIBE, "");
            zmqSocket_.Connect(host_);
        }
    }

    ~PubSub()
    {
    }

    protected override bool CanSend()
    {
        return isServer_;
    }

    protected override void DidSend()
    {
    }

    protected override bool CanRecv()
    {
        return !isServer_;
    }

    protected override void DidRecv()
    {
    }
}