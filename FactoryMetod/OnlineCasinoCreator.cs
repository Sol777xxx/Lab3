using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3.Base;
using Lab3.Games;

namespace Lab3.FactoryMethod
{
    public class OnlineCasinoCreator : GameCreator
    {
        public override BaseGame FactoryMethod(string name, int ram = 0, int cpu = 0, int gpu = 0, int hdd = 0)
        {
            return new OnlineCasino(name);
        }
    }
}

