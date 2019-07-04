using System;
using System.Linq.Expressions;

namespace LandonApi.Infrastructure
{
    public class DecimalToIntSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!Decimal.TryParse(input, out var dec))
            {
                throw new ArgumentException("Invalid search value");
            }
            //len kvoli tomu, ze decimal hodnota (101.19) je v DB ulozena ako INT (10119)
            var places = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2]; //ziska pocet desatinnych miest z decimal hodnoty , pr. pre cislo 101.19 ziska info o 2 des miestach
            if (places < 2)
            {
                places = 2;
            }
            var justDigits = (int)(dec * (decimal)Math.Pow(10, places)); //pre zbavenia sa desatinnych miest, pr cislo 101.19 vynasobi 10^pocet_des_miest (t.j. 10^2) pre ziskanie int hodnoty 
            return Expression.Constant(justDigits);
        }

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case "gt":
                    return Expression.GreaterThan(left, right);
                case "gte":
                    return Expression.GreaterThanOrEqual(left, right);
                case "lt":
                    return Expression.LessThan(left, right);
                case "lte":
                    return Expression.LessThanOrEqual(left, right);
                //if nothing matches, fall back to base implementation
                default:
                    return base.GetComparison(left, op, right);
            }
        }
    }
}
