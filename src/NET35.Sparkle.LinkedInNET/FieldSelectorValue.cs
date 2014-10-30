
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FieldSelectorValue
    {
        public FieldSelectorValue()
        {
        }

        public FieldSelectorValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The value cannot be empty", "value");
            if (value[0] == '(')
                throw new ArgumentException("The value cannot start with a opening parenthesis", "value");
            if (value[0] == ':')
                throw new ArgumentException("The value cannot start with a colon", "value");

            // location
            // location:(name)
            // location:(name,code)
            // location:(country:(code))

            var colon = value.IndexOf(':');
            this.FullName = value;

            if (colon > 0)
            {
                this.Name = value.Substring(0, colon);
                if (value[colon + 1] != '(')
                    throw new ArgumentException("The value should have a opening parenthesis after the colon", "value");
                if (value[value.Length - 1] != ')')
                    throw new ArgumentException("The value should have a opening parenthesis after the colon", "value");

                this.Values = new List<FieldSelectorValue>();
                var valuesString = value.Substring(colon + 2, value.Length - colon - 3);

                // location:(country:(code)),name
                //       ^ ^^         ^    ^^^   ^ 
                //       | ||         |    |||   |> flush
                //       | ||         |    |||
                //       | ||         |    |||---> inName
                //       | ||         |    ||----> parenthesis=0, flush
                //       | ||         |    |-----> parenthesis=1
                //       | ||         |----------> parenthesis=2
                //       | ||--------------------> parenthesis=1
                //       | |----------------------> 
                //       |------------------------> inName

                string name = string.Empty;
                string fullname = string.Empty;
                int parenthesis = 0;
                for (int i = 0; i < valuesString.Length; i++)
                {
                    var c = valuesString[i];

                    if (c == '(')
                    {
                        parenthesis++;
                        fullname += c;
                    }
                    else if (c == ')')
                    {
                        parenthesis--;
                        fullname += c;
                    }
                    else if (c == ':')
                    {
                        if (parenthesis == 0)
                        {
                            fullname += c;
                        }
                        else
                        {
                            fullname += c;
                        }
                    }
                    else if (c == ',')
                    {
                        if (parenthesis == 0)
                        {
                            goto flush;
                        }
                        else
                        {
                            fullname += c;
                        }
                    }
                    else
                    {
                        if (parenthesis == 0)
                        {
                            name += c;
                            fullname += c;
                        }
                        else
                        {
                            fullname += c;
                        }
                    }

                    // last char
                    if ((i + 1) == valuesString.Length)
                        goto flush;

                    continue;
                flush:
                    {
                        var field = new FieldSelectorValue(fullname);
                        this.Values.Add(field);
                        name = string.Empty;
                        fullname = string.Empty;
                    }

                }
            }
            else
            {
                this.Name = value;
            }
        }

        public FieldSelectorValue(string name, FieldSelectorValue[] values)
        {
            this.Name = name;
            this.Values = values.ToList();
        }

        public string FullName { get; set; }

        public string Name { get; set; }

        public List<FieldSelectorValue> Values { get; set; }

        public override string ToString()
        {
            if (this.Values == null || this.Values.Count == 0)
            {
                return this.Name;
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(this.Name);
                sb.Append(":(");
                var sep = "";
                for (int i = 0; i < this.Values.Count; i++)
                {
                    sb.Append(sep);
                    sb.Append(this.Values[i].ToString());
                    sep = ",";
                }

                sb.Append(")");

                return sb.ToString();
            }
        }
    }
}
