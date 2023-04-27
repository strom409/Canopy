using Canopy.API.Common.Specifications;
using Canopy.Core.Common.Models;
using System.Linq.Expressions;

namespace Canopy.Core.Common.Specifications
{
    public class SystemErroLogByClientIdSpec : Specification<SystemErrorLog>
    {
        private readonly List<int>? _clientIds;

        public SystemErroLogByClientIdSpec(List<int>? clientIds)
        {
            _clientIds = clientIds;
        }

        public override Expression<Func<SystemErrorLog, bool>> ToExpression()
        {
            return con => _clientIds == null || !_clientIds.Any() ||
                          (con.ClientId.HasValue && _clientIds.Contains(con.ClientId.Value));
        }
    }
}