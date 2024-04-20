using System;

namespace Example;

public class Program
{
    static void Main(string[] args)
    {
        bool done = false;

        ZeroMQ.ZContext myContext = new ZeroMQ.ZContext();

        ZmqPb.ReqRep myTestServer = new ZmqPb.ReqRep("tcp://127.0.0.1:55555", true);
        myTestServer.Subscribe(new SFG.Test.Message(), rawMsg =>
        {
            var actualMessage = rawMsg as SFG.Test.Message;
            System.Console.WriteLine($"Test received: '{actualMessage}'");

            SFG.Test.EmptyAnswer rep = new SFG.Test.EmptyAnswer();
            myTestServer.SendMessage(rep);

            done = true;
        });
        myTestServer.Subscribe(new SFG.Test.OtherMessage(), message =>
        {
            SFG.Test.EmptyAnswer rep = new SFG.Test.EmptyAnswer();
            myTestServer.SendMessage(rep);
        });

        ZmqPb.ReqRep myTestClient = new ZmqPb.ReqRep("tcp://127.0.0.1:55555", false);
        myTestClient.Subscribe(new SFG.Test.EmptyAnswer(), message => { });

        SFG.Test.OtherMessage msg1 = new SFG.Test.OtherMessage();
        msg1.Something = "My Something";
        msg1.Else = "My Else";
        myTestClient.SendMessage(msg1);

        SFG.Test.Message msg2 = new SFG.Test.Message();
        msg2.Woah = "My Woah";
        msg2.Content = "My Content";
        myTestClient.SendMessage(msg2);

        while (!done)
        {
            myTestServer.Run();
            myTestClient.Run();
        }

        myContext.Shutdown();
    }
}