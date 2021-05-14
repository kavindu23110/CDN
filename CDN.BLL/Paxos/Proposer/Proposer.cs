using CDN.BLL.Zookeeper;
using CDN.GRPC.protobuf;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.Proposer
{
  public  class Proposer
    {
        private RewriteContext context;
        Thread actionThread;
        private ZookeeperService zKService;
        long PID = 0;
        private Task task;
        private CancellationTokenSource tokenSource;

        public PaxosResponse Response { get; private set; }

        public Proposer(RewriteContext context)
        {
            this.context = context;
            this.zKService = BLL.Statics.zk;
        }
        public Proposer()
        {
            this.zKService = BLL.Statics.zk;
        }

        public async System.Threading.Tasks.Task<string> FindNearestNodeAsync(System.Net.IPAddress requestIp)
        {
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var clients = CreateGRPCClients(LstNodes);
            var url = await StartProposingProcess(clients);

            return url;
        }
        private List<GRPC.NodeVoting.GRPCClien_NodeVoting> CreateGRPCClients(List<KeyValuePair<long, string>> lstNodes)
        {
            List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients = new List<GRPC.NodeVoting.GRPCClien_NodeVoting>();
            foreach (var item in lstNodes)
            {

                var client = new GRPC.NodeVoting.GRPCClien_NodeVoting(item.Value, BOD.SystemPorts.nearestnode);
                clients.Add(client);

            }
            return clients;
        }


        private async System.Threading.Tasks.Task<string> StartProposingProcess(List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients)
        {
            Console.WriteLine("Proposing Process started " + DateTime.Now.ToString());
            PID = 0;
            bool acceptance = false;
            while (acceptance == false)
            {
                PID= DateTime.UtcNow.Ticks;
                Console.WriteLine("sending propose message " + DateTime.Now.ToString());
                acceptance = SendInitiateMessageAsync(PID, clients).Result;
            }
            Console.WriteLine("Preparation finished" + DateTime.Now.ToString());
            SendAcceptanceMessage(clients, PID);
            try
            {
                tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                task = new Task(WaitForresponse, token, TaskCreationOptions.LongRunning);

                task.Start();
                task.Wait();
            }
            catch (Exception ex)
            {


            }
            finally
            {
                tokenSource.Dispose();
                task.Dispose();
            }

            if (Response== null)
            {
                Console.WriteLine("paxos response null" + DateTime.Now.ToString());
                return null;
            }
            Console.WriteLine("Redirected to " + Response.FileURL+"  " + DateTime.Now.ToString());

            return Response.FileURL;


        }

        private void WaitForresponse()
        {

            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (Response != null)
            {
                if (stopWatch.ElapsedMilliseconds==BOD.SystemParameters.RequestDeadline+2000   )
                {
                    Statics.ProposerEvent.OnResponserecieved -= this.AddresponseToObject;
                    tokenSource.Cancel();
                    Console.WriteLine("Response timeout -" + Response.FileURL+" "+ DateTime.Now.ToString());
                }
                else if (Response != null)
                {
                    tokenSource.Cancel();
                    Console.WriteLine("Response recieved -" + Response.FileURL + " " + DateTime.Now.ToString());
                }
            }
       
            
        }

        private void SendAcceptanceMessage(List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients, long id)
        {
            Statics.ProposerEvent.OnResponserecieved += this.AddresponseToObject;
            Console.WriteLine("Registered to event -" + " " + DateTime.Now.ToString());
            var req = new CDN.GRPC.protobuf.PaxosRequest();
            var x = context.HttpContext.Connection.RemoteIpAddress.ToString();
            req.PID = id;
            req.ClientIp = x;
            req.FileURL = context.HttpContext.Request.Path.ToString();

            var deadline = DateTime.UtcNow.AddSeconds(BOD.SystemParameters.RequestDeadline);

            foreach (var item in clients)
            {
                try
                {
                    item.Client.AcceptAcceptanceRequestAsync(req, deadline: deadline);

                    item.Stop_Channel();
                }
                catch (Exception ex)
                {
              

                }
            }

   

        }
        private async System.Threading.Tasks.Task<bool> SendInitiateMessageAsync(long Id, List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients)
        {
            int count = 0;
            var req = new CDN.GRPC.protobuf.Initia1PaxosRequest() { PID = Id, Success = false };
            var deadline = DateTime.UtcNow.AddSeconds(5);

            foreach (var item in clients)
            {

                Console.WriteLine("Propos message sen to "+item.channel.Target+" "+ DateTime.Now.ToString());
                var response = await item.Client.InitiatePaxosRequestAsync(req);
                if (response.Success)
                {
                    count++;
                }

            }
            Console.WriteLine("Proposer Finished" + DateTime.Now.ToString());
            return clients.Count == count;
        }
        private void AddresponseToObject(object sender, ProposerEventArgs e)
        {
            if (PID==e.Response.PID)
            {
                Response = e.Response;
            }
        }
    }
}
