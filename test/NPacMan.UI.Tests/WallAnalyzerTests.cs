using System;
using FluentAssertions;
using NPacMan.Game;
using Xunit;

namespace NPacMan.UI.Tests
{
    public class WallAnalyzerTests
    {
        [Fact]
        public void ShouldReturnTopHorizontalLine()
        {
            var board = BoardLoader.Load(
                @" XXX 
 ... ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 0), board.Width, board.Height);

            type.Should().Be(WallType.HorizontalLine);
        }

        [Fact]
        public void ShouldReturnBottomHorizontalLine()
        {
            var board = BoardLoader.Load(
                @" ... 
 XXX ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);

            type.Should().Be(WallType.HorizontalLine);
        }


        [Fact]
        public void ShouldReturnLeftVerticalLine()
        {
            var board = BoardLoader.Load(
                @" X.. 
 X.. 
 X.. ");
            var type = WallAnalyzer.GetWallType(board.Walls, (0, 1), board.Width, board.Height);

            type.Should().Be(WallType.VerticalLine);
        }

        [Fact]
        public void ShouldReturnRightVerticalLine()
        {
            var board = BoardLoader.Load(
                @" ..X 
 ..X 
 ..X ");
            var type = WallAnalyzer.GetWallType(board.Walls, (2, 1), board.Width, board.Height);

            type.Should().Be(WallType.VerticalLine);
        }

        [Fact]
        public void ShouldAnalyzeTopLeftCornerAsTopLeftArc()
        {
            var board = BoardLoader.Load(
                @" XX 
 X.");
            var type = WallAnalyzer.GetWallType(board.Walls, (0, 0), board.Width, board.Height);


            type.Should().Be(WallType.TopLeftArc);
        }

        [Fact]
        public void ShouldAnalyzeTopMiddleCornerAsTopRightArc()
        {
            var board = BoardLoader.Load(
                @" XXX 
 .XX ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 0), board.Width, board.Height);


            type.Should().Be(WallType.TopRightArc);
        }

        [Fact]
        public void ShouldAnalyzeTopRightCornerAsTopRightArc()
        {
            var board = BoardLoader.Load(
                @" XX 
 .X ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 0), board.Width, board.Height);


            type.Should().Be(WallType.TopRightArc);
        }

        [Fact]
        public void ShouldAnalyzeTopLeftEdgeAsTopLeftArc()
        {
            var board = BoardLoader.Load(
                @" ... 
 .XX 
 .X ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.TopLeftArc);
        }

        [Fact]
        public void ShouldAnalyzeBottomLeftEdgeAsBottomLeftArc()
        {
            var board = BoardLoader.Load(
                @" .X  
 .XX 
 ...");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.BottomLeftArc);
        }

        [Fact]
        public void ShouldAnalyzeTopRightEdgeAsTopRightArc()
        {
            var board = BoardLoader.Load(
                @" ...
 XX.
  X. ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.TopRightArc);
        }

        [Fact]
        public void ShouldAnalyzeBottomRightEdgeAsBottomRightArc()
        {
            var board = BoardLoader.Load(
                @"  X.
 XX.
 ... ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.BottomRightArc);
        }

        
        [Fact]
        public void ShouldAnalyzeVerticalLineAsVerticalLine()
        {
            var board = BoardLoader.Load(
                @" XXX
 .XX
 .XX");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.VerticalLine);
        }

        [Fact]
        public void ShouldAnalyzeVerticalLine2AsVerticalLine()
        {
            var board = BoardLoader.Load(
                @" .XX
 .XX
 .XX");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.VerticalLine);
        }

        [Fact]
        public void ShouldAnalyzeVerticalLine3AsVerticalLine()
        {
            var board = BoardLoader.Load(
                @" XXX
 XX.
 XX.");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.VerticalLine);
        }

        [Fact]
        public void ShouldAnalyzeVerticalLine4AsVerticalLine()
        {
            var board = BoardLoader.Load(
                @" XX.
 XX.
 XX.");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.VerticalLine);
        }

        
        [Fact]
        public void ShouldAnalyzeHorizontalLine1AsHorizontalLine()
        {
            var board = BoardLoader.Load(
                @" ...
 XXX
 XXX");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.HorizontalLine);
        }

        [Fact]
        public void ShouldAnalyzeHorizontalLine2AsHorizontalLine()
        {
            var board = BoardLoader.Load(
                @" XXX 
 XXX 
 ... ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.HorizontalLine);
        }

        
        [Fact]
        public void ShouldAnalyzeBottomLeftCorner1AsBottomLeftArc()
        {
            var board = BoardLoader.Load(
                @" XX. 
 XXX 
 XXX ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.BottomLeftArc);
        }

                
        [Fact]
        public void ShouldAnalyzeTopLeftCorner1AsTopLeftArc()
        {
            var board = BoardLoader.Load(
                @" XXX 
 XXX 
 XX. ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.TopLeftArc);
        }

        [Fact]
        public void ShouldAnalyzeTopRightCorner1AsTopRightArc()
        {
            var board = BoardLoader.Load(
                @" XXX 
 XXX 
 .XX ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.TopRightArc);
        }

        
        [Fact]
        public void ShouldAnalyzeBottomRightCorner1AsBottomRightArc()
        {
            var board = BoardLoader.Load(
                @" .XX 
 XXX 
 XXX ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.BottomRightArc);
        }

        [Fact]
        public void ShouldAnalyzeLeftHorizontalLineEdgeAsHorizontalLine()
        {
            var board = BoardLoader.Load(
                @"    
  XX 
     ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.HorizontalLine);
        }

        [Fact]
        public void ShouldAnalyzeRightHorizontalLineEdgeAsHorizontalLine()
        {
            var board = BoardLoader.Load(
                @"    
   XX
     ");
            var type = WallAnalyzer.GetWallType(board.Walls, (1, 1), board.Width, board.Height);


            type.Should().Be(WallType.HorizontalLine);
        }

    }
}
