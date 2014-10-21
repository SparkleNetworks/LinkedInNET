
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IDataStore<TData>
        where TData : class, new()
    {
        DataTransaction<TData> Write();

        TData Read();
    }
}
