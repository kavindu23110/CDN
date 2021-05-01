using System;
using ZooKeeperNet;

namespace CDN.BLL.Zookeeper
{
    class Watcher : IWatcher
    {
  
        private ZookeeperService zKService;

 
        public Watcher(ZookeeperService zKService)
        {
            this.zKService = zKService;
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeChildrenChanged)
            {
              
                Console.WriteLine(@event.Path);
            }


            if (@event.Type == EventType.NodeDeleted)
            {
               new  LeaderElection(zKService).ElectLeader();
            }
        }
    }
}
