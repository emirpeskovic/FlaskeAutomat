using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public abstract class Bottle
    {
        // Keep track of the produced bottle number
        public int Id { get; set; }

        public Bottle(int id)
        {
            Id = id;
        }

        // Bottle types will override this method to return the correct bottle name and id
        public abstract string GetName();
    }

    public class Beer : Bottle
    {
        public Beer(int id) : base(id) {}

        public override string GetName() => $"Beer #{Id}";
    }
    
    public class Water : Bottle
    {
        public Water(int id) : base(id) {}

        public override string GetName() => $"Water #{Id}";
    }
}
