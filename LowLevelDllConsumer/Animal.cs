using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LowLevelDllConsumer
{
    /// <summary>
    /// Třída Animal je definována v DLL na straně C++.
    /// Toto je její obraz. Tato C# třída neuchovává sama žádná data, ale odkazuje se ukazatelem
    /// na objekt C++, který se vytvořil v DLL pomocí new.
    /// </summary>
    public class Animal : LowLevelClass // Důležité odvození kvůli GCC!
    {
        protected Animal() { }

        /// <summary>
        /// Na straně C# nemá třída žádná data. Načítají se ze strany C++ a i ukládají se tam
        /// </summary>
        public string Name
        {
            get => NativeStr.PtrToStr(AnimalNative.GetName(Ptr));
            set => AnimalNative.SetName(Ptr, value);
        }

        /// <summary>
        /// Na straně C# nemá třída žádná data. Načítají se ze strany C++ a i ukládají se tam
        /// </summary>
        public int Age
        {
            get => AnimalNative.GetAge(Ptr);
            set => AnimalNative.SetAge(Ptr, value);
        }

        public virtual string Speak()
            => NativeStr.PtrToStr(AnimalNative.Speak(Ptr));

        /// <summary>
        /// Tato metoda pochází z C++ a může logicky vracet jen zvířata, která
        /// byla definována na straně C++.
        /// </summary>
        public static Animal GetRandomAnimal(string name, int age)
        => new Animal()
        {
            Ptr = AnimalNative.GetRandomAnimal(name, age)
        };

        /// <summary>
        /// Kontrola počtu existujících instancí
        /// </summary>
        public static int GetAnimalsNum() => AnimalNative.GetAnimalsNum();

        public override string ToString()
            => $"{Speak()}, {Name}, {Age}";

        /// <summary>
        /// Skrytě volá destruktor C++
        /// Není třeba volat manuálně (i když je to lepší), protože GC občas smaže nepoužívané instance
        /// </summary>
        public void Dispose()
        {
            AnimalNative.Dispose(Ptr);
        }

        /// <summary>
        /// Destruktor je v C# neobvyklý, tady je však nutný, protože na straně C++ potřebuju zavolat delete.
        /// Odvozené třídy nepotřebují definovat destruktor znovu, stačí když je v základní třídě.
        /// </summary>
        ~Animal() => Dispose();
    }

    /// <summary>
    /// Tato třída existuje i na straně C++.
    /// Většina nutných funkcí je ale již definována výše ve třídě Animal
    /// </summary>
    public class Dog : Animal
    {
        public Dog(string name, int age)
        {
            Ptr = AnimalNative.Dog_Create(name, age);
        }
    }

    /// <summary>
    /// Tato třída existuje i na straně C++.
    /// Většina nutných funkcí je ale již definována výše ve třídě Animal
    /// </summary>
    public class Sheep : Animal
    {
        public Sheep(string name, int age)
        {
            Ptr = AnimalNative.Sheep_Create(name, age);
        }
    }

    /// <summary>
    /// Tato třída existuje i na straně C++.
    /// Většina nutných funkcí je ale již definována výše ve třídě Animal
    /// </summary>
    public class Bird : Animal
    {
        public Bird(string name, int age)
        {
            Ptr = AnimalNative.Bird_Create(name, age);
        }
    }

    /// <summary>
    /// Tato třída již neexistuje na straně C++, ale je odvozena ze třídy, která
    /// je definována v C++.
    /// Tato třída dokazuje, že je možné vzít třídu z C++ a rozšířit ji.
    /// </summary>
    public class SpeakingParrot : Bird
    {
        public string RandomGreeting { get; private init; }

        public SpeakingParrot(string name, int age) : base(name, age)
        {
            RandomGreeting = new Random().Next(3) switch
            {
                0 => "Ahoj!",
                1 => "Dobrý den!",
                _ => "Nazdar!"
            };
        }

        /// <summary>
        /// Mluvicí papoušek nepoužívá metodu Speak, která byla definována na straně C++,
        /// ale používá svoji vlastní metodu definovanou tady v C#.
        /// Využívá properties (členské proměnné), přičemž RandomGreeting je místní, ostatní
        /// byly definovány ve třídě v C++
        /// </summary>
        /// <returns></returns>
        public override string Speak()
            => $"{RandomGreeting} Já jsem {Name}, {Age} roků!";
    }

    /// <summary>
    /// Obsahuje funkce definované v DLL, které jsou určeny k zaobalení objektů C++.
    /// </summary>
    internal static class AnimalNative
    {
        const string DllPath = @"C:\Users\xmart\source\repos\CppClassDll\x64\Release\CppClassDll.dll";

        [DllImport(DllPath, EntryPoint = "GetRandomAnimal")]
        internal extern static IntPtr GetRandomAnimal(string name, int age);

        [DllImport(DllPath, EntryPoint = "Animal_Dispose")]
        internal extern static void Dispose(IntPtr animal);

        [DllImport(DllPath, EntryPoint = "Animal_Speak")]
        internal extern static IntPtr Speak(IntPtr animal);

        [DllImport(DllPath, EntryPoint = "Animal_GetName")]
        internal extern static IntPtr GetName(IntPtr animal);
        
        [DllImport(DllPath, EntryPoint = "Animal_SetName")]
        internal extern static void SetName(IntPtr animal, string val);

        [DllImport(DllPath, EntryPoint = "Animal_GetAge")]
        internal extern static int GetAge(IntPtr animal);

        [DllImport(DllPath, EntryPoint = "Animal_SetAge")]
        internal extern static void SetAge(IntPtr animal, int val);

        [DllImport(DllPath, EntryPoint = "GetAnimalsNum")]
        internal extern static int GetAnimalsNum();

        [DllImport(DllPath, EntryPoint = "Dog_Create")]
        internal extern static IntPtr Dog_Create(string name, int age);

        [DllImport(DllPath, EntryPoint = "Bird_Create")]
        internal extern static IntPtr Bird_Create(string name, int age);

        [DllImport(DllPath, EntryPoint = "Sheep_Create")]
        internal extern static IntPtr Sheep_Create(string name, int age);
}

    internal static class NativeStr
    {
        /// <summary>
        /// Převádí char* na C# string
        /// </summary>
        internal static string PtrToStr(IntPtr /* = char* */ i)
            => Marshal.PtrToStringAnsi(i);
    }
}
