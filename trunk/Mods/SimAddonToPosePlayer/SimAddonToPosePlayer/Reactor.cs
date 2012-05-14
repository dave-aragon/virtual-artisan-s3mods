using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.UI;

namespace Misukisu.PosePlayerAddon
{
    class React : ImmediateInteraction<Sim, Sim>
    {

        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            // Reactor.React(Actor, Target, ReactionTypes.Angry);
            Target.InteractionQueue.AddNext(DoReact.Singleton.CreateInstance(
                Actor, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, React>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPoseBoxAvailable();
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Play Reaction";
            }
        }
    }

    public class DoReact : Interaction<Sim, Sim>
    {
        public static InteractionDefinition Singleton = new Definition();

        public class Definition : InteractionDefinition<Sim, Sim, DoReact>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Play Reaction";
            }
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }

        public override bool Run()
        {

            List<ObjectListPickerInfo> Entries = ListExpressions();
            string text = (string)ObjectListPickerDialog.Show("My List", Entries);
            if (text != null && text != "")
            {
                ReactionTypes reaction = (ReactionTypes)Enum.Parse(typeof(ReactionTypes), text);
               
                Array reactionTypes = Enum.GetValues(typeof(ReactionTypes));
                CustomOverlayData data = null;
                CmoPoseBox box = PoseManager.FindPoseBox();
                string poseData = PoseManager.GetCurrentPose(Actor);
                if (poseData == null)
                {
                    return false;
                }
                Actor.LookAtManager.DisableLookAts();
                box.PlaySoloAnimation(Actor.SimDescription.IsHuman, Actor, poseData, true, ProductVersion.BaseGame);
                Actor.ResetAllAnimation();
                //SetCurrentPose(actor, poseName);
                Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayUpperbody);
                StateMachineClient stateMachineClient = StateMachineClient.Acquire(Actor.ObjectId, "facial_idle", AnimationPriority.kAPDefault, true);
                //StateMachineClient genericStateMachine = OverlayData.GetGenericStateMachine(Actor, false, false, true, false);
                data = (CustomOverlayData)OverlayComponent.GetOverlayData(ReactionTypes.FacialFear, Actor);
                //Debugger log = new Debugger(this);
                //log.Debug(this, "Enterstate-" + data.EnterState);
                //log.Debug(this, "Clip-" + data.AnimClipName);

                stateMachineClient.UseActorBridgeOrigins = false;
                stateMachineClient.SetActor("x", Actor);
                stateMachineClient.RemoveEventHandler(new SacsEventHandler(Actor.OverlayComponent.InteractionPartLevelCallback));
                stateMachineClient.RemoveEventHandler(new SacsEventHandler(Actor.OverlayComponent.ClearInteractionPartLevelCallback));
                stateMachineClient.EnterState("x", "Enter");
                stateMachineClient.SetProductVersion(data.ProductVersion);
                //   stateMachineClient.SetParameter("AnimClipName", data.AnimClipName);
                stateMachineClient.RequestState("x", data.AnimClipName);
                //Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayFace);

                box.PlaySoloAnimation(Actor.SimDescription.IsHuman, Actor, poseData, true, ProductVersion.BaseGame);
                Actor.ResetAllAnimation();

                //"SeatedOverlay"
                //Actor.OverlayComponent.PlayReaction(reaction, null, true, jazzStateName, animationTime);
                //this.Actor.IdleManager.PlayReactionAnimation(ReactionTypes.Awe);
                //this.Actor.IdleManager.PlayReactionAnimation(ReactionTypes.FacialEvil);

                Actor.WaitForExitReason(3.40282347E+38f, ExitReason.UserCanceled);
                Actor.LookAtManager.EnableLookAts();
                return true;
            }
            return false;
        }

        private static List<ObjectListPickerInfo> ListExpressions()
        {
            List<ObjectListPickerInfo> Entries = new List<ObjectListPickerInfo>();
            string rType = ReactionTypes.FacialDepressed.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialAnger.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialDisgust.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialEvil.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialFear.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialHappy.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialSad.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialSleepy.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            rType = ReactionTypes.FacialSurprise.ToString();
            Entries.Add(new ObjectListPickerInfo(rType, rType));
            return Entries;
        }
    }

    public class Reactor
    {
        public Sim Sim;
        public Sim Target;
        public ReactionTypes Reaction;
        public static void React(Sim sim, Sim target, ReactionTypes reaction)
        {
            Reactor reactor = new Reactor();
            reactor.Sim = sim;
            reactor.Target = target;
            reactor.Reaction = reaction;
            AlarmManager.Global.AddAlarm(RandomUtil.GetFloat(4f), TimeUnit.Seconds, new AlarmTimerCallback(reactor.Callback), "Reactor Callback", AlarmType.NeverPersisted, sim);
        }
        public void Callback()
        {
            this.Sim.PlayReaction(this.Reaction, this.Target, ReactionSpeed.Immediate);
        }
    }




}
