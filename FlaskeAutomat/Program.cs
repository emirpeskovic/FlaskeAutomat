using FlaskeAutomat;

// Count how many bottles are produced total
int beerProduced = 0;
int waterProduced = 0;

// RNG
Random random = new();

// Storage with max size
var production = new Product?[10];
var beers = new Product?[5];
var waters = new Product?[5];

// Producer thread
Thread producer = new(delegate ()
{
    while (true)
    {
        Monitor.Enter(production);

        // We wait until there is space in the storage
        while (!production.Any(p => p == null))
        {
            Monitor.Wait(production);
        }

        // We produce a random product
        bool isBeer = random.Next(2) == 0;

        // We take the first available index in the storage and set it to a new beer
        var index = production.ToList().IndexOf(production.First(p => p == null));
        production[index] = new Product()
        {
            Name = isBeer ? "Beer #" + beerProduced++ : "Water #" + waterProduced++,
            IsBeer = isBeer
        };

        Console.WriteLine("[Production] {0}", production[index]!.Name);

        // We notify the consumer that there is a new product
        Monitor.PulseAll(production);
        Monitor.Exit(production);

        Thread.Sleep(100 / 15);
    }
});

Thread splitter = new(delegate ()
{
    while (true)
    {
        Monitor.Enter(production);

        // We wait until there is a product in the storage
        while (!production.Any(p => p != null))
        {
            Monitor.Wait(production);
        }

        // We take the product out and make sure it's consumed immediately by the splitter
        var product = production.First(p => p != null)!;
        var productIndex = production.ToList().IndexOf(product);
        production[productIndex] = null;

        // We notify the producer that there is a slot available
        Monitor.PulseAll(production);
        Monitor.Exit(production);

        // Select the correct storage based on which product we got
        var storage = product.IsBeer ? beers : waters;

        Monitor.Enter(storage);

        // We wait until there is space in the storage
        while (!storage.Any(p => p == null))
        {
            Monitor.Wait(storage);
        }

        // We take the first available index in the storage and set it to the product
        var index = storage.ToList().IndexOf(storage.First(p => p == null));
        storage[index] = product;

        Console.WriteLine("[Splitter] {0}", product.Name);

        // We notify the consumer that there is a new product
        Monitor.PulseAll(storage);
        Monitor.Exit(storage);

        Thread.Sleep(100 / 15);
    }
});

Thread beerConsumer = new(delegate ()
{
    while (true)
    {
        Monitor.Enter(beers);

        // We wait until there is a product in the storage
        while (!beers.Any(p => p != null))
        {
            Monitor.Wait(beers);
        }

        // We take the product out and make sure it's consumed immediately by the consumer
        var product = beers.First(p => p != null)!;
        beers[beers.ToList().IndexOf(product)] = null;

        Console.WriteLine("[Consumer] {0}", product.Name);

        // We notify the producer that there is a slot available
        Monitor.PulseAll(beers);
        Monitor.Exit(beers);

        Thread.Sleep(100 / 15);
    }
});

Thread waterConsumer = new(delegate ()
{
    while (true)
    {
        Monitor.Enter(waters);

        // We wait until there is a product in the storage
        while (!waters.Any(p => p != null))
        {
            Monitor.Wait(waters);
        }

        // We take the product out and make sure it's consumed immediately by the consumer
        var product = waters.First(p => p != null)!;
        waters[waters.ToList().IndexOf(product)] = null;

        Console.WriteLine("[Consumer] {0}", product.Name);

        // We notify the producer that there is a slot available
        Monitor.PulseAll(waters);
        Monitor.Exit(waters);

        Thread.Sleep(100 / 15);
    }
});

// Start the threads
producer.Start();
splitter.Start();
beerConsumer.Start();
waterConsumer.Start();

// Wait for the threads to finish
producer.Join();
splitter.Join();
beerConsumer.Join();
waterConsumer.Join();