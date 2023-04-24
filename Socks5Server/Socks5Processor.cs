using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Socks5Server;

public class Socks5Processor
{
    private readonly BlockingCollection<Socket> _queue;

    public Socks5Processor(BlockingCollection<Socket> queue)
    {
        _queue = queue;
    }

    public void Process()
    {
        while (!_queue.IsCompleted)
            Socks5Handler.Handle(_queue.Take(), false);
    }
}