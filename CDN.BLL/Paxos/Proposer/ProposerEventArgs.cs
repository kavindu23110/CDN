using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.Proposer
{
   public class ProposerEventArgs : System.EventArgs
    {
        public CDN.GRPC.protobuf.PaxosResponse Response { get; set; }
        public ProposerEventArgs(CDN.GRPC.protobuf.PaxosResponse Response)
        {
            this.Response = Response;
        }

       
    }
}
