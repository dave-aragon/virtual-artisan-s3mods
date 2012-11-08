using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace.RouteDestinations;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay;

using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Objects.Misukisu;

namespace Misukisu.HomeBarBusiness
{

    public class AssignBartender : Interaction<Sim, EmployeeCertificate>
    {

        public class Definition : InteractionDefinition<Sim, EmployeeCertificate, AssignBartender>, IAllowedOnClosedVenues
        {

            public override string GetInteractionName(Sim actor, EmployeeCertificate target, InteractionObjectPair iop)
            {
                return "Hire Bartender...";
            }


            public override bool Test(Sim a, EmployeeCertificate target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.LotCurrent.LotType != LotType.Residential)
                {
                    return false;
                }
                if (isAutonomous)
                {
                    return false;
                }

                return true;

            }
        }

        public static InteractionDefinition Singleton = new AssignBartender.Definition();

        public override bool Run()
        {
            try
            {
                SimDescription simToHire = ShowObjectSelectionDialog(Target);
                if (simToHire != null)
                {

                    EmployeeCertificate poster = this.Target;
                    if (poster.CurrentRole != null)
                    {
                        Role current = poster.CurrentRole;
                        current.RemoveSimFromRole();
                    }

                    if (simToHire.AssignedRole != null)
                    {
                        simToHire.AssignedRole.RemoveSimFromRole();
                    }

                    Sims3.Gameplay.Skills.Bartending.Bartender role = new HomeBartender(simToHire, poster);

                    RoleManager.sRoleManager.AddRole(role);
                    poster.CurrentRole = role;
                    role.StartRole();
                }
            }
            catch (Exception ex)
            {
                Debugger debugger = HomeBartender.debugger;
                debugger.DebugError(this, "cannot assign bartender", ex);
                
            }
            return true;
        }

        private SimDescription ShowObjectSelectionDialog(GameObject sim)
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("Object", null, 400));


            List<ObjectPicker.RowInfo> tableData = new List<ObjectPicker.RowInfo>();

            int roomId = sim.RoomId;
            List<SimDescription> residents = Household.AllSimsLivingInWorld();
            List<SimDescription> townies = Household.AllTownieSimDescriptions();

            List<SimDescription> simdescriptions = new List<SimDescription>();
            foreach (SimDescription townie in residents)
            {
                if (townie.IsHuman && !townie.ChildOrBelow && !simdescriptions.Contains(townie))
                {
                    simdescriptions.Add(townie);
                }
            }

            foreach (SimDescription gameObject in simdescriptions)
            {
                ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(gameObject, new List<ObjectPicker.ColumnInfo>());
                ThumbnailKey thumbnail;
                if (gameObject.CreatedSim != null)
                {
                    thumbnail = gameObject.CreatedSim.GetThumbnailKey();
                }
                else
                {
                    thumbnail = gameObject.GetEverydayThumbnail(ThumbnailSize.Medium);
                }
                rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, gameObject.FullName));
                tableData.Add(rowInfo);
            }

            List<ObjectPicker.TabInfo> list3 = new List<ObjectPicker.TabInfo>();
            list3.Add(new ObjectPicker.TabInfo("shop_all_r2", "Hire A Sim", tableData));
            string buttonOk = Localization.LocalizeString("Ui/Caption/Global:Ok", new object[0]);
            string buttonCancel = Localization.LocalizeString("Ui/Caption/Global:Cancel", new object[0]);
            List<ObjectPicker.RowInfo> userSelection = BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator,
                "Select A Sim to Hire", buttonOk, buttonCancel, list3, headers, 1);
            if (userSelection == null || userSelection.Count < 1)
            {
                return null;
            }
            return userSelection[0].Item as SimDescription;
        }


    }




}

