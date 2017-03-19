using System.Threading.Tasks;
using System.Windows.Forms;


namespace BoggleClient
{
    /// <summary>
    /// Keeps track of how many top-level forms are running, shuts down
    /// the application when there are no more.
    /// </summary>
    public class BoggleApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int windowCount = 0;

        // Singleton ApplicationContext
        private static BoggleApplicationContext context;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private BoggleApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static BoggleApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new BoggleApplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        public void RunNew()
        {
            // Create the window and the controller
            Boggle window = new Boggle();

            new Controller(window);

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }
    }
}
