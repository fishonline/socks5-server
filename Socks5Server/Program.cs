using System.Collections.Concurrent;
using System.Net.Sockets;
using Socks5Server;

var queue = new BlockingCollection<Socket>();
Task.Run(() => new Socks5Acceptor(7582, queue).Accept());
Task.Run(() => new Socks5Processor(queue).Process());
Thread.CurrentThread.Join();