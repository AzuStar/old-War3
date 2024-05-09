namespace NoxRaven.Data
{
    /// <summary>
    /// Enum for all arithmetic operations for NDataModifier a and b
    /// </summary>
    public enum EArithmetic
    {
        SET,

        /// <summary>
        /// Normally deprecated <br/>
        /// Will resolve into a+b
        /// </summary>
        ADD,

        /// <summary>
        /// Normally deprecated <br/>
        /// Will resolve into a-b
        /// </summary>
        SUBTRACT,

        /// <summary>
        /// Will resolve into a*b
        /// </summary>
        MULTIPLY,

        /// <summary>
        /// Will resolve into a/b
        /// </summary>
        DIVIDE,

        /// <summary>
        /// Will resolve into a*(1+b)
        /// </summary>
        PERCENT_MULTIPLY,

        /// <summary>
        /// Will resolve into a/(1+b)
        /// </summary>
        PERCENT_DIVIDE,

        /// <summary>
        /// Will resolve into (1+a)^b-1
        /// </summary>
        TO_PERCENT_POWER,

        /// <summary>
        /// Will resolve into a^b
        /// </summary>
        TO_POWER,

        /// <summary>
        /// Will resolve into b+(1-b)/(1+a) <br/> lim(a->inf)=b <br/> lim(a->0)=1
        /// </summary>
        LIMIT_TO,

        /// <summary>
        /// Will resolve into Min(a,b)
        /// </summary>
        MIN,

        /// <summary>
        /// Will resolve into Max(a,b)
        /// </summary>
        MAX,

        /// <summary>
        /// Will resolve into (1-b)/(1+1/a) <br/> lim(a->inf)=1-b <br/> lim(a->0)=0
        /// </summary>
        LIMIT_BY,
    }
}
