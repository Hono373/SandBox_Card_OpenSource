public static class ModifierQuery
{
    public static ModifierInfo GetInfo(this Modifier modifier)
    {
        return ModifierInfo.Get(modifier.data.key);
    }
}
