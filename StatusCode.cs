using Canopy.API.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Models
{
    
    public class StatusCode: IEntity
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? TypeID { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int? NumericValue { get; set; }

    }
}
