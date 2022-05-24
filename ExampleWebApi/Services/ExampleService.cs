using EntityConverter.Core;
using ExampleWebApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi.Services
{
    public class ExampleService
    {
        private readonly IOrganizationService _crmService;

        public ExampleService(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public List<ExampleModel> ListItems()
        {
            var fetchXml = $@"
                <fetch top='20'>
                    <entity name='uds_contract'>
                    <all-attributes />
                    <order attribute='uds_name' descending='true' />
                    <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0'/>                        
                    </filter>
                    <link-entity name='new_fuhrpark' from='new_fuhrparkid' to='uds_latestassigned' link-type='outer' alias='latestassigned'>
                        <attribute name = 'new_name' /> 
                        <attribute name = 'new_standortid' /> 
                        <attribute name = 'uds_herstellerid' /> 
                        <attribute name = 'new_model' /> 
                        <attribute name = 'new_basecamp_status' /> 
                    </link-entity>
                    </entity>
                </fetch>";

            var query = new FetchExpression(fetchXml);

            //тут 200+ мс
            var entities = _crmService.RetrieveMultiple(query).Entities;


            //тут без кэша ~5мс
            //с кэшем ~2мс
            var models = entities
                .Select(e => e.ToModel<ExampleModel>())
                .ToList();


            return models;
        }
    }
}