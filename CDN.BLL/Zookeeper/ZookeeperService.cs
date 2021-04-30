using ZooKeeperNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.Zookeeper
{
  public  class ZookeeperService
    {
        private ZooKeeper zk;

        public ZookeeperService()
        {
            zk = new ZooKeeper(BOD.NodeDetails.ZookeeperHost, new TimeSpan(0, 0, 0, 50000), new CDN.BLL.Zookeeper.Watcher());
            using (ZooKeeper zk = new ZooKeeper("127.0.0.1:8569", new TimeSpan(0, 0, 0, 50000), new Watcher()))
            {
                var stat = zk.Exists("/root", true);

                ////create a node root, data mydata, not ACL access control, node is permanent
                //zk.Create("/root", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

                //Create the root following a childone znode, data childone, not ACL access control, node is permanent   
                zk.Create("/root/childone", "childone".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                //Get the name of the child node under the/root node, returns List <String>   
                zk.GetChildren("/root", true);
                //data acquired in the root//childone node, returns byte []   
                zk.GetData("/root/childone", true, null);

                //modify data in a node/root/childone, third parameter version, if it is -1, it will
                zk.SetData("/root/childone", "childonemodify".GetBytes(), -1);
                //delete/root/childone this node, the second parameter version, -1, then delete, ignore version   
                zk.Delete("/root/childone", -1);
            }
            CreateNodeForcurrent();
        }

        private void CreateNodeForcurrent()
        {
           zk.Exists("/root", true);
            //zk.Create("/root", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            ////create a node root, data mydata, not ACL access control, node is permanent
            //zk.Create("/root", "mydata".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            //Create the root following a childone znode, data childone, not ACL access control, node is permanent   
            zk.Create($"/root/{BOD.NodeDetails.Ip}", BOD.NodeDetails.Country.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            //Get the name of the child node under the/root node, returns List <String>   
            zk.GetChildren("/root", true);
            //data acquired in the root//childone node, returns byte []   
            zk.GetData("/root/childone", true, null);

            //modify data in a node/root/childone, third parameter version, if it is -1, it will
            zk.SetData("/root/childone", "childonemodify".GetBytes(), -1);
            //delete/root/childone this node, the second parameter version, -1, then delete, ignore version   
            zk.Delete("/root/childone", -1);
        }
    }
}
