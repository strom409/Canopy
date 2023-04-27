using Canopy.API.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Services.Interfaces
{  
    public interface IActionPlanDropDownServices
    {
        Task<List<DropDownDTO>?> GetActionPlanGoalCategories();
        Task<List<DropDownDTO>?> GetActionPlanGoalOwnerRoles();
        Task<List<DropDownDTO>?> GetStatusCodes();
        Task<List<DropDownDTO>?> GetStatusCodeTypes();
        Task RebuildActionPlanCache();

    }
}
