
using Microsoft.AspNetCore.Http;
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
            var nearestnode = GetNearestNodeAsync(context).Result;
            var response = context.HttpContext.Response;
            response.Headers[HeaderNames.Location] = nearestnode;
            if (nearestnode.Length > 0)
            {
                if (new Uri(nearestnode).Host == BOD.NodeDetails.Ip)
                {
                    context.Result = RuleResult.ContinueRules;
                    return;
                }
            } 
            response.StatusCode = (int)HttpStatusCode.MovedPermanently; ;
            context.Result = RuleResult.EndResponse;

        }

        private async System.Threading.Tasks.Task<string> GetNearestNodeAsync(RewriteContext context)
        {
            string res = string.Empty;
            try
            {
                var zk = new BLL.Services.VotingForNearestNode(context);
                res = await zk.FindNearestNodeAsync(context.HttpContext.Connection.RemoteIpAddress);
            }
            catch (System.Exception ex)
            {

               
            }
            return res;
        }
    }
}