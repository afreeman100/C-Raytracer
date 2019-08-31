﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;

namespace Raytracer
{
    public class Camera
    {
        // Camera properties
        private readonly Vector3 focalPoint;
        private readonly int focalLength;
        private readonly Tuple<int, int> canvasDimensions;


        // Constructor with default camera parameters
        public Camera()
        {
            focalPoint = new Vector3(0, 0, 0);
            focalLength = 1;
            canvasDimensions = new Tuple<int, int>(1, 1);
        }


        // Constructor for user-specified paraeters
        public Camera(Vector3 focalPoint, int focalLength, Tuple<int, int> canvasDimensions)
        {
            this.focalPoint = focalPoint;
            this.focalLength = focalLength;
            this.canvasDimensions = canvasDimensions;
        }


        // If a single value is given for resulution then create square image
        public void Render(int resolution, Scene scene)
        {
            Render(new Tuple<int, int>(resolution, resolution), scene);
        }


        // Main loop which fires rays though each pixel of canvas 
        public void Render(Tuple<int, int> resolution, Scene scene)
        {
            Bitmap newImage = new Bitmap(resolution.Item2, resolution.Item1);

            // Determine color of each pixel
            for (int row = 0; row < resolution.Item1; row++)
            {
                for (int col = 0; col < resolution.Item2; col++)
                {

                    // Map from pixel coordinates to scene 
                    float r = (float)(canvasDimensions.Item1 * ((row + 0.5) / resolution.Item1 - 0.5));
                    float c = (float)(canvasDimensions.Item2 * ((col + 0.5) / resolution.Item2 - 0.5));
                    Vector3 worldCoord = focalPoint + focalLength * Vector3.UnitZ + c * Vector3.UnitX - r * Vector3.UnitY;
                    Vector3 direction = Vector3.Normalize(worldCoord - focalPoint);

                    // intersection occurred?,  distance,  normal,  scene object
                    Tuple<bool, double, Vector3, ISceneObject> intersection = scene.ClosestIntersection(focalPoint, direction);

                    if (!intersection.Item1)
                    {
                        // No intersection -> set pixel to black
                        newImage.SetPixel(col, row, Color.FromName("Black"));
                    }
                    else
                    {
                        // Determine color based on scene object
                        Vector3 intersectionPoint = focalPoint + (float)(intersection.Item2) * direction;
                        Color objColor = intersection.Item4.PointColor(scene, intersectionPoint, intersection.Item3, direction);
                        newImage.SetPixel(col, row, objColor);
                    }
                }
            }
            newImage.Save("img.png", ImageFormat.Png);
            Console.WriteLine("Done!");
        }
    }
}
