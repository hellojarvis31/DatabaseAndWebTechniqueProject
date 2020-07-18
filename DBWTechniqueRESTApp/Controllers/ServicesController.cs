using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBWTechniqueBackend;
using System.Web.Razor.Parser;
using DBWTechniqueData;
using DBWTechniqueData.Models;

namespace DBWTechniqueRESTApp.Controllers
{
    public class ServicesController : ApiController
    {
        //https://localhost:44366/api/Services/GetProxies
        [ActionName("GetProxies")]
        [HttpGet]
        public List<ProxyModel> getAllUpdatedProxyData()
        {
            ProxyData DataLayerObj = new ProxyData();
            return DataLayerObj.GetProxyFromDatabase();
        }

        //https://localhost:44366/api/Services/UpdateProxies
        [ActionName("UpdateProxies")]
        [HttpGet]
        public void updateProxiesInDatabase()
        {
            BusinessLogic BusinessLayerObject = new BusinessLogic();
            BusinessLayerObject.getProxyFromDatabaseAndUpdateStatus();
        }

        //https://localhost:44366/api/Services/FindProxies
        [ActionName("FindProxies")]
        [HttpGet]
        public void findAndStoreProxiesInDatabase()
        {
            BusinessLogic BusinessLayerObject = new BusinessLogic();
            BusinessLayerObject.getProxyFromProvidersAndStoreInDatabase();
        }

        //https://localhost:44366/api/Services/DeleteOneWeekOldProxy
        [ActionName("DeleteOneWeekOldProxy")]
        [HttpGet]
        public void deleteOneWeekOldNonActiveProxy()
        {
            BusinessLogic BusinessLayerObject = new BusinessLogic();
            BusinessLayerObject.deleteOneWeekOldNonActiveProxy();
        }
    }
}
