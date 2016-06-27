using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Pixies
{
    /// <summary>
    /// Main object containing all project info, layers etc.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Constructor. Initializes layer list. Creates temp directory.
        /// </summary>
        public Project()
        {
            Layers = new ObservableCollection<Layer>();

            FullPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(FullPath);
        }

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Directory hosting the project.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// A list of Layers.
        /// </summary>
        public ObservableCollection<Layer> Layers { get; set; }
    }
}
