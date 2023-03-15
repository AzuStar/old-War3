namespace NoxRaven.Data
{
    public enum EArithmetic : int
    {
        /// <summary>
        /// value+=chain
        /// </summary>
        ADD,
        /// <summary>
        /// value*=(1+chain)
        /// </summary>
        MULT,
        /// <summary>
        /// value*=100/(100+chain)
        /// </summary>
        LOG
    }
}