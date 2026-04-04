using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class GenIco {
    [STAThread]
    public static void Main() {
        string svg = "M840.838602 147.210649H182.479867c-48.899834 0-88.428619 41.232612-88.428619 92.177038v395.799002c0 50.774043 39.528785 92.177038 88.428619 92.177038h291.865557v84.339434h-162.715474c-11.415641 0-20.616306 12.949085-20.616306 29.135441v12.949085c0 16.015973 9.200666 29.135441 20.616306 29.135441H707.9401c11.415641 0 20.616306-12.949085 20.616306-29.135441v-12.949085c0-16.015973-9.200666-29.135441-20.616306-29.135441h-157.603994V727.53411h290.672879c48.899834 0 88.428619-41.232612 88.428619-92.177038V239.387687c-0.340765-50.774043-40.039933-92.177038-88.599002-92.177038zM645.409651 587.649917c-7.326456 7.156073-19.082862 7.156073-26.750084-0.170383L432.260899 405.340433v125.231281c0 10.052579-8.689517 18.230948-18.742097 18.230948-10.393344 0-18.742097-8.519135-18.742097-18.230948v-161.352413c0-1.363062 0.170383-2.726123 0.511149-3.918802-0.340765-1.192679-0.511148-2.55574-0.511149-3.918802 0-10.052579 8.689517-18.230948 18.742097-18.230948h165.441597c10.393344 0 18.742097 8.519135 18.742097 18.230948 0 10.052579-8.689517 18.230948-18.742097 18.230949h-120.290183l186.569052 182.139101c7.496839 7.156073 7.326456 18.912479 0.170383 25.89817z";
        
        DrawingVisual dv = new DrawingVisual();
        using (DrawingContext dc = dv.RenderOpen()) {
            
            // Draw the monitor+arrow icon on top
            GeometryDrawing gd = new GeometryDrawing();
            gd.Geometry = Geometry.Parse(svg);
            gd.Brush = new SolidColorBrush(Color.FromRgb(2, 248, 233));
            dc.DrawDrawing(gd);
        }

        // Render to 1024x1024 base image
        RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(dv);

        // write app_icon.png (full 1024x1024 for embedded resource)
        using (FileStream fs = File.Create("app_icon.png")) {
            PngBitmapEncoder enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(rtb));
            enc.Save(fs);
        }

        // write app.ico with multiple sizes for proper Windows display
        using (FileStream fs = File.Create("app.ico")) {
            using (BinaryWriter bw = new BinaryWriter(fs)) {
                int[] sizes = new int[] { 256, 128, 64, 48, 32, 24, 16 };
                bw.Write((ushort)0); // reserved
                bw.Write((ushort)1); // type = icon
                bw.Write((ushort)sizes.Length); // count

                int currentOffset = 6 + 16 * sizes.Length;
                System.Collections.Generic.List<byte[]> images = new System.Collections.Generic.List<byte[]>();

                for (int i = 0; i < sizes.Length; i++) {
                    int s = sizes[i];
                    var sT = new ScaleTransform(s / 1024.0, s / 1024.0);
                    var sImg = new TransformedBitmap(rtb, sT);
                    
                    using (MemoryStream pngMs = new MemoryStream()) {
                        PngBitmapEncoder enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(sImg));
                        enc.Save(pngMs);
                        byte[] pngBytes = pngMs.ToArray();
                        images.Add(pngBytes);

                        bw.Write((byte)(s == 256 ? 0 : s)); // width
                        bw.Write((byte)(s == 256 ? 0 : s)); // height
                        bw.Write((byte)0); // colors
                        bw.Write((byte)0); // reserved
                        bw.Write((ushort)1); // planes
                        bw.Write((ushort)32); // bpp
                        bw.Write((uint)pngBytes.Length);
                        bw.Write((uint)currentOffset); // offset
                        currentOffset += pngBytes.Length;
                    }
                }
                
                foreach (byte[] pngBytes in images) {
                    bw.Write(pngBytes);
                }
            }
        }
        Console.WriteLine("Done generating icon files.");
    }
}
