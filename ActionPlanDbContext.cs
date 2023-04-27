using Canopy.API.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Models
{

    public class ActionPlanDbContext : BaseDbContext
    {
        public ActionPlanDbContext(DbContextOptions<ActionPlanDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActionPlanGoalCategory>().ToTable("ActionPlanGoalCategories", "cmt");
            modelBuilder.Entity<ActionPlanGoalOwnerRole>().ToTable("ActionPlanGoalOwnerRoles", "cmt");
            modelBuilder.Entity<StatusCode>().ToTable("StatusCodes", "cmt");
            modelBuilder.Entity<StatusCodeType>().ToTable("StatusCodeTypes", "cmt");
            modelBuilder.Entity<ActionPlanGoal>().ToTable("ActionPlanGoals", "cmt");
            modelBuilder.Entity<ActionPlanGoalComment>().ToTable("ActionPlanGoalComments", "cmt");
        }
    }
}
