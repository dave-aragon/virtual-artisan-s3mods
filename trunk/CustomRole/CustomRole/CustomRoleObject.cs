using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Misukisu.Common;
using Sims3.Gameplay.Roles.Misukisu;
using System.Diagnostics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Roles;

namespace Sims3.Gameplay.Objects.Misukisu
{
    class CustomRoleObject : GameObject, IRoleGiver, IRoleGiverExtended
    {
        private Roles.Role mCurrentRole;

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            // Our custom role will create alarms for the role based on these times
            if (base.LotCurrent == null)
            {
                startTime = 0f;
                endTime = 0f;
            }
            else
            {
                Bartending.GetRoleTimes(out startTime, out endTime, base.LotCurrent.GetMetaAutonomyType);
            }

        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {
           // When role is activated, this one gets called (once per day)
        }

        public Roles.Role CurrentRole
        {
            get
            {
                return this.mCurrentRole;
            }
            set
            {
                CustomRole newRole = value as CustomRole;
                if (newRole != null)
                {
                    this.mCurrentRole = newRole;
                }
                else if (value != null)
                {
                    EndRoleAndReplaceWithNew(value);

                }
                else
                {
                    this.mCurrentRole = value;
                }

            }

        }

        private void EndRoleAndReplaceWithNew(Role value)
        {
            if (value != null)
            {
                try
                {
                    Sim currentActor = value.SimInRole;
                    if (currentActor != null)
                    {
                        value.RemoveSimFromRole();
                        CustomRole aRole = CustomRole.clone(value, currentActor);

                        if (aRole != null)
                        {
                            this.mCurrentRole = aRole;
                            RoleManager.sRoleManager.AddRole(aRole);
                            //Message.Show("Cloned pianist and swapped it to custom role");
                        }
                        else
                        {
                            Message.ShowError("Custom Role", "Cannot create custom role, clone failed", true, null);
                        }


                    }
                    else
                    {
                        Message.ShowError("Custom Role", "Cannot create custom role, Pianist was not instantiated", true, null);
                      
                    }

                }
                catch (Exception ex)
                {
                    Message.ShowError("Custom Role", "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                Message.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
            }
            catch (Exception ex)
            {
                Message.Show("PushRoleStartingInteraction to null " + ex.Message + " : " + new StackTrace().ToString());
            }
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            // This is also called once a day, when role time ends
        }

        public string RoleName(bool isFemale)
        {
            // Shows after name in tooltip
            return "custom person";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }
    }
}
