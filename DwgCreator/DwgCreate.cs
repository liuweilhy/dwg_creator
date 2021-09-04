using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace DwgCreator
{
    public class DwgCreate
    {
        public static double Degree2Radian(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        /// <summary>
        /// 生成矩形
        /// </summary>
        /// <param name="lengh">长</param>
        /// <param name="width">宽</param>
        /// <param name="round">圆角</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool CreateRactangle(double lengh, double width, double round, string path)
        {
            if (lengh <= 0 || width <= 0 || string.IsNullOrWhiteSpace(path))
                return false;
            using (Services svcs = new Services())
            {
                Database db = new Database();
                BlockTableRecord btr = (BlockTableRecord)db.CurrentSpaceId.Open(OpenMode.ForWrite);
                if (round > 0)
                {
                    Point3d p1 = new Point3d(-lengh / 2 + round, -width / 2, 0);
                    Point3d p2 = new Point3d(-lengh / 2, -width / 2 + round, 0);
                    Point3d p3 = new Point3d(-lengh / 2, width / 2 - round, 0);
                    Point3d p4 = new Point3d(-lengh / 2 + round, width / 2, 0);
                    Point3d p5 = new Point3d(lengh / 2 - round, width / 2, 0);
                    Point3d p6 = new Point3d(lengh / 2, width / 2 - round, 0);
                    Point3d p7 = new Point3d(lengh / 2, -width / 2 + round, 0);
                    Point3d p8 = new Point3d(lengh / 2 - round, -width / 2, 0);
                    Line l1 = new Line(p2, p3);
                    Line l2 = new Line(p4, p5);
                    Line l3 = new Line(p6, p7);
                    Line l4 = new Line(p8, p1);

                    Point3d p11 = new Point3d(-lengh / 2 + round, -width / 2 + round, 0);
                    Point3d p12 = new Point3d(-lengh / 2 + round, width / 2 - round, 0);
                    Point3d p13 = new Point3d(lengh / 2 - round, width / 2 - round, 0);
                    Point3d p14 = new Point3d(lengh / 2 - round, -width / 2 + round, 0);
                    Arc a1 = new Arc(p11, round, Degree2Radian(180.0), Degree2Radian(270.0));
                    Arc a2 = new Arc(p12, round, Degree2Radian(90.0), Degree2Radian(180.0));
                    Arc a3 = new Arc(p13, round, Degree2Radian(0.0), Degree2Radian(90.0));
                    Arc a4 = new Arc(p14, round, Degree2Radian(270.0), Degree2Radian(360.0));

                    btr.AppendEntity(l1);
                    btr.AppendEntity(l2);
                    btr.AppendEntity(l3);
                    btr.AppendEntity(l4);
                    btr.AppendEntity(a1);
                    btr.AppendEntity(a2);
                    btr.AppendEntity(a3);
                    btr.AppendEntity(a4);

                    l1.Dispose();
                    l2.Dispose();
                    l3.Dispose();
                    l4.Dispose();
                    a1.Dispose();
                    a2.Dispose();
                    a3.Dispose();
                    a4.Dispose();
                }
                else
                {
                    Point3d p1 = new Point3d(-lengh / 2, -width / 2, 0);
                    Point3d p2 = new Point3d(-lengh / 2, width / 2, 0);
                    Point3d p3 = new Point3d(lengh / 2, width / 2, 0);
                    Point3d p4 = new Point3d(lengh / 2, -width / 2, 0);
                    Line l1 = new Line(p1, p2);
                    Line l2 = new Line(p2, p3);
                    Line l3 = new Line(p3, p4);
                    Line l4 = new Line(p4, p1);
                    btr.AppendEntity(l1);
                    btr.AppendEntity(l2);
                    btr.AppendEntity(l3);
                    btr.AppendEntity(l4);
                    l1.Dispose();
                    l2.Dispose();
                    l3.Dispose();
                    l4.Dispose();
                }
                db.SaveAs(path, DwgVersion.Current);
                btr.Dispose();
            }
            return true;
        }

        /// <summary>
        /// 生成圆形
        /// </summary>
        /// <param name="diameter">直径</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool CreateCircle(double diameter, string path)
        {
            if (diameter <= 0 || string.IsNullOrWhiteSpace(path))
                return false;
            using (Services svcs = new Services())
            {
                Database db = new Database();
                Circle cirl = new Circle
                {
                    Center = new Point3d(0, 0, 0),
                    Radius = diameter
                };
                BlockTableRecord btr = (BlockTableRecord)db.CurrentSpaceId.Open(OpenMode.ForWrite);
                btr.AppendEntity(cirl);
                db.SaveAs(path, DwgVersion.Current);
                cirl.Dispose();
                btr.Dispose();
            }
            return true;
        }

        /// <summary>
        /// 生成圆环
        /// </summary>
        /// <param name="diameter1">外径</param>
        /// <param name="diameter2">内径</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool CreateRing(double diameter1, double diameter2, string path)
        {
            if (diameter1 <= 0 || diameter2 <= 0 || diameter1 == diameter2 || string.IsNullOrWhiteSpace(path))
                return false;
            using (Services svcs = new Services())
            {
                Database db = new Database();
                Circle cirl = new Circle
                {
                    Center = new Point3d(0, 0, 0),
                    Radius = diameter1
                };
                Circle cirl2 = new Circle
                {
                    Center = new Point3d(0, 0, 0),
                    Radius = diameter2
                };
                BlockTableRecord btr = (BlockTableRecord)db.CurrentSpaceId.Open(OpenMode.ForWrite);
                btr.AppendEntity(cirl);
                btr.AppendEntity(cirl2);
                db.SaveAs(path, DwgVersion.Current);
                cirl.Dispose();
                cirl2.Dispose();
                btr.Dispose();
            }
            return true;
        }


    }
}
