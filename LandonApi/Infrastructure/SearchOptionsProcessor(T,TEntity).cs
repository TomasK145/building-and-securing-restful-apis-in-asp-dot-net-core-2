using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LandonApi.Infrastructure
{
    public class SearchOptionsProcessor<T,TEntity>
    {
        private readonly string[] _searchQuery;

        public SearchOptionsProcessor(string[] searchQuery)
        {
            _searchQuery = searchQuery;
        }

        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (_searchQuery == null)
            {
                yield break;
            }

            foreach (var expression in _searchQuery)
            {
                if (String.IsNullOrWhiteSpace(expression))
                {
                    continue;
                }

                var tokens = expression.Split(' ');  //rozdelenie expression na Name, Operator, Value

                if (tokens.Length == 0 || tokens.Length < 3)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = tokens[0]
                    };
                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = true,
                    Name = tokens[0],
                    Operator = tokens[1],
                    Value = String.Join(" ", tokens.Skip(2)) //vsetko od 2. indexu je povazovane za value
                };
            }
        }

        public IEnumerable<SearchTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms().Where(x => x.ValidSyntax).ToArray();

            if (!queryTerms.Any())
            {
                yield break;
            }

            var declaredTerms = GetTermsFromModel();

            foreach (var term in queryTerms)
            {
                var declaredTerm = declaredTerms.SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
                if (declaredTerm == null)
                {
                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = term.ValidSyntax,
                    Name = declaredTerm.Name,
                    Operator = term.Operator,
                    Value = term.Value
                };
            }
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel()
        {
            return typeof(TEntity).GetTypeInfo()
                                    .DeclaredProperties
                                    .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
                                    .Select(p => new SearchTerm { Name = p.Name });
        }
    }
}
