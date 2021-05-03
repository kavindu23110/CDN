
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace CDN
{
    internal class RedirectWwwRule : IRule
    {
        

        public HttpContext http { get; private set; }
    

        public void ApplyRule(RewriteContext context)
        {
            if (context.HttpContext.Items.Count > 0)
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            var x = context.HttpContext.Request.Headers;
            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.MovedPermanently; ;
            response.Headers[HeaderNames.Location]  = GetNearestNodeAsync(context).Result; 
            context.Result = RuleResult.EndResponse;
            context.HttpContext.Items.Add(response.Headers[HeaderNames.Location], response.StatusCode);
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