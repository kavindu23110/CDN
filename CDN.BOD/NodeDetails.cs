namespace CDN.BOD
{
    public static class NodeDetails
    {
        public static string Country = "SriLanka";
        public static long Countrylongitude = (long)7.8731;
        public static long Countrylattitude = (long)80.7718;

        public static string ZookeeperHost = "127.0.0.1:12587";
        public static string ClusterName = "cluster1";
        public static int Vote = 0;

        public static string Ip = "127.0.0.1";
        public static string UniqueId { get; set; }
        public static long Priority { get; set; }
        public static string LeaderNode { get; set; }
        public static int Port = 81;

        public static long PaxosPriority = 0;
    }


}
