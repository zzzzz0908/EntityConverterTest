using EntityConverter.Core;
using ExampleWebApi.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi.Converters
{
    public class OptionSetConverter : CrmAttributeConverter<OptionSetModel, OptionSetValue>
    {
        public override OptionSetModel GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var optionSet = entity.GetUnaliasedAttributeValue<OptionSetValue>(attributeName);

            return new OptionSetModel
            {
                Value = optionSet.Value,
                Label = entity.FormattedValues[attributeName]
            };
        }
    }
}