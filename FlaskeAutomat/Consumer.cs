using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public class Consumer
    {
        // Just to keep track of the number of consumers
        public int Id { get; set; }

        // The specific bottle type storage for this consumer
        private readonly Bottle[] storage;

        public Consumer(int id, ref Bottle[] storage)
        {
            Id = id;
            this.storage = storage;
        }

        // The loop
        public void ConsumeBottle()
        {
            while (true)
            {
                Monitor.Enter(storage);

                // Wait until there's any bottle in the storage
                while (!storage.Any(bottle => bottle != null))
                {
                    Monitor.Wait(storage);
                }

                // Take all the bottles available
                var bottles = storage.Take(bottle => bottle != null);

                // Signal splitter it's fine to continue filling this storage up
                Monitor.PulseAll(storage);
                Monitor.Exit(storage);
                
                // "Consume" every bottle
                foreach (var bottle in bottles)
                {
                    Console.WriteLine("Consumed {0}", bottle.GetName());
                }

                // Machines cool down
                Thread.Sleep(1000 / 15);
            }
        }
    }
}
