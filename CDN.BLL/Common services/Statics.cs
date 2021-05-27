using CDN.BLL.GRPC.FileSystem;
using CDN.BLL.GRPC.LeaderElection;
using CDN.BLL.GRPC.NodeVoting;
using CDN.BLL.Leraner;
using System.Collections.Generic;

namespace CDN.BLL
{
    public static class Statics
    {
        public static CDN.BLL.Zookeeper.ZookeeperService zk { get; set; }
        public static GRPCServer_FileSystem FileShare { get; set; }
        public static GRPCServer_NodeVoting NodeVoting { get; set; }
        public static GRPCServer LeaderElection { get; set; }
        public static List<Learner> learners = new List<Learner>();
        public static Proposer.ProposerEvent ProposerEvent = new Proposer.ProposerEvent();
    }
}
