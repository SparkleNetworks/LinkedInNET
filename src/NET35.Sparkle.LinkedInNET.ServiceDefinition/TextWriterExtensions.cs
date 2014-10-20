
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class TextWriterExtensions
    {
        public static void WriteLine(this TextWriter writer, int indent, string value)
        {
            writer.Write(new string(Enumerable.Repeat(' ', indent * 4).ToArray()));
            writer.WriteLine(value);
        }

        public static void Write(this TextWriter writer, int indent, string value)
        {
            writer.Write(new string(Enumerable.Repeat(' ', indent * 4).ToArray()));
            writer.Write(value);
        }
    }
}
