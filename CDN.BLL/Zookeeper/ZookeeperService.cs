using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace CDN.BLL.Zookeeper
{
    public class ZookeeperService
    {
        private ZookeeperService ZKService;

        private ZooKeeper zk;
        public ZookeeperService()
        {
            if (zk == null)
            {
                zk = new ZooKeeper(BOD.NodeDetails.ZookeeperHost, new TimeSpan(0, 0, 0, 50000), new CDN.BLL.Zookeeper.Watcher(ZKService));

                Task.Delay(2000);

            }
        }

        public ZookeeperService GetZookeeperService()
        {
            if (ZKService == null)
            {

                ZKService = new ZookeeperService();

            }

            return ZKService;
        }
        public void CreateLeaderNode(string LeaderIp)
        {
            try
            {
                var ssn = GetChildNodes($"/");
                var root = BOD.NodeDetails.ClusterName + "-Leader";
                var d = GetChildNodes("/").Where(p => p == root).ToList().Count;
                if (d == 0)
                {
                    zk.Create($"/{root}", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
                var ss = GetChildNodes($"/{root}");
                foreach (var item in GetChildNodes($"/{root}"))
                {
                    zk.Delete($"/{root}/{item}", 0);
                }
                zk.Create($"/{root}/{LeaderIp}", LeaderIp.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public bool CreateNodeForcurrent()
        {
            try
            {
                var xsdf = zk.GetChildren("/", false);
                var d = GetChildNodes("/").Where(p => p == BOD.NodeDetails.ClusterName).ToList().Count;
                if (d == 0)
                {
                    zk.Create($"/{BOD.NodeDetails.ClusterName}", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
                zk.Create($"/{BOD.NodeDetails.ClusterName}/{BOD.NodeDetails.Ip}-{BOD.NodeDetails.UniqueId}", BOD.NodeDetails.Priority.ToString().GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
                setLeaderNode();
            }
            catch (Exception ex)
            {

                throw;
            }

            return true;
        }

        private void setLeaderNode()
        {
            try
            {
                var root = BOD.NodeDetails.ClusterName + "-Leader";
                var d = GetChildNodes("/").Where(p => p == root).ToList().Count;
                if (d == 0)
                {
                    return;
                }
                var data = GetChildNodes($"/{root}").FirstOrDefault();
                if (null != data)
                {
                    BOD.NodeDetails.LeaderNode = data;
                    return;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //private bool CheckChildNodesexist(string path)
        //{
        //    var x = (List<string>)zk.GetChildren(path, true);
        //    var c = (x.Count) > 0;
        //  return c;
        //}
        public List<string> GetChildNodes(string path)
        {
            return (List<string>)zk.GetChildren(path, true);
        }
        public List<KeyValuePair<long, string>> GetClusterNodes(string ClusterName)
        {
            List<KeyValuePair<long, string>> kv = new List<KeyValuePair<long, string>>();
            var nodes = (List<string>)zk.GetChildren("/" + ClusterName, true);
            foreach (var item in nodes)
            {
                var byteArray = zk.GetData($"/{ BOD.NodeDetails.ClusterName}/{item}", true, null);

                kv.Add(new KeyValuePair<long, string>(ByteToLong(byteArray), item.Substring(0, item.IndexOf("-"))));
            }
            return kv;
        }
        public string GetDataFromNode(string path)
        {
            var s = zk.GetData(path, true, null);
            if (s.Length == 0)
            {
                return null;
            }
            return ByteToString(s);
        }
        public void SetDataToNode(string path, string data)
        {
            zk.SetData(path, data.GetBytes(), -1);

        }

        private long ByteToLong(byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            return BitConverter.ToInt64(byteArray, 0);
        }

        private string ByteToString(byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            return BitConverter.ToString(byteArray, 0);
        }
    }
}
