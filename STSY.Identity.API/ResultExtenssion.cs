namespace STSY.Identity.API
{
    public static class ResultExtenssion
    {
        public static object AsResult(this string message)
        {
            return new { Message = message };
        }
    }
}
