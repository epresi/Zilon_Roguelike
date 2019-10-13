﻿using System;
using System.Collections.Generic;

namespace Zilon.Core
{
    public sealed class OffsetCoords : IEquatable<OffsetCoords>
    {
        public int X { get; }
        public int Y { get; }

        public OffsetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public bool Equals(OffsetCoords other)
        {
            if ((Object)this == (Object)other)
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetCoords coords && Equals(coords);
        }

        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            if ((Object)left == (Object)right)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right); ;
        }

        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }
    }
}
