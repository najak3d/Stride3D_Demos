using Stride.Engine;

namespace Falon
{
    class FalonApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
