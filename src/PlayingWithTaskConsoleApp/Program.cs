using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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

            var taskOne = Task.Run(() => GetUserWithDelay(NameTwo)).ContinueWith(DisplayResult);
            tasks.Add(taskOne);
            
            var taskTwo = Task.Run(() => GetUser(NameOne)).ContinueWith(DisplayResult);
            tasks.Add(taskTwo);

            var taskthree = Task.Run(() => GetUserWithDelay("Strange name")).ContinueWith(DisplayResult);
            tasks.Add(taskthree);

            var cancellationTokenSource = new CancellationTokenSource();
            var taskFour = Task.Run(() => GetUserWithDelay("To cancel"), cancellationTokenSource.Token).ContinueWith(DisplayResult, cancellationTokenSource.Token);
            tasks.Add(taskFour);
            cancellationTokenSource.Cancel();

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
                // Display information about each exception.  
                foreach (var v in e.InnerExceptions)
                {
                    if (v is TaskCanceledException) {
                        Console.WriteLine("   TaskCanceledException: Task {0}", ((TaskCanceledException) v).Task.Id);
                    }
                    else {
                        Console.WriteLine("   Exception: {0}", v.GetType().Name);
                    }
                }
                Console.WriteLine();
            } 
        }

        private static void DisplayResult(Task<User> task) {
            Console.WriteLine("==================");

            Console.WriteLine("Task ID: " + task.Id);
            Console.WriteLine("Status: " + task.Status);
            Console.WriteLine("Is Faulted: " + task.IsFaulted);

            if (task.IsFaulted && task.Exception != null) {
                Console.WriteLine(task.Exception.Message);
            }
            else {
                Console.WriteLine(task.Result);
            }

            Console.WriteLine("==================");
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
