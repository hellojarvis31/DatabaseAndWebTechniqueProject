using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using DBWTechniqueData;
using DBWTechniqueData.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace DBWTechniqueBackend
{
    public class BusinessLogic
    {
        WebClient wc = new WebClient();
        ProxyData DataLayerObj = new ProxyData();
        public void getProxyFromDatabaseAndUpdateStatus()
        {
            var ListA = DataLayerObj.GetProxyFromDatabase();
            foreach (var value in ListA)
            {
                DataLayerObj.updateProxyStatus(value.ID, PingFunction(value.Ip), DateTime.Now);
            }
        }

        public void getProxyFromProvidersAndStoreInDatabase()
        {
            int count = checkHowManyPrxiesNeeded();
            if(count < 120)
            {
                decimal missing = 120 - count;
                decimal Iteration = Math.Ceiling((missing / 5));
                getProxyDataFromGimmeProxy((int)Iteration);
                getProxyDataFromGetProxyList((int)Iteration);
                getProxyDataFromPubProxy((int)Iteration);
            }
            deleteOneWeekOldNonActiveProxy();
        }

        public int checkHowManyPrxiesNeeded()
        {
            return DataLayerObj.countNumberOfProxiesInDatabase();
        }

        public void getProxyDataFromGimmeProxy(int iterations)
        {
            try
            {
                for(int i=1;i<=iterations;i++)
                {
                    var jsonString = wc.DownloadString("https://gimmeproxy.com/api/getProxy");
                    JObject json = JObject.Parse(jsonString);
                    if(json.SelectToken("type").ToString().Equals("http"))
                        DataLayerObj.storeProxyFromProviderList(json.SelectToken("ipPort").ToString(), json.SelectToken("ip").ToString(), json.SelectToken("port").ToString(), json.SelectToken("country").ToString(), json.SelectToken("type").ToString(),PingFunction(json.SelectToken("ip").ToString()), "GimmeProxy");
                }
            }
            catch(Exception)
            {

            }
        }

        public void getProxyDataFromGetProxyList(int iterations)
        {
            try
            {
                for (int i = 1; i <= iterations; i++)
                {
                    var jsonString = wc.DownloadString("https://api.getproxylist.com/proxy");
                    JObject json = JObject.Parse(jsonString);
                    if (json.SelectToken("protocol").ToString().Equals("http"))
                        DataLayerObj.storeProxyFromProviderList(json.SelectToken("ip").ToString() + json.SelectToken("port").ToString(), json.SelectToken("ip").ToString(), json.SelectToken("port").ToString(), json.SelectToken("country").ToString(), json.SelectToken("protocol").ToString(), PingFunction(json.SelectToken("ip").ToString()), "GetProxyList");
                }
            }
            catch (Exception)
            {

            }
        }

        public void getProxyDataFromPubProxy(int iterations)
        {
            try
            {
                for (int i = 1; i <= iterations; i++)
                {
                    var jsonString = wc.DownloadString("http://pubproxy.com/api/proxy");
                    JObject json = JObject.Parse(jsonString);
                    if (json.SelectToken("data[0].type").ToString().Equals("http"))
                        DataLayerObj.storeProxyFromProviderList(json.SelectToken("data[0].ipPort").ToString(), json.SelectToken("data[0].ip").ToString(), json.SelectToken("data[0].port").ToString(), json.SelectToken("data[0].country").ToString(), json.SelectToken("data[0].type").ToString(), PingFunction(json.SelectToken("data[0].ip").ToString()), "PubProxy");
                }
            }
            catch (Exception)
            {

            }
        }

        public void deleteOneWeekOldNonActiveProxy()
        {
            DataLayerObj.findOneWeekOldNonActiveProxy();
        }

        public int PingFunction(string ip)
        {
            Ping pinger = new Ping();
            PingReply reply = pinger.Send(ip);
            var pingReply = reply.Status;
            return pingReply.Equals(IPStatus.Success) ? 1 : 0;
        }

        #region usefullCommentedCode
        //public void getProxyFromDatabaseAndUpdateStatus()
        //{
        //    DataLayerObj.GetProxyFromDatabaseAndUpdate();
        //}

        //public int PingFunction(string host, string port)
        //{
        //    int intport = Convert.ToInt32(port);
        //    int is_success = 0;
        //    try
        //    {
        //        var connsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        connsock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 200);
        //        var hip = IPAddress.Parse(host);
        //        var ipep = new IPEndPoint(hip, intport);
        //        connsock.Connect(ipep);
        //        if (connsock.Connected)
        //        {
        //            is_success = 1;
        //        }
        //        connsock.Close();
        //    }
        //    catch (Exception)
        //    {
        //        is_success = 0;
        //    }
        //    return is_success;
        //}
        #endregion

    }
}
