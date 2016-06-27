using System.Collections.Generic;

namespace Pixies
{
    interface IExporter
    {
        bool Export(Project project, string path);
    }
}
