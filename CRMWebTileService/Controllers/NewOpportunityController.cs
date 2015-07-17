using CRMWebTileService.Models;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Results;

namespace CRMWebTileService.Controllers
{
    public class NewOpportunityController : ApiController
    {
        [HttpGet]
        public JsonResult<NewOppCount> Get(string userId)
        {
            OrganizationService orgService;

            CrmConnection connection = CrmConnection.Parse(
                ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString);

            using (orgService = new OrganizationService(connection))
            {
                //Query the CRM data based on the user id being passed in
                FetchExpression query = new FetchExpression(@"
                    <fetch distinct='true' aggregate='true' >
                        <entity name='opportunity' >
                        <attribute name='opportunityid' alias='NewOppCount' aggregate='count' />
                        <attribute name='ownerid' alias='ownerid' groupby='true' />              
                        <filter type='and' >
                            <condition attribute='ownerid' operator='eq' value='" + userId + @"' />
                            <condition attribute='createdon' operator='today' />
                        </filter>
                        </entity>
                    </fetch>");

                //Get the result values for output
                EntityCollection results = orgService.RetrieveMultiple(query);
                string username = 
                    (string)results.Entities[0].GetAttributeValue<AliasedValue>("ownerid_owneridname").Value;
                int count = 
                    (int)results.Entities[0].GetAttributeValue<AliasedValue>("NewOppCount").Value;

                NewOppCount result = new NewOppCount
                {
                    Username = username,
                    Message = "New Opps Today:",
                    Count = count
                };

                //Return JSON or XML
                return Json(result);
            }
        }
    }
}
