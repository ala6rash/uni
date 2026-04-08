using System;

namespace Uni_Connect.Tests
{
    public class TestFixtures : IDisposable
    {
        public TestFixtures()
        {
            // Setup code can go here, though xUnit injects DbContexts per test when using TestHelpers.
        }

        public void Dispose()
        {
            // Cleanup code can go here
        }
    }
}
