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

        public bool Export(Project project, string path)
        {
            float x, y, z = 0;

            var generatedPython = new StringBuilder();

            generatedPython.AppendLine("import bpy");

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

                        generatedPython.AppendLine($"mat = bpy.data.materials.new(\"Mat_{materialName}\")");
                        generatedPython.AppendLine($"mat.diffuse_color = ({pixel.R}, {pixel.G}, {pixel.B})");
                        generatedPython.AppendLine($"mat.diffuse_shader = \'TOON\'");
                        generatedPython.AppendLine($"c = bpy.ops.mesh.primitive_cube_add(location = ({x}, {y}, {z}), radius=({CubeSize / 2}))");
                        generatedPython.AppendLine("bpy.ops.object.mode_set(mode=\'EDIT\')");
                        generatedPython.AppendLine($"bpy.context.object.data.materials.append(mat)");
                        generatedPython.AppendLine("####");
                    }
                }

                z += CubeSize;
            }

            generatedPython.AppendLine("bpy.ops.object.mode_set(mode=\'OBJECT\')");

            generatedPython.AppendLine("for ob in bpy.context.scene.objects:");
            generatedPython.AppendLine("    if ob.type == 'MESH':");
            generatedPython.AppendLine("        ob.select = True");
            generatedPython.AppendLine("        bpy.context.scene.objects.active = ob");
            generatedPython.AppendLine("    else:");
            generatedPython.AppendLine("        ob.select = False");
            generatedPython.AppendLine();
            generatedPython.AppendLine("bpy.ops.object.join()");
            generatedPython.AppendLine();
            generatedPython.AppendLine($"bpy.ops.wm.save_as_mainfile(filepath=\"output.blend\")");

            var pythonFile = WritePythonToFile(generatedPython.ToString());
            RunBlenderJob(pythonFile, path);

            return true;
        }

        private string WritePythonToFile(string generatedPython)
        {
            var path = Path.GetTempFileName() + ".py";

            File.WriteAllText(path, generatedPython);

            return path;
        }

        private void RunBlenderJob(string pythonFile, string path)
        {
            File.Delete(path);
            File.Copy("empty.blend", path);

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "blender.exe";
            startInfo.Arguments = $"{path} --background --python {pythonFile}";
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
