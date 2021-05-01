using System;

namespace CDN.BOD
{
    public static  class NodeDetails
    {
        public static string Country = "SriLanka";
        public static long Countrylongitude = (long)7.8731;
        public static long Countrylattitude= (long)80.7718;

        public static string ZookeeperHost= "127.0.0.1:8569";
        public static string ClusterName= "cluster1";
        public static int Vote =0;

        public static string Ip { get; set; }
        public static string UniqueId { get; set; }
        public static long Priority { get; set; }
        public static string LeaderNode { get; set; }
    }

    public static class SystemParameters
    {
        public static string FileHostPath = "D:/temp";
    }

    public static class SystemPorts
    {
        public static int LeaderElection= 1550;
    }
}
