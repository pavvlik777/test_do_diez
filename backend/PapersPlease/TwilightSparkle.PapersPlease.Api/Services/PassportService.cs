using System;
using System.Collections.Generic;
using OpenCvSharp;
using Tesseract;
using Rect = OpenCvSharp.Rect;

namespace TwilightSparkle.PapersPlease.Api.Services
{
    public sealed class PassportService : IPassportService
    {
        public string ReadMachineReadingZoneData(byte[] passportImage)
        {
            using (var image = Cv2.ImDecode(passportImage, ImreadModes.Color))
            {
                using (var blackhat = GetBlackhat(image))
                {
                    using (var gradX = GetGradX(blackhat))
                    {
                        using (var threshold = GetThreshold(gradX))
                        {
                            var contours = Cv2.FindContoursAsArray(threshold, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                            var textFromImage = GetTextFromImage(image, contours);
                            GC.Collect();

                            return textFromImage;
                        }
                    }
                }
            }
        }


        private static Mat GetBlackhat(Mat image)
        {
            using (var rectKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(13, 5)))
            {
                using (var gray = new Mat())
                {
                    Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
                    Cv2.GaussianBlur(gray, gray, new Size(3, 3), 0);
                    var blackhat = new Mat();
                    Cv2.MorphologyEx(gray, blackhat, MorphTypes.BlackHat, rectKernel);

                    return blackhat;
                }
            }
        }

        private static Mat GetGradX(Mat blackhat)
        {
            using (var rectKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(13, 5)))
            {
                var gradX = new Mat();
                Cv2.Sobel(blackhat, gradX, MatType.CV_32F, 1, 0, -1);
                gradX = gradX.Abs();
                gradX.MinMaxLoc(out double minVal, out var maxVal);
                gradX = 255 * ((gradX - minVal) / (maxVal - minVal));
                gradX.ConvertTo(gradX, MatType.CV_8U);
                Cv2.MorphologyEx(gradX, gradX, MorphTypes.Close, rectKernel);

                return gradX;
            }
        }

        private static Mat GetThreshold(Mat gradX)
        {
            using (var sqKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(21, 21)))
            {
                var threshold = new Mat();
                Cv2.Threshold(gradX, threshold, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                Cv2.MorphologyEx(threshold, threshold, MorphTypes.Close, sqKernel);
                Cv2.Erode(threshold, threshold, null, iterations: 4);

                return threshold;
            }
        }

        private static string GetTextFromImage(Mat image, IReadOnlyCollection<Point[]> contours)
        {
            foreach (var contour in contours)
            {
                var rect = Cv2.BoundingRect(contour);
                var area = rect.Width / (float)rect.Height;
                var crWidth = rect.Width / (float)image.Width;
                if (area <= 5 || crWidth <= 0.75)
                {
                    continue;
                }

                var pX = (int)((rect.X + rect.Width) * 0.03);
                var pY = (int)((rect.Y + rect.Height) * 0.03);

                var x = rect.X - pX;
                var y = rect.Y - pY;
                var width = rect.Width + pX * 2;
                var height = rect.Height + pY * 2;

                var readRect = new Rect(x, y, width, height);
                using (var engine = new TesseractEngine(@"./tesseractData", "mrz", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ<");
                    using (var grayImage = new Mat(image, readRect))
                    {
                        using (var img = Pix.LoadFromMemory(grayImage.ToBytes()))
                        {
                            using (var page = engine.Process(img, PageSegMode.SingleBlock))
                            {
                                var text = page.GetText();

                                return text;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}