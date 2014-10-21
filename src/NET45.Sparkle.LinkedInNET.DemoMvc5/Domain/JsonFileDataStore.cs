
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Newtonsoft.Json;

    public class JsonFileDataStore<TData> : FileDataStore<TData>
        where TData : class, new()
    {
        public JsonFileDataStore(string path)
            : base(path)
        {
        }

        protected override TData Parse(FileStream stream)
        {
            if (stream.Length > 0)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                var json = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                return JsonConvert.DeserializeObject<TData>(json);
            }
            else
            {
                return null;
            }
        }

        protected override void Save(FileStream stream, TData data)
        {
            var json = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}