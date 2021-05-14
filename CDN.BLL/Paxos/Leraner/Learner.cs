using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDN.BLL.Leraner
{
    public class Learner
    {
        public long PId;
        public List<CDN.GRPC.protobuf.PaxosResponse> lstPaxosResponses = new List<CDN.GRPC.protobuf.PaxosResponse>();
 

        public Learner()
        {
            Task.Delay(new TimeSpan(0,0,0,0,BOD.SystemParameters.LearnerWaitingTime)).ContinueWith(o => { var response = SelectLowestDistance();
                
                sendResponseToLeader(response); });
        }

        public Learner(bool v)
        {
          
        }

        internal void AddResponseToLearners(CDN.GRPC.protobuf.PaxosResponse request)
        {
            CDN.BLL.Leraner.Learner learner;

            learner = Statics.learners.Where(p => p.PId == request.PID).FirstOrDefault();
            if (learner == null)
            {
                learner = new Leraner.Learner() { PId = request.PID };
                Statics.learners.Add(learner);
            }
            learner.lstPaxosResponses.Add(request);
            Console.WriteLine("Add request to queue  " + DateTime.Now.ToString());
        }


        private CDN.GRPC.protobuf.PaxosResponse SelectLowestDistance()
        {
            CDN.GRPC.protobuf.PaxosResponse result = null;
            var distance = lstPaxosResponses.Min(p => p.Distance);
            var distances = lstPaxosResponses.FindAll(p => p.Distance == distance);

            if (distances.Count > 1)
            {
                int index = new Random().Next(distances.Count);
                result = distances[index];
            }
            else
            {
                result = distances.FirstOrDefault();
            }
            return result;
        }

        private void sendResponseToLeader(CDN.GRPC.protobuf.PaxosResponse paxosResponse)
        {


            try
            {
                Console.WriteLine("Send paxos response to leader  "+ BOD.NodeDetails.LeaderNode +"  "+ DateTime.Now.ToString());
                var GRPCClient = new BLL.GRPC.NodeVoting.GRPCClien_NodeVoting(BOD.NodeDetails.LeaderNode, BOD.SystemPorts.nearestnode);
                GRPCClient.Client.SendPaxosAccptanceToLeader(paxosResponse);
                GRPCClient.Stop_Channel();
                Statics.learners.Remove(this);
            }
            catch (Exception ex)
            {

               
            }
        }
    }
}
