namespace p2pchat
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        { 
            ApplicationConfiguration.Initialize();
            Application.Run(new ChooseMode());
        }
    }
}