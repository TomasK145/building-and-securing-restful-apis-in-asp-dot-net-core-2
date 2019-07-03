using System;

namespace LandonApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)] //definuje ze moze byt atribut pouzity na property a nemoze byt viac krat
    public class SortableAttribute : Attribute
    {
        //kod nie je potrebny, cez reflection sa overuje existencia atributu
        public bool Default { get; set; }
    }
}
