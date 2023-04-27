using Canopy.API.Common.Models.Interfaces;
using Canopy.API.Common.Repositories;
using Canopy.CMT.ActionPlans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Repositories
{
   
    public class ActionPlanDbRepository<T> : BaseRepository<T> where T : class, IEntity, new()
    {
        public ActionPlanDbRepository(ActionPlanDbContext dbContext) : base(dbContext) { }
    }
}
