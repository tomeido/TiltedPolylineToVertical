using System;
using System.Linq;
using SharpDX;
using System.Collections.Generic;

namespace HelixPlayground
{
    public partial class MainWindow
    {
        List<Vector3> tiltedPolyline = new List<Vector3>();
        List<Vector3> tiltedPointsGroupStart = new List<Vector3>();
        List<Vector3> tiltedPointsGroupMid = new List<Vector3>();
        List<Vector3> tiltedPointsGroupEnd = new List<Vector3>();
        List<Vector3> newMidPoints = new List<Vector3>();
        List<Vector3> newPointsGroup = new List<Vector3>();

        public List<Vector3> TiltedPolylineToPerpendicular(List<Vector3> tiltedPolyline)
        {
            SplitPointsList();
            ReverseEndList();
            SynchronizeTwoCoordinates(tiltedPointsGroupStart);
            SynchronizeTwoCoordinates(tiltedPointsGroupEnd);
            NewMidPointsBoundingBox();
            PickNewTwoMidPoints();
            ReverseEndList();
            AddToNewList();
            return tiltedPolyline;
        }

        public void AddToNewList()
        {
            for (int i = 0; i < tiltedPointsGroupStart.Count; i++)
            {
                newPointsGroup.Add(tiltedPointsGroupStart[i]);
            }
            for (int i = 0; i < tiltedPointsGroupEnd.Count; i++)
            {
                newPointsGroup.Add(tiltedPointsGroupEnd[i]);
            }
        }

        public void PickNewTwoMidPoints()
        {
            List<(float, int)> minDistance = new List<(float, int)>();
            for (int i = 0; i < newMidPoints.Count; i++)
            {
                Vector3 v = newMidPoints[i] - tiltedPointsGroupMid[0];
                float l = v.Length();
                minDistance.Add((l, i));
            }
            minDistance = minDistance.OrderByDescending(t => t.Item1).ToList();
            int midPt1 = minDistance[newMidPoints.Count - 1].Item2;
            int midPt2 = minDistance[newMidPoints.Count - 2].Item2;
            if ((newMidPoints[midPt1] - tiltedPointsGroupStart.Last()).Length()
                >= (newMidPoints[midPt2] - tiltedPointsGroupStart.Last()).Length())
            {
                tiltedPointsGroupStart.Add(newMidPoints[midPt2]);
                tiltedPointsGroupEnd.Add(newMidPoints[midPt1]);
            }
            else
            {
                tiltedPointsGroupStart.Add(newMidPoints[midPt1]);
                tiltedPointsGroupEnd.Add(newMidPoints[midPt2]);
            }
        }

        public void NewMidPointsBoundingBox()
        {
            if (tiltedPointsGroupStart.Last().X != tiltedPointsGroupEnd.Last().X
                && tiltedPointsGroupStart.Last().Y != tiltedPointsGroupEnd.Last().Y
                && tiltedPointsGroupStart.Last().Z != tiltedPointsGroupEnd.Last().Z)
            {
                newMidPoints.Add(new Vector3(tiltedPointsGroupStart.Last().X, tiltedPointsGroupEnd.Last().Y, tiltedPointsGroupEnd.Last().Z));
                newMidPoints.Add(new Vector3(tiltedPointsGroupStart.Last().X, tiltedPointsGroupEnd.Last().Y, tiltedPointsGroupStart.Last().Z));
                newMidPoints.Add(new Vector3(tiltedPointsGroupStart.Last().X, tiltedPointsGroupStart.Last().Y, tiltedPointsGroupEnd.Last().Z));
                newMidPoints.Add(new Vector3(tiltedPointsGroupEnd.Last().X, tiltedPointsGroupStart.Last().Y, tiltedPointsGroupStart.Last().Z));
                newMidPoints.Add(new Vector3(tiltedPointsGroupEnd.Last().X, tiltedPointsGroupStart.Last().Y, tiltedPointsGroupEnd.Last().Z));
                newMidPoints.Add(new Vector3(tiltedPointsGroupEnd.Last().X, tiltedPointsGroupEnd.Last().Y, tiltedPointsGroupStart.Last().Z));
            }
        }

        public List<Vector3> SynchronizeTwoCoordinates(List<Vector3> pointsList)
        {
            for (int i = 0; i < pointsList.Count - 1; i++)
            {
                float absX = Math.Abs(pointsList[i].X - pointsList[i + 1].X);
                float absY = Math.Abs(pointsList[i].Y - pointsList[i + 1].Y);
                float absZ = Math.Abs(pointsList[i].Z - pointsList[i + 1].Z);
                if (absX >= absY && absX >= absZ)
                {
                    pointsList[i + 1] = new Vector3(pointsList[i + 1].X, pointsList[i].Y, pointsList[i].Z);
                }
                else if (absY >= absX && absY >= absZ)
                {
                    pointsList[i + 1] = new Vector3(pointsList[i].X, pointsList[i + 1].Y, pointsList[i].Z);
                }
                else
                {
                    pointsList[i + 1] = new Vector3(pointsList[i].X, pointsList[i].Y, pointsList[i + 1].Z);
                }
            }
            return pointsList;
        }

        public void ReverseEndList()
        {
            tiltedPointsGroupEnd.Reverse();
        }

        public void SplitPointsList()
        {
            if (tiltedPolyline.Count % 2 == 1)
            {
                int t = this.tiltedPolyline.Count;
                foreach (Vector3 s in tiltedPolyline.GetRange(0, (t - 1) / 2))
                    tiltedPointsGroupStart.Add(s);
                tiltedPointsGroupMid.Add(tiltedPolyline[(t - 1) / 2]);
                foreach (Vector3 e in tiltedPolyline.GetRange(((t - 1) / 2) + 1, (t - 1) / 2))
                    tiltedPointsGroupEnd.Add(e);
            }
            else
            {
                int t = tiltedPolyline.Count;
                foreach (Vector3 s in tiltedPolyline.GetRange(0, t / 2))
                    tiltedPointsGroupStart.Add(s);
                tiltedPointsGroupMid.Add(tiltedPolyline[t / 2]);
                foreach (Vector3 e in tiltedPolyline.GetRange((t / 2) + 1, (t / 2) - 1))
                    tiltedPointsGroupEnd.Add(e);
            }
        }


        public void InitPolyline()
        {
            tiltedPolyline.Add(new Vector3(-10.4f, -10.9f, 0f));
            tiltedPolyline.Add(new Vector3(4.6f, -8.8f, 0f));
            tiltedPolyline.Add(new Vector3(9.5f, -7.1f, 12f));
            tiltedPolyline.Add(new Vector3(25.2f, -7.4f, 12f));
            tiltedPolyline.Add(new Vector3(22.6f, 10.5f, 12.6f));
            tiltedPolyline.Add(new Vector3(26.2f, 9.5f, 28.2f));
            tiltedPolyline.Add(new Vector3(28f, 22.8f, 26.9f));
            tiltedPolyline.Add(new Vector3(49.7f, 22.8f, 26.9f));
            tiltedPolyline.Add(new Vector3(53.1f, 21.9f, 45.5f));
        }
    }
}


