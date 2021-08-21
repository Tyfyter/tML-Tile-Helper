using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace Tyfyter.tML_Tile_Helper {
    public class TML_Tile_Helper {
        static Color transparency = Color.Magenta;
        static bool noDirtMerge = false;
        public static void Main(string[] args) {
            int mode = -1;
            string filename1 = "";
            string filename2 = "";
            string filename3 = "";
            string outputFile = null;
            foreach (string arg in args) {
                if (mode == -1) {
                    switch (arg.Replace("-", "")) {
                        case "helk":
                        case "help":
                        Console.WriteLine("help menu coming soon");
                        return;
                        case "all":
                        mode = 0;
                        break;
                        case "part1":
                        mode = 1;
                        break;
                        case "part2":
                        mode = 2;
                        break;
                        case "part3":
                        mode = 3;
                        break;
                        case "output":
                        mode = 4;
                        break;
                        case "transparency":
                        mode = 5;
                        break;
                        case "noDirtMerge":
                        noDirtMerge = true;
                        break;
                    }
                } else {
                    switch (mode) {
                        case 0:
                        filename1 = arg;
                        filename2 = arg;
                        filename3 = arg;
                        break;
                        case 1:
                        filename1 = arg;
                        break;
                        case 2:
                        filename2 = arg;
                        break;
                        case 3:
                        filename3 = arg;
                        break;
                        case 4:
                        outputFile = arg;
                        break;
                        case 5:
                        transparency = ColorTranslator.FromHtml(arg.StartsWith('#')? arg : ("#" + arg));
                        break;
                    }
                    mode = -1;
                }
            }
            ProcessImages(filename1, filename2, filename3, outputFile??(Regex.Replace(filename1, "\\.[^.]*$", "") +"_Sheet.png"));
        }
        public delegate void DrawAt(int x, int y);
        public static void ProcessImages(string file1, string file2, string file3, string outputName) {
            ImageSet[] images = {new ImageSet(file1),new ImageSet(file2),new ImageSet(file3)};
            int width = 288;
            int height = noDirtMerge ? 90 : 270;
            Bitmap output = new Bitmap(width, height);
            int i = 0;
            using (Graphics g = Graphics.FromImage(output)) {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                #region definitions
                DrawAt Skip = (int x, int y) => { };
                DrawAt None = (int x, int y) => { };
                DrawAt NormalBorder = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Border,
                            new PointF[]{
                                new PointF(x, y),
                                new PointF(x+16, y),
                                new PointF(x, y+16)
                            },
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt TopEdge = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Edge,
                            new PointF[]{
                                new PointF(x, y),
                                new PointF(x+16, y),
                                new PointF(x, y+16)
                            },
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt LeftEdge = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Edge,
                            new PointF[]{
                                new PointF(x, y+16),
                                new PointF(x, y),
                                new PointF(x+16, y+16)
                            },
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt RightEdge = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Edge,
                            new PointF[]{
                                new PointF(x+16, y),
                                new PointF(x+16, y+16),
                                new PointF(x, y)
                            },
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt BottomEdge = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Edge,
                            new PointF[]{
                                new PointF(x+16, y+16),
                                new PointF(x, y+16),
                                new PointF(x+16, y)
                            },
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt BorderTop = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Border,
                            new PointF[]{
                                new PointF(x, y),
                                new PointF(x+16, y),
                                new PointF(x, y+4)
                            },
                            new Rectangle(0, 0, 8, 2),
                            GraphicsUnit.Pixel);
                };
                DrawAt BorderLeft = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Border,
                            new PointF[]{
                                new PointF(x, y),
                                new PointF(x+4, y),
                                new PointF(x, y+16)
                            },
                            new Rectangle(0, 0, 2, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt BorderRight = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Border,
                            new PointF[]{
                                new PointF(x+12, y),
                                new PointF(x+16, y),
                                new PointF(x+12, y+16)
                            },
                            new Rectangle(6, 0, 2, 8),
                            GraphicsUnit.Pixel);
                };
                DrawAt BorderBottom = (int x, int y) => {
                    g.DrawImage(
                        images[i % images.Length].Border,
                            new PointF[]{
                                new PointF(x, y+12),
                                new PointF(x+16, y+12),
                                new PointF(x, y+16)
                            },
                            new Rectangle(0, 6, 8, 2),
                            GraphicsUnit.Pixel);
                };
                DrawAt[] actions = new DrawAt[]{
                    //Top Row
                    LeftEdge,
                    TopEdge,
                    TopEdge,
                    TopEdge,
                    RightEdge,
                    LeftEdge+RightEdge,
                    LeftEdge+RightEdge+BorderTop,
                    LeftEdge+RightEdge+BorderTop,
                    LeftEdge+RightEdge+BorderTop,
                    TopEdge+BottomEdge+BorderLeft,
                    None,
                    None,
                    TopEdge+BottomEdge+BorderRight,
                    TopEdge,
                    TopEdge,
                    TopEdge,

                    //Second Row
                    LeftEdge,
                    None,
                    None,
                    None,
                    RightEdge,
                    LeftEdge+RightEdge,
                    None,
                    None,
                    None,
                    TopEdge+BottomEdge+BorderLeft,
                    None,
                    None,
                    TopEdge+BottomEdge+BorderRight,
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,

                    //Third Row
                    LeftEdge,
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,
                    RightEdge,
                    LeftEdge+RightEdge,
                    None,
                    None,
                    None,
                    TopEdge+BottomEdge+BorderLeft,
                    None,
                    None,
                    TopEdge+BottomEdge+BorderRight,
                    LeftEdge,
                    LeftEdge,
                    LeftEdge,

                    //Fourth Row
                    LeftEdge+TopEdge,
                    RightEdge+TopEdge,
                    LeftEdge+TopEdge,
                    RightEdge+TopEdge,
                    LeftEdge+TopEdge,
                    RightEdge+TopEdge,
                    LeftEdge+RightEdge+BorderBottom,
                    LeftEdge+RightEdge+BorderBottom,
                    LeftEdge+RightEdge+BorderBottom,
                    NormalBorder,
                    NormalBorder,
                    NormalBorder,
                    Skip,
                    RightEdge,
                    RightEdge,
                    RightEdge,

                    //Fifth Row
                    LeftEdge+BottomEdge,
                    RightEdge+BottomEdge,
                    LeftEdge+BottomEdge,
                    RightEdge+BottomEdge,
                    LeftEdge+BottomEdge,
                    RightEdge+BottomEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    Skip, Skip, Skip, Skip, Skip, Skip, Skip,

                    //Sixth Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderTop,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Seventh Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderTop,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Eigth Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderTop,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Ninth Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderBottom,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Tenth Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderBottom,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Eleventh Row
                    None, None, None, None,
                    LeftEdge,
                    RightEdge,
                    LeftEdge+RightEdge+BorderBottom,
                    LeftEdge+RightEdge,
                    None, None, None, None, None,
                    Skip, Skip, Skip,

                    //Twelfth Row
                    TopEdge,
                    TopEdge,
                    TopEdge,
                    TopEdge,
                    TopEdge,
                    TopEdge,
                    None, None, None,
                    TopEdge+BottomEdge,
                    TopEdge+BottomEdge,
                    TopEdge+BottomEdge,
                    Skip, Skip, Skip, Skip,

                    //Thirteenth Row
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,
                    BottomEdge,
                    LeftEdge+RightEdge,
                    Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip,

                    //Fourteenth Row
                    BottomEdge+TopEdge+BorderRight,
                    BottomEdge+TopEdge+BorderRight,
                    BottomEdge+TopEdge+BorderRight,
                    BottomEdge+TopEdge+BorderLeft,
                    BottomEdge+TopEdge+BorderLeft,
                    BottomEdge+TopEdge+BorderLeft,
                    LeftEdge+RightEdge,
                    Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip,

                    //Fifteenth Row
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    BottomEdge+TopEdge,
                    LeftEdge+RightEdge,
                    Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip,
                };
                #endregion
                for (int y = 0; y < height; y += 18) {
                    for (int x = 0; x < width; x += 18) {
                        if (actions[i % actions.Length] == Skip) {
                            i++;
                            continue;
                        }
                        g.DrawImage(
                            images[i % images.Length].Base,
                            new PointF[]{
                                new PointF(x, y),
                                new PointF(x+16, y),
                                new PointF(x, y+16)
                            }, 
                            new Rectangle(0, 0, 8, 8),
                            GraphicsUnit.Pixel);
                        actions[i % actions.Length](x, y);
                        i++;
                    }
                }
            }

            output.MakeTransparent(transparency);

            output.Save(outputName);
        }
    }
    public struct ImageSet {
        readonly Bitmap baseImage;
        readonly Bitmap borderImage;
        readonly Bitmap edgeImage;
        public Bitmap Base => baseImage;
        public Bitmap Border => borderImage;
        public Bitmap Edge => edgeImage;
        public ImageSet(string baseName, string borderName = null, string edgeName = null) {
            try {
                baseImage = (Bitmap)Image.FromFile(baseName + ".png");
            } catch (Exception) {
                baseImage = new Bitmap(0,0);
            }
            try {
                borderImage = (Bitmap)Image.FromFile(borderName ?? (baseName + "_Border.png"));
            } catch (Exception) {
                borderImage = new Bitmap(0,0);
            }
            try {
                edgeImage = (Bitmap)Image.FromFile(edgeName??(baseName + "_Edge.png"));
            } catch (Exception) {
                edgeImage = new Bitmap(0,0);
            }
        }
    }
}