using System;
using System.Drawing;


namespace gitHistoryVisualization
{
    public class Canvas : IDisposable
    {
        private bool _disposedValue;

        public Canvas(int size, int padding)
        {
            int width  = size + 2 * padding;
            int height = size + 2 * padding;

            CenterX = width / 2f;
            CenterY = height / 2f;

            Bitmap   = new Bitmap(width, height);
            Graphics = Graphics.FromImage(Bitmap);
        }

        ~Canvas()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public float CenterX { get; }
        public float CenterY { get; }

        public Bitmap   Bitmap   { get; }
        public Graphics Graphics { get; }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    Bitmap?.Dispose();
                    Graphics?.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
