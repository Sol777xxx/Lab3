using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3.Base;

namespace Lab3.FactoryMethod
{
    public abstract class GameCreator
    {
        public abstract BaseGame FactoryMethod(string name, int ram = 0, int cpu = 0, int gpu = 0, int hdd = 0);
    }
}
