using System;

namespace LabWork
{
    /// <summary>
    /// Represents a 2D point with X and Y coordinates.
    /// </summary>
    public readonly struct Point
    {
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Initializes a new instance of the Point struct with default coordinates (0,0).
        /// </summary>
        public Point() : this(0, 0) { }

        /// <summary>
        /// Initializes a new instance of the Point struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}, {Y})";
    }

    /// <summary>
    /// Represents a triangle defined by three vertices in a 2D plane.
    /// </summary>
    public class Triangle
    {
        private readonly Point[] _vertices;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the Triangle class.
        /// </summary>
        public Triangle()
        {
            _vertices = new Point[3];
            _isInitialized = false;
        }

        /// <summary>
        /// Gets a copy of the triangle's vertices.
        /// </summary>
        /// <returns>Array of vertices</returns>
        protected Point[] GetVertices()
        {
            return (Point[])_vertices.Clone();
        }

        /// <summary>
        /// Calculates the area of a triangle formed by three points.
        /// </summary>
        private static double CalculateTriangleArea(Point p1, Point p2, Point p3)
        {
            return Math.Abs(
                p1.X * (p2.Y - p3.Y) +
                p2.X * (p3.Y - p1.Y) +
                p3.X * (p1.Y - p2.Y)
            ) / 2.0;
        }

        /// <summary>
        /// Checks if three points form a valid triangle by verifying they're not collinear.
        /// </summary>
        private static bool IsValidTriangle(Point p1, Point p2, Point p3)
        {
            const double Epsilon = 1e-10;
            return CalculateTriangleArea(p1, p2, p3) > Epsilon;
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        private static double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /// <summary>
        /// Sets the coordinates of the triangle's vertices.
        /// </summary>
        /// <param name="p1">First vertex</param>
        /// <param name="p2">Second vertex</param>
        /// <param name="p3">Third vertex</param>
        /// <exception cref="ArgumentException">Thrown when points don't form a valid triangle</exception>
        public virtual void SetCoordinates(Point p1, Point p2, Point p3)
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

        /// <summary>
        /// Displays the coordinates of the triangle's vertices.
        /// </summary>
        public virtual void DisplayVertices()
        {
            ValidateInitialization();

            Console.WriteLine("Triangle vertices:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Vertex {i + 1}: {_vertices[i]}");
            }
        }

