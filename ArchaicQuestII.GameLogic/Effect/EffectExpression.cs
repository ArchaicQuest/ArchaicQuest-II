namespace ArchaicQuestII.GameLogic.Effect
{
    public enum EffectExpression
    {
        None = 0,
        Addition = 1 << 0,
        Divide = 1 << 1,
        Equal = 1 << 2,
        Multiply = 1 << 3,
        Substract = 1 << 4
    }
}