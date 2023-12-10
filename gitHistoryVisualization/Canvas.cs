﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace gitHistoryVisualization
{
    public class Canvas : IDisposable
    {
        private readonly int      _margin;
        private readonly DrawUtil _drawUtil;
        private readonly Graphics _graphics;

        private bool _disposedValue;

        public Canvas(int size, int margin)
        {
            _margin = margin;
            int width  = size;
            int height = size;

            CenterX = width / 2f;
            CenterY = height / 2f;

            Bitmap    = new Bitmap(width, height);
            _graphics = Graphics.FromImage(Bitmap);

            _drawUtil = new DrawUtil(size);

            PrepareGraphics();
        }

        ~Canvas()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public float CenterX { get; }
        public float CenterY { get; }

        public Bitmap Bitmap { get; }

        public void Save(string outputPath, ImageFormat format)
        {
            int newWidth  = Bitmap.Width + _margin * 2;
            int newHeight = Bitmap.Height + _margin * 2;

            Bitmap withMargin = new(newWidth, newHeight);

            using Graphics graphics = Graphics.FromImage(withMargin);
            graphics.Clear(_drawUtil.BackgroundColor);
            graphics.DrawImage(Bitmap, _margin, _margin, Bitmap.Width, Bitmap.Height);

            withMargin.Save(outputPath, format);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DrawCommit(float x, float y, DateSummary dateSummary)
        {
            _drawUtil.DrawDateSummary(_graphics, dateSummary, x, y);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    Bitmap?.Dispose();
                    _graphics?.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void PrepareGraphics()
        {
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics.Clear(_drawUtil.BackgroundColor);
        }
    }
}
