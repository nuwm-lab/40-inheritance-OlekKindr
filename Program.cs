using System;

namespace LabWork
{
    /// <summary>
    /// Represents a 2D point with X and Y coordinates.
    /// </summary>
    public readonly struct Point
    {
        public double X { get; }
        public double Y { get; }

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
        protected Point[] vertices;
        protected bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the Triangle class.
        /// </summary>
        public Triangle()
        {
            vertices = new Point[3];
            _isInitialized = false;
        }

        /// <summary>
        /// Checks if three points form a valid triangle by verifying they're not collinear.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <returns>True if points form a valid triangle, false otherwise</returns>
        protected bool IsValidTriangle(Point p1, Point p2, Point p3)
        {
            // Calculate the area using the formula: 
            // Area = 1/2 * |x1(y2 - y3) + x2(y3 - y1) + x3(y1 - y2)|
            double area = Math.Abs(
                p1.X * (p2.Y - p3.Y) +
                p2.X * (p3.Y - p1.Y) +
                p3.X * (p1.Y - p2.Y)
            ) / 2.0;

            // If area is zero, points are collinear
            return area > 1e-10; // Using small epsilon for floating-point comparison
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

            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;
            _isInitialized = true;
        }

        /// <summary>
        /// Displays the coordinates of the triangle's vertices.
        /// </summary>
        public virtual void DisplayVertices()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Triangle coordinates have not been initialized");
            }

            Console.WriteLine("Triangle vertices:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Vertex {i + 1}: {vertices[i]}");
            }
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        protected double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /// <summary>
        /// Calculates the area of the triangle using Heron's formula.
        /// </summary>
        /// <returns>The area of the triangle</returns>
        /// <exception cref="InvalidOperationException">Thrown when coordinates haven't been set</exception>
        public virtual double CalculateArea()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Cannot calculate area: triangle coordinates have not been initialized");
            }

            // Calculate sides using the distance formula
            double a = CalculateDistance(vertices[0], vertices[1]);
            double b = CalculateDistance(vertices[1], vertices[2]);
            double c = CalculateDistance(vertices[2], vertices[0]);

            // Calculate semi-perimeter
            double s = (a + b + c) / 2;

            // Calculate area using Heron's formula: A = √(s(s-a)(s-b)(s-c))
            // where s is the semi-perimeter and a, b, c are the sides of the triangle
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

    /// <summary>
    /// Represents a tetrahedron with a triangular base and a fourth vertex.
    /// </summary>
    public class Tetrahedron : Triangle
    {
        private Point _fourthVertex;
        private double _height;
        private bool _tetrahedronInitialized;

        /// <summary>
        /// Initializes a new instance of the Tetrahedron class.
        /// </summary>
        public Tetrahedron() : base()
        {
            _fourthVertex = new Point();
            _tetrahedronInitialized = false;
        }

        /// <summary>
        /// Sets the coordinates of the tetrahedron's vertices and its height.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when height is invalid or points don't form a valid base</exception>
        public void SetCoordinates(Point p1, Point p2, Point p3, Point p4, double height)
        {
            if (height <= 0)
            {
                throw new ArgumentException("Height must be positive");
            }

            // First set the base triangle coordinates (this will validate the triangle)
            base.SetCoordinates(p1, p2, p3);
            
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
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Base vertex {i + 1}: {vertices[i]}");
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
            double baseArea = base.CalculateArea();
            return (1.0 / 3.0) * baseArea * _height;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }
    }
}
