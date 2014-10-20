
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseApi
    {
        protected static string FormatUrl(string format, params string[] values)
        {
            var result = format;

            var dic = new Dictionary<string, string>(values.Length / 2);
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                {

                }
                else
                {
                    dic.Add(values[i - 1], values[i]);
                }
            }

            foreach (var key in dic.Keys)
            {
                result = result.Replace("{" + key + "}", dic[key]);
            }

            return result;
        }
    }
}
