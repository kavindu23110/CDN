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

        }
    }
}
