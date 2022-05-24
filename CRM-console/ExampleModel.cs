using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public class ExampleModel : BaseModel
    {
        [CrmAttribute("uds_contractid")]
        public Guid Id { get; set; }

        [CrmAttribute("uds_name")]
        public string ContractId { get; set; }

        [CrmAttribute("uds_vertragsartcode")]
        public OptionSetModel ContractType { get; set; }

        [CrmAttribute("uds_startdate")]
        public DateTime? NullableDate { get; set; }

        [CrmAttribute("uds_startdate")]
        public DateTime StartDate { get; set; }


        [CrmAttribute("uds_differenz_kilometer")]
        public decimal DecimalTest { get; set; }

        [CrmAttribute("uds_differenz_kilometer")]
        public decimal? NullableDecimalTest { get; set; }

        [CrmAttribute("uds_durationdays")]
        public int IntTest { get; set; }

        [CrmAttribute("uds_durationdays")]
        public int NullableIntTest { get; set; }


        [CrmAttribute("uds_durationdays")]
        public double IntToDouble { get; set; }

        [CrmAttribute("uds_differenz_kilometer")]
        public double DecimalToDouble { get; set; }

        [CrmAttribute("uds_conditionpervehicle")]
        public decimal? Price { get; set; }

        [CrmAttribute("uds_contractbase", ConverterType = typeof(BoolOptionSetConverter))]
        public OptionSetModel ContractBase { get; set; } // month - 0/false,  day - 1/true

        [CrmAttribute("uds_customerid")]
        public EntityReferenceModel Customer { get; set; }

        [CrmAttribute("latestassigned.new_standortid")]
        public EntityReferenceModel Location { get; set; }

        [CrmAttribute("latestassigned.new_model")]
        public string AliasedString { get; set; }

        [CrmAttribute("latestassigned.new_basecamp_status")]
        public OptionSetModel AliasedOptionSet { get; set; }
    }

    public class BoolOptionSetConverter : CrmAttributeConverter<OptionSetModel, bool>
    {
        public override OptionSetModel GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var value = (entity[attributeName] is AliasedValue aliasedValue)
                ? (bool)aliasedValue.Value
                : (bool)entity[attributeName];

            return new OptionSetModel
            {
                Value = value ? 1 : 0,
                Label = entity.FormattedValues[attributeName]
            };
        }
    }
}
