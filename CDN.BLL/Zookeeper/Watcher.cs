using System;
using ZooKeeperNet;

namespace CDN.BLL.Zookeeper
{
    class Watcher : IWatcher
    {
        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeChildrenChanged)
            {
              
                Console.WriteLine(@event.Path);
            }
        }
    }
}
