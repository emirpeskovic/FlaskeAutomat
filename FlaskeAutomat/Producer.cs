using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public class Producer
    {
        // Production storage
        private readonly Bottle[] storage;

        // Keep track of each bottle produced so we can add numbers on the bottles per type
        private int beersProduced = 0;
        private int watersProduced = 0;

        public Producer(ref Bottle[] storage)
        {
            this.storage = storage;
        }

        // The loop
        public void ProduceBottle()
        {
            // Randomization
            var random = new Random();
            
            while (true)
            {
                Monitor.Enter(storage);

                // Check if there is any slots available in storage for production
                while (!storage.Any(bottle => bottle == null))
                {
                    Console.WriteLine("Production halted...");
                    Monitor.Wait(storage);
                }

                // Produce a bottle for every slot available
                for (int i = 0; i < storage.Count(bottle => bottle == null); i++)
                {
                    if (storage[i] == null)
                    {
                        // Randomly choose a bottle
                        Bottle bottle = random.Next(2) % 2 == 0 ? new Beer(beersProduced++) : new Water(watersProduced++);

                        // Put it in the storage
                        storage[i] = bottle;

                        Console.WriteLine("Bottle produced: " + bottle.GetName());
                    }
                }

                // Signal availability to splitter
                Monitor.Pulse(storage);
                Monitor.Exit(storage);

                // Machines cool down
                Thread.Sleep(1000 / 15);
            }
        }
    }
}
