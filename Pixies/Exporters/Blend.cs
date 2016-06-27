using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Pixies.Exporters
{
    public class Blend : IExporter
    {
        public const int CubeSize = 2;
        public const int MinAlpha = 1;

        public bool Export(Project project)
        {
            int x, y, z = 0;
            var generatedPython = String.Empty;

            generatedPython += "import bpy\n";

            var materials = new HashSet<string>();

            foreach(var layer in project.Layers)
            {
                var img = new Bitmap(Path.Combine(project.FullPath, layer.Filename));

                for (int i = 0; i < img.Width; i++)
                {
                    x = i * CubeSize;

                    for (int j = 0; j < img.Height; j++)
                    {
                        y = j * CubeSize;

                        var pixel = img.GetPixel(i, j);

                        if (pixel.A < MinAlpha)
                            continue;

                        var materialName = pixel.R.ToString() + pixel.G.ToString() + pixel.B.ToString() + pixel.A.ToString();

                        generatedPython += $"mat = bpy.data.materials.new(\"Mat_{materialName}\")\n";
                        generatedPython += $"mat.diffuse_color = (float(255 * {pixel.R}), float(255 * {pixel.G}), float(255 * {pixel.B}))\n";
                        generatedPython += $"c = bpy.ops.mesh.primitive_cube_add(location = ({x}, {y}, {z}), radius=({CubeSize / 2}))\n";
                        generatedPython += $"bpy.context.object.data.materials.append(mat)\n";
                        generatedPython += "####\n";
                    }
                }

                z += CubeSize;
            }

            generatedPython += $"bpy.ops.wm.save_as_mainfile(filepath=\"output.blend\")";

            var pythonFile = WritePythonToFile(generatedPython);
            RunBlenderJob(pythonFile);

            return true;
        }

        private string WritePythonToFile(string generatedPython)
        {
            var path = Path.GetTempFileName() + ".py";

            File.WriteAllText(path, generatedPython);

            return path;
        }

        private void RunBlenderJob(string pythonFile)
        {
            File.Delete("output.blend");

            File.Copy("empty.blend", "output.blend");

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "blender.exe";
            startInfo.Arguments = $"output.blend --background --python {pythonFile}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            var process = Process.Start(startInfo);
            var stderr = process.StandardError.ReadToEnd();
            var stdout = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
        }
    }
}
