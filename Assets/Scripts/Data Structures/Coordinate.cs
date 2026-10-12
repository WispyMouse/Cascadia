namespace Cascadia.DataStructures
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Coordinate defining 2D space.
    /// Very, very similar to Vector2Int. Most code lifted from that struct.
    /// This is split out so that we can refer to a coordinate without using Unity's core module.
    /// Caches the HashCode so that this can be used for lookup frequently.
    /// </summary>
    public readonly struct Coordinate : IEquatable<Coordinate>, IFormattable
    {
        public readonly int X;
        public readonly int Y;

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Coordinate operator -(Coordinate v) => new(-v.X, -v.Y);
        public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.X + b.X, a.Y + b.Y);
        public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.X - b.X, a.Y - b.Y);
        public static Coordinate operator *(Coordinate a, Coordinate b) => new(a.X * b.X, a.Y * b.Y);
        public static Coordinate operator *(int a, Coordinate b) => new(a * b.X, a * b.Y);
        public static Coordinate operator *(Coordinate a, int b) => new(a.X * b, a.Y * b);
        public static Coordinate operator /(Coordinate a, int b) => new(a.X / b, a.Y / b);
        public static bool operator ==(Coordinate lhs, Coordinate rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
        public static bool operator !=(Coordinate lhs, Coordinate rhs) => !(lhs.X == rhs.X && lhs.Y == rhs.Y);

        public override readonly bool Equals(object other)
        {
            if (other is Coordinate v)
            {
                return this.Equals(in v);
            }
            return false;
        }

        public readonly bool Equals(Coordinate other) => this.X == other.X && this.Y == other.Y;
        public readonly bool Equals(in Coordinate other) => this.X == other.X && this.Y == other.Y;

        public override readonly string ToString() => this.ToString(null, null);
        public readonly string ToString(string format) => this.ToString(format, null);
        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;
                
            return string.Format("({0}, {1})", X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
        }

        public static readonly Coordinate Zero = new(0, 0);
        public static readonly Coordinate One = new(1, 1);
        public static readonly Coordinate Up = new(0, 1);
        public static readonly Coordinate Down = new(0, -1);
        public static readonly Coordinate Left = new(-1, 0);
        public static readonly Coordinate Right = new(1, 0);

        public override readonly int GetHashCode() => HashCode.Combine(this.X, this.Y);
    }
}
