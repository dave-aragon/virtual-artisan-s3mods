using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Misukisu.SimAddonToPosePlayer;

namespace Misukisu.PosePlayerAddon
{
    class PoseManager
    {
        public static CmoPoseBox PoseBox = null;

        public PoseManager()
            : base()
        {
            PoseBox = FindPoseBox();

            List<SimDescription> sims = Household.AllSimsLivingInWorld();
            foreach (SimDescription sim in sims)
            {
                Sim aSim = sim.CreatedSim;
                if (aSim != null)
                {
                    aSim.AddInteraction(PoseInteraction.Singleton);
                    aSim.AddInteraction(StopPosing.Singleton);
                    aSim.AddInteraction(SnapshotPose.Singleton);
                }
            }

        }

        public static CmoPoseBox FindPoseBox()
        {
            CmoPoseBox box = null;
            List<CmoPoseBox> poseboxes = new List<CmoPoseBox>(Sims3.Gameplay.Queries.GetObjects<CmoPoseBox>());
            if (poseboxes.Count > 0)
            {
                box = poseboxes[0];
            }

            PoseBox = box;
            return box;
        }
    }
}
