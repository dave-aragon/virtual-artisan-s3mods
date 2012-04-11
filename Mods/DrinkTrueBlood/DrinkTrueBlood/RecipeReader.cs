using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using System;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Counters;
using System.Collections.Generic;
using Misukisu.Interactions;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.CookingObjects.Misukisu;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.CookingObjects;

namespace Misukisu.DrinkTrueBlood
{
    // Even though the name is recipereader, this also does interaction injections. 
    //The class name is linked to several places in package so I did not want to change it.
    public class RecipeReader
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        private static RecipeReader instance = new RecipeReader();
        private Debugger debugger;

        public RecipeReader()
        {
            //debugger = new Debugger("RecipeReader");
            //debugger.Debug(this, "Recipe reader instance created");
        }

        static RecipeReader()
        {
            try
            {
                World.sOnWorldLoadFinishedEventHandler += new EventHandler(instance.ReadRecipeAndAddInteractions);
                EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(instance.OnObjectBought));
            }
            catch (Exception ex)
            {
                // This should never happen, but just in case
            }
        }

        public void ReadRecipeAndAddInteractions(object sender, EventArgs e)
        {
            try
            {
                ReadRecipe();
                AddOrderInteractionToProfessionalBars();
            }
            catch (Exception ex)
            {
                Debugger debugger = new Debugger("RecipeReader");
                debugger.DebugError("RecipeReader", ex.Message, ex);
                debugger.EndDebugLog();
            }

        }

        protected ListenerAction OnObjectBought(Event e)
        {
            BarProfessional bar = e.TargetObject as BarProfessional;

            if (bar != null)
            {
                AddOrderInteractionToBar(bar);
            }

            return ListenerAction.Keep;

        }

        private void AddOrderInteractionToProfessionalBars()
        {
            List<BarProfessional> bars = new List<BarProfessional>(Sims3.Gameplay.Queries.GetObjects<BarProfessional>());
            foreach (BarProfessional bar in bars)
            {
                AddOrderInteractionToBar(bar);
            }
        }

        private static void AddOrderInteractionToBar(BarProfessional bar)
        {
            bar.AddInteraction(OrderTrueBlood.Singleton);
        }

        public void ReadRecipe()
        {

            //debugger.Debug(this, "Starting recipe reading");
            Recipe.LoadRecipeData(XmlDbData.ReadData("MisukisuRecipeMasterList"), false);
            //foreach (Recipe current2 in Recipe.Recipes)
            //{
            //    string singleServingFull = current2.ModelsAndMaterials.SingleServingFull;
            //    ulong inst = (ulong)ResourceUtils.XorFoldHashString32(singleServingFull);
            //    for (uint num3 = 0u; num3 < 3u; num3 += 1u)
            //    {
            //        UIManager.GetThumbnailImage(new ThumbnailKey(new ResourceKey(inst, 23466547u, 1u), (ThumbnailSize)num3));
            //    }
            //}
            //debugger.Debug(this, "Recipes loaded ");
            //debugger.EndDebugLog();
            //TestStuff();
            
        }



        public static void InitTrueBloodInstance(TrueBlood tb)
        {
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            tb.CreateFinishedFoodFromRecipe(recipe, Recipe.MealQuantity.Single, Quality.Nice,
                Math.Max(5, Cooking.RecipeLevelFoodPoints[recipe.CookingSkillLevelRequired]), null, false);
        }

        



        //private void TestStuff()
        //{
        //    Recipe recipe = Recipe.NameToRecipeHash["VampireJuice"];
        //    debugger.Debug(this, "VAMPJUICE Object: " + recipe.ObjectToCreateInFridge + ", Code:" + recipe.CodeVersion.ToString());
        //    ulong guid = NameGuidMap.GetGuidByName(recipe.ObjectToCreateInFridge);
        //    if (guid == NameGuidMap.kInvalidNameGuid)
        //    {
        //        debugger.Debug(this, "VAMPJUICE GUID is null");
        //    }
        //    else
        //    {
        //        debugger.Debug(this, " VAMPJUICEGUID found: " + guid);
        //    }
        //    GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
        //    recipe = Recipe.NameToRecipeHash["TrueBlood"];
        //    debugger.Debug(this, "TB Object: " + recipe.ObjectToCreateInFridge + ", Code:" + recipe.CodeVersion.ToString());

        //     guid = NameGuidMap.GetGuidByName(recipe.ObjectToCreateInFridge);
        //    if (guid == NameGuidMap.kInvalidNameGuid)
        //    {
        //        debugger.Debug(this, "GUID is null");
        //    }
        //    else
        //    {
        //        debugger.Debug(this, "GUID found");
        //    }

        //    ResourceKey key = GlobalFunctions.CreateProductKey(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
        //    if (key == null)
        //    {
        //        debugger.Debug(this, "Resource key is null");
        //    }
        //    else
        //    {
        //        debugger.Debug(this, "Resource key is " + key.InstanceId);
        //    }

        //    try
        //    {
        //        GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
        //    }
        //    finally
        //    {
        //        debugger.EndDebugLog();
        //    }
        //}
    }
}
