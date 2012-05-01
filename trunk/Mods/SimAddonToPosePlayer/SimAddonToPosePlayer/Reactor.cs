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
                return  !isAutonomous;
            }
        }
        
        public override bool Run()
        {

            this.Actor.LoopIdle();
            this.Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayUpperbody);
            Debugger log = new Debugger(Actor);
            string jazzStateName=null;
            float animationTime=0;ReactionTypes reaction=ReactionTypes.FacialEvil;
            Array reactionTypes=Enum.GetValues(typeof(ReactionTypes));
            CustomOverlayData data = null;
            foreach (ReactionTypes reactionType in reactionTypes)
            {
                IdleAnimationInfo idleAnimationInfo;
                IdleManager.sReactionAnimations.TryGetValue(reactionType, out idleAnimationInfo);
                if (idleAnimationInfo != null)
                {
                    log.Debug(Actor, "Animation for " + reactionType.ToString() + " is " + idleAnimationInfo.AnimationType);
                    animationTime =idleAnimationInfo.AnimationTime;
                    jazzStateName = "Reaction - " + idleAnimationInfo.AnimationType;
                    reaction=reactionType;

                    data = (CustomOverlayData)OverlayComponent.GetOverlayData(reactionType, Actor);
                    if (data != null)
                    {
                        log.Debug(Actor, "Animation clip name is " + data.AnimClipName);
                    }
                }
                else
                {
                    log.Debug(Actor, "no animation for " + reactionType.ToString());
                }
                //string animationType = Actor.IdleManager.GetAnimationType(reactionType);
                //float animationTime = Actor.IdleManager.GetAnimationTime(reactionType);
            }
            //"SeatedOverlay"
            log.Debug(Actor, "Now playing " + data.AnimClipName);
            Actor.OverlayComponent.PlayReaction(reaction, null, true, jazzStateName, animationTime);
            //this.Actor.IdleManager.PlayReactionAnimation(ReactionTypes.Awe);
            //this.Actor.IdleManager.PlayReactionAnimation(ReactionTypes.FacialEvil);

            StateMachineClient genericStateMachine = OverlayData.GetGenericStateMachine(Actor, false, false, true, false);
            
            genericStateMachine.SetProductVersion(data.ProductVersion);
            genericStateMachine.SetParameter("AnimClipName", data.AnimClipName);
            genericStateMachine.RequestState("x", jazzStateName);
            if (animationTime > 0f)
            {
                Actor.WaitForExitReason(animationTime, ExitReason.Default);
            }
            genericStateMachine.RequestState("x", "Exit");
            return true; 
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
