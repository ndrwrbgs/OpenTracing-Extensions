namespace OpenTracing.Contrib.Extensions
{
    using System;

    [Flags]
    public enum CallerInfoLogFlags
    {
        None = 0,

        CallerMemberName = 1 << 0,
        CallerLineNumber = 1 << 1,

        AllInformation = CallerMemberName | CallerLineNumber
    }
}