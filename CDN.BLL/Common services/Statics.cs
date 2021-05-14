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
        internal static GRPCServer_FileSystem FileShare { get; set; }
        internal static GRPCServer_NodeVoting NodeVoting { get; set; }
        internal static GRPCServer LeaderElection { get; set; }
        internal static List<Learner> learners = new List<Learner>();
        public static Proposer.ProposerEvent ProposerEvent = new Proposer.ProposerEvent();
    }
}
