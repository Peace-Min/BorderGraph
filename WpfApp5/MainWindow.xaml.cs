using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp5
{
    public class Range
    {
        public double Start { get; set; }
        public double End { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<Range> RangeList;
        public MainWindow() 
        {
            RangeList = new List<Range> 
            { 
                new Range(){ Start = 6000, End=9000},
                new Range(){ Start = 8000, End=14000},
                new Range(){ Start = 10000, End=18000},
            };

            // Call UpdateBorders after the window is fully loaded to ensure the Canvas dimensions are available
            this.Loaded += MainWindow_Loaded;
        }

        // Modify the constructor to accept List<Range> as a parameter
        public MainWindow(List<Range> ranges)
        {
            InitializeComponent();

            // Assign the passed ranges to the RangeList
            RangeList = ranges;

            // Call UpdateBorders after the window is fully loaded to ensure the Canvas dimensions are available
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBorders();
        }

        private void UpdateBorders()
        {
            // Define column ranges within 6000 to 18000 range
            CheckRangeOverlap(Canvas0, 6000, 8000); // 6000-8000
            CheckRangeOverlap(Canvas1, 8000, 10000); // 8000-10000
            CheckRangeOverlap(Canvas2, 10000, 12000); // 10000-12000
            CheckRangeOverlap(Canvas3, 12000, 14000); // 12000-14000
            CheckRangeOverlap(Canvas4, 14000, 16000); // 14000-16000
            CheckRangeOverlap(Canvas5, 16000, 18000); // 16000-18000
        }

        private void CheckRangeOverlap(Canvas canvas, double rangeStart, double rangeEnd)
        {
            canvas.Children.Clear(); // 이전의 사각형 지우기

            List<Range> overlappingRanges = new List<Range>();

            // 현재 범위(rangeStart, rangeEnd)와 겹치는 범위를 추가
            foreach (var range in RangeList)
            {
                if (range.End > rangeStart && range.Start < rangeEnd)
                {
                    overlappingRanges.Add(new Range
                    {
                        Start = Math.Max(range.Start, rangeStart),
                        End = Math.Min(range.End, rangeEnd)
                    });
                }
            }

            // 겹치는 구간 순서대로 처리
            double currentPos = rangeStart;

            foreach (var currentRange in overlappingRanges)
            {
                // 공백이 있는 경우 검은색으로 채우기
                if (currentRange.Start > currentPos)
                {
                    DrawRectangle(canvas, currentPos, currentRange.Start, rangeStart, rangeEnd, Colors.Black);
                }

                // 중첩된 범위가 있는지 확인 (중첩되는 경우)
                var overlapRanges = overlappingRanges
                    .Where(r => r != currentRange && r.Start < currentRange.End && r.End > currentRange.Start)
                    .ToList();

                double overlapEnd = currentRange.End;

                if (overlapRanges.Count > 0)
                {
                    // 중첩된 부분을 먼저 보라색으로 채우고 나머지를 파란색으로 채우기
                    foreach (var overlapRange in overlapRanges)
                    {
                        double overlapStart = Math.Max(currentRange.Start, overlapRange.Start);
                        overlapEnd = Math.Min(currentRange.End, overlapRange.End);

                        // 중첩된 부분 보라색으로 채우기
                        DrawRectangle(canvas, overlapStart, overlapEnd, rangeStart, rangeEnd, Colors.Purple);
                    }

                    // 남은 부분이 있다면 파란색으로 채우기
                    if (overlapEnd < currentRange.End)
                    {
                        DrawRectangle(canvas, overlapEnd, currentRange.End, rangeStart, rangeEnd, Colors.Blue);
                    }
                }
                else
                {
                    // 중첩이 없을 때는 파란색으로 채우기
                    DrawRectangle(canvas, currentRange.Start, currentRange.End, rangeStart, rangeEnd, Colors.Blue);
                }

                // 현재 범위 끝 지점 업데이트
                currentPos = currentRange.End;
            }

            // 마지막 범위 이후에 남은 부분이 있으면 검은색으로 채우기
            if (currentPos < rangeEnd)
            {
                DrawRectangle(canvas, currentPos, rangeEnd, rangeStart, rangeEnd, Colors.Black);
            }
        }

        private void DrawRectangle(Canvas canvas, double overlapStart, double overlapEnd, double rangeStart, double rangeEnd, Color color)
        {
            double rangeLength = rangeEnd - rangeStart;
            double overlapLength = overlapEnd - overlapStart;

            // Calculate the width percentage of the overlap in relation to the total border range
            double overlapWidth = (overlapLength / rangeLength) * canvas.ActualWidth;

            // Calculate the left margin for the start position
            double leftMargin = ((overlapStart - rangeStart) / rangeLength) * canvas.ActualWidth;

            // Create the rectangle representing the overlap range
            Rectangle rect = new Rectangle
            {
                Width = overlapWidth,
                Height = canvas.ActualHeight,
                Fill = new SolidColorBrush(color),
            };

            // Set the position of the rectangle on the Canvas
            Canvas.SetLeft(rect, leftMargin);
            canvas.Children.Add(rect);
        }
    }
}
