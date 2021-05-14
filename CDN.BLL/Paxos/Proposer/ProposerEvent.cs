using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.Proposer
{
    public delegate void ResponseRecieveEventHandler(object sender, BLL.Proposer.ProposerEventArgs e);

  public class ProposerEvent
    {
        public event ResponseRecieveEventHandler OnResponserecieved;

        public ProposerEvent()
        {
          

        }

        public void ResponseRecieved(CDN.GRPC.protobuf.PaxosResponse response)
        {
            try
            {

                OnResponserecieved(this, new ProposerEventArgs(response));
            }
            catch (Exception ex)
            {

              
            }
        }
    }
   
}
