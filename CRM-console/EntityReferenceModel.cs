using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public class EntityReferenceModel
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public string Name { get; set; }
    }
}
