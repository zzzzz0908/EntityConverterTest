using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var basecampDevService = new CrmServiceClient(ConfigurationManager.ConnectionStrings["DevCRM"].ConnectionString);
            var basecampProdService = new CrmServiceClient(ConfigurationManager.ConnectionStrings["ProdCRM"].ConnectionString);


            //var ent = new Entity("uds_contract", new Guid("013f784f-beaa-ec11-983f-0022489dc988"));
            //ent["uds_startdate"] = null;
            //basecampDevService.Update(ent);

            //var entity = basecampDevService.Retrieve("uds_contract", new Guid("013f784f-beaa-ec11-983f-0022489dc988"), new ColumnSet(true));
            var entity = GetContract(basecampDevService);

            var model = entity.ToModel<ExampleModel>();



        }
        

        static Entity GetContract(CrmServiceClient crmService)
        {
            var id = new Guid("013f784f-beaa-ec11-983f-0022489dc988");

            var fetchXml = $@"
                <fetch top='1'>
                    <entity name='uds_contract'>
                    <all-attributes />
                    <filter type='and'>
                        <condition attribute='uds_contractid' operator='eq' value='{id}'/>                        
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
            return crmService.RetrieveMultiple(query).Entities.First();
        }

        static void CreateWorkshopServiceGroups(CrmServiceClient crmClient)
        {
            var separator = "\t";

            var lines = File.ReadAllLines("./../../../groups.txt");

            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                var name = parts[0].Trim();
                var area = parts[1].Trim();
                var iconUrl = parts[2].Trim();


                var entity = new Entity("uds_werkstattleistungengroup");
                entity["uds_name"] = name;
                entity["uds_iconurl"] = iconUrl;

                switch (area)
                {
                    case "Service":
                        entity["uds_bereich"] = new OptionSetValue(100_000_000);
                        break;
                    case "Montage":
                        entity["uds_bereich"] = new OptionSetValue(100_000_001);
                        break;
                    case "Wartung":
                        entity["uds_bereich"] = new OptionSetValue(100_000_002);
                        break;
                    case "Maintenance":
                        entity["uds_bereich"] = new OptionSetValue(100_000_003);
                        break;
                    case "Inspektion":
                        entity["uds_bereich"] = new OptionSetValue(100_000_004);
                        break;
                }

                crmClient.Create(entity);
            } 
        }

        static void CreateWorkshopServices(CrmServiceClient crmClient)
        {
            var separator = "\t";

            var lines = File.ReadAllLines("./../../../service.txt");

            var groups = RetrieveGroups(crmClient);

            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                var name = parts[0].Trim();
                var groupName = parts[1].Trim();


                var entity = new Entity("uds_werkstattleistungen");
                entity["uds_name"] = name;

                var group = groups.FirstOrDefault(e => e.GetAttributeValue<string>("uds_name") == groupName);

                if (group == null)
                {
                    var a = 5;
                }

                entity["uds_groupid"] = group.ToEntityReference();
                crmClient.Create(entity);
            }
        }

        static List<Entity> RetrieveGroups(CrmServiceClient crmClient)
        {
            var query = new QueryExpression("uds_werkstattleistungengroup");
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return crmClient.RetrieveMultiple(query).Entities.ToList();
        }
    }
}
