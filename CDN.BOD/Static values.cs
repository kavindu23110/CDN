using System;

namespace CDN.BOD
{

    public static class SystemParameters
    {
        public static string FileHostPath = "D:/files/" + NodeDetails.Ip;

        public static double RequestDeadline = 200;

        public static int LearnerWaitingTime = 200;
    }

    public static class SystemPorts
    {
        public static int Fileshare = 12113;
        public static int LeaderElection = 12225;
        public static int nearestnode = 12115;
    }
    public static class StaticLists
    {

        public enum FileOperations
        {
            Rename = 0, Delete = 1, Change = 2, Create = 3
        }
    }


}
