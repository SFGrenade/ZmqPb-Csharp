﻿using ZeroMQ;

namespace ZmqPb;

public class ReqRep : ZmqWrap
{
    public enum Status
    {
        Receiving,
        Sending
    }

    private bool isServer_ = false;
    private Status status_ = Status.Receiving;

    public ReqRep(string host, bool isServer, ZContext zmqContext = null) : base(host, isServer ? ZSocketType.REP : ZSocketType.REQ, zmqContext)
    {
        isServer_ = isServer;
        status_ = isServer ? Status.Receiving : Status.Sending;
        if (isServer_)
        {
            zmqSocket_.Bind(host_);
        }
        else
        {
            zmqSocket_.Connect(host_);
        }
    }

    ~ReqRep()
    {
    }

    protected override bool CanSend()
    {
        return status_ == Status.Sending;
    }

    protected override void DidSend()
    {
        status_ = Status.Receiving;
    }

    protected override bool CanRecv()
    {
        return status_ == Status.Receiving;
    }

    protected override void DidRecv()
    {
        status_ = Status.Sending;
    }
}