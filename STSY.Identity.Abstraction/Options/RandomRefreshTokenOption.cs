namespace STSY.Identity.Abstraction.Options
{
    public class RandomRefreshTokenOption
    {
        public int RefreshTokenSize { get; set; } = 64;
        public int ExpireHours { get; set; } = 1;
    }
}
