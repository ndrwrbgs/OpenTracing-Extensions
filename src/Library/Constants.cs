namespace OpenTracing.Contrib.Extensions
{
    /// <remarks>
    /// Note that the callerMember- key names do not preface with 'caller', since the 'caller' here is
    /// the code that is calling the Log method, and it is just trying to log it's own member name, not
    /// its caller's member name.
    /// </remarks>
    public static class Constants
    {
        public const string CallerMemberNameTagName = "memberName";
        
        public const string CallerMemberNameLogKey = "memberName";
        public const string CallerMemberLineNumberLogKey = "memberLineNumber";
    }
}