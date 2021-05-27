using CDN.BLL.Models;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using System;
using System.Net;

namespace CDN.BLL.Vote
{
    public class GeoDistance
    {


        public long calcuateIpDistance(string IpAddress)
        {
            var ipinfo = GetUserCountryByIp(IpAddress);
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

        private long GetCountryDistance(IpApi info)
        {
            var sCoord = new GeoCoordinate(BOD.NodeDetails.Countrylattitude, BOD.NodeDetails.Countrylattitude);
            var eCoord = new GeoCoordinate(info.lon, info.lat);

            return (long)(sCoord.GetDistanceTo(eCoord) * 0.01);
        }
    }
}
