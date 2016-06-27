using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pixies
{
    /// <summary>
    /// Main object containing all project info, layers etc.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Directory hosting the project.
        /// </summary>
        public string Path
        {
            get; set;
        }

        /// <summary>
        /// A list of Layers.
        /// </summary>
        public ObservableCollection<Layer> Layers
        {
            get; set;
        }

        public Project()
        {
            Layers = new ObservableCollection<Layer>();
        }
    }
}
