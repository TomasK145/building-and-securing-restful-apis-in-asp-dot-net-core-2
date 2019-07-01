using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class Room : Resource //objekt reprezentujuci room kt bude k dispozicii pre klienta (API)
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}
