
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TupleStruct<T1, T2>
    {
        public TupleStruct(T1 value1, T2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public T1 Value1;
        public T2 Value2;
    }
}
