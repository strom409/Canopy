using Canopy.API.Common.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Repositories
{
 
    public class ActionPlanRedisRepository : RedisCacheRepository
    {
        public ActionPlanRedisRepository(IDistributedCache cache, IConfiguration configuration) : base(cache, configuration) { }
    }
}
