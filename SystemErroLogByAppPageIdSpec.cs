using Canopy.API.Common.Specifications;
using Canopy.Core.Common.Models;
using System.Linq.Expressions;

namespace Canopy.Core.Common.Specifications
{
    public class SystemErroLogByAppPageIdSpec : Specification<SystemErrorLog>
    {
        private readonly List<int>? _appPageIds;

        public SystemErroLogByAppPageIdSpec(List<int>? appPageIds)
        {
            _appPageIds = appPageIds;
        }

        public override Expression<Func<SystemErrorLog, bool>> ToExpression()
        {
            return con => _appPageIds == null || !_appPageIds.Any() ||
                          (con.AppPageId.HasValue && _appPageIds.Contains(con.AppPageId.Value));
        }
    }
}