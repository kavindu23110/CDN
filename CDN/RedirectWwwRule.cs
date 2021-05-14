using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;

namespace CDN
{
    internal class RedirectWwwRule : IRule
    {




        public void ApplyRule(RewriteContext context)
        {
            Console.WriteLine("Request started " + DateTime.Now.ToString());
            if (BOD.NodeDetails.LeaderNode == BOD.NodeDetails.Ip)
            {
                Console.WriteLine("paxos started " + DateTime.Now.ToString());

                var nearestnode = GetNearestNodeAsync(context).Result;
                if (nearestnode != null)
                {

                    if (nearestnode.Length > 0)
                    {
                        if (new Uri(nearestnode).Host == BOD.NodeDetails.Ip)
                        {
                            Console.WriteLine("Redirected to own-1 " + DateTime.Now.ToString());
                            context.Result = RuleResult.ContinueRules;
                            return;
                        }
                    }
                    Console.WriteLine("Redirected to other node  " + nearestnode + " " + DateTime.Now.ToString());
                    var response = context.HttpContext.Response;
                    response.Headers[HeaderNames.Location] = nearestnode;
                    response.StatusCode = (int)HttpStatusCode.MovedPermanently; ;
                    context.Result = RuleResult.EndResponse;
                }
                else
                {
                    Console.WriteLine("Redirected to own-2 " + DateTime.Now.ToString());
                    context.Result = RuleResult.ContinueRules;
                    return;
                }



            }
            else
            {
                Console.WriteLine("Redirected to own-3");
                context.Result = RuleResult.ContinueRules;
                return;
            }

        }

        private async System.Threading.Tasks.Task<string> GetNearestNodeAsync(RewriteContext context)
        {
            string res = string.Empty;
            try
            {
                var proposer = new BLL.Proposer.Proposer(context);
                res = await proposer.FindNearestNodeAsync(context.HttpContext.Connection.RemoteIpAddress);
            }
            catch (System.Exception ex)
            {


            }
            return res;
        }
    }
}