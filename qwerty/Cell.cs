﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using qwerty.Objects;

namespace qwerty
{
    class Cell
    {
        public SpaceObject spaceObject = null;
        public int id;
        public int x;
        public int y;
        public int xcenter;
        public int ycenter;

        public int xpoint1 => (int)CellPoints[3].X;
        public int xpoint2 => (int)CellPoints[4].X;
        public int xpoint3 => (int)CellPoints[5].X;
        public int xpoint4 => (int)CellPoints[0].X;
        public int xpoint5 => (int)CellPoints[1].X;
        public int xpoint6 => (int)CellPoints[2].X;

        public int ypoint1 => (int)CellPoints[3].Y;
        public int ypoint2 => (int)CellPoints[4].Y;
        public int ypoint3 => (int)CellPoints[5].Y;
        public int ypoint4 => (int)CellPoints[0].Y;
        public int ypoint5 => (int)CellPoints[1].Y;
        public int ypoint6 => (int)CellPoints[2].Y;

        public PointF[] CellPoints;

        public Cell(float sideLength, int cellX, int cellY, int cellId, Size fieldOffset = default(Size))
        {
            x = cellX;
            y = cellY;
            id = cellId;

            float xCoord = (float)Math.Truncate(sideLength*Math.Cos(Math.PI/3));
            float yCoord = (float)Math.Truncate(sideLength*Math.Sin(Math.PI/3));
            CellPoints = new[]
            {
                new PointF(sideLength, 0),
                new PointF(xCoord, yCoord),
                new PointF(-xCoord, yCoord),
                new PointF(-sideLength, 0),
                new PointF(-xCoord, -yCoord),
                new PointF(xCoord, -yCoord)
            };

            Size cellOffset = new Size((int)(cellX * (2 * sideLength - xCoord)), (int)(cellY * 2 * yCoord + (cellX % 2 == 0 ? 0 : yCoord)));
            for (int i = 0; i < CellPoints.Length; i++)
            {
                CellPoints[i] = PointF.Add(CellPoints[i], fieldOffset + cellOffset);
            }

            xcenter = (xpoint2 + xpoint3) / 2;
            ycenter = (ypoint2 + ypoint6) / 2;
        }

        public bool IsNeighborCell(int newCellX, int newCellY)
        {
            int deltaX = newCellX - x;
            int deltaY = newCellY - y;

            switch (deltaX)
            {
                case 0:
                    return deltaY == -1 || deltaY == 1;
                case 1:
                case -1:
                    return deltaY == 0 ||
                           (x%2 == 0
                               ? deltaY == -1
                               : deltaY == 1);
                default:
                    return false;
            }
        }
    }
}
