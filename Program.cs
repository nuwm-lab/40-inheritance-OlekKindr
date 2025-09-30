using System;

namespace LabWork
{
    public readonly struct Point
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}, {Y})";
    }

    public class Triangle
    {
        protected Point[] vertices;

        public Triangle()
        {
            vertices = new Point[3];
        }

        public virtual void SetCoordinates(Point p1, Point p2, Point p3)
        {
            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;
        }

        public virtual void DisplayCoordinates()
        {
            Console.WriteLine("Triangle coordinates:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Point {i + 1}: {vertices[i]}");
            }
        }

        protected double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public virtual double CalculateArea()
        {
            double a = CalculateDistance(vertices[0], vertices[1]);
            double b = CalculateDistance(vertices[1], vertices[2]);
            double c = CalculateDistance(vertices[2], vertices[0]);
            double s = (a + b + c) / 2; // semi-perimeter
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c)); // Heron's formula
        }
    }

    public class Tetrahedron : Triangle
    {
        private Point fourthVertex;
        private double height;

        public Tetrahedron() : base()
        {
            fourthVertex = new Point();
        }

        public void SetCoordinates(Point p1, Point p2, Point p3, Point p4, double height)
        {
            base.SetCoordinates(p1, p2, p3);
            fourthVertex = p4;
            this.height = height;
        }

        public override void DisplayCoordinates()
        {
            Console.WriteLine("Tetrahedron coordinates:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Point {i + 1}: {vertices[i]}");
            }
            Console.WriteLine($"Point 4: {fourthVertex}");
            Console.WriteLine($"Height: {height}");
        }

        public double CalculateVolume()
        {
            // Volume of a tetrahedron = (1/3) * base area * height
            return (1.0 / 3.0) * base.CalculateArea() * height;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create and test Triangle
            Triangle triangle = new Triangle();
            triangle.SetCoordinates(
                new Point(0, 0),
                new Point(3, 0),
                new Point(0, 4)
            );
            
            triangle.DisplayCoordinates();
            double area = triangle.CalculateArea();
            Console.WriteLine($"Triangle area: {area:F2} square units\n");

            // Create and test Tetrahedron
            Tetrahedron tetrahedron = new Tetrahedron();
            tetrahedron.SetCoordinates(
                new Point(0, 0),
                new Point(3, 0),
                new Point(0, 4),
                new Point(1, 1),
                5.0 // height
            );
            
            tetrahedron.DisplayCoordinates();
            double volume = tetrahedron.CalculateVolume();
            Console.WriteLine($"Tetrahedron volume: {volume:F2} cubic units");
        }
    }
}
