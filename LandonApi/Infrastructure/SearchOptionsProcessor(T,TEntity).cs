using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        internal IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();

            if (!terms.Any())
            {
                return query;
            }
            var modifiedQuery = query;
            foreach (var term in terms)
            {
                var propertyInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name); //ziskanie PropertyInfo pre Property objektu (nazov property z Term.Name)
                var obj = ExpressionHelper.Parameter<TEntity>(); //ziskanie referencie na entity model, pre ktory je property ziskavana

                //build up the LINQ expression backwards
                //query = query.Where(x => x.Property == "Value");
                //ziskanie casti x.Property
                var left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                //ziskanie hodnoty "Value"
                var right = term.ExpressionProvider.GetValue(term.Value);//Expression.Constant(term.Value);
                //vytvorenie comparison --> x.Property == "Value"   
                //var comparisonExpression = Expression.Equal(left, right);  --> umoznuje len vyuzit ==
                var comparisonExpression = term.ExpressionProvider.GetComparison(left, term.Operator, right);
                //vytvorenie lambda expression --> x => x.Property == "Value"
                var lambdaExpression = ExpressionHelper.GetLambda<TEntity, bool>(obj, comparisonExpression);

                //aplikovanie na query --> query = query.Where(x => x.Property == "Value");
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }
            return modifiedQuery;
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
                    Value = term.Value,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo()
                                    .DeclaredProperties
                                    .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
                                    .Select(p => new SearchTerm {
                                                                    Name = p.Name,
                                                                    ExpressionProvider = p.GetCustomAttribute<SearchableAttribute>().ExpressionProvider
                                                                });
        }
    }
}
