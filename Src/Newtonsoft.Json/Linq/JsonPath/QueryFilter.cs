using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
    %Meta.Trace
    internal class QueryFilter : PathFilter
    {
        public QueryExpression Expression { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                foreach (JToken v in t)
                {
                    if (Expression.IsMatch(v))
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}