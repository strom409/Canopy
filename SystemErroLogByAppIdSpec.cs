using Canopy.API.Common.Specifications;
using Canopy.Core.Common.Models;
using System.Linq.Expressions;

namespace Canopy.Core.Common.Specifications
{
    public class SystemErroLogByAppIdSpec : Specification<SystemErrorLog>
    {
        private readonly List<int>? _appIds;

        public SystemErroLogByAppIdSpec(List<int>? appIds)
        {
            _appIds = appIds;
        }

        public override Expression<Func<SystemErrorLog, bool>> ToExpression()
        {
            return con => _appIds == null || !_appIds.Any() ||
                          (con.ApplicationId.HasValue && _appIds.Contains(con.ApplicationId.Value));
        }
    }
}