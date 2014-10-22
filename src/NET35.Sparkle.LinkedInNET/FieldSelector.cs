
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FieldSelector<TEntity>
        where TEntity : class
    {
        private List<string> fields = new List<string>();

        public FieldSelector()
        {
        }

        public FieldSelector<TEntity> Add(string field)
        {
            this.fields.Add(field);
            return this;
        }

        public FieldSelector<TEntity> AddRange(params string[] fields)
        {
            this.fields.AddRange(fields);
            return this;
        }

        public FieldSelector<TEntity> AddRange(IEnumerable<string> fields)
        {
            this.fields.AddRange(fields);
            return this;
        }

        public FieldSelector<TEntity> Remove(string field)
        {
            this.fields.Remove(field);
            return this;
        }
    }

    public static class FieldSelector
    {
        public static FieldSelector<TEntity> For<TEntity>()
            where TEntity : class
        {
            return new FieldSelector<TEntity>();
        }
    }
}
