using EntityConverter.Core;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi.Converters
{
    public class MoneyDecimalConverter : CrmAttributeConverter<decimal, Money>
    {
        public override decimal GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var moneyValue = entity.GetUnaliasedAttributeValue<Money>(attributeName);

            return moneyValue.Value;
        }
    }
}