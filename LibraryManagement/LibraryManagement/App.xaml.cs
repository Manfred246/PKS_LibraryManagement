using System.Windows;

namespace LibraryManagement
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var context = new Data.LibraryContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}