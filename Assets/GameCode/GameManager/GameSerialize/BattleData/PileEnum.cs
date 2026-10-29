    public enum PileEnum
    {
        None,
        TotalPile,
        Draw,
        Hand,
        Discard,
        /// <summary>消耗堆（= exhaust）：虚无、以及打出后消耗的牌。本场不再回到抽牌堆，
        /// 但属于"可被取回"的池子（发掘 Exhume 从这里回收）。</summary>
        Removed,
        /// <summary>移出游戏：彻底出局，不走任何回收流程。能力牌打出处在这里。</summary>
        Destroy,
    }
