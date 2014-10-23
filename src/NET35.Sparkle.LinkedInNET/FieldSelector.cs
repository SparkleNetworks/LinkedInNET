
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
    public class FieldSelector<TEntity>
        where TEntity : class
    {
        private List<string> fields = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSelector{TEntity}"/> class.
        /// </summary>
        public FieldSelector()
        {
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
        public FieldSelector<TEntity> Add(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("The value cannot be empty", "field");

            this.fields.Add(field);
            return this;
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentNullException">fields</exception>
        public FieldSelector<TEntity> AddRange(params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            this.fields.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Adds a range of fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The current instance of field selector.</returns>
        public FieldSelector<TEntity> AddRange(IEnumerable<string> fields)
        {
            this.fields.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Removes the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The current instance of field selector.</returns>
        /// <exception cref="System.ArgumentException">The value cannot be empty;field</exception>
        public FieldSelector<TEntity> Remove(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("The value cannot be empty", "field");

            this.fields.Remove(field);
            return this;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>The current instance of field selector.</returns>
        public FieldSelector<TEntity> Clear()
        {
            this.fields.Clear();
            return this;
        }
    }

    /// <summary>
    /// Field selector extension methods.
    /// </summary>
    public static class FieldSelector
    {
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
    }
}
