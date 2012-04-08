using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using System;
using Sims3.Gameplay;

namespace Misukisu.DrinkTrueBlood
{
    public class RecipeReader
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        private static RecipeReader instance = new RecipeReader();
        private Debugger debugger;

        public RecipeReader()
        {
            debugger = new Debugger("RecipeReader");
            //debugger.Debug(this, "Recipe reader instance created");
        }

        static RecipeReader()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(instance.ReadRecipe);
        }

        public void ReadRecipe(object sender, EventArgs e)
        {
            try
            {
                //debugger.Debug(this, "Starting recipe reading");
                Recipe.LoadRecipeData(XmlDbData.ReadData("MisukisuRecipeMasterList"), false);
                foreach (Recipe current2 in Recipe.Recipes)
                {
                    string singleServingFull = current2.ModelsAndMaterials.SingleServingFull;
                    ulong inst = (ulong)ResourceUtils.XorFoldHashString32(singleServingFull);
                    for (uint num3 = 0u; num3 < 3u; num3 += 1u)
                    {
                        UIManager.GetThumbnailImage(new ThumbnailKey(new ResourceKey(inst, 23466547u, 1u), (ThumbnailSize)num3));
                    }
                }
                //debugger.Debug(this, "Recipes loaded ");
                //debugger.EndDebugLog();
                TestStuff();
            }
            catch (Exception ex)
            {
                //debugger.DebugError(this, "Cannot read recipes, " + ex.Message, ex);
            }
        }

        
        private void TestStuff()
        {
            Recipe recipe = Recipe.NameToRecipeHash["VampireJuice"];
            debugger.Debug(this, "VAMPJUICE Object: " + recipe.ObjectToCreateInFridge + ", Code:" + recipe.CodeVersion.ToString());
            ulong guid = NameGuidMap.GetGuidByName(recipe.ObjectToCreateInFridge);
            if (guid == NameGuidMap.kInvalidNameGuid)
            {
                debugger.Debug(this, "VAMPJUICE GUID is null");
            }
            else
            {
                debugger.Debug(this, " VAMPJUICEGUID found: " + guid);
            }
            GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
            recipe = Recipe.NameToRecipeHash["TrueBlood"];
            debugger.Debug(this, "TB Object: " + recipe.ObjectToCreateInFridge + ", Code:" + recipe.CodeVersion.ToString());

             guid = NameGuidMap.GetGuidByName(recipe.ObjectToCreateInFridge);
            if (guid == NameGuidMap.kInvalidNameGuid)
            {
                debugger.Debug(this, "GUID is null");
            }
            else
            {
                debugger.Debug(this, "GUID found");
            }

            ResourceKey key = GlobalFunctions.CreateProductKey(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
            if (key == null)
            {
                debugger.Debug(this, "Resource key is null");
            }
            else
            {
                debugger.Debug(this, "Resource key is " + key.InstanceId);
            }

            try
            {
                GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion);
            }
            finally
            {
                debugger.EndDebugLog();
            }
        }
    }
}
