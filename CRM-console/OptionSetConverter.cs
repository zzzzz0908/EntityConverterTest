using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public class OptionSetConverter : CrmAttributeConverter<OptionSetModel, OptionSetValue>
    {
        public override OptionSetModel GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var optionSet = (entity[attributeName] is AliasedValue aliasedValue)
                            ? aliasedValue.Value as OptionSetValue
                            : entity[attributeName] as OptionSetValue;

            return new OptionSetModel
            {
                Value = optionSet.Value,
                Label = entity.FormattedValues[attributeName]
            };
        }
    }
}
