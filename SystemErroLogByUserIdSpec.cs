using Canopy.API.Common.Specifications;
using Canopy.Core.Common.Models;
using System.Linq.Expressions;

namespace Canopy.Core.Common.Specifications
{
    public class SystemErroLogByUserIdSpec : Specification<SystemErrorLog>
    {
        private readonly List<int>? _userIds;

        public SystemErroLogByUserIdSpec(List<int>? userIds)
        {
            _userIds = userIds;
        }

        public override Expression<Func<SystemErrorLog, bool>> ToExpression()
        {
            return con => _userIds == null || !_userIds.Any() ||
                          _userIds.Contains(con.CreateUserId);
        }
    }
}