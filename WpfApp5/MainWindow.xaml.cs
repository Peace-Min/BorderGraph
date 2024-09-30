using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp5
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
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
            InitializeComponent();

            // Sample ranges with overlap
            RangeList = new List<Range>()
            {
                new Range() { Start = 6000, End = 8000}, // Example of partial range
                new Range() { Start = 10000, End = 0},
                new Range() { Start = 0, End = 0 },
            };

            // Call UpdateBorders after the window is fully loaded to ensure the Canvas dimensions are available
            this.Loaded += MainWindow_Loaded;
        }

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
            // Define column ranges
            CheckRangeOverlap(Canvas0, 6000, 8000); // 2000-4000
            CheckRangeOverlap(Canvas1, 8000, 10000); // 4000-6000
            CheckRangeOverlap(Canvas2, 10000, 12000); // 6000-8000
            CheckRangeOverlap(Canvas3, 12000, 14000); // 8000-10000
            CheckRangeOverlap(Canvas4, 14000, 16000); // 10000-12000
            CheckRangeOverlap(Canvas5, 16000, 18000); // 12000-14000
        }

        private void CheckRangeOverlap(Canvas canvas, double rangeStart, double rangeEnd)
        {
            canvas.Children.Clear(); // Clear any previous rectangles

            List<Range> overlappingRanges = new List<Range>();

            // Add all ranges that overlap with the current range (rangeStart to rangeEnd)
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

            // Step 1: Sort ranges by Start position to handle overlaps properly
            overlappingRanges = overlappingRanges.OrderBy(r => r.Start).ToList();

            // Step 2: Loop through the ranges and check for overlaps
            double lastEnd = rangeStart;
            foreach (var currentRange in overlappingRanges)
            {
                // If there is a gap between the last processed end and the current range start, fill it with the default color (black)
                if (currentRange.Start > lastEnd)
                {
                    DrawRectangle(canvas, lastEnd, currentRange.Start, rangeStart, rangeEnd, Colors.Black);
                }

                // Check if the current range overlaps with any other range
                var overlapRanges = overlappingRanges.Where(r => r != currentRange && r.Start < currentRange.End && r.End > currentRange.Start).ToList();

                if (overlapRanges.Count > 0)
                {
                    // There is overlap, color the overlapping section purple
                    foreach (var overlapRange in overlapRanges)
                    {
                        double overlapStart = Math.Max(currentRange.Start, overlapRange.Start);
                        double overlapEnd = Math.Min(currentRange.End, overlapRange.End);

                        DrawRectangle(canvas, overlapStart, overlapEnd, rangeStart, rangeEnd, Colors.Purple);

                        // Adjust the current range to avoid recoloring the same section
                        if (overlapStart > currentRange.Start)
                        {
                            DrawRectangle(canvas, currentRange.Start, overlapStart, rangeStart, rangeEnd, Colors.Blue);
                        }
                        lastEnd = overlapEnd;
                    }
                }
                else
                {
                    // No overlap, draw the range in blue
                    DrawRectangle(canvas, currentRange.Start, currentRange.End, rangeStart, rangeEnd, Colors.Blue);
                    lastEnd = currentRange.End;
                }
            }

            // Fill the remaining part after the last range with the default color (black)
            if (lastEnd < rangeEnd)
            {
                DrawRectangle(canvas, lastEnd, rangeEnd, rangeStart, rangeEnd, Colors.Black);
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
