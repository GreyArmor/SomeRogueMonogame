using System;

namespace NamelessRogue.Engine.Generation
{
    public class EasternSlavicGenerator : ICharacterNameGenerator
    {

        string[] maleFirstNames;
        string[] femaleFirstNames;
        string[] lastNames;
        public EasternSlavicGenerator()
        {
            string[] maleFirstNames = new string[]
            {
                "Aleksandr", "Dmitry", "Ivan", "Sergey", "Nikolai",
                "Mikhail", "Viktor", "Andrei", "Pavel", "Yuri",
                "Vladimir", "Oleg", "Roman", "Anatoly", "Maxim",
                "Kirill", "Boris", "Artem", "Stepan", "Leonid",
                "Gennady", "Vyacheslav", "Denis", "Grigory", "Igor",
                "Konstantin", "Evgeny", "Semyon", "Arkady", "Timofey",
                "Fedor", "Yaroslav", "German", "Stanislav", "Valentin",
                "Alexei", "Platon", "Rodion", "Matvei", "Makar",
                "Vsevolod", "Bogdan", "Zakhar", "Daniil", "Savely",
                "Ilya", "Anton", "Artur", "Vadim", "Eduard",
                "Nazar", "Vladislav", "Prokhor", "Filipp", "Lev",
                "Miron", "Yegor", "Ruslan", "Tikhon", "Sviatoslav"
            };

            string[] femaleFirstNames = new string[]
            {
                "Anna", "Olga", "Elena", "Maria", "Natalia",
                "Irina", "Svetlana", "Tatiana", "Ekaterina", "Marina",
                "Galina", "Valentina", "Ludmila", "Nadezhda", "Polina",
                "Vera", "Lyubov", "Anastasia", "Yulia", "Oksana",
                "Daria", "Alina", "Larisa", "Alla", "Kristina",
                "Ksenia", "Zoya", "Inna", "Alyona", "Evgenia",
                "Tamara", "Varvara", "Vasilisa", "Sofiya", "Ulyana",
                "Milana", "Margarita", "Violetta", "Karina", "Lilia",
                "Elizaveta", "Angelina", "Agnia", "Yelena", "Snezhana",
                "Viktoria", "Yana", "Lada", "Raisa", "Miroslava",
                "Svetlana", "Yelizaveta", "Nina", "Taisiya", "Arina",
                "Kseniya", "Vasilina", "Vladislava", "Olesya", "Zlata"
            };

            string[] lastNames = new string[]
            {
                "Ivanov", "Petrov", "Sidorov", "Smirnov", "Kuznetsov",
                "Popov", "Volkov", "Morozov", "Novikov", "Fedorov",
                "Mikhailov", "Borisov", "Yakovlev", "Grigoryev", "Orlov",
                "Nikiforov", "Panov", "Vasiliev", "Romanov", "Gusev",
                "Pavlov", "Antonov", "Karpov", "Sorokin", "Baranov",
                "Semyonov", "Stepanov", "Zaitsev", "Voronov", "Alexeyev",
                "Belov", "Kolesnikov", "Lebedev", "Vinogradov", "Zhukov",
                "Bogdanov", "Gavrilov", "Danilov", "Andreev", "Filippov",
                "Chernov", "Egorov", "Denisov", "Kravtsov", "Maltsev",
                "Kudryavtsev", "Prokhorov", "Polyakov", "Vladimirov", "Korolev",
                "Savelyev", "Yermakov", "Ignatov", "Belousov", "Markov",
                "Kostin", "Gorbachev", "Shirokov", "Troitsky", "Babushkin",
                "Losev", "Samoylov", "Rogov", "Arsenyev", "Yushkov",
                "Konovalov", "Platonov", "Chesnokov", "Golubev", "Nikonov",
                "Pankratov", "Melnikov", "Safonov", "Timofeev", "Dorofeev"
            };
        }
        public string GenerateName(bool isMale)
        {
            Random rnd = new Random();
            if (isMale)
            {
                string name = maleFirstNames[rnd.Next(maleFirstNames.Length)];
                string surname = lastNames[rnd.Next(lastNames.Length)];
                return name + " " + surname;
            }
            else
            {
                string name = femaleFirstNames[rnd.Next(femaleFirstNames.Length)];
                string surname = MakeFemaleSurname(lastNames[rnd.Next(lastNames.Length)]);
                return name + " " + surname;
            }
        }

        string MakeFemaleSurname(string baseSurname)
        {
            if (baseSurname.EndsWith("ov") || baseSurname.EndsWith("ev"))
                return baseSurname + "a";
            if (baseSurname.EndsWith("in"))
                return baseSurname + "a";
            if (baseSurname.EndsWith("sky") || baseSurname.EndsWith("skiy"))
                return baseSurname.Replace("sky", "skaya").Replace("skiy", "skaya");
            if (baseSurname.EndsWith("oy"))
                return baseSurname.Substring(0, baseSurname.Length - 2) + "aya";
            return baseSurname; // Some surnames don't change
        }
    }
}
