using Canopy.API.Common.Models.Interfaces;
using Canopy.API.Common.Specifications;
using Canopy.CMT.ActionPlans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Specifications
{
    public class EntityByIdSpec<T> : Specification<T> where T : IEntity
    {
     
        public override Expression<Func<T, bool>> ToExpression()
        {
            return pwp => pwp.Id==pwp.Id;
        }
    }
}

