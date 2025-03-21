using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3.Base;
using Lab3.Games;

namespace Lab3.FactoryMethod
{
    public class SimulatorGameCreator : GameCreator
    {
        public override BaseGame FactoryMethod(string name, int ram, int cpu, int gpu, int hdd)
        {
            return new SimulatorGame(name, ram, cpu, gpu, hdd);
        }
    }
}
