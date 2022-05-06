using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public class Splitter
    {
        // Reference to the different storages
        private readonly Bottle[] production, beers, waters;
        
        public Splitter(ref Bottle[] production, ref Bottle[] beers, ref Bottle[] waters)
        {
            this.production = production;
            this.beers = beers;
            this.waters = waters;
        }

        // Splitter loop
        public void SplitBottle()
        {
            while (true)
            {
                Monitor.Enter(production);

                // We check if any new bottle is in production, otherwise wait until one is
                while (!production.Any(bottle => bottle != null))
                {
                    Monitor.Wait(production);
                }

                // Take all the bottles in production
                var bottles = production.Take(bottle => bottle != null);

                // Allow production to continue working while we do our thing
                Monitor.Pulse(production);
                Monitor.Exit(production);

                // Go through each bottle
                foreach (var bottle in bottles)
                {
                    // Select the appropriate storage depending on bottle type
                    var storage = bottle is Beer ? beers : waters;

                    Monitor.Enter(storage);

                    // Check if the bottle type storage is full, and wait if it is
                    while (storage.Any(bottle => bottle != null))
                    {
                        Monitor.Wait(storage);
                    }

                    // Find the first available slot in the storage and put our bottle in it
                    for (int i = 0; i < storage.Length; i++)
                    {
                        if (storage[i] == null)
                        {
                            storage[i] = bottle;
                            break;
                        }
                    }

                    Console.WriteLine("{0} is split", bottle.GetName());

                    // Notify consumers
                    Monitor.Pulse(storage);
                    Monitor.Exit(storage);
                }

                // Machines cool down
                Thread.Sleep(1000 / 15);
            }
        }
    }
}
