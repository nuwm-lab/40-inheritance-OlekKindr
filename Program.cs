using System;
using System.Globalization;

namespace LabWork
{
    public static class MathUtils
    {
        public const double Epsilon = 1e-10;

        public static bool IsValidNumber(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }

    public readonly struct Vector3D
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Vector3D() : this(0, 0, 0) { }

        public Vector3D(double x, double y, double z)
        {
            if (!MathUtils.IsValidNumber(x) || !MathUtils.IsValidNumber(y) || !MathUtils.IsValidNumber(z))
            {
                throw new ArgumentException("Coordinates must be valid numbers (not NaN or Infinity)");
            }

            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
            => new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static double Dot(Vector3D a, Vector3D b)
            => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3D Cross(Vector3D a, Vector3D b)
            => new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

        public static double ScalarTripleProduct(Vector3D a, Vector3D b, Vector3D c)
            => Dot(a, Cross(b, c));

        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture, "({0:G}, {1:G}, {2:G})", X, Y, Z);
    }

    public class Triangle
    {
        private readonly Vector3D[] _vertices;
        private bool _isInitialized;

        public Triangle()
        {
            _vertices = new Vector3D[3];
            _isInitialized = false;
        }

        protected Vector3D[] GetVertices()
        {
            return (Vector3D[])_vertices.Clone();
        }

        public Vector3D GetVertex(int index)
        {
            if (index < 0 || index >= 3)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 2");
            }
            ValidateInitialization();
            return _vertices[index];
        }

        protected static bool IsValidTriangle(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            var v1 = p2 - p1;
            var v2 = p3 - p1;
            var cross = Vector3D.Cross(v1, v2);
            var area = Vector3D.Dot(cross, cross);
            return area > MathUtils.Epsilon;
        }

        public virtual void SetCoordinates(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            if (!IsValidTriangle(p1, p2, p3))
            {
                throw new ArgumentException("The given points do not form a valid triangle (they may be collinear)");
            }

            _vertices[0] = p1;
            _vertices[1] = p2;
            _vertices[2] = p3;
            _isInitialized = true;
        }

        public virtual void DisplayVertices()
        {
            ValidateInitialization();
            Console.WriteLine("Triangle vertices:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Vertex {i + 1}: {_vertices[i]}");
            }
        }

        protected void ValidateInitialization()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Triangle coordinates have not been initialized");
            }
        }

        public virtual double CalculateArea()
        {
            ValidateInitialization();
            var v1 = _vertices[1] - _vertices[0];
            var v2 = _vertices[2] - _vertices[0];
            var cross = Vector3D.Cross(v1, v2);
            return Math.Sqrt(Vector3D.Dot(cross, cross)) / 2.0;
        }
    }

    public class Tetrahedron : Triangle
    {
        private Vector3D _fourthVertex;
        private bool _tetrahedronInitialized;

        public Tetrahedron() : base()
        {
            _fourthVertex = new Vector3D();
            _tetrahedronInitialized = false;
        }

        public Vector3D ApexVertex
        {
            get
            {
                if (!_tetrahedronInitialized)
                {
                    throw new InvalidOperationException("Tetrahedron is not initialized");
                }
                return _fourthVertex;
            }
        }

        private static double CalculateSignedVolume(Vector3D p1, Vector3D p2, Vector3D p3, Vector3D p4)
        {
            var v1 = p2 - p1;
            var v2 = p3 - p1;
            var v3 = p4 - p1;
            return Vector3D.ScalarTripleProduct(v1, v2, v3) / 6.0;
        }

        public void SetCoordinates(Vector3D p1, Vector3D p2, Vector3D p3, Vector3D p4)
        {
            base.SetCoordinates(p1, p2, p3);
            var volume = CalculateSignedVolume(p1, p2, p3, p4);
            if (Math.Abs(volume) <= MathUtils.Epsilon)
            {
                throw new ArgumentException("The given points do not form a valid tetrahedron (they may be coplanar)");
            }
            _fourthVertex = p4;
            _tetrahedronInitialized = true;
        }

        public override void DisplayVertices()
        {
            if (!_tetrahedronInitialized)
            {
                throw new InvalidOperationException("Tetrahedron coordinates have not been initialized");
            }
            Console.WriteLine("Tetrahedron vertices:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Base vertex {i + 1}: {GetVertex(i)}");
            }
            Console.WriteLine($"Apex vertex: {_fourthVertex}");
        }

        public double CalculateVolume()
        {
            if (!_tetrahedronInitialized)
            {
                throw new InvalidOperationException("Cannot calculate volume: tetrahedron is not properly initialized");
            }
            return Math.Abs(CalculateSignedVolume(
                GetVertex(0),
                GetVertex(1),
                GetVertex(2),
                _fourthVertex
            ));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Testing Triangle class:");
                Console.WriteLine("----------------------");
                
                Triangle triangle = new Triangle();
                
                try
                {
                    triangle.CalculateArea();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}");
                }

                triangle.SetCoordinates(
                    new Vector3D(0, 0, 0),
                    new Vector3D(3, 0, 0),
                    new Vector3D(0, 4, 0)
                );
                
                triangle.DisplayVertices();
                double area = triangle.CalculateArea();
                Console.WriteLine($"Triangle area: {area:F2} square units");
                Console.WriteLine($"Expected area of 3-4-5 triangle: 6.00 square units\n");

                try
                {
                    Triangle invalidTriangle = new Triangle();
                    invalidTriangle.SetCoordinates(
                        new Vector3D(0, 0, 0),
                        new Vector3D(1, 1, 1),
                        new Vector3D(2, 2, 2)
                    );
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}\n");
                }

                Console.WriteLine("Testing Tetrahedron class:");
                Console.WriteLine("-------------------------");
                
                Tetrahedron tetrahedron = new Tetrahedron();
                
                try
                {
                    tetrahedron.SetCoordinates(
                        new Vector3D(0, 0, 0),
                        new Vector3D(1, 0, 0),
                        new Vector3D(0, 1, 0),
                        new Vector3D(0.5, 0.5, 0)
                    );
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}");
                }

                tetrahedron.SetCoordinates(
                    new Vector3D(0, 0, 0),
                    new Vector3D(1, 0, 0),
                    new Vector3D(0.5, Math.Sqrt(0.75), 0),
                    new Vector3D(0.5, Math.Sqrt(0.75)/3, Math.Sqrt(2.0/3.0))
                );
                
                tetrahedron.DisplayVertices();
                double volume = tetrahedron.CalculateVolume();
                Console.WriteLine($"Tetrahedron volume: {volume:F2} cubic units");
                Console.WriteLine($"Expected volume of unit regular tetrahedron: {1.0/6.0:F2} cubic units");

                Tetrahedron defaultTetrahedron = new Tetrahedron();
                try
                {
                    defaultTetrahedron.DisplayVertices();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"\nExpected error with default constructor: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }
    }
}
