﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using qwerty.Objects;

namespace qwerty
{
    class CombatMap
    {
        public int FieldWidth;
        public int FieldHeight;
        private const float CellSideLength = 40;
        public readonly List<Cell> Cells = new List<Cell>(); 
        public CombatMap(int w, int h) 
        {
            FieldWidth = w;
            FieldHeight = h;

            InitializeMap();
        }

        public int FieldWidthPixels
        {
            get
            {
                return (int)(Cells.Last().CellPoints.Max(cell => cell.X) + 10);
            }
        }

        public int FieldHeightPixels
        {
            get
            {
                return (int)(Cells.Last().CellPoints.Max(cell => cell.Y) + 10);
            }
        }

        public Cell GetCellByCellCoordinates(int x, int y)
        {
            if (x < 0 || y < 0 || x*FieldHeight + y > FieldWidth*FieldHeight)
            {
                return null;
            }

            return Cells[x*FieldHeight + y];
        }

        public Cell GetCellByPixelCoordinates(int x, int y)
        {
            // TODO: better check if point is inside polygon
            return
                Cells.FirstOrDefault(
                    cell => x > cell.CellPoints[2].X && x < cell.CellPoints[1].X && y > cell.CellPoints[5].Y && y < cell.CellPoints[1].Y);
        }

        public double GetAngle(int sourceCellId, int targetCellId, int activePlayerId)
        {
            double angle;

            double shipx = Cells[sourceCellId].CellCenter.X;
            double shipy = Cells[sourceCellId].CellCenter.Y;
            double targetx = Cells[targetCellId].CellCenter.X;
            double targety = Cells[targetCellId].CellCenter.Y;

            if (shipx == targetx) // избегаем деления на ноль
            {
                if (shipy > targety)
                {
                    angle = -90;
                }
                else
                {
                    angle = 90;
                }
                if (activePlayerId == 2) angle = -angle;

            }
            else // находим угол, на который нужно повернуть корабль (если он не равен 90 градусов)
            {
                angle = Math.Atan((targety - shipy) / (targetx - shipx)) * 180 / Math.PI;
            }
            // дальше идет коррекция, не пытайся разобраться как это работает, просто оставь как есть
            if (activePlayerId == 1)
            {
                if (shipy == targety && shipx > targetx)
                {
                    angle = 180;
                }
                else if (shipx > targetx && shipy < targety)
                {
                    angle += 180;
                }
                else if (shipx > targetx && shipy > targety)
                {
                    angle = angle - 180;
                }
            }
            else if (activePlayerId == 2)
            {
                if (shipy == targety && shipx < targetx)
                {
                    angle = 180;
                }
                else if (shipx < targetx && shipy < targety)
                {
                    angle -= 180;
                }
                else if (shipx < targetx && shipy > targety)
                {
                    angle += 180;
                }
            }

            if (angle > 150) angle = 150;
            else if (angle < -150) angle = -150;
            return angle;
        }

        public void PlaceShip(Ship ship)
        {
            var rand = new Random();
            int randomBoxId;
            do
            {
                randomBoxId = rand.Next(0, FieldHeight*2);
                if (ship.player == 2)
                {
                    randomBoxId = Cells.Count - randomBoxId - 1;
                }
            } while (Cells[randomBoxId].spaceObject != null);

            Cells[randomBoxId].spaceObject = ship;
            ship.boxId = randomBoxId;
        }

        private void InitializeMap()
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    Cells.Add(new Cell(CellSideLength, i, j, i * FieldHeight + j,
                        new Size((int)(CellSideLength + 10), (int)(Math.Sin(Math.PI / 3) * CellSideLength + 10))));
                }
            }
        }
    }

 
}
