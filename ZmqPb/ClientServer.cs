using ZeroMQ;

namespace ZmqPb;
/*
public class ClientServer : ZmqWrap
{
    private bool isServer_ = false;

    public ClientServer(string host, bool isServer, ZContext zmqContext = null) : base(host, isServer ? ZSocketType.SERVER : ZSocketType.CLIENT, zmqContext)
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

    ~ClientServer()
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
*/