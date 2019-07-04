using LandonApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class Room : Resource //objekt reprezentujuci room kt bude k dispozicii pre klienta (API)
    {
        [Sortable] //custom atribut ktory definuje ci je mozne sortovat podla danej property
        [Searchable] //custom atribut definuje ze property bude searchovatelna
        public string Name { get; set; }
        [Sortable(Default = true)] //property je oznacena ako default sort property
        [SearchableDecimal]
        public decimal Rate { get; set; }
    }
}
