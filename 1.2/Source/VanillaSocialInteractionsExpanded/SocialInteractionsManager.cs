using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
    public class Aspiration : IExposable
    {
        private Pawn pawn;
        private ThoughtDef thoughtDefSuccess;
        private ThoughtDef thoughtDefFail;
        public InspirationDef inspirationDef;

        private int count;
        private int finalCount;
        public Aspiration()
        {

        }
        public Aspiration(Pawn pawn, InspirationDef inspirationDef, int finalCount, ThoughtDef thoughtDefSuccess, ThoughtDef thoughtDefFail)
        {
            this.pawn = pawn;
            this.inspirationDef = inspirationDef;
            this.finalCount = finalCount;
            this.thoughtDefSuccess = thoughtDefSuccess;
            this.thoughtDefFail = thoughtDefFail;
        }
        public void Notify_Progress(out bool finished)
        {
            count++;
            if (count >= finalCount)
            {
                finished = true;
            }
            else
            {
                finished = false;
            }
        }

        public void OnComplete()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDefSuccess);
        }

        public void OnExpired()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDefFail);
        }
        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Defs.Look(ref thoughtDefSuccess, "thoughtDefSuccess");
            Scribe_Defs.Look(ref thoughtDefFail, "thoughtDefFail");
            Scribe_Values.Look(ref count, "count");
            Scribe_Values.Look(ref finalCount, "finalCount");
            Scribe_Defs.Look(ref inspirationDef, "inspirationDef");
        }
    }
    public class SocialInteractionsManager : GameComponent
    {
        private Dictionary<Pawn, Aspiration> activeAspirations = new Dictionary<Pawn, Aspiration>();
        public SocialInteractionsManager()
		{
		}

		public SocialInteractionsManager(Game game)
		{

		}
        public void PreInit()
        {

        }
        public override void StartedNewGame()
        {
            PreInit();
            base.StartedNewGame();
        }

        public override void LoadedGame()
        {
            PreInit();
            base.LoadedGame();
        }

        public void Notify_AspirationProgress(Pawn pawn)
        {
            if (activeAspirations.TryGetValue(pawn, out var aspiration))
            {
                if (aspiration.inspirationDef == pawn.InspirationDef)
                {
                    aspiration.Notify_Progress(out bool finished);
                    if (finished)
                    {
                        Log.Message(pawn + " aspiration is done: " + aspiration.inspirationDef);
                        aspiration.OnComplete();
                        activeAspirations.Remove(pawn);
                    }
                }
            }
        }

        public void TryRegisterNewIspiration(Pawn pawn, InspirationDef inspirationDef)
        {
            if (activeAspirations.ContainsKey(pawn))
            {
                activeAspirations.Remove(pawn);
            }
            var aspirationDef = GetAspirationDefFor(inspirationDef);
            if (aspirationDef != null)
            {
                activeAspirations[pawn] = new Aspiration(pawn, inspirationDef, aspirationDef.workCount, aspirationDef.thoughtDefSuccess, aspirationDef.thoughtDefFail);
            }
        }

        public AspirationDef GetAspirationDefFor(InspirationDef inspirationDef)
        {
            foreach (var aspirationDef in DefDatabase<AspirationDef>.AllDefs)
            {
                if (aspirationDef.inspirationDef == inspirationDef)
                {
                    return aspirationDef;
                }
            }
            return null;
        }
        public void Notify_InspirationExpired(Pawn pawn, InspirationDef inspirationDef)
        {
            if (activeAspirations.TryGetValue(pawn, out var aspiration) && inspirationDef == aspiration.inspirationDef)
            {
                Log.Message(pawn + " aspiration is expired: " + aspiration.inspirationDef);
                aspiration.OnExpired();
                activeAspirations.Remove(pawn);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref activeAspirations, "activeAspirations", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref aspirationValues);
        }

        private List<Pawn> pawnKeys;
        private List<Aspiration> aspirationValues;
    }
}
