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

                Task.Delay(50);

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
            var root = BOD.NodeDetails.ClusterName + "-Leader";
            var d = GetChildNodes("/").Where(p => p == root).ToList().Count;
            if (d == 0)
            {
                zk.Create($"/{root}", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
            }
            foreach (var item in GetChildNodes($"/{root}"))
            {
                zk.Delete($"/{root}/{item}", 0);
            }
            zk.Create($"/{root}/{LeaderIp}", LeaderIp.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
        }


        public void CreateNodeForcurrent()
        {
            var xsdf = zk.GetChildren("/", false);


            var d = GetChildNodes("/").Where(p => p == BOD.NodeDetails.ClusterName).ToList().Count;
            if (d == 0)
            {
                zk.Create($"/{BOD.NodeDetails.ClusterName}", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            //  zk.SetData("/root/childone", "childonemodify".GetBytes(), -1);


            // var xf = zk.GetChildren("/", true);
            zk.Create($"/{BOD.NodeDetails.ClusterName}/{BOD.NodeDetails.Ip}-{BOD.NodeDetails.UniqueId}", BOD.NodeDetails.Priority.ToString().GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
            setLeaderNode();
            //zk.Create("/root", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            ////create a node root, data mydata, not ACL access control, node is permanent
            //zk.Create("/root", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            //Create the root following a childone znode, data childone, not ACL access control, node is permanent   

            //Get the name of the child node under the/root node, returns List <String>   
            //  var x=zk.GetChildren("/", true);

            //    var xy= zk.GetChildren($"/{ BOD.NodeDetails.ClusterName}", true);
            //data acquired in the root//childone node, returns byte []   
            //var ss= zk.GetData($"/root/{ BOD.NodeDetails.ClusterName}", true, null);

            //modify data in a node/root/childone, third parameter version, if it is -1, it will
            //  zk.SetData("/root/childone", "childonemodify".GetBytes(), -1);
            //delete/root/childone this node, the second parameter version, -1, then delete, ignore version   
            //  zk.Delete("/root/childone", -1);
        }

        private void setLeaderNode()
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
                var byteArray = zk.GetData($"/r{ BOD.NodeDetails.ClusterName}/{item}", true, null);

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
