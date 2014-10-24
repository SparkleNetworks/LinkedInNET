
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Allows to accumulate a list of fields for API calls.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class FieldSelector<TEntity> : FieldSelector
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSelector{TEntity}"/> class.
        /// </summary>
        public FieldSelector()
        {
        }

        /// <summary>
        /// Adds the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentException">The value cannot be empty;field</exception>
        public new FieldSelector<TEntity> Add(string field)
        {
            base.Add(field);
            return this;
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentNullException">fields</exception>
        public new FieldSelector<TEntity> AddRange(params string[] fields)
        {
            base.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        public new FieldSelector<TEntity> AddRange(IEnumerable<string> fields)
        {
            base.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Removes the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentException">The value cannot be empty;field</exception>
        public new FieldSelector<TEntity> Remove(string field)
        {
            base.Remove(field);
            return this;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>The current instance of field selector.</returns>
        public new FieldSelector<TEntity> Clear()
        {
            base.Clear();
            return this;
        }
    }

    /// <summary>
    /// Field selector extension methods.
    /// </summary>
    public class FieldSelector
    {
        private List<string> fields = new List<string>();

        /// <summary>
        /// Creates a field selector for the specified entityt type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public static FieldSelector<TEntity> For<TEntity>()
            where TEntity : class
        {
            return new FieldSelector<TEntity>();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public string[] Items
        {
            get { return this.fields.ToArray(); }
        }

        /// <summary>
        /// Adds the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentException">The value cannot be empty;field</exception>
        public void Add(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("The value cannot be empty", "field");

            if (!this.fields.Contains(field))
                this.fields.Add(field);
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentNullException">fields</exception>
        public void AddRange(params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            for (int i = 0; i < fields.Length; i++)
            {
                if (!this.fields.Contains(fields[i]))
                    this.fields.Add(fields[i]);
            }
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        public void AddRange(IEnumerable<string> fields)
        {
            foreach (var field in fields)
            {
                if (!this.fields.Contains(field))
                    this.fields.Add(field);
            }
        }

        /// <summary>
        /// Removes the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentException">The value cannot be empty;field</exception>
        public void Remove(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("The value cannot be empty", "field");

            if (this.fields.Contains(field))
                this.fields.Remove(field);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>The current instance of field selector.</returns>
        public void Clear()
        {
            this.fields.Clear();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.fields.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                var items = this.Items
                    .Select(f => f.IndexOf('/') > 0 ? f.Substring(0, f.IndexOf('/')) : f)
                    .GroupBy(f => f)
                    .Select(g => g.Key)
                    .ToArray();
                return ":(" + string.Join(",", items) + ")";
            }
        }
    }
}
