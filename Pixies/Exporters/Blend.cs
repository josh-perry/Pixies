using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Pixies.Exporters
{
    public class Blend : IExporter
    {
        public const float CubeSize = 0.1f;
        public const int MinAlpha = 1;

        public bool Export(Project project)
        {
            float x, y, z = 0;

            var generatedPython = new StringBuilder();

            generatedPython.Append("import bpy\n");

            foreach (var layer in project.Layers)
            {
                var img = new Bitmap(Path.Combine(project.FullPath, layer.Filename));

                for (var i = 0; i < img.Width; i++)
                {
                    x = i * CubeSize;

                    for (var j = 0; j < img.Height; j++)
                    {
                        y = j * CubeSize;

                        var pixel = img.GetPixel(i, j);

                        if (pixel.A < MinAlpha)
                            continue;

                        var materialName = pixel.R.ToString() + pixel.G.ToString() + pixel.B.ToString() + pixel.A.ToString();

                        generatedPython.Append($"mat = bpy.data.materials.new(\"Mat_{materialName}\")\n");
                        generatedPython.Append($"mat.diffuse_color = ({pixel.R}, {pixel.G}, {pixel.B})\n");
                        generatedPython.Append($"mat.diffuse_shader = \'TOON\'\n");
                        generatedPython.Append($"c = bpy.ops.mesh.primitive_cube_add(location = ({x}, {y}, {z}), radius=({CubeSize / 2}))\n");
                        generatedPython.Append($"bpy.context.object.data.materials.append(mat)\n");
                        generatedPython.Append("####\n");
                    }
                }

                z += CubeSize;
            }
            
            generatedPython.Append("for ob in bpy.context.scene.objects:\n");
            generatedPython.Append("    if ob.type == 'MESH':\n");
            generatedPython.Append("        ob.select = True\n");
            generatedPython.Append("        bpy.context.scene.objects.active = ob\n");
            generatedPython.Append("    else:\n");
            generatedPython.Append("        ob.select = False\n");
            generatedPython.AppendLine();
            generatedPython.Append("bpy.ops.object.join()\n");
            generatedPython.Append($"bpy.ops.wm.save_as_mainfile(filepath=\"output.blend\")");

            var pythonFile = WritePythonToFile(generatedPython.ToString());
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
            
            //var stderr = process.StandardError.ReadToEnd();
            //var stdout = process.StandardOutput.ReadToEnd();
        }
    }
}
