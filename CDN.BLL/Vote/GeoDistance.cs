using CDN.BLL.Models;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CDN.BLL.Vote
{
  public  class GeoDistance
    {
       
        public GeoDistance()
        {
           
        }

       public double calcuateIpDistance(string IpAddress )
        {
           var ipinfo= GetUserCountryByIp(IpAddress);
            return GetCountryDistance(ipinfo);
        }

        private IpApi GetUserCountryByIp(string ip)
        {
            IpApi ipInfo = new IpApi();
            try
            {
                string info = new WebClient().DownloadString($"http://ip-api.com/json/{ip}?fields=status,lat,lon");
                ipInfo = JsonConvert.DeserializeObject<IpApi>(info);
    
            }
            catch (Exception)
            {
                ipInfo.status = null;
            }

            return ipInfo;
        }

        private double GetCountryDistance(IpApi info)
        {
            var sCoord = new GeoCoordinate(BOD.NodeDetails.Countrylattitude, BOD.NodeDetails.Countrylattitude);
            var eCoord = new GeoCoordinate(info.lon, info.lat);

            return sCoord.GetDistanceTo(eCoord)*0.1;
        }
    }
}
