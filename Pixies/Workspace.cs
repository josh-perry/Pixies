namespace Pixies
{
    public class Workspace
    {
        /// <summary>
        /// Currently opened project.
        /// </summary>
        public Project Project;

        public Workspace(bool newWorkspace = true)
        {
            if (newWorkspace)
            {
                Project = new Project();
            }
            else
            {
                // Load an existing one.
            }
        }
    }
}
