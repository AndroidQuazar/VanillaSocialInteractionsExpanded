using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class InteractionWorker_Teaching : InteractionWorker
	{
		private static readonly SimpleCurve OpinionFactorCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0.1f),
			new CurvePoint(5f, 0.2f),
			new CurvePoint(10f, 0.3f),
			new CurvePoint(20f, 0.5f),
			new CurvePoint(30f, 0.7f),
			new CurvePoint(40f, 0.9f),
			new CurvePoint(50f, 1f),
			new CurvePoint(60f, 1.2f),
			new CurvePoint(70f, 1.5f)
		};
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableTeaching)
			{
				if (initiator.relations.OpinionOf(recipient) < 0 || initiator.IsPrisoner && initiator.RecruitDifficulty(recipient.Faction) > 10)
				{
					return 0f;
				}
				var skillDef = GetSkillDefToTeach(initiator, recipient);
				if (skillDef != null)
				{
					VSIE_Utils.SocialInteractionsManager.teachersWithPupils[initiator] = new TeachingTopic(recipient, skillDef);
					return 0.1f * OpinionFactorCurve.Evaluate(initiator.relations.OpinionOf(recipient));
				}
			}
			return 0f;
		}
		
		private SkillDef GetSkillDefToTeach(Pawn teacher, Pawn pupil)
        {
			var pupilsSkills = pupil.skills.skills.Where(x => !x.TotallyDisabled);
			var relevantTeacherSkills = teacher.skills.skills.Where(x => !x.TotallyDisabled && pupilsSkills.FirstOrDefault(y => x.def == y.def && x.Level > y.Level + 3) != null);
			if (relevantTeacherSkills.Any())
            {
				return relevantTeacherSkills.RandomElementByWeight(x => x.Level).def;
			}
			return null;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
			var teachingTopic = VSIE_Utils.SocialInteractionsManager.teachersWithPupils[initiator];
			VSIE_Utils.SocialInteractionsManager.teachersWithPupils.Remove(initiator);
			if (teachingTopic.pupil == recipient)
            {
				var teacherSkill = initiator.skills.GetSkill(teachingTopic.skillDef);
				var pupilSkill = recipient.skills.GetSkill(teachingTopic.skillDef);
				var levelDiff = teacherSkill.Level - pupilSkill.Level;
				var newExpToLearnPupil = GetExpNeededToLearn(teacherSkill, pupilSkill, levelDiff) / Mathf.Max(10 - levelDiff, 1f);
				pupilSkill.Learn(newExpToLearnPupil);
				var newExpToLearnTeacher = newExpToLearnPupil / 3f;
				teacherSkill.Learn(newExpToLearnTeacher);
				extraSentencePacks.Add(RulePackDef.Named("VSIE_Teaching_" + teachingTopic.skillDef.defName));
			}
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
        }

		public float GetExpNeededToLearn(SkillRecord teacherSkill, SkillRecord pupilSkill, int levelDiff)
		{
			float num = 0f;
			if (levelDiff > 5) levelDiff = 5;
			for (int i = 0; i < levelDiff; i++)
			{
				num += SkillRecord.XpRequiredToLevelUpFrom(i) / 4f;
			}
			return num;
		}
	}
}