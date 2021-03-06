using System;

namespace CDN.BOD
{
    public static  class NodeDetails
    {
        public static string Country = "SriLanka";
        public static long Countrylongitude = (long)7.8731;
        public static long Countrylattitude= (long)80.7718;

       // public static string ZookeeperHost= "192.168.1.100:8569";
        public static string ZookeeperHost= "127.0.0.1:12269";
        public static string ClusterName= "cluster1";
        public static int Vote =0;

    //  public static string Ip = "127.0.0.1";
       public static string Ip = "192.168.208.1";

        public static string UniqueId { get; set; }
        public static long Priority { get; set; }
        public static string LeaderNode { get; set; }
        public static int Port = 12245;

        public static long PaxosPriority = 0;
    }

    public static class SystemParameters
    {
        public static string FileHostPath = "D:/temp";

        public static double RequestDeadline = 200;

        public static int LearnerWaitingTime = 200;
    }

    public static class SystemPorts
    {
        public static int Fileshare= 12113;
        public static int LeaderElection= 12114;
        public static int nearestnode= 12115;
    }
    public static class StaticLists
    {
        
      public enum FileOperations
        {
            Rename = 0, Delete = 1, Change = 2, Create = 3
        }
    }

  
}
