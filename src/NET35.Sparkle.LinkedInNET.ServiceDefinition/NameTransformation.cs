
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;

    [Flags]
    public enum NameTransformation
    {
        None = 0,
        CamelCase =  0x0001,
        PascalCase = 0x0002,
    }
}
