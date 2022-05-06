using FlaskeAutomat;

// Storage
var production = new Bottle?[5];
var beers = new Bottle?[5];
var waters = new Bottle?[5];

// Workers
var producer = new Producer(ref production!);
var splitter = new Splitter(ref production!, ref beers!, ref waters!);

// Create consumers
var consumer = new Consumer[2];
for (int i = 0; i < consumer.Length; i++)
{
    var storage = i % 2 == 0 ? beers : waters;
    consumer[i] = new Consumer(i, ref storage!);
}

// Threads
var threads = new Thread[4];

// Create and start all threads
for (int i = 0; i < threads.Length; i++)
{
    if (i < 2)
    {
        if (i % 2 == 0)
        {
            threads[i] = new Thread(producer.ProduceBottle);
        }
        else
        {
            threads[i] = new Thread(splitter.SplitBottle);
        }
    }
    else
    {
        threads[i] = new Thread(consumer[i - 2].ConsumeBottle);
    }
    threads[i].Start();
}

// Wait for all threads to finish
foreach (var thread in threads)
{
    thread.Join();
}
