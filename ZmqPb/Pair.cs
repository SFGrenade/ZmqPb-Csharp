using ZeroMQ;

namespace ZmqPb;

public class Pair : ZmqWrap
{
    private bool isServer_ = false;

    public Pair(string host, bool isServer, ZContext zmqContext = null) : base(host, ZSocketType.PAIR, zmqContext)
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

    ~Pair()
    {
    }

    protected override bool CanSend()
    {
        return true;
    }

    protected override void DidSend()
    {
    }

    protected override bool CanRecv()
    {
        return true;
    }

    protected override void DidRecv()
    {
    }
}