using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Models
{
    public class Colony
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Colony(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
