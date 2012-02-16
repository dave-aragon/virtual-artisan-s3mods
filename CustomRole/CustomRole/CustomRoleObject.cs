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
            //Message.Show("Someone is asking role times " + new StackTrace().ToString());
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
            //try
            //{

            //    Message.Show("Adding role actions to " + (sim != null ? sim.FullName : "null"));
            //}
            //catch (Exception ex)
            //{
            //    Message.Show("Adding role actions to null " + ex.Message + " - " + new StackTrace().ToString());
            //}
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
                    Message.Show("Custom role set! who the hack did this: " + new StackTrace().ToString());
                    this.mCurrentRole = newRole;
                }
                else if (value != null)
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
                                Message.Show("Cloned pianist and swapped it to custom role");
                            }
                            else
                            {
                                Message.Show("Cloning failed");
                            }

                            
                        }
                        else
                        {
                            Message.Show("Cannot clone, actor not instantiated");
                        }

                    }
                    catch (Exception ex)
                    {
                        Message.Show("Cloning custom role failed " + ex.Message + " : " + new StackTrace().ToString());
                        this.mCurrentRole = value;
                    }

                }
                else
                {
                    Message.Show("Null role was set " + new StackTrace().ToString());
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
            //Message.Show("RemoveRoleGivingInteraction from " + (sim != null ? sim.FullName : "null"));
        }

        public string RoleName(bool isFemale)
        {
            return "custom person";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }
    }
}
