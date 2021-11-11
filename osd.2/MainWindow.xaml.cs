using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace osd._2
{
    public partial class MainWindow : Window
    {
        double tileHeight = 10.0;
        double tileWidth = 10.0;

        int corridorMinLength = 2;
        int corridorMaxLength = 10;

        int roomMinWith = 3;
        int roomMinHeight = 3;

        int roomMaxWidth = 10;
        int roomMaxHeight = 10;

        private Random _rnd;
        int iterationCount = 10;
        private int corridoToNewRoomMinOffset = 1;
        private int corridoToNewRoomMaxOffset = 10;

        public MainWindow()
        {
            InitializeComponent();

            _rnd = new Random();
        }

        private void btn_gen_Click(object sender, RoutedEventArgs e)
        {
            gameArea.Children.Clear();
            var surfaceWidth = gameArea.ActualWidth;
            var surfaceHeight = gameArea.ActualHeight;

            List<Point> starts = new List<Point>
            {
                new Point(0,0)
            };
            List<PreviousIterationRoom> previousRooms = new List<PreviousIterationRoom>();

            //PreviousIterationRoom previousRoom = null;
            for (int i = 0; i < iterationCount; i++)
            {
                bool isRoom = i % 2 == 0;

                if (isRoom)
                {
                    foreach (var start in starts)
                    {
                        var width = _rnd.Next(roomMinWith, roomMaxWidth + 1);
                        var height = _rnd.Next(roomMinHeight, roomMaxHeight + 1);

                        int realWidth = (int)(width * tileWidth);
                        int realHeight = (int)(height * tileHeight);

                        System.Windows.Shapes.Rectangle room = new System.Windows.Shapes.Rectangle { Height = realHeight, Width = realWidth, Fill = new SolidColorBrush(Colors.DarkGray) };

                        gameArea.Children.Add(room);

                        Canvas.SetTop(room, start.Y);
                        Canvas.SetLeft(room, start.X);

                        previousRooms.Add(new PreviousIterationRoom
                        {
                            StartX = (int)start.X,
                            StartY = (int)start.Y,
                            RealWidth = realWidth,
                            RealHeight = realHeight,
                            UnitWidth = width,
                            UnitHeight = height
                        });
                    }
                    starts.Clear();

                }
                else
                {
                    foreach (var previousRoom in previousRooms)
                    {

                        var direction = _rnd.NextDouble();


                        bool isEast = _rnd.NextDouble() > 0.5;

                        if (direction <= 0.4)
                        {
                            DrawEastCorridor(previousRoom, starts);
                        }
                        else if (direction > 0.4 && direction <= 0.8)
                        {
                            DrawSouthCorridor(previousRoom, starts);
                        }
                        else
                        {
                            DrawEastCorridor(previousRoom, starts);
                            DrawSouthCorridor(previousRoom, starts);
                        }


                    }
                    previousRooms.Clear();

                }
            }
        }

        private void DrawSouthCorridor(PreviousIterationRoom previousRoom, List<Point> starts)
        {
            //take a coordinate on the bottom side of the previous room (its a X coordinate)
            var corrStart = _rnd.Next(previousRoom.StartX, previousRoom.StartX + previousRoom.RealWidth - (int)(1 * tileWidth));

            //corridor going south is 1 unit large
            int corrWidth = (int)(1 * tileWidth);

            int corrHeight = (int)(_rnd.Next(corridorMinLength, corridorMaxLength) * tileHeight);

            System.Windows.Shapes.Rectangle corridor = new System.Windows.Shapes.Rectangle
            { Height = corrHeight, Width = corrWidth, Fill = new SolidColorBrush(Colors.LightGray) };

            gameArea.Children.Add(corridor);

            Canvas.SetTop(corridor, previousRoom.StartY + previousRoom.RealHeight);
            Canvas.SetLeft(corridor, corrStart);

            //compute new start point = corridor end +/- Y offset to avoid next room to have its corridor on top left corner
            var startY = previousRoom.StartY + previousRoom.RealHeight + corrHeight;


            var startX = corrStart;
            //(int)(corrStart +
            //           (_rnd.NextDouble() > 0.5
            //               ? _rnd.Next(corridoToNewRoomMinOffset, corridoToNewRoomMaxOffset + 1) * tileWidth
            //               : _rnd.Next(corridoToNewRoomMinOffset, corridoToNewRoomMaxOffset + 1) * -tileWidth)); ;

            starts.Add(new Point(startX, startY));
        }

        private void DrawEastCorridor(PreviousIterationRoom previousRoom, List<Point> starts)
        {
            //take a coordinate on the right side of the previous room (its a Y coordinate)
            var corrStart = _rnd.Next(previousRoom.StartY,
                previousRoom.StartY + previousRoom.RealHeight - (int)(1 * tileHeight));

            //corridor going east is 1 unit high
            int corrHeight = (int)(1 * tileHeight);

            int corrWidth = (int)(_rnd.Next(corridorMinLength, corridorMaxLength) * tileWidth);

            System.Windows.Shapes.Rectangle corridor = new System.Windows.Shapes.Rectangle
            { Height = corrHeight, Width = corrWidth, Fill = new SolidColorBrush(Colors.LightGray) };

            gameArea.Children.Add(corridor);

            Canvas.SetTop(corridor, corrStart);
            Canvas.SetLeft(corridor, previousRoom.StartX + previousRoom.RealWidth);

            //compute new start point = corridor end +/- Y offset to avoid next room to have its corridor on top left corner
            var startY = corrStart;
            //(int)(corrStart +
            //           (_rnd.NextDouble() > 0.5
            //               ? _rnd.Next(corridoToNewRoomMinOffset, corridoToNewRoomMaxOffset + 1) * tileHeight
            //               : _rnd.Next(corridoToNewRoomMinOffset, corridoToNewRoomMaxOffset + 1) * -tileHeight));

            var startX = previousRoom.StartX + previousRoom.RealWidth + corrWidth;

            starts.Add(new Point(startX, startY));
        }
    }

    class PreviousIterationRoom
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int RealWidth { get; set; }
        public int UnitWidth { get; set; }
        public int RealHeight { get; set; }
        public int UnitHeight { get; set; }
    }
}
