namespace DefaultNamespace;

public class PathFInderService
{{
    public class PathFinderService
    {
        public class PathResult
        {
            public List<Route> Routes { get; }
            public int Distance { get; }
            public bool HasPath => Routes.Count > 0;

            public PathResult(List<Route> routes, int distance)
            {
                Routes = routes;
                Distance = distance;
            }

            public static PathResult Empty => new(new List<Route>(), 0);
        }

        public PathResult FindShortestPath(IEnumerable<CityNode> cities, IEnumerable<Route> routes, CityNode startCity,
            CityNode endCity)
        {
            if (startCity is null || endCity is null) return PathResult.Empty;

            var routeMap = routes.ToLookup(r => r.Start);
            var distances = new Dictionary<CityNode, int> { [startCity] = 0 };
            var previous = new Dictionary<CityNode, CityNode>();
            var queue = new PriorityQueue<CityNode, int>();
            queue.Enqueue(startCity, 0);

            while (queue.TryDequeue(out var current, out _) && current != endCity)
            {
                var currentDistance = distances[current];

                foreach (var route in routeMap[current])
                {
                    var newDistance = currentDistance + route.Distance;
                    if (!distances.TryGetValue(route.Destination, out var oldDistance) || newDistance < oldDistance)
                    {
                        distances[route.Destination] = newDistance;
                        previous[route.Destination] = current;
                        queue.Enqueue(route.Destination, newDistance);
                    }
                }
            }
            
            if (!previous.ContainsKey(endCity)) return PathResult.Empty;

            var path = new List<Route>();
            for (var city = endCity; previous.TryGetValue(city, out var prev); city = prev)
            {
                path.Insert(0, routeMap[prev].First(r => r.Destination == city));
            }
            
            return new PathResult(path, distances[endCity]);
        }
    }
}