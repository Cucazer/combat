﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using qwerty.Objects;
using Hex = Barbar.HexGrid;

namespace qwerty
{
    /// <summary>
    /// Hexagon neighbor direction names as they are listed in "Directions" list of the cube coordinates in HexGrid library
    /// </summary>
    public enum HexagonNeighborDirection
    {
        SouthEast = 0,
        NorthEast,
        North,
        NorthWest,
        SouthWest,
        South
    }
    
    class CombatMap
    {
        public readonly int FieldWidth;
        public readonly int FieldHeight;
        private const float CellSideLength = 40;

        public Hex.HexLayout<Hex.Point, Hex.PointPolicy> HexGrid = Hex.HexLayoutFactory.CreateFlatHexLayout(
            new Hex.Point(CellSideLength, CellSideLength), new Hex.Point((int)(CellSideLength + 10),  (int)(Math.Sin(Math.PI / 3) * CellSideLength + 10)),
            Hex.Offset.Odd);

        public CombatMap(int w, int h) 
        {
            FieldWidth = w;
            FieldHeight = h;
        }

        public int FieldWidthPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth - 1, FieldHeight - 1)));
                var cornerOffset = HexGrid.HexCornerOffset(0);
                return (int)(hexagonOffset.X + cornerOffset.X);
            }
        }

        public int FieldHeightPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth - 1, FieldHeight - 1)));
                var cornerOffset = HexGrid.HexCornerOffset(5);
                return (int)(hexagonOffset.Y + cornerOffset.Y);
            }
        }

        public Point HexToPixel(int x, int y)
        {
            var cubeCoordinates = HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(x,y));
            return HexGrid.HexToPixel(cubeCoordinates).ConvertToDrawingPoint();
        }
        
        public Point GetHexagonCornerOffset(int cornerIndex)
        {
            return HexGrid.HexCornerOffset(cornerIndex).ConvertToDrawingPoint();
        }

        public bool AreNeighbors(Hex.OffsetCoordinates firstHexagon, Hex.OffsetCoordinates secondHexagon)
        {
            return AreNeighbors(HexGrid.ToCubeCoordinates(firstHexagon), HexGrid.ToCubeCoordinates(secondHexagon));
        }
        
        public bool AreNeighbors(Hex.CubeCoordinates firstHexagon, Hex.CubeCoordinates secondHexagon)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Hex.CubeCoordinates.Neighbor(firstHexagon, i).Equals(secondHexagon))
                {
                    return true;
                }
            }
            return false;
        }

        public PointF[] GetHexagonCorners(int x, int y)
        {
			return GetHexagonCorners(new Hex.OffsetCoordinates(x, y));
        }

		public PointF[] GetHexagonCorners(Hex.OffsetCoordinates offsetCoordinates)
		{
			var coordinates = HexGrid.ToCubeCoordinates(offsetCoordinates);
			return HexGrid.PolygonCorners(coordinates).Select(c => c.ConvertToDrawingPointF()).ToArray();
		}

        public List<PointF[]> AllHexagonCorners
        {
            get
            {
                var cornerPointList = new List<PointF[]>();
                for (int x = 0; x < FieldWidth; x++)
                {
                    for (int y = 0; y < FieldHeight; y++)
                    {
                        cornerPointList.Add(GetHexagonCorners(x, y));
                    }
                }
                return cornerPointList;
            }
        }

		public double GetAngle(Hex.CubeCoordinates sourceCoordinates, Hex.CubeCoordinates targetCoordinates)
		{
			double dx = HexGrid.HexToPixel(targetCoordinates).X - HexGrid.HexToPixel(sourceCoordinates).X;
			double dy = HexGrid.HexToPixel(targetCoordinates).Y - HexGrid.HexToPixel(sourceCoordinates).Y;
			return Math.Atan2(dy, dx) * 180 / Math.PI;
		}

		public double GetAngle(Hex.OffsetCoordinates sourceOffsetCoordinates, Hex.OffsetCoordinates targetOffsetCoordinates)
        {
			return GetAngle(HexGrid.ToCubeCoordinates(sourceOffsetCoordinates), HexGrid.ToCubeCoordinates(targetOffsetCoordinates));
        }

        public Hex.OffsetCoordinates PixelToOffsetCoordinates(Point pixelCoordinates)
        {
            var cubeCoordinates = HexGrid.PixelToHex(pixelCoordinates.ConvertToHexPoint()).Round();
            var offsetCoordinates = HexGrid.ToOffsetCoordinates(cubeCoordinates);
            if (offsetCoordinates.Column < 0 || offsetCoordinates.Column > FieldWidth ||
                offsetCoordinates.Row < 0 || offsetCoordinates.Row > FieldHeight)
            {
                throw new ArgumentOutOfRangeException($"Pixel ({pixelCoordinates.X},{pixelCoordinates.Y}) is outside game field.");
            }
            return offsetCoordinates;
        }

        public int GetDistance(Hex.CubeCoordinates firstHexagon, Hex.CubeCoordinates secondHexagon)
        {
            return Hex.CubeCoordinates.Distance(firstHexagon,secondHexagon);
        }
        
        public int GetDistance(Hex.OffsetCoordinates firstHexagon, Hex.OffsetCoordinates secondHexagon)
        {
            return Hex.CubeCoordinates.Distance(HexGrid.ToCubeCoordinates(firstHexagon),HexGrid.ToCubeCoordinates(secondHexagon));
        }

		public List<Hex.OffsetCoordinates> GetAllHexagonsInRange(Hex.OffsetCoordinates centerHexagon, int range)
		{
			var allHexagons = new List<Hex.OffsetCoordinates>();
			//  TODO: implement spiral ring algorithm from redblobgames
			for (int x = 0; x<FieldWidth; x++)
            {
                for (int y = 0; y<FieldHeight; y++)
                {
					var coordinates = new Hex.OffsetCoordinates(x, y);
					var distance = GetDistance(centerHexagon, coordinates);
					if (distance <= range && distance > 0)
					{
						allHexagons.Add(coordinates);
					}
                }
            }
			return allHexagons;
		}

		public List<PointF[]> GetAllHexagonCornersInRange(Hex.OffsetCoordinates centerHexagon, int range)
		{
			var allHexagonCorners = new List<PointF[]>();
			foreach (var hexagonCoordinates in GetAllHexagonsInRange(centerHexagon, range))
			{
				allHexagonCorners.Add(GetHexagonCorners(hexagonCoordinates));
			}
			return allHexagonCorners;
		}

        public Point HexToPixel(Hex.OffsetCoordinates objectCoordinates)
        {
            return HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(objectCoordinates)).ConvertToDrawingPoint();
        }

        public Hex.OffsetCoordinates GetNeighborCoordinates(Hex.OffsetCoordinates hexagonOffsetCoordinates, int neighborDirection)
        {
            return HexGrid.ToOffsetCoordinates(
                Hex.CubeCoordinates.Neighbor(HexGrid.ToCubeCoordinates(hexagonOffsetCoordinates), neighborDirection));
        }
    } 
}
