using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Roles;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class DancerPodium:TableBar,IRoleGiverExtended
    {
        private Role mCurrentRole;

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            startTime = 0f;
            endTime = 0f;
            if (base.LotCurrent != null)
            {
                if (Bartending.TryGetHoursOfOperation(base.LotCurrent, ref startTime, ref endTime))
                {
                    startTime -= Pianist.kPianistStartTimeDelta;
                    endTime -= Pianist.kPianistEndTimeDelta;
                }
                else if (this.CurrentRole != null)
                {
                    startTime = this.CurrentRole.Data.StartTime[0];
                    endTime = this.CurrentRole.Data.EndTime[0];
                }
            }
        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {
        }

        public Roles.Role CurrentRole
        {
            get
            {
                return this.mCurrentRole;

            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            throw new System.NotImplementedException();
        }

        public string RoleName(bool isFemale)
        {
            throw new System.NotImplementedException();
        }

        public Roles.Role.RoleType RoleType
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
