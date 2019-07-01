using System;

namespace LandonApi.Models
{
    public class RoomEntity //objekt reprezentujuci room kt bude k dispozicii pre EF
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; } //PROBLEM S UKLADANYM DECIMAL HODNOT???
    }
}
