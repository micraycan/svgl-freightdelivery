using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace Client
{
    public static class Routes
    {
        private static readonly Random rnd = new Random();

        public static List<Route> RouteList = new List<Route>
        {
            // destination, dropofflocation
            //new Route(new Vector3(115.5f, -1618.17f, 29.21f), new Vector3(100.41f, -1606.15f, 29.52f)), gated loading area
            //new Route(new Vector3(-448.06f, 292.85f, 83.23f), new Vector3(-448.06f, 292.85f, 83.23f)), // comedy club
            //new Route(new Vector3(875.75f, 2169.64f, 51.95f), new Vector3(875.75f, 2169.64f, 51.95f)), // foreclosed property
            new Route(new Vector3(33.58f, -2645.2f, 6.01f), new Vector3(34.06f, -2664.75f, 6.01f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // shipyard
            new Route(new Vector3(-264.94f, -2507.62f, 6.0f), new Vector3(-264.94f, -2507.62f, 6.0f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // port of los santos
            new Route(new Vector3(-1058.86f, -2396.05f, 13.95f), new Vector3(-1058.86f, -2396.05f, 13.95f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // airport
            new Route(new Vector3(-1349.09f, -749.43f, 22.29f), new Vector3(-1349.09f, -749.43f, 22.29f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // shop alley
            new Route(new Vector3(-249.04f, -251.99f, 36.52f), new Vector3(-249.04f, -251.99f, 36.52f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // rockford plaza mall
            new Route(new Vector3(1241.8f, 1863.33f, 79.18f), new Vector3(1241.8f, 1863.33f, 79.18f), new List<string> { "trailers", "trailers2", "trailers4" }), // cement works
            new Route(new Vector3(193.94f, 2760.09f, 43.43f), new Vector3(193.94f, 2760.09f, 43.43f), new List<string> { "trailers", "trailers2", "trailers4" }), // industrial/construction supply
            new Route(new Vector3(582.96f, 2789.07f, 42.19f), new Vector3(582.96f, 2789.07f, 42.19f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // strip mall 
            new Route(new Vector3(-601.94f, 5316.65f, 70.41f), new Vector3(-601.94f, 5316.65f, 70.41f), new List<string> { "trailers", "trailers2", "trailers4" }), // lumber yard
            new Route(new Vector3(-435.43f, 6139.46f, 31.48f), new Vector3(-435.43f, 6139.46f, 31.48f), new List<string> { "trailers", "trailers2", "trailers4" }), // blaine county postops
            new Route(new Vector3(-367.07f, 6062.17f, 31.5f), new Vector3(-367.07f, 6062.17f, 31.5f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // paleto market store
            new Route(new Vector3(-173.03f, 6282.41f, 31.49f), new Vector3(-173.03f, 6282.41f, 31.49f), new List<string> { "trailers", "trailers2", "trailers4" }), // paleto machine supplies
            new Route(new Vector3(-80.95f, 6495.04f, 31.53f), new Vector3(-80.95f, 6495.04f, 31.53f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // paleto willies supermarket
            new Route(new Vector3(63.36f, 6512.78f, 31.44f), new Vector3(63.36f, 6512.78f, 31.44f), new List<string> { "trailers", "trailers2", "trailers4" }), // paleto construction site
            new Route(new Vector3(2894.65f, 4381.28f, 50.37f), new Vector3(2894.65f, 4381.28f, 50.37f), new List<string> { "trailers", "trailers2", "trailers4" }), // grain supply co
            new Route(new Vector3(1372.49f, 3617.53f, 34.89f), new Vector3(1372.49f, 3617.53f, 34.89f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // liquor store
            new Route(new Vector3(1979.11f, 3778.3f, 32.18f), new Vector3(1979.11f, 3778.3f, 32.18f), new List<string> { "trailers3", "trailers4" }), // gas station
            new Route(new Vector3(2349.22f, 3135.02f, 48.21f), new Vector3(2349.22f, 3135.02f, 48.21f), new List<string> { "trailers", "trailers2", "trailers4" }), // recycling center in desert
            new Route(new Vector3(2552.8f, 419.39f, 108.46f), new Vector3(2552.8f, 419.39f, 108.46f), new List<string> { "trailers", "trailers2", "trailers3", "trailers4" }), // rest stop
            new Route(new Vector3(1206.35f, -1267.69f, 35.23f), new Vector3(1206.35f, -1267.69f, 35.23f), new List<string> { "trailers", "trailers2", "trailers4" }), // lumber yard in city
            new Route(new Vector3(937.97f, -2443.03f, 28.48f), new Vector3(937.97f, -2443.03f, 28.48f), new List<string> { "trailers", "trailers2", "trailers4" }), // industrial area
        };

        public static Route GetRandomRoute()
        {
            int index = rnd.Next(RouteList.Count);
            return RouteList[index];
        }
    }

    public class Route
    {
        public Vector3 Destination { get; private set; }
        public Vector3 DropoffLocation { get; private set; }
        public List<string> PossibleTrailers { get; private set; }

        private static readonly Random rnd = new Random();

        public Route(Vector3 destination, Vector3 dropoffLocation, List<string> possibleTrailers)
        {
            Destination = destination;
            DropoffLocation = dropoffLocation;
            PossibleTrailers = possibleTrailers;
        }

        public string GetRandomTrailer()
        {
            if (PossibleTrailers != null && PossibleTrailers.Count > 0)
            {
                int index = rnd.Next(PossibleTrailers.Count);
                return PossibleTrailers[index];
            }

            return null;
        }
    }
}
