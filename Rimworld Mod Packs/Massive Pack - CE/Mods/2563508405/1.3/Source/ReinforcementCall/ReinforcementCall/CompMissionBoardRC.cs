using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReinforcementCall
{
	// Token: 0x02000004 RID: 4
	public class CompMissionBoardRC : ThingComp
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
		public CompProperties_MissionBoardRC Props
		{
			get
			{
				return (CompProperties_MissionBoardRC)this.props;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002095 File Offset: 0x00000295
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
		{
			FloatMenuOption failureReason = this.GetFailureReason(myPawn);
			bool flag = failureReason != null;
			if (flag)
			{
				yield return failureReason;
			}
			else
			{
				FloatMenuOption option = this.IncidentFloatMenuOption(myPawn);
				bool flag2 = option != null;
				if (flag2)
				{
					yield return option;
				}
				option = null;
			}
			yield break;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020AC File Offset: 0x000002AC
		public FloatMenuOption IncidentFloatMenuOption(Pawn negotiator)
		{
			string text = "SelectMissionBoardRC".Translate();
			return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate()
			{
				this.GiveUseMissionBoardJob(negotiator);
			}, MenuOptionPriority.Default, null, null, 0f, null, null), negotiator, this.parent, "ReservedBy");
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000211C File Offset: 0x0000031C
		public void GiveUseMissionBoardJob(Pawn negotiator)
		{
			Job job = new Job(MBDefsOf.UseMissionBoardRC, this.parent);
			negotiator.jobs.TryTakeOrderedJob(job, JobTag.Misc);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002150 File Offset: 0x00000350
		private FloatMenuOption GetFailureReason(Pawn myPawn)
		{
			bool flag = !ReachabilityUtility.CanReach(myPawn, this.parent, PathEndMode.InteractionCell, Danger.Some, false, TraverseMode.ByPawn);
			FloatMenuOption result;
			if (flag)
			{
				result = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000006 RID: 6
		public int tickAtLastMission = -1;
	}
}
