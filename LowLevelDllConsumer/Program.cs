using System;
using System.Threading;

namespace LowLevelDllConsumer
{
    class Program
    {
        /// <summary>
        /// Testování mapování tříd C++ na C# s Garbage COllectorem
        /// </summary>
        static void Main(string[] args)
        {
            // Tyto třídy byly definovány na straně C++ v DLL.
            Dog testDog = new("Azor", age: 2);  
            Console.WriteLine(testDog.Speak());

            Sheep testSheep = new("Betsy", age: 4);
            Console.WriteLine(testSheep.Speak());
            
            Bird testBird = new("Andulka", age: 12);
            Console.WriteLine(testBird.Speak());
            
            // Mluvicí papoušek byl definován tady v C#, ale je odvozen z C++ třídy
            SpeakingParrot testSP = new("Ferda", 31);
            Console.WriteLine(testSP.Speak());

            int lastInstancesNum = -1;
            // Nikdy by nemělo vzniknout mnoho instancí v paměti, protože GC má občas automaticky
            // zlikvidovat staré instance.
            for (int i=0; i<800000; ++i)
            {
                var thisShoulBeRemovedByGC1 = new SpeakingParrot("Ferda", 7);
                var thisShoulBeRemovedByGC2 = Animal.GetRandomAnimal(Guid.NewGuid().ToString(), DateTime.Now.Millisecond % 17);
       
                int instances = Animal.GetAnimalsNum();
                if (instances < lastInstancesNum)
                    Console.WriteLine($"Byl použit GC! Minulé kolo bylo {lastInstancesNum} instancí. Nyní máme {instances} instancí na {i} průchodů");
                lastInstancesNum = instances;
            }

            Console.WriteLine("Ukončen cyklus");
            Console.WriteLine("Zůstalo instancí: " + Animal.GetAnimalsNum());
            Console.ReadLine();
        }
    }
}
