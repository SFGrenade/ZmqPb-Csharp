using ZeroMQ;

namespace ZmqPb;

public class PushPull : ZmqWrap
{
    private bool isServer_ = false;

    public PushPull(string host, bool isServer, ZContext zmqContext = null) : base(host, isServer ? ZSocketType.PUSH : ZSocketType.PULL, zmqContext)
    {
        isServer_ = isServer;
        if (isServer_)
        {
            zmqSocket_.Bind(host_);
        }
        else
        {
            zmqSocket_.Connect(host_);
        }
    }

    ~PushPull()
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