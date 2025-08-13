using System;

namespace NamelessRogue.Engine.Generation
{
    public class EnglishNameGenerator : ICharacterNameGenerator
    {
        string[] maleFirstNames;
        string[] femaleFirstNames;
        string[] lastNames;
        public EnglishNameGenerator()
        {
            maleFirstNames = new string[]
            {
                "James", "John", "Robert", "Michael", "William",
                "David", "Richard", "Charles", "Joseph", "Thomas",
                "Daniel", "Matthew", "Anthony", "Mark", "Donald",
                "Steven", "Paul", "Andrew", "Joshua", "Kevin",
                "Brian", "George", "Edward", "Ronald", "Timothy",
                "Jason", "Jeffrey", "Ryan", "Jacob", "Gary",
                "Nicholas", "Eric", "Jonathan", "Stephen", "Larry",
                "Justin", "Brandon", "Scott", "Benjamin", "Adam",
                "Patrick", "Gregory", "Samuel", "Frank", "Alexander",
                "Raymond", "Jack", "Dennis", "Jerry", "Tyler",
                "Aaron", "Jose", "Henry", "Douglas", "Peter",
                "Kyle", "Walter", "Ethan", "Arthur", "Carl"
            };

            femaleFirstNames = new string[]
             {
                "Emily", "Sarah", "Jessica", "Ashley", "Amanda",
                "Jennifer", "Elizabeth", "Hannah", "Samantha", "Olivia",
                "Lauren", "Megan", "Rachel", "Brittany", "Nicole",
                "Grace", "Victoria", "Emma", "Abigail", "Madison",
                "Chloe", "Isabella", "Sophia", "Ava", "Natalie",
                "Kayla", "Allison", "Anna", "Amber", "Danielle",
                "Courtney", "Katherine", "Julia", "Haley", "Taylor",
                "Brooke", "Alexis", "Morgan", "Sydney", "Savannah",
                "Bailey", "Kaitlyn", "Hailey", "Jasmine", "Paige",
                "Brianna", "Erin", "Molly", "Faith", "Lillian",
                "Stella", "Audrey", "Lucy", "Caroline", "Peyton",
                "Zoe", "Brooklyn", "Claire", "Scarlett", "Evelyn"
             };

            lastNames = new string[]
            {
                "Smith", "Johnson", "Williams", "Brown", "Jones",
                "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris",
                "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
                "Walker", "Young", "Allen", "King", "Wright",
                "Scott", "Torres", "Nguyen", "Hill", "Flores",
                "Green", "Adams", "Nelson", "Baker", "Hall",
                "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
                "Gomez", "Phillips", "Evans", "Turner", "Diaz",
                "Parker", "Cruz", "Edwards", "Collins", "Reyes",
                "Stewart", "Morris", "Morales", "Murphy", "Cook",
                "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
                "Peterson", "Bailey", "Reed", "Kelly", "Howard"
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
                string surname = lastNames[rnd.Next(lastNames.Length)];
                return name + " " + surname;
            }
        }
    }
}
