using System;
using System.Collections.Generic;
using UnityEngine;
public class BattleCardModify
{
    public bool needSettle;
    public BattleCard card;
    public BattleCardModified modified;
    public BattleCardModify_Key keyModify = new();
    public BattleCardModify_Name nameModify = new();
    public BattleCardModify_Owner ownerModify = new();
    public BattleCardModify_Frequency frequencyModify = new();
    public BattleCardModify_CostRuleMode costRuleModeModify = new();
    public BattleCardModify_CostRule costRuleModify = new(); 
    public BattleCardModify_InteractiveMode interactiveModeModify = new();
    public BattleCardModify_ChoiceRule choiceRuleModify = new();
    public BattleCardModify_Graphic graphicModify = new();
    public BattleCardModify_Timings timmingModify = new();
    public BattleCardModify_Useable useableModify = new();
    public BattleCardModify Start(BattleCard card, bool needSettle)
    {
        this.card = card;
        this.needSettle = needSettle;
        card.modified = modified = new();
        modified.id = card.data.id;

        modified.key = keyModify.Start(this, needSettle).keyResult;
        modified.cardName = nameModify.Start(modified, needSettle).cardNameResult;
        modified.owner = ownerModify.Start(modified, needSettle).ownerResult;
        modified.frequency = frequencyModify.Start(modified, needSettle).frequencyResult;
        modified.costRule = costRuleModeModify.Start(modified, needSettle).costRuleResult;
        modified.costResult = costRuleModify.Start(modified, needSettle).costResult;
        modified.interactiveMode = interactiveModeModify.Start(modified, needSettle).interactiveModeResult;
        modified.rangeRule = choiceRuleModify.Start(modified, needSettle).choiceResult;
        modified.graphic = graphicModify.Start(modified, needSettle).graphicResult;
        timmingModify.Start(modified, needSettle);
        modified.useBlock = useableModify.Start(modified, needSettle).useBlockResult;

        return this;
    }
}