        /// <summary>
        /// Validates that the triangle has been properly initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the triangle is not initialized</exception>
        protected void ValidateInitialization()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Triangle coordinates have not been initialized");
            }
        }

        /// <summary>
        /// Calculates the area of the triangle using Heron's formula.
        /// </summary>
        /// <returns>The area of the triangle</returns>
        /// <exception cref="InvalidOperationException">Thrown when coordinates haven't been set</exception>
        public virtual double CalculateArea()
        {
            ValidateInitialization();

            // Calculate sides using the distance formula
            double a = CalculateDistance(_vertices[0], _vertices[1]);
            double b = CalculateDistance(_vertices[1], _vertices[2]);
            double c = CalculateDistance(_vertices[2], _vertices[0]);

            // Calculate semi-perimeter
            double s = (a + b + c) / 2;

            // Calculate area using Heron's formula: A = √(s(s-a)(s-b)(s-c))
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

    /// <summary>
    /// Represents a tetrahedron with a triangular base and a fourth vertex.
    /// </summary>
    public class Tetrahedron : Triangle
    {
        private readonly Point _defaultApexPoint = new Point(0, 0);
        private Point _fourthVertex;
        private double _height;
        private bool _tetrahedronInitialized;

        /// <summary>
        /// Initializes a new instance of the Tetrahedron class.
        /// </summary>
        public Tetrahedron() : base()
        {
            _fourthVertex = _defaultApexPoint;
            _tetrahedronInitialized = false;
        }

        /// <summary>
        /// Checks if a point lies in the plane of the base triangle.
        /// </summary>
        private bool IsPointInBasePlane(Point point)
        {
            Point[] baseVertices = GetVertices();
            // If the point forms a valid triangle with any two vertices of the base,
            // and these triangles' total area equals the base triangle's area,
            // then the point lies in the plane
            double baseArea = CalculateTriangleArea(baseVertices[0], baseVertices[1], baseVertices[2]);
            double area1 = CalculateTriangleArea(point, baseVertices[1], baseVertices[2]);
            double area2 = CalculateTriangleArea(baseVertices[0], point, baseVertices[2]);
            double area3 = CalculateTriangleArea(baseVertices[0], baseVertices[1], point);
            
            const double Epsilon = 1e-10;
            return Math.Abs(baseArea - (area1 + area2 + area3)) < Epsilon;
        }

        /// <summary>
        /// Sets the coordinates of the tetrahedron's vertices and its height.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when height is invalid or points don't form a valid tetrahedron</exception>
        public void SetCoordinates(Point p1, Point p2, Point p3, Point p4, double height)
        {
            if (height <= 0)
            {
                throw new ArgumentException("Height must be positive");
            }

            // First set the base triangle coordinates (this will validate the triangle)
            base.SetCoordinates(p1, p2, p3);
            
            // Verify that the fourth point doesn't lie in the base plane
            if (IsPointInBasePlane(p4))
            {
                throw new ArgumentException("The fourth vertex cannot lie in the same plane as the base triangle");
            }

            _fourthVertex = p4;
            _height = height;
            _tetrahedronInitialized = true;
        }

        /// <summary>
        /// Displays the coordinates of all vertices and the height of the tetrahedron.
        /// </summary>
        public override void DisplayVertices()
        {
            if (!_tetrahedronInitialized)
            {
                throw new InvalidOperationException("Tetrahedron coordinates have not been initialized");
            }

            Console.WriteLine("Tetrahedron vertices:");
            Point[] baseVertices = GetVertices();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Base vertex {i + 1}: {baseVertices[i]}");
            }
            Console.WriteLine($"Apex vertex: {_fourthVertex}");
            Console.WriteLine($"Height: {_height}");
        }

        /// <summary>
        /// Calculates the volume of the tetrahedron.
        /// </summary>
        /// <returns>The volume of the tetrahedron</returns>
        /// <exception cref="InvalidOperationException">Thrown when coordinates haven't been set</exception>
        public double CalculateVolume()
        {
            if (!_tetrahedronInitialized)
            {
                throw new InvalidOperationException("Cannot calculate volume: tetrahedron is not properly initialized");
            }

            // Volume of a tetrahedron = (1/3) * base area * height
            return (1.0 / 3.0) * CalculateArea() * _height;
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
                
                // Create and test Triangle
                Triangle triangle = new Triangle();
                
                // Try to calculate area before initialization (should throw exception)
                try
                {
                    triangle.CalculateArea();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}");
                }

                // Set valid triangle coordinates
                triangle.SetCoordinates(
                    new Point(0, 0),
                    new Point(3, 0),
                    new Point(0, 4)
                );
                
                triangle.DisplayVertices();
                double area = triangle.CalculateArea();
                Console.WriteLine($"Triangle area: {area:F2} square units\n");

                // Try to create invalid triangle (collinear points)
                try
                {
                    Triangle invalidTriangle = new Triangle();
                    invalidTriangle.SetCoordinates(
                        new Point(0, 0),
                        new Point(1, 1),
                        new Point(2, 2)  // Collinear with previous points
                    );
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}\n");
                }

                Console.WriteLine("Testing Tetrahedron class:");
                Console.WriteLine("-------------------------");
                
                // Create and test Tetrahedron
                Tetrahedron tetrahedron = new Tetrahedron();
                
                // Try with invalid height
                try
                {
                    tetrahedron.SetCoordinates(
                        new Point(0, 0),
                        new Point(3, 0),
                        new Point(0, 4),
                        new Point(1, 1),
                        -5.0  // Invalid negative height
                    );
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}");
                }

                // Try with fourth point in base plane
                try
                {
                    tetrahedron.SetCoordinates(
                        new Point(0, 0),
                        new Point(3, 0),
                        new Point(0, 4),
                        new Point(1.5, 2),  // Point in the base plane
                        5.0
                    );
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Expected error: {ex.Message}");
                }

                // Set valid tetrahedron coordinates
                tetrahedron.SetCoordinates(
                    new Point(0, 0),
                    new Point(3, 0),
                    new Point(0, 4),
                    new Point(1, 1),
                    5.0  // height
                );
                
                tetrahedron.DisplayVertices();
                double volume = tetrahedron.CalculateVolume();
                Console.WriteLine($"Tetrahedron volume: {volume:F2} cubic units");

                // Test default constructor behavior
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
