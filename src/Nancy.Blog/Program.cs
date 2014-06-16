namespace Nancy.Blog
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Server running at http://localhost:1234 ...");
                Console.ReadLine();
            }
        }
    }
}
