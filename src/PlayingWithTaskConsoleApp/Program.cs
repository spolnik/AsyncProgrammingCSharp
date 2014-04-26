using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayingWithTaskConsoleApp
{
    class Program {

        private static readonly IDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

        private const string NameOne = "Mikolaj";
        private const string NameTwo = "Julia";

        static Program()
        {
            Users.Add(NameOne, new User(NameOne, 1) );
            Users.Add(NameTwo, new User(NameTwo, 4) );
        }

        static void Main()
        {
            var tasks = new List<Task>();

            var taskOne = Task.Run(() => GetUserWithDelay(NameTwo)).ContinueWith(x => Console.WriteLine(x.Result));
            tasks.Add(taskOne);
            var taskTwo = Task.Run(() => GetUser(NameOne)).ContinueWith(x => Console.WriteLine(x.Result));
            tasks.Add(taskTwo);

            Task.WaitAll(tasks.ToArray());
        }

        private static User GetUser(string name) {
            return Users[name];
        }

        private static User GetUserWithDelay(string name) {
            Task.Delay(TimeSpan.FromSeconds(3)).Wait();
            return Users[name];
        }
    }

    class User {

        public User() {}

        public User(string name, int age) {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString() {
            return string.Format("Name: {0}, Age: {1}", Name, Age);
        }
    }
}
