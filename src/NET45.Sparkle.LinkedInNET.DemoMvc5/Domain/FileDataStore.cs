
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    public abstract class FileDataStore<TData> : IDataStore<TData>
        where TData : class, new()
    {
        private string path;

        public FileDataStore(string path)
        {
            this.path = path;
        }

        public DataTransaction<TData> Write()
        {
            var stream = new FileStream(this.path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            try
            {
                stream.Seek(0L, SeekOrigin.Begin);
                var data = this.Parse(stream);

                var transaction = new DataTransaction<TData>(
                    data ?? new TData(),
                    changedData =>
                    {
                        this.Save(stream, changedData);

                        stream.Flush();
                        stream.Dispose();
                    });
                return transaction;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public TData Read()
        {
            using (var stream = new FileStream(this.path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                return this.Parse(stream);
            }
        }

        abstract protected TData Parse(FileStream stream);
        abstract protected void Save(FileStream stream, TData data);
    }
}
