using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi.Models
{
    public class EntityReferenceModel
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public string Name { get; set; }
    }
}