namespace STSY.Identity.Abstraction.Models.Output
{
    public class STSYIdentityResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public static STSYIdentityResult SuccessResult => new STSYIdentityResult { Success = true };


        public static STSYIdentityResult FailureResult => new STSYIdentityResult { Success = false };
        public static STSYIdentityResult BuildFailure(string error)
        {
            return new STSYIdentityResult { Success = false, Message = error };
        }
        public static STSYIdentityResult BuildSuccess(string message)
        {
            return new STSYIdentityResult { Success = true, Message = message };
        }
    }
}
