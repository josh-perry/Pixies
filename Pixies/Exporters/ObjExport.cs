using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ObjParser;
using ObjParser.Types;

namespace Pixies.Exporters
{
    internal class ObjExport : IExporter
    {
        public const float CubeSize = 0.1f;
        public const int MinAlpha = 1;

        public bool Export(Project project, string path)
        {
            var obj = new Obj();
            obj.VertexList = new List<Vertex>();
            obj.FaceList = new List<Face>();
            obj.TextureList = new List<TextureVertex>();

            float x, y, z = 0;

            foreach (var layer in project.Layers)
            {
                var img = new Bitmap(Path.Combine(project.FullPath, layer.Filename));

                for (var i = 0; i < img.Width; i++)
                {
                    x = i*CubeSize;

                    for (var j = 0; j < img.Height; j++)
                    {
                        y = j*CubeSize;

                        var pixel = img.GetPixel(i, j);

                        if (pixel.A < MinAlpha)
                            continue;
                        
                        // Add cube
                        CreateCube(ref obj, x, y, z, CubeSize);
                    }
                }

                z += CubeSize;
            }

            File.Delete(path);
            obj.WriteObjFile(path);

            return true;
        }

        public void CreateCube(ref Obj obj, float x, float y, float z, float width)
        {
            CreateCubeFace(ref obj, x, y, z, width);
        }

        public void CreateCubeFace(ref Obj obj, float x, float y, float z, float width)
        {
            var id = obj.VertexList.Count;
            
            var verts = new List<Vertex>
            {
                // Bottom
                new Vertex
                {
                    X = x,
                    Y = y,
                    Z = z,
                    Index = id + (int) IdOffset.Bottom1
                },
                new Vertex
                {
                    X = x + width,
                    Y = y,
                    Z = z,
                    Index = id + (int) IdOffset.Bottom2
                },
                new Vertex
                {
                    X = x + width,
                    Y = y + width,
                    Z = z,
                    Index = id + (int) IdOffset.Bottom3
                },
                new Vertex
                {
                    X = x,
                    Y = y + width,
                    Z = z,
                    Index = id + (int) IdOffset.Bottom4
                },

                // Top
                new Vertex
                {
                    X = x,
                    Y = y,
                    Z = z + width,
                    Index = id + (int) IdOffset.Top1
                },
                new Vertex
                {
                    X = x + width,
                    Y = y,
                    Z = z + width,
                    Index = id + (int) IdOffset.Top2
                },
                new Vertex
                {
                    X = x + width,
                    Y = y + width,
                    Z = z + width,
                    Index = id + (int) IdOffset.Top3
                },
                new Vertex
                {
                    X = x,
                    Y = y + width,
                    Z = z + width,
                    Index = id + (int) IdOffset.Top4
                }
            };

            foreach (var vert in verts)
            {
                obj.VertexList.Add(vert);
            }
            
            var bottomFace = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Bottom1,
                    id + (int) IdOffset.Bottom2,
                    id + (int) IdOffset.Bottom3,
                    id + (int) IdOffset.Bottom4,
                }
            };

            bottomFace.TextureVertexIndexList = new int[] {};
            obj.FaceList.Add(bottomFace);

            var topFace = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Top1,
                    id + (int) IdOffset.Top2,
                    id + (int) IdOffset.Top3,
                    id + (int) IdOffset.Top4,
                }
            };

            topFace.TextureVertexIndexList = new int[] { };
            obj.FaceList.Add(topFace);

            var sideFace1 = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Top1,
                    id + (int) IdOffset.Top2,
                    id + (int) IdOffset.Bottom2,
                    id + (int) IdOffset.Bottom1
                }
            };

            sideFace1.TextureVertexIndexList = new int[] { };
            obj.FaceList.Add(sideFace1);

            var sideFace2 = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Top3,
                    id + (int) IdOffset.Top4,
                    id + (int) IdOffset.Bottom4,
                    id + (int) IdOffset.Bottom3
                }
            };

            sideFace2.TextureVertexIndexList = new int[] { };
            obj.FaceList.Add(sideFace2);

            var sideFace3 = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Top1,
                    id + (int) IdOffset.Top4,
                    id + (int) IdOffset.Bottom4,
                    id + (int) IdOffset.Bottom1
                }
            };

            sideFace3.TextureVertexIndexList = new int[] { };
            obj.FaceList.Add(sideFace3);

            var sideFace4 = new Face
            {
                VertexIndexList = new[]
                {
                    id + (int) IdOffset.Top2,
                    id + (int) IdOffset.Top3,
                    id + (int) IdOffset.Bottom3,
                    id + (int) IdOffset.Bottom2
                }
            };

            sideFace4.TextureVertexIndexList = new int[] { };
            obj.FaceList.Add(sideFace4);
        }

        private enum IdOffset
        {
            Bottom1 = 1,
            Bottom2,
            Bottom3,
            Bottom4,
            //
            Top1,
            Top2,
            Top3,
            Top4
        }
    }
}